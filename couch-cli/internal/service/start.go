package service

import (
	"couch-cli/internal/dotnet"
	"couch-cli/internal/osutil"
	"fmt"
	"log"
	"os"
)

func StartService(name string) error {
	projectRootDir, err := osutil.GetProjectRootDir()
	if err != nil {
		return err
	}

	err = os.Chdir(projectRootDir)
	if err != nil {
		return err
	}

	err = osutil.ChangeDirectoryRelative(fmt.Sprintf("./src/Services/%v", name))
	if err != nil {
		return nil
	}

	currentDir, err := os.Getwd()
	log.Println(currentDir)
	if err != nil {
		return nil
	}

	maybeEnv, err := osutil.FindDotEnvFile(currentDir)

	if maybeEnv.HasEnvFile {
		err = osutil.ApplyDotenv(maybeEnv.EnvFilePath)
		if err != nil {
			log.Println("HERE")
			return err
		}
	}

	err = osutil.ChangeDirectoryRelative(fmt.Sprintf("./%v.API", name))
	if err != nil {
		return err
	}

	err = dotnet.Run()
	if err != nil {
		return err
	}

	return nil
}
