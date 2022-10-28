open System
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core

type CardSuit =
    | Hearts
    | Diamonds
    | Clubs
    | Spades

type CardRank =
    | Six
    | Seven
    | Eight
    | Nine
    | Ten
    | Jack
    | Queen
    | King
    | Ace

[<StructuredFormatDisplay("{DisplayText}")>]
type Card =
    { Suit: CardSuit
      Rank: CardRank }
    member this.DisplayText = this.ToString()
    override this.ToString() = $"{this.Rank} of {this.Suit}"

let suits =
    [ Diamonds; Clubs; Diamonds; Hearts ]

let ranks =
    [ Six
      Seven
      Eight
      Nine
      Ten
      Jack
      Queen
      King
      Ace ]

type Player = { Hand: Card []; Name: string }

type MovedCard = { Card: Card; Player: Player }

type Move =
    | TakeFromDeck of MovedCard
    | Play of MovedCard
    | Beat of Beaten: MovedCard * Played: MovedCard
    | Take
    | Done

type Game =
    { Deck: Card []
      Players: Player []
      AttackPlayer: Player option
      DefencePlayer: Player option
      Moves: Move []
      GeneralCard: Card }

    static member DefaultDeck =
        [| for suit in suits do
               for rank in ranks do
                   yield { Suit = suit; Rank = rank } |]

let canBeat playedCard beatCard suit =
    if playedCard.Suit = beatCard.Suit then
        playedCard.Rank < beatCard.Rank
    else
        beatCard.Suit = suit

let shuffle (deck: Card []) =
    // swap two elements in the supplied array
    let swap i j (array: _ []) =
        let tmp = array.[i]
        array.[i] <- array.[j]
        array.[j] <- tmp

    let rnd = Random()
    let n = Array.length deck

    for i in [ 0 .. (n - 2) ] do
        let j = rnd.Next(i, n - 1)
        swap i j deck

    deck

let getCard (deck: Card []) = deck[Random.Shared.Next(0, 35)]

let (|ParseRegex|_|) regex str =
    let m = Regex("^" + regex + "$").Match(str)
    if (m.Success) then Some m else None

let updatePlayer oldPlayer newPlayer game =
    let game =
        match game.AttackPlayer with
        | Some (x) when x = oldPlayer -> { game with AttackPlayer = Some(newPlayer) }
        | Some _ -> game
        | None -> game

    let game =
        match game.DefencePlayer with
        | Some (x) when x = oldPlayer -> { game with DefencePlayer = Some(newPlayer) }
        | Some _ -> game
        | None -> game

    let index =
        Array.IndexOf(game.Players, oldPlayer)

    { game with Players = (Array.updateAt index newPlayer game.Players) }

let getTable game =
    game.Moves
    |> Seq.rev
    |> Seq.takeWhile (fun move ->
        match move with
        | Done
        | Take
        | TakeFromDeck _ -> false
        | _ -> true)
    |> Seq.map (fun x ->
        match x with
        | Play p -> (p, None)
        | Beat (played, beaten) -> (played, Some(beaten)))
    |> Seq.groupBy (fun (card, _) -> card)
    |> Seq.map (fun (key, y) -> (key, Seq.map (fun (_, beaten) -> beaten) y |> Seq.max))

let registerMove move game =
    { game with Moves = Array.append game.Moves [| move |] }

let takeCard count player game =
    if count <= 0 then
        game
    else
        match player with
        | Some (player) ->
            let cards = Array.take count game.Deck
            printfn $"Taken cards for %s{player.Name} is : %A{cards}"

            ((player, game), cards)
            ||> Seq.fold (fun (player, game) card ->
                let newPlayer =
                    { player with Hand = (Array.append player.Hand [| card |]) }

                updatePlayer player newPlayer { game with Deck = game.Deck[1..] }
                |> registerMove (TakeFromDeck { Card = card; Player = player })
                |> (fun x y -> (x, y)) newPlayer)
            |> (fun (_, game) -> game)
        | _ -> failwith "Player is required"

let beatCard beaten played game =
    registerMove (Beat(beaten, played)) game

let nextPlayer currentPlayer game =
    let mutable indexNext =
        Array.IndexOf(game.Players, currentPlayer) + 1

    if game.Players.Length <= indexNext then
        indexNext <- 0

    game.Players[indexNext]

let updateDefencePlayer attackPlayer game =
    { game with DefencePlayer = Some(nextPlayer attackPlayer game) }

let updateAttackPlayer player game =
    { game with AttackPlayer = Some(player) }
    |> updateDefencePlayer player

let unwrap optionValue =
    match optionValue with
    | Some x -> x
    | _ -> failwith "player is unknown"

let fillPlayersHand game =
    (game, game.Players)
    ||> Seq.fold (fun game p -> (takeCard (6 - p.Hand.Length) (Some(p)) game))

let rec continueGame (game: Game) =

    if Array.isEmpty game.Deck
       && game.Players
          |> Array.map (fun x -> Array.isEmpty x.Hand)
          |> Array.forall id then
        printfn "game done"
        ()

    let (attackPlayer, defencePlayer, game) =
        match (game.AttackPlayer, game.DefencePlayer, game) with
        | None, None, _ ->
            updateAttackPlayer game.Players[0] game
            |> fun g -> (unwrap g.AttackPlayer, unwrap g.DefencePlayer, g)
        | Some (attack), Some (defence), _ -> (attack, defence, game)
        | _ -> failwith "game is corrupted"

    printfn $"Attack player is %s{attackPlayer.Name}"
    printfn $"Defence player is %s{defencePlayer.Name}"

    match Console.ReadLine() with
    | "state" ->
        printfn $"Now Game in action current player is %s{attackPlayer.Name}"
        ()
    | "table" ->
        let table =
            getTable game
            |> Seq.map (fun (card, beat) ->
                (card.Card,
                 match beat with
                 | Some (x) -> Some(x.Card)
                 | None -> None))

        printfn $"On table now: %A{table}"
        continueGame game
    | ParseRegex "beat (\d+) (\d+)" r ->
        let tableIndex =
            (Int32.Parse r.Groups[1].Value - 1)

        let handIndex =
            (Int32.Parse r.Groups[2].Value - 1)

        let hand =
            Array.removeAt handIndex defencePlayer.Hand

        let handCard = defencePlayer.Hand[handIndex]

        let tableCard =
            (Seq.toArray (getTable game))[tableIndex]

        match tableCard with
        | card, beaten when beaten = None ->
            if (canBeat card.Card handCard game.GeneralCard.Suit) then
                printfn $"You just beat %A{card.Card} by {handCard}"

                updatePlayer defencePlayer { defencePlayer with Hand = hand } game
                |> beatCard
                    card
                    { Card = handCard
                      Player = defencePlayer }
            else
                printfn $"You cant use card %A{handCard} for beat %A{card.Card}"
                game
        | _ -> game
        |> continueGame
    | ParseRegex "play (\d+)" cardNumber ->
        let index =
            (Int32.Parse cardNumber.Groups[1].Value - 1)

        let hand = attackPlayer.Hand
        let card = hand[index]
        printfn $"You just pushed %A{card}"
        let hand = (Array.removeAt index hand)
        printfn $"After the operation you hand is %A{hand}"

        registerMove (Play({ Card = card; Player = attackPlayer })) game
        |> updatePlayer attackPlayer { attackPlayer with Hand = hand }
        |> continueGame
    | ParseRegex "hand (\d+)" playerNumber ->
        let playerIndex =
            (Int32.Parse playerNumber.Groups[1].Value - 1)

        let player = game.Players[playerIndex]
        printfn $"%A{player.Hand}"
        (continueGame game)
    | "deck" ->
        printfn $"In deck remains %i{game.Deck.Length} cards"
        continueGame game
    | "done" ->
        registerMove (Done) game
        |> fillPlayersHand
        |> fun g -> updateAttackPlayer (nextPlayer attackPlayer g) g
        |> continueGame
    | "take" ->
        ([||], getTable game)
        ||> Seq.fold (fun x (play, beat) ->
            (Array.append
                x
                [| Some(play.Card)
                   match beat with
                   | Some (x) -> Some(x.Card)
                   | None -> None |]))
        |> Seq.choose id
        |> Seq.toArray
        |> (fun x -> (updatePlayer defencePlayer { defencePlayer with Hand = (Array.append defencePlayer.Hand x) } game))
        |> registerMove (Take)
        |> continueGame
    | "moves" ->
        printfn $"Moves : %A{game.Moves}"
        (continueGame game)
    | "exit" -> ()
    | _ -> (continueGame game)

let currentDeck = shuffle Game.DefaultDeck

let game =
    { Deck = currentDeck
      Moves = [||]
      AttackPlayer = None
      DefencePlayer = None
      GeneralCard = Array.last currentDeck
      Players =
        [| { Hand = [||]; Name = "Player1" }
           { Hand = [||]; Name = "Player2" } |] }

fillPlayersHand game |> continueGame
