package docker

import (
	"bufio"
	"couch-cli/internal/osutil"
	"gopkg.in/yaml.v3"
	"io/fs"
	"io/ioutil"
	"log"
	"os/exec"
	"path/filepath"
	"strings"
)

type Compose struct {
	Services map[string]ComposeService `yaml:"services"`
	Networks ComposeNetworks           `yaml:"networks"`
}

type ComposeService struct {
	ContainerName string       `yaml:"container_name"`
	Build         ComposeBuild `yaml:"build"`
	Ports         []string     `yaml:"ports"`
	EnvFile       []string     `yaml:"env_file"`
}

type ComposeBuild struct {
	Context    string `yaml:"context"`
	DockerFile string `yaml:"dockerfile"`
}

type ComposeNetworks struct {
	Default ComposeDefaultNetwork `yaml:"default"`
}

type ComposeDefaultNetwork struct {
	Name     string `yaml:"name"`
	External bool   `yaml:"external"`
}

func NewComposeService(serviceName string, composeService ComposeService, networks ComposeNetworks) *Compose {
	services := make(map[string]ComposeService)
	services[strings.ToLower(serviceName)] = composeService
	return &Compose{services, networks}
}

func WriteComposeFile(serviceName string) error {
	service := ComposeService{
		ContainerName: strings.ToLower(serviceName) + "-service",
		Ports:         []string{"8080:80"},
		EnvFile:       []string{"./.env"},
		Build: ComposeBuild{
			Context:    ".",
			DockerFile: "./Dockerfile",
		},
	}

	networks := ComposeNetworks{
		Default: ComposeDefaultNetwork{
			Name:     "couch-potatoes-network",
			External: true,
		},
	}

	compose := NewComposeService(serviceName, service, networks)

	data, err := yaml.Marshal(&compose)
	if err != nil {
		return err
	}

	err = ioutil.WriteFile("docker-compose.yaml", data, 0644)
	if err != nil {
		return err
	}

	return nil
}

func StartAllDockerComposes(detached bool, build bool) error {
	log.Println("Searching for docker-compose.yaml and docker-compose.yml files...")
	composeFilePaths, err := GetAllDockerComposeFilesInProject()
	if err != nil {
		return nil
	}

	log.Printf("Found %v docker compose files\n", len(composeFilePaths))

	for _, composeFile := range composeFilePaths {
		log.Printf("Starting %v\n", composeFile)
		err2 := ComposeUp(composeFile, detached, build)
		if err2 != nil {
			return nil
		}
	}

	return nil
}

func ShutdownAllDockerComposes() error {
	composeFilePaths, err := GetAllDockerComposeFilesInProject()
	if err != nil {
		return err
	}
	log.Printf("Shutting down %v configurations\n", len(composeFilePaths))

	for _, composeFile := range composeFilePaths {
		err2 := ComposeDown(composeFile)
		if err2 != nil {
			return err2
		}
	}

	return nil
}

func GetAllDockerComposeFilesInProject() ([]string, error) {
	composeFilePaths := make([]string, 0)
	projectRootDir, err := osutil.GetProjectRootDir()
	if err != nil {
		return composeFilePaths, err
	}

	err = filepath.Walk(projectRootDir, func(path string, info fs.FileInfo, err error) error {
		if info.IsDir() && info.Name() == "node_modules" {
			return filepath.SkipDir
		}

		if info.Name() == "docker-compose.yaml" || info.Name() == "docker-compose.yml" {
			composeFilePaths = append(composeFilePaths, path)
		}
		return nil
	})

	return composeFilePaths, nil
}

func ComposeUp(composeFile string, detached bool, build bool) error {
	args := []string{"compose", "-f", composeFile, "up"}

	if detached {
		args = append(args, "-d")
	}

	if build {
		args = append(args, "--build")
	}

	cmd := exec.Command("docker", args...)
	stderr, err := cmd.StderrPipe()
	if err != nil {
		return err
	}

	err = cmd.Start()
	if err != nil {
		return err
	}

	scanner := bufio.NewScanner(stderr)
	scanner.Split(bufio.ScanLines)
	for scanner.Scan() {
		msg := scanner.Text()
		log.Println(msg)
	}

	err = cmd.Wait()
	if err != nil {
		return err
	}

	return nil
}

func ComposeDown(composeFile string) error {

	cmd := exec.Command("docker", "compose", "-f", composeFile, "down")
	stderr, err := cmd.StderrPipe()
	if err != nil {
		return err
	}

	err = cmd.Start()
	if err != nil {
		return err
	}

	scanner := bufio.NewScanner(stderr)
	scanner.Split(bufio.ScanWords)
	for scanner.Scan() {
		msg := scanner.Text()
		log.Println(msg)
	}

	err = cmd.Wait()
	if err != nil {
		return err
	}

	return nil
}

func CreateDockerNetwork(networkName string, driver string) error {
	cmd := exec.Command("docker", "network", "create", networkName, "--driver", driver)

	stderr, err := cmd.StderrPipe()
	if err != nil {
		return err
	}

	err = cmd.Start()
	if err != nil {
		return err
	}

	scanner := bufio.NewScanner(stderr)
	scanner.Split(bufio.ScanLines)
	for scanner.Scan() {
		msg := scanner.Text()
		log.Println(msg)
	}

	err = cmd.Wait()
	if err != nil {
		return err
	}

	return nil
}
