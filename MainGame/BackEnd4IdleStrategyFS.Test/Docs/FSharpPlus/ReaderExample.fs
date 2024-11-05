module BackEnd4IdleStrategyFS.Test.Docs.FSharpPlus.ReaderExample

open System
open FSharpPlus
open FSharpPlus.Data

type IUserRepository =
    abstract GetUser: email: string -> string

type IShoppingListRepository =
    abstract AddToCart: shoppingList: string list -> string

let getUser email =
    Reader(fun (env: #IUserRepository) -> env.GetUser email)

let addToShoppingList shoppingListItems =
    Reader(fun (env: #IShoppingListRepository) -> env.AddToCart shoppingListItems)

let addShoppingListM email =
    monad {
        let! user = getUser email
        //
        let shoppingListItems = [ "Apple"; "Pear" ]
        return! addToShoppingList shoppingListItems
    }

type MockDataEnv() = // This is how an environment could be constructed
    interface IUserRepository with
        member this.GetUser email = "Sandeep"

    interface IShoppingListRepository with
        member this.AddToCart shoppingListItems =
            sprintf "Added the following items %A to the cart" shoppingListItems

Reader.run (addShoppingListM "sandeep@test.com") (MockDataEnv())

// Haskell Wiki 上的 Reader monad 示例

/// This the abstract syntax representation of a template
type Template =
    /// Text
    | T of string
    /// Variable
    | V of Template
    /// Quote
    | Q of Template
    /// Include
    | I of Template * (Definition list)
    /// Compound
    | C of Template list

and Definition = D of Template * Template

/// Our environment consists of an association list of named templates and
/// an association list of named variable values.
type Environment =
    { templates: Map<string, Template>
      variables: Map<string, string> }

/// lookup a variable from the environment
let lookupVar (name: string) (env: Environment) : string option = tryItem name env.variables

/// lookup a template from the environment
let lookupTemplate (name: string) (env: Environment) : Template option = tryItem name env.templates

/// add a list of resolved definitions to the environment
let addDefs (defs: (string * string) list) env =
    { env with
        variables = plus (Map.ofList defs) env.variables }

/// resolve a template into a string
let rec resolve: Template -> Reader<Environment, string> =
    function
    | T s -> result s
    | V t ->
        monad {
            let! varName = resolve t
            let! env = ask
            let varValue = lookupVar varName env
            return option id "" varValue
        }
    | Q t ->
        monad {
            let! tmplName = resolve t
            let! env = ask
            let body = lookupTemplate tmplName env
            return option string "" body
        }
    | I(t, ds) ->
        monad {
            let! tmplName = resolve t
            let! env = ask
            let body = lookupTemplate tmplName env

            match body with
            | Some t' ->
                let! defs = List.traverse resolveDef ds
                return! local (addDefs defs) (resolve t')
            | None -> return ""
        }
    | C ts ->
        monad {
            let! resolved = List.traverse resolve ts
            return String.Concat<string> resolved
        }

/// resolve a Definition and produce a (name,value) pair
and resolveDef: Definition -> Reader<Environment, string * string> =
    function
    | D(t, d) ->
        monad {
            let! name = resolve t
            let! value = resolve d
            return (name, value)
        }
