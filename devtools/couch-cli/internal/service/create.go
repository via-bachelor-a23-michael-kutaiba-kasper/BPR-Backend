package service

import (
	"couch-cli/internal/docker"
	"couch-cli/internal/dotnet"
	"couch-cli/internal/osutil"
	"fmt"
	"io/fs"
	"log"
	"os"
	"path"
	"runtime"
)

func CreatNewService(name string, withDbUp bool) error {
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

	err = os.Mkdir(name, fs.FileMode(getPermissionCode()))
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
	if withDbUp {
		projectsToCreate = append(projectsToCreate, "DbUp")
	}

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

	for _, project := range projectsToCreate {
		csProjPath := fmt.Sprintf("%v/%v.%v/%v.%v.csproj", currentDir, name, project, name, project)
		err := addReferencesForProject(project, csProjPath, name)
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
		break
	case "Application":
		template = "classlib"
		break
	case "Domain":
		template = "classlib"
		break
	case "Infrastructure":
		template = "console"
		break
	case "DbUp":
		template = "console"
		break
	}

	return template
}

func addReferencesForProject(project string, csprojPath string, serviceName string) error {
	message := "Adding references for %v project..."
	getCsProjForProject := func(project string, serviceName string) (string, error) {
		projectRootDir, err := osutil.GetProjectRootDir()
		if err != nil {
			return "", err
		}

		serviceRootDirPath := path.Join(projectRootDir, "src", "Services", serviceName)
		toBeAddedCsProj := path.Join(serviceRootDirPath, fmt.Sprintf("%v.%v", serviceName, project), fmt.Sprintf("%v.%v.csproj", serviceName, project))
		return toBeAddedCsProj, nil
	}

	switch project {
	case "API":
		log.Printf(message, project)
		projectsToAdd := []string{"Application", "DbUp", "Domain", "Infrastructure"}
		for _, p := range projectsToAdd {
			toCsProjPath, err := getCsProjForProject(p, serviceName)
			if err != nil {
				return err
			}
			output, err := dotnet.AddProjectReference(csprojPath, toCsProjPath)
			if err != nil {
				return err
			}
			log.Println(output)
		}
		break

	case "Application":
		log.Printf(message, project)
		projectsToAdd := []string{"Domain", "Infrastructure"}
		for _, p := range projectsToAdd {
			toCsProjPath, err := getCsProjForProject(p, serviceName)
			if err != nil {
				return err
			}
			output, err := dotnet.AddProjectReference(csprojPath, toCsProjPath)
			if err != nil {
				return err
			}
			log.Println(output)
		}
		break

	case "Infrastructure":
		log.Printf(message, project)
		projectsToAdd := []string{"Domain"}
		for _, p := range projectsToAdd {
			toCsProjPath, err := getCsProjForProject(p, serviceName)
			if err != nil {
				return err
			}
			output, err := dotnet.AddProjectReference(csprojPath, toCsProjPath)
			if err != nil {
				return err
			}
			log.Println(output)
		}
		break
	}
	return nil
}

func addDependenciesForProject(project string, csprojPath string) error {
	installMessage := "Installing dependencies for %v project..."

	switch project {
	case "API":
		log.Printf(installMessage, project)
		dependencies := []string{"MediatR", "AutoMapper"}
		for _, dependency := range dependencies {
			output, err := dotnet.AddNugetPackageToProject(csprojPath, dependency)
			if err != nil {
				return err
			}
			log.Println(output)
		}
		break

	case "Application":
		log.Printf(installMessage, project)
		output, err := dotnet.AddNugetPackageToProject(csprojPath, "MediatR")
		if err != nil {
			return err
		}
		log.Println(output)
		break

	case "Infrastructure":
		log.Printf(installMessage, project)
		dependencies := []string{"AutoMapper", "Google.Cloud.PubSub.V1"}
		for _, dependency := range dependencies {
			output, err := dotnet.AddNugetPackageToProject(csprojPath, dependency)
			if err != nil {
				return err
			}
			log.Println(output)
		}
		break

	case "DbUp":
		log.Printf(installMessage, project)
		dependencies := []string{"Dapper", "Npgsql", "dbup", "dbup-postgresql"}
		for _, dependency := range dependencies {
			output, err := dotnet.AddNugetPackageToProject(csprojPath, dependency)
			if err != nil {
				return err
			}
			log.Println(output)

		}
		break
	}

	return nil
}

func getPermissionCode() int {
	if runtime.GOOS == "darwin" {
		return 0777
	}

	return 0644
}
