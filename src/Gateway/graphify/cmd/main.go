package main

import (
	"fmt"
	"log"
	"net/http"

	"github.com/graphql-go/graphql"
	"github.com/graphql-go/handler"
	"github.com/michaelbui99/graphify/internal/config"
	internal_graphql "github.com/michaelbui99/graphify/internal/graphql"
)

func main() {
	graphifySchema, err := config.LoadSchema("/home/mibui/Dev/projects/Graphify/graphify.yaml")
	if err != nil {
		log.Fatal(err)
	}

	rootQuery, err := internal_graphql.GenerateRootQuery(graphifySchema)
	if err != nil {
		log.Fatal(err)
	}

	qlSchema, _ := graphql.NewSchema(graphql.SchemaConfig{
		Query: rootQuery,
	})

	h := handler.New(&handler.Config{
		Schema:   &qlSchema,
		Pretty:   true,
		GraphiQL: true,
	})

	http.Handle("/graphql", h)
	log.Printf("Server listening on port: %s", graphifySchema.Server.Port)
	log.Printf("GraphQL playground served at http://<IP>:%s/graphql", graphifySchema.Server.Port)
	http.ListenAndServe(fmt.Sprintf(":%s", graphifySchema.Server.Port), nil)
}
