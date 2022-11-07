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

    let mutable startedGames: (Game * Guid) list =
        list.Empty

    let mutable connectedPlayers: ConnectedPlayer list =
        list.Empty

    let newGame (playersCount: int) : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let players =
                    seq { 1..playersCount }
                    |> Seq.map (fun x -> { Hand = []; Name = "Player" })
                    |> Seq.toList

                let deck =
                    (Program.shuffle Game.DefaultDeck)

                startedGames <-
                    ({ Deck = deck
                       Players = players
                       AttackPlayer = None
                       DefencePlayer = None
                       Moves = []
                       GeneralCard = List.last deck },
                     Guid.NewGuid())
                    :: startedGames

                let (_, guid) = startedGames.Head
                return! json guid next ctx
            }

    let gameList =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {

                let listGames =
                    startedGames
                    |> List.map (fun x ->
                        let (game, id) = x

                        {| Id = id
                           PlayersCount = game.Players.Length |})

                return! json listGames next ctx
            }

    let join (gameId: Guid) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {

                let maxPlayers =
                    startedGames
                    |> List.filter (fun (game, id) -> id = gameId)
                    |> List.map (fun (game, id) -> game.Players.Length)
                    |> List.head

                let currentCount =
                    connectedPlayers
                    |> List.filter (fun (x) -> x.GameId = gameId)
                    |> List.length

                if (maxPlayers > currentCount) then
                    connectedPlayers <-
                        { GameId = gameId
                          PlayerIndex = currentCount - 1
                          Token = Guid.NewGuid() }
                        :: connectedPlayers

                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

    let play =
        fun (next: HttpFunc) (ctx: HttpContext) -> task { return! json startedGames next ctx }

    let take =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

    let beat =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

    let leave =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }
