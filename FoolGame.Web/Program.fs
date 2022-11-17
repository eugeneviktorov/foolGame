module FoolGame.Web.App

open System


open System.Net
open System.Text.Json
open System.Text.Json.Serialization
open FoolGame.Core
open FoolGame.Core.Card
open FoolGame.Core.Game
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open FoolGame.Web.HttpHandlers
open Microsoft.OpenApi.Models
open OpenApi


type SpecFactory(jsonOptions) =

    let v1Factory =
        OpenApiFactory.create jsonOptions "Products API" "v1"

    let addOperation (factory: OpenApiFactory) verb path operation =
        factory.AddOperation verb path operation
        path

    member _.V1 = v1Factory

    member _.GetProducts path =
        apiOperation {
            tags [ apiTag { name "Products" } ]
            summary "Get the list of products."

            responses [ HttpStatusCode.OK,
                        apiResponse {
                            description "Success"

                            jsonContent (
                                v1Factory.MakeJsonContent [ {|Test = "123"|} ]
                            )
                        } ]
        }
        |> addOperation v1Factory OperationType.Get path

let jsonOptions =
    JsonSerializerOptions()
    |> fun x ->
            x.Converters.Add(JsonStringEnumConverter())
            x
let spec = SpecFactory jsonOptions

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [ subRoute
                 "/api"
                 (choose [ GET
                           >=> choose [ routef "/new/%i" newGame
                                        routef "/start/%O" startGame
                                        route "/list" >=> gameList
                                        routef "/join/%O" joinGame
                                        routef "/state/%O" stateGame
                                        routef "/play/%O/%i" play
                                        routef "/take/%O" take
                                        routef "/beat/%O/%i/%i" beat
                                        route "/leave" ] ])
             route (spec.GetProducts "/products") >=> text "tehello"
             route spec.V1.SpecificationUrl >=> (spec.V1.Write text)
             setStatusCode 404 >=> text "Not Found"


              ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000", "https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
    |> ignore

let configureApp (app: IApplicationBuilder) =
    let env =
        app.ApplicationServices.GetService<IWebHostEnvironment>()

    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false ->
         app
             .UseGiraffeErrorHandler(errorHandler)
             .UseHttpsRedirection())
        .UseRouting()
        .UseCors(configureCors)
        .UseSwaggerUI(fun x -> x.SwaggerEndpoint(spec.V1.SpecificationUrl, spec.V1.Version))
        .UseGiraffe(webApp)


let configureServices (services: IServiceCollection) =
    services
        .AddCors()
        .AddGiraffe()
        .AddSwaggerGen(fun x -> x.SwaggerDoc("v1", OpenApiInfo(Title = "My API", Version = "v1")))
    |> ignore

let configureLogging (builder: ILoggingBuilder) =
    builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =

    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
