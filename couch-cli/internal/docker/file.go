package docker

import "io/ioutil"

func WriteDockerfile(serviceName string) error {
	template := GetDockerfileTemplateForService(serviceName)

	bytes := []byte(template)

	err := ioutil.WriteFile("Dockerfile", bytes, 0644)
	if err != nil {
		return err
	}

	return nil
}
