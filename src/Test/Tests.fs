module Test

open System
open System.Net
open Xunit
open FsUnit.Xunit
open FsHttp
open FsMinimalOpenApi

let endpoint = "http://localhost:8080"


[<Fact>]
let health () =
    let resp = http { GET $"{endpoint}/health" } |> Request.send
    resp.statusCode |> should equal HttpStatusCode.OK

[<Fact>]
let hello_world () =
    let resp = http { GET $"{endpoint}/" } |> Request.send
    resp.statusCode |> should equal HttpStatusCode.OK
    let content = resp.content.ReadAsStringAsync().Result
    content |> should equal "Hello World!"

[<Fact>]
let post_todo () =
    let id = "test_" + Guid.NewGuid().ToString()

    let resp =
        http {
            POST $"{endpoint}/todo_items"
            body

            jsonSerialize {
                id = id
                title = $"Title {id}"
                description = $"Description {id}"
            }
        }
        |> Request.send

    resp.statusCode |> should equal HttpStatusCode.OK
