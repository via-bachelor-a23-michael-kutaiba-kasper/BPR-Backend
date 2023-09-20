package dotnet

import (
	"fmt"
	"os/exec"
)

func CreateSolution(serviceName string) (string, error) {
	cmd := exec.Command("dotnet", "new", "sln", "--name", serviceName)
	out, err := cmd.Output()
	if err != nil {
		return "", err
	}

	return string(out), nil
}

func CreateDotNetProject(template string, name string) (string, error) {
	cmd := exec.Command("dotnet", "new", template, "-o", name)

	output, err := cmd.Output()
	if err != nil {
		return "", err
	}

	return string(output), nil
}

func AddProjectToSolution(slnFile string, csProjFile string) (string, error) {
	cmd := exec.Command("dotnet", "sln", slnFile, "add", csProjFile)
	output, err := cmd.Output()
	if err != nil {
		return "", nil
	}

	return string(output), nil
}

func AddNugetPackageToProject(csprojFile string, packageName string) (string, error) {
	cmd := exec.Command("dotnet", "add", csprojFile, "package", packageName)
	output, err := cmd.Output()
	if err != nil {
		return "", nil
	}

	return string(output), nil
}

func Run() error {
	cmd := exec.Command("dotnet", "run")

	stdout, err := cmd.StdoutPipe()
	if err != nil {
		return err
	}

	err = cmd.Start()
	if err != nil {
		return err
	}

	for {
		tmp := make([]byte, 1024)
		_, err := stdout.Read(tmp)
		fmt.Print(string(tmp))
		if err != nil {
			break
		}
	}

	err = cmd.Wait()
	if err != nil {
		return err
	}

	return nil
}
