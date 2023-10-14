package osutil

import (
	"errors"
	"fmt"
	"os"
	"path/filepath"
	"strings"
)

func ChangeDirectoryRelative(relativePath string) error {
	path, err := os.Getwd()
	if err != nil {
		return err
	}

	if err = os.Chdir(filepath.Join(path, relativePath)); err != nil {
		return err
	}

	return nil
}

func GetProjectRootDir() (string, error) {
	projectRootDirName := "BPR-Backend"

	cwd, err := os.Getwd()
	if err != nil {
		return "", err
	}

	pathParts := strings.Split(filepath.ToSlash(cwd), "/")

	for idx, part := range pathParts {
		if part == projectRootDirName {
			return strings.Join(pathParts[0:idx+1], string(os.PathSeparator)), nil
		}
	}

	return "", errors.New(fmt.Sprintf("Failed to locate %v", projectRootDirName))
}
