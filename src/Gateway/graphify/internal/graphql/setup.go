package graphql

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"strings"

	"github.com/graphql-go/graphql"
	"github.com/michaelbui99/graphify/internal/config"
)

func GenerateRootQuery(graphifySchema config.GraphifySchema) (*graphql.Object, error) {
	querySpecs := graphifySchema.QuerySpecs
	fieldsConfig := graphql.Fields{}

	rootQuery := graphql.NewObject(graphql.ObjectConfig{
		Name:   "RootQuery",
		Fields: fieldsConfig,
	})

	typeMap, err := generateTypes(graphifySchema)
	if err != nil {
		return &graphql.Object{}, err
	}

	for query := range graphifySchema.QuerySpecs {
		delcaredType := querySpecs[query].Type
		var resolvedType *graphql.Object

		delcaredTypeParts := strings.Split(delcaredType, "::")
		if len(delcaredTypeParts) == 2 {
			resolvedType, err = typeMap[delcaredTypeParts[1]], nil
		} else {
			return &graphql.Object{}, fmt.Errorf("failed to parse type %s for query %s", delcaredType, query)
		}

		argsConfig := graphql.FieldConfigArgument{}
		queryArguments := make(map[string]config.Arg)
		for arg := range querySpecs[query].Args {
			if querySpecs[query].Args[arg].ForRequestBody {
				queryArguments[arg] = querySpecs[query].Args[arg]
				continue
			}

			argType, err := parseNativeType(querySpecs[query].Args[arg].Type)
			if err != nil {
				return &graphql.Object{}, err
			}

			argsConfig[arg] = &graphql.ArgumentConfig{
				Type: argType,
			}
		}

		field := &graphql.Field{
			Type:        resolvedType,
			Description: querySpecs[query].Description,
			Args:        argsConfig,
			Resolve: func(params graphql.ResolveParams) (interface{}, error) {
				httpMethod := querySpecs[query].DataSource.Method
				var res *http.Response
				var err error
				switch httpMethod {
				case "GET":
					url, err := expandUrlArguments(querySpecs[query].DataSource.Url, params)
					log.Printf("Sending GET request to %s\n", url)
					if err != nil {
						log.Printf("%v", err.Error())
						return nil, err
					}
					res, err = http.Get(url)
					if err != nil {
						log.Printf("%v", err.Error())
						return nil, err
					}
					break

				case "POST":
					// TODO: Implement this when mutations are supported.
					break
				}

				if err != nil {
					return struct{}{}, err
				}

				body, err := io.ReadAll(res.Body)
				defer res.Body.Close()
				if err != nil {
					return struct{}{}, err
				}

				raw := make(map[string]interface{})
				json.Unmarshal(body, &raw)

				return raw, nil
			},
		}

		fieldsConfig[query] = field
	}

	return rootQuery, nil
}

func generateTypes(graphifySchema config.GraphifySchema) (map[string]*graphql.Object, error) {
	typeMap := make(map[string]*graphql.Object)

	for typeDelcaration := range graphifySchema.TypeDeclarations {
		createdTypeObject, err := generateType(graphifySchema, graphifySchema.TypeDeclarations[typeDelcaration])
		if err != nil {
			return make(map[string]*graphql.Object), err
		}
		typeMap[typeDelcaration] = createdTypeObject
	}

	return typeMap, nil
}

func generateType(graphifySchema config.GraphifySchema, typeDelcaration config.TypeDeclaration) (*graphql.Object, error) {
	fields := graphql.Fields{}
	generatedTypes := make(map[string]*graphql.Output)
	for field := range typeDelcaration.Fields {
		delcaredType := typeDelcaration.Fields[field].Type
		description := typeDelcaration.Fields[field].Description

		var resolvedType graphql.Output
		var err error

		if isCustomType(delcaredType) {
			typeName, err := getCustomTypeName(delcaredType)
			if err != nil {
				return &graphql.Object{}, err
			}

			if existingType, ok := generatedTypes[typeName]; ok {
				resolvedType = *existingType
			} else {
				resolvedType, err = generateType(graphifySchema, graphifySchema.TypeDeclarations[typeName])
				if err != nil {
					return &graphql.Object{}, err
				}

				generatedTypes[typeName] = &resolvedType
			}
		} else {
			resolvedType, err = parseNativeType(delcaredType)
			if err != nil {
				return &graphql.Object{}, err
			}
		}

		fields[field] = &graphql.Field{
			Type:        resolvedType,
			Description: description,
		}
	}

	objConfig := graphql.ObjectConfig{
		Name:        typeDelcaration.Name,
		Description: typeDelcaration.Description,
		Fields:      fields,
	}

	return graphql.NewObject(objConfig), nil
}

func parseNativeType(delcaredType string) (graphql.Output, error) {
	var resolvedType graphql.Output
	if delcaredType == "string" {
		resolvedType = graphql.String
	} else if delcaredType == "boolean" {
		resolvedType = graphql.Boolean
	} else if delcaredType == "float" {
		resolvedType = graphql.Float
	} else if delcaredType == "int" {
		resolvedType = graphql.Int
	} else if delcaredType == "datetime" {
		resolvedType = graphql.DateTime
	} else {
		return &graphql.Object{}, fmt.Errorf("failed to parse type %s", delcaredType)
	}

	return resolvedType, nil
}

func isCustomType(declaredType string) bool {
	parts := strings.Split(declaredType, "::")
	if len(parts) == 1 {
		return false
	}

	if len(parts) == 2 {
		return parts[0] == "typeDeclarations"
	}

	return false
}

func getCustomTypeName(declaredType string) (string, error) {
	if !isCustomType(declaredType) {
		return "", fmt.Errorf("declared type %s is not custom", declaredType)
	}

	return strings.Split(declaredType, "::")[1], nil
}

func expandUrlArguments(url string, params graphql.ResolveParams) (string, error) {
	urlArgs := make([]string, 0)

	parts := strings.Split(url, "/")
	for _, part := range parts {
		log.Println(part)
		if len(strings.TrimSpace(part)) == 0 {
			continue
		}
		if string(part[0]) == "{" && string(part[len(part)-1]) == "}" {
			urlArgs = append(urlArgs, part)
		}
	}

	res := url
	for _, arg := range urlArgs {
		if resolvedArg, ok := params.Args[arg[1:len(arg)-1]]; !ok {
			return "", fmt.Errorf("missing arg %s", arg)
		} else {
			res = strings.ReplaceAll(res, arg, string(resolvedArg.(string)))
		}
	}

	return res, nil
}
