package service

import (
	"couch-cli/internal/docker"
	"couch-cli/internal/dotnet"
	"couch-cli/internal/osutil"
	"fmt"
	"log"
	"os"
)

func CreatNewService(name string) error {
	log.Printf("Service to create: %v\n", name)
	if err := changeToServicesDirectory(); err != nil {
		return err
	}

	cwd, err := os.Getwd()
	if err != nil {
		return err
	}

	if _, err := os.Stat(fmt.Sprintf("%v/%v", cwd, name)); !os.IsNotExist(err) {
		log.Fatal(fmt.Sprintf("Existing service with name %v detected. Please use another name or delete existing service\n", name))
	}

	err = os.Mkdir(name, 0644)
	if err != nil {
		return err
	}

	err = osutil.ChangeDirectoryRelative(fmt.Sprintf("./%v", name))
	if err != nil {
		return err
	}

	log.Println("Creating new solution...")
	solutionOutput, err := dotnet.CreateSolution(name)
	if err != nil {
		return err
	}
	log.Println(solutionOutput)
	solutionFile := fmt.Sprintf("%v.sln", name)

	currentDir, err := os.Getwd()
	if err != nil {
		return err
	}
	solutionFilePath := fmt.Sprintf("%v/%v", currentDir, solutionFile)

	projectsToCreate := []string{"API", "Application", "Domain", "Infrastructure"}

	for _, project := range projectsToCreate {
		template := getTemplateForProject(project)

		log.Printf("Creating new %v project...\n", project)
		projectOutput, err := dotnet.CreateDotNetProject(template, getProjectName(name, project))
		if err != nil {
			return err
		}
		log.Println(projectOutput)

		currentDir, err = os.Getwd()
		if err != nil {
			return err
		}

		csProjPath := fmt.Sprintf("%v/%v.%v/%v.%v.csproj", currentDir, name, project, name, project)
		log.Printf("Adding project %v.%v to solution...", name, project)
		solutionOutput, err := dotnet.AddProjectToSolution(solutionFilePath, csProjPath)
		println(solutionOutput)

		err = addDependenciesForProject(project, csProjPath)
		if err != nil {
			return err
		}
	}

	log.Println("Creating docker-compose.yaml...")
	err = docker.WriteComposeFile(name)
	if err != nil {
		return err
	}

	log.Println("Creating Dockerfile...")
	err = docker.WriteDockerfile(name)
	if err != nil {
		return err
	}

	log.Println("Creating .env file...")
	err = osutil.WriteDotEnvFile(currentDir)
	if err != nil {
		return err
	}

	log.Println("New service has been created!")
	return nil
}

func getProjectName(serviceName string, extension string) string {
	return fmt.Sprintf("%v.%v", serviceName, extension)
}

func changeToServicesDirectory() error {

	projectRoot, err := osutil.GetProjectRootDir()
	if err != nil {
		return err
	}

	if err = os.Chdir(projectRoot); err != nil {
		return err
	}

	err = osutil.ChangeDirectoryRelative("./src/Services/")
	if err != nil {
		return err
	}

	return nil
}

func getTemplateForProject(project string) string {
	var template string
	switch project {
	case "API":
		template = "webapi"
	case "Application":
		template = "classlib"
	case "Domain":
		template = "classlib"
	case "Infrastructure":
		template = "console"
	}

	return template
}

func addDependenciesForProject(project string, csprojPath string) error {
	installMessage := "Installing dependencies for %v project..."

	switch project {
	case "API":
		log.Printf(installMessage, project)
		output, err := dotnet.AddNugetPackageToProject(csprojPath, "MediatR")
		if err != nil {
			return err
		}
		log.Println(output)

		output, err = dotnet.AddNugetPackageToProject(csprojPath, "AutoMapper")
		if err != nil {
			return err
		}
		log.Println(output)

	case "Application":
		log.Printf(installMessage, project)
		output, err := dotnet.AddNugetPackageToProject(csprojPath, "MediatR")
		if err != nil {
			return err
		}
		log.Println(output)

	case "Infrastructure":
		log.Printf(installMessage, project)
		output, err := dotnet.AddNugetPackageToProject(csprojPath, "AutoMapper")
		if err != nil {
			return err
		}
		log.Println(output)
	}

	return nil
}
