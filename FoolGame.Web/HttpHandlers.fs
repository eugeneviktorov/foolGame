namespace FoolGame.Web

open System
open FoolGame.Core

module HttpHandlers =
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Giraffe
    open Game
    open Player

    type ConnectedPlayer =
        { GameId: Guid
          PlayerIndex: int
          Token: Guid }

    type GameStatus =
        | Lobby
        | Playing
        | Done

    let mutable games: (Game * Guid * GameStatus) list =
        list.Empty

    let mutable connectedPlayers: ConnectedPlayer list =
        list.Empty

    type GameMessage =
        | Create of Game * AsyncReplyChannel<Guid>
        | Update of Game * Guid
        | Remove of Guid

    let getGame id : Game * GameStatus =
        games
        |> List.find (fun (_, gid, _) -> gid = id)
        |> (fun (game, _, state) -> (game, state))

    let gameByToken token =
        connectedPlayers
        |> List.find (fun x -> x.Token = token)
        |> fun x -> getGame x.GameId, x.GameId
        |> fun ((game, status), gameId) -> (game, status, gameId)

    let playerByToken token =
        connectedPlayers
        |> List.find (fun x -> x.Token = token)
        |> fun x ->
            (getGame x.GameId)
            |> fun (game, _) -> game.Players[x.PlayerIndex]

    let playerBox =
        MailboxProcessor.Start (fun inbox ->
            let rec loop =
                async {
                    let! pl = inbox.Receive()
                    connectedPlayers <- pl :: connectedPlayers
                    return! loop
                }

            loop)

    let gameBox =
        MailboxProcessor.Start (fun inbox ->
            let rec loop =
                async {
                    let! msg = inbox.Receive()

                    match msg with
                    | Create (game, reply) ->
                        let id = Guid.NewGuid()
                        games <- (game, id, Lobby) :: games
                        reply.Reply(id)
                        return! loop
                    | Update (game, id) ->
                        games <-
                            games
                            |> List.findIndex (fun (_, gid, _) -> gid = id)
                            |> (fun x -> List.updateAt x (game, id, Playing) games)

                        return! loop
                    | Remove (id) ->
                        games <-
                            games
                            |> List.findIndex (fun (_, gid, _) -> gid = id)
                            |> fun x -> List.removeAt x games

                        return! loop
                }

            loop)

    let newGame (playersCount: int) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let players =
                    seq { 1..playersCount }
                    |> Seq.map (fun x -> { Hand = []; Name = $"Player%i{x}" })
                    |> Seq.toList

                let deck = (shuffle Game.DefaultDeck)

                let guid =
                    gameBox.PostAndReply (fun x ->
                        Create(
                            { Deck = deck
                              Players = players
                              AttackPlayer = None
                              DefencePlayer = None
                              Moves = []
                              GeneralCard = List.last deck },
                            x
                        ))

                return! json guid next ctx
            }

    let gameList =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {

                let listGames =
                    games
                    |> List.map (fun x ->
                        let (game, id, status) = x

                        {| Id = id
                           Status = status
                           PlayersCount = game.Players.Length |})

                return! json listGames next ctx
            }

    let joinGame (gameId: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {

                let maxPlayers =
                    games
                    |> List.filter (fun (_, _, status) -> status = Lobby)
                    |> List.map (fun (game, id, _) -> (game, id))
                    |> List.filter (fun (game, id) -> id = gameId)
                    |> List.map (fun (game, id) -> game.Players.Length)
                    |> List.head

                let currentCount =
                    connectedPlayers
                    |> List.filter (fun (x) -> x.GameId = gameId)
                    |> List.length

                if (maxPlayers > currentCount) then
                    let session =
                        { GameId = gameId
                          PlayerIndex = currentCount
                          Token = Guid.NewGuid() }

                    playerBox.Post(session)
                    return! json session.Token next ctx
                else
                    let response = sprintf $"%A{games}"
                    return! json response next ctx
            }

    /// start game
    let startGame (gameId: Guid) =
        connectedPlayers
        |> List.filter (fun x -> x.GameId = gameId)
        |> List.length
        |> fun x ->
            (match x with
             | x when x < 2 -> failwith "Need 2 players or more"
             | _ -> ignore)
        |> ignore

        getGame gameId
        |> fun (game, _) -> fillPlayersHand game
        |> (fun game -> updateAttackPlayer game.Players[0] game)
        |> (fun game -> gameBox.Post(Update(game, gameId)))

        fun (next: HttpFunc) (ctx: HttpContext) -> task { return! json "Game was started" next ctx }

    let stateGame (token: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let game, status, gameId = gameByToken token

                // we need get game from the player view
                // for this we need filter data

                let deckCount = game.Deck.Length

                let players =
                    game.Players
                    |> List.map (fun x ->
                        {| CardCount = x.Hand.Length
                           Name = x.Name
                           IsAttack = game.AttackPlayer = Some(x)
                           IsDefence = game.DefencePlayer = Some(x) |})

                let hand = (playerByToken token).Hand
                let table = getTable game

                return!
                    json
                        {| DeckCount = deckCount
                           Players = players
                           Hand = hand
                           Status = status
                           Table = table |}
                        next
                        ctx
            }

    let play (token: Guid, cardIndex: int) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let (game, status, gameId) =
                    gameByToken token

                let player = playerByToken token

                play cardIndex player game
                |> fun x -> gameBox.Post(Update(x, gameId))

                return! json games next ctx
            }

    let take (token: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                // let (game, _, gameId) = gameByToken token
                // let player = playerByToken token
                //
                // if game.DefencePlayer <> Some(player) then
                //     failwith "This player not defence now"
                //
                // ([], getTable game)
                // ||> Seq.fold (fun x (play, beat) ->
                //     (x
                //      @ [ Some(play.Card)
                //          match beat with
                //          | Some (x) -> Some(x.Card)
                //          | None -> None ]))
                // |> Seq.choose id
                // |> Seq.toList
                // |> (fun x -> (updatePlayer player { player with Hand = (player.Hand @ x) } game))
                // |> registerMove (Take)
                // |> fillPlayersHand
                // |> fun x -> gameBox.Post(Update(x, gameId))

                return! json "taken" next ctx
            }

    let beat (token: Guid, handCardIndex: int, tableCardIndex: int) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                //
                // let (game, _, gameId) = gameByToken token
                // let player = playerByToken token
                //
                // if Some(player) <> game.DefencePlayer then
                //     failwith "This player not defence now"
                //
                // let handCard =
                //     player.Hand |> List.item handCardIndex
                //
                // let tableCard =
                //     getTable game
                //     |> Seq.toList
                //     |> List.item tableCardIndex
                //
                // let hand =
                //     List.removeAt handCardIndex player.Hand
                //
                // let game =
                //     match tableCard with
                //     | card, beaten when beaten = None ->
                //         if (canBeat card.Card handCard game.GeneralCard.Suit) then
                //             updatePlayer player { player with Hand = hand } game
                //             |> beatCard card { Card = handCard; Player = player }
                //         else
                //             failwith "You can't beat this card"
                //     | _ -> failwith "This card already beaten"
                //     |> updateAttackPlayer player
                //     
                // gameBox.Post(Update(game, gameId))
                //
                // let response = sprintf $"%A{games}"
                return! json "" next ctx
            }

    let leave =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{games}"
                return! json response next ctx
            }
