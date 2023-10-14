package osutil

import (
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

	for {
		path, err := os.Getwd()
		if err != nil {
			return "", err
		}

		pathParts := strings.Split(filepath.ToSlash(path), "/")
		if pathParts[len(pathParts)-1] == projectRootDirName {
			return path, nil
		}

		if err = ChangeDirectoryRelative(".."); err != nil {
			return "", err
		}
	}
}
