package config

import (
	"os"

	"gopkg.in/yaml.v3"
)

func LoadSchema(path string) (GraphifySchema, error) {
	bytes, err := os.ReadFile(path)
	if err != nil {
		return GraphifySchema{}, err
	}

	schema := GraphifySchema{}
	err = yaml.Unmarshal(bytes, &schema)
	if err != nil {
		return GraphifySchema{}, err
	}

	return schema, nil
}
