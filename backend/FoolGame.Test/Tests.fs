module Tests

open System
open System.IO
open System.Text.Json
open FoolGame.Test
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Xunit
open HttpFunc
open Xunit.Abstractions

[<Fact>]
let ``My test`` () = Assert.True(true)

let createHost () =
    WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseEnvironment("Test")
        .Configure(Action<IApplicationBuilder> FoolGame.Web2.App.configureApp)
        .ConfigureServices(Action<IServiceCollection> FoolGame.Web.App.configureServices)

let shouldContains actual expected = Assert.Contains(actual, expected)
let shouldEqual expected actual = Assert.Equal(expected, actual)
let shouldNotNull expected = Assert.NotNull(expected)

type ApiTests(output: ITestOutputHelper) =

    let createGame client playerCount =
        get client $"api/new/%i{playerCount}"
            |> ensureSuccess
            |> readText
            |> JsonSerializer.Deserialize<Guid>
    let join client gameId =
        get client $"api/join/%O{gameId}"
            |> ensureSuccess
            |> readText
            |> JsonSerializer.Deserialize<Guid>
    
    let start client gameId =        
        get client $"api/start/%O{gameId}"
        |> ensureSuccess
        |> readText
        |> ignore        
    
    [<Fact>]
    member _.``GET /api/new/ should response id``() =
        
        use server = new TestServer(createHost ())
        use client = server.CreateClient()
        
        createGame client 2

    [<Fact>]
    member _.``GET /api/list``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        createGame client 2 |> ignore

        get client "api/list"
        |> ensureSuccess
        |> readText
        |> output.WriteLine


    [<Fact>]
    member _.``GET /api/join``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        let gameId = createGame client 2
        (join client gameId)
        |> fun x-> output.WriteLine $"%O{x}"
        
    [<Fact>]
    member _.``GET /api/start``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        let gameId = createGame client 2
        let p1 = join client gameId
        let p2 = join client gameId
        
        get client $"api/start/%O{gameId}"
        |> ensureSuccess
        |> readText
        |> output.WriteLine
        
    [<Fact>]
    member _.``GET /api/play``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        let gameId = createGame client 2
        let p1 = join client gameId
        let p2 = join client gameId
        
        start client gameId
        
        get client $"api/play/1/%O{p1}"
        |> ensureSuccess
        |> ignore
        
        get client $"api/table/%O{gameId}"
        |> ensureSuccess
        |> readText
        |> output.WriteLine

    [<Fact>]
    member _.``GET /api/state``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        let gameId = createGame client 2
        let p1 = join client gameId
        let p2 = join client gameId
        
        start client gameId
        
        get client $"api/play/1/%O{p1}"
        |> ensureSuccess
        |> ignore
                
        get client $"api/state/%O{p1}"
        |> ensureSuccess
        |> readText
        |> output.WriteLine
        
    [<Fact>]
    member _.``GET /api/take``() =
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        let gameId = createGame client 2
        let p1 = join client gameId
        let p2 = join client gameId
        
        start client gameId
        
        get client $"api/play/1/%O{p1}"
        |> ensureSuccess
        |> ignore
                
        get client $"api/take/%O{p2}"
        |> ensureSuccess
        |> ignore
                    
        get client $"api/state/%O{p1}"
        |> ensureSuccess
        |> readText
        |> output.WriteLine