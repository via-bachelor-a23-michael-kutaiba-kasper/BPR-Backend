# Graphify

Schema-first GraphQL API Gateway concept. The general concept is that we define all the types such as `event`, `profile` etc. that the user can interact with. Then we define the `queries` and `mutations` in the `querySpecs` and `mutationSpecs`. Each query will have a ` data source` with a `url`and`method` that defines how the request should be delegated to another service.

## Disclaimer

This has been built in go, since the graphql server SDK for go allows for building these types at runtime. I tried to also build this in C# ASP.NET, but the SDK does not allow for doing this dynamically at runtime. Otherwise we would have to recompile the gateway on each new service.

Writing this in go does make it faster to start up, so this is an advantage if we decide to deploy with Cloud Run again.

# Schema

The schema defines all the custom GraphQL types e.g. `event`, `location` etc. These types can then be used as return types for the different queries.

Example:

```yaml
schemaVersion: "0.1.0"

# The following is a configuration for https://jsonplaceholder.typicode.com/todos
# just to show the concept

server:
    port: 8080

typeDeclarations:
    todo: #(1)
        name: "Todo"
        description: "A todo"
        fields:
            userId:
                type: int # string | boolean | float | int | datetime | list<type>
                description: "Id of the user that created the todo"
            id:
                type: int
                description: "Id of the todo"
            title:
                type: string
                description: "Title of the todo"
            completed:
                type: boolean
                description: "Indicates if the todo has been completed yet."
            user:
                type: "typeDeclarations::user" #(2)
    user:
        name: "User"
        description: "User"
        fields:
            userId:
                type: int
                description: "Id of user"
            name:
                type: string
                description: "full name of user"

querySpecs:
    todo: #(3)
        type: "typeDeclarations::todo" # Must point to one of the types delcared in typeDelcarations for now. Currently using other types such as string, int etc. here is not supported.
        description: "Gets a single todo"
        args:
            id:
                type: string
                forRequestBody: false #(4)
        dataSource:
            url: "https://jsonplaceholder.typicode.com/todos/{id}"
            method: GET
```

`(1)` defines a custom GrahpQL return type. All the provided field descriptions will show up in the GraphQL documentation. <image  src="docs/graphql_screenshot1.png">

`(2)` custom GraphQL type declarations can reference other declarations.

`(3)` defines all the queries that are available. Currently only queries have been implemented for the sake of PoC. Mutations shall be supported in the future.

`(4)` Flag that indicates if the argument is supposed to be sent as a HTTP request body. If set to `false`, then it will be expanded in the url, e.g.:

```graphql
query {
    todo(id: "1") {
        title
    }
}
```

will send a `GET` request to `https://jsonplaceholder.typicode.com/todos/1` and return the title.
