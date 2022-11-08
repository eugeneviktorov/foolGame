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

    let getGameByToken token =
        connectedPlayers
        |> List.find (fun x -> x.Token = token)
        |> fun x -> getGame x.GameId

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
                    |> Seq.map (fun x -> { Hand = []; Name = "Player" })
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
                          PlayerIndex = currentCount - 1
                          Token = Guid.NewGuid() }

                    playerBox.Post(session)
                    return! json session.Token next ctx
                else
                    let response = sprintf $"%A{games}"
                    return! json response next ctx
            }

    let startGame (gameId: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) -> task { return! json 1 next ctx }

    let stateGame (token: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let game, status = getGameByToken token
                return! json {| Game = game; Status = status |} next ctx
            }

    let play (token: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {

                let game =
                    connectedPlayers
                    |> List.filter (fun x -> x.Token = token)
                    |> (fun x ->
                        (List.filter (fun (_, id, status) -> id = x.Head.GameId && status = Playing) games)
                        |> List.map (fun (game, _, _) -> game))
                    |> List.head


                return! json games next ctx
            }

    let take =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{games}"
                return! json response next ctx
            }

    let beat =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{games}"
                return! json response next ctx
            }

    let leave =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{games}"
                return! json response next ctx
            }
