module FoolGame.Core.Game

open System
open System.Text.RegularExpressions
open FoolGame.Core.Card
open FoolGame.Core.Player

type MovedCard = { Card: Card; Player: Player }

type Move =
    | TakeFromDeck of MovedCard
    | Play of MovedCard
    | Beat of Beaten: MovedCard * Played: MovedCard
    | Take
    | Done

type TableCard = { Played: Card; Beaten: Card option }

type Game =
    { Deck: Card list
      Players: Player list
      AttackPlayer: Player option
      DefencePlayer: Player option
      Moves: Move list
      GeneralCard: Card }

    static member DefaultDeck =
        [ for suit in suits do
              for rank in ranks do
                  yield { Suit = suit; Rank = rank } ]

let indexOf item list = List.findIndex (fun x -> x = item) list

let canBeat playedCard beatCard suit =
    if playedCard.Suit = beatCard.Suit then
        playedCard.Rank < beatCard.Rank
    else
        beatCard.Suit = suit

let shuffle (deck: Card list) =
    let deck = List.toArray deck
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

    Array.toList deck

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

    let index = (indexOf oldPlayer game.Players)

    { game with Players = (List.updateAt index newPlayer game.Players) }

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
    |> Seq.map (fun (played, beaten) ->
        { Played = played.Card
          Beaten =
            match beaten with
            | Some (x) -> Some(x.Card)
            | None -> None })

let registerMove move game =
    { game with Moves = game.Moves @ [ move ] }

let takeCard count player game =
    if count <= 0 then
        game
    else
        match player with
        | Some (player) ->
            let cards = List.take count game.Deck
            printfn $"Taken cards for %s{player.Name} is : %A{cards}"

            ((player, game), cards)
            ||> Seq.fold (fun (player, game) card ->
                let newPlayer =
                    { player with Hand = (player.Hand @ [ card ]) }

                updatePlayer player newPlayer { game with Deck = game.Deck[1..] }
                |> registerMove (TakeFromDeck { Card = card; Player = player })
                |> (fun x y -> (x, y)) newPlayer)
            |> (fun (_, game) -> game)
        | _ -> failwith "Player is required"

let beatCard beaten played game =
    registerMove (Beat(beaten, played)) game

let nextPlayer currentPlayer game =
    let mutable indexNext =
        (indexOf currentPlayer game.Players) + 1

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

let play cardIndex player game =
    let hand = player.Hand
    let card = hand[cardIndex]
    let hand = (List.removeAt cardIndex hand)

    registerMove (Play({ Card = card; Player = player })) game
    |> updatePlayer player { player with Hand = hand }
