module FsMinimalOpenApi

#nowarn"3391"

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults 

type TodoItem = {
    id: string
    title: string
    description: string
}

let mutable todo_items = [
    {
        id = "1"
        title = "Todo 1"
        description = "todo1 todo1"
    }
    {
        id = "2"
        title = "Todo 2"
        description = "todo1 todo2"
    }
]

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddEndpointsApiExplorer() |> ignore
    builder.Services.AddSwaggerGen() |> ignore

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseSwagger() |> ignore
        app.UseSwaggerUI() |> ignore

    app
        .MapGet("/", Func<string>(fun () -> "Hello World!"))
        .WithName("HelloWorld")
        .WithTags("Hello World")
        .WithOpenApi()

    |> ignore

    app
        .MapGet("/health", Func<string>(fun () -> "OK"))
        .WithName("Health")
        .WithTags("Health")
        .WithOpenApi()
    |> ignore

    app
        .MapGet("/todo_items/all", Func<TodoItem list>(fun () -> todo_items))
        .WithName("Get All Todos")
        .WithTags("Todo")
        .WithOpenApi()
    |> ignore

    app
        .MapGet(
            "/todo_items/{id}",
            Func<string, TodoItem option>(fun id ->
                todo_items |> List.tryFind (fun x -> x.id = id))
        )
        .WithTags("Todo")
        .WithOpenApi()
    |> ignore

    app
        .MapPost(
            "/todo_items",
            Func<TodoItem, TodoItem>(fun item ->
                todo_items <- todo_items @ [ item ]
                item)
        )
        .WithTags("Todo")
        .WithOpenApi()
    |> ignore
    
    app
        .MapDelete(
            "/todo_items/{id}",
            Func<string, Results<Ok<TodoItem>, BadRequest<string>>>(fun id ->
                let item = todo_items |> List.tryFind (fun x -> x.id = id)

                match item with
                | Some item -> 
                    todo_items <- todo_items |> List.filter (fun x -> x.id <> id) 
                    let resp = TypedResults.Ok(item)  
                    resp
                | None -> 
                    let error = TypedResults.BadRequest ("error")
                    error
            )
        )
        .WithTags("Todo")
        .WithOpenApi()
    |> ignore


    app.Run()

    0 // Exit code
