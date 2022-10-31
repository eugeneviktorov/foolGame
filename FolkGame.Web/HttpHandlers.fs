namespace FolkGame.Web

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks
    open Giraffe
    open FolkGame.Web.Models
    open FolkGame.Core
    
    let newGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                

                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }