namespace FoolGame.Web

open FoolGame.Core

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Giraffe
    open FoolGame.Web.Models
    open Game
    open Player

    let mutable startedGames: Game list =
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
                    { Deck = deck
                      Players = players
                      AttackPlayer = None
                      DefencePlayer = None
                      Moves = []
                      GeneralCard = List.last deck }
                    :: startedGames

                return! json "Ok!" next ctx
            }

    let gameList =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

    let join =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

    let play =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let response = sprintf $"%A{startedGames}"
                return! json response next ctx
            }

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
