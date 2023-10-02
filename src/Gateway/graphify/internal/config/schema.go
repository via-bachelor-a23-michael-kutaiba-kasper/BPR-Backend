package config

type GraphifySchema struct {
	SchemaVersion    string                     `yaml:"schemaVersion"`
	TypeDeclarations map[string]TypeDeclaration `yaml:"typeDeclarations"`
	QuerySpecs       map[string]QuerySpec       `yaml:"querySpecs"`
	Server           ServerConfig               `yaml:"server"`
}

type TypeDeclaration struct {
	Name        string           `yaml:"name"`
	Description string           `yaml:"description"`
	Fields      map[string]Field `yaml:"fields"`
}

type Field struct {
	Type        string `yaml:"type"`
	Description string `yaml:"description"`
}

type QuerySpec struct {
	Type        string         `yaml:"type"`
	Description string         `yaml:"description"`
	Args        map[string]Arg `yaml:"args"`
	DataSource  DataSource     `yaml:"dataSource"`
}

type Arg struct {
	Type           string `yaml:"type"`
	ForRequestBody bool   `yaml:"forRequestBody"`
}

type DataSource struct {
	Url    string `yaml:"url"`
	Method string `yaml:"method"`
}

type ServerConfig struct {
	Port string `yaml:"port"`
}
