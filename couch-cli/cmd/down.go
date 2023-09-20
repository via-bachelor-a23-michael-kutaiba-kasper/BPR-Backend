/*
Copyright © 2023 Michael Bui 293886@via.dk
*/
package cmd

import (
	"couch-cli/internal/docker"
	"github.com/spf13/cobra"
	"log"
)

// downCmd represents the down command
var downCmd = &cobra.Command{
	Use:   "down",
	Short: "Shuts down all containers started by docker-compose",
	Long: `Shuts down all containers spun up based off the docker-compose.yaml 
and docker-compose.yml templates in the project`,
	RunE: func(cmd *cobra.Command, args []string) error {
		err := docker.ShutdownAllDockerComposes()
		if err != nil {
			log.Fatal(err)
		}

		return nil
	},
}

func init() {
	composeCmd.AddCommand(downCmd)

	// Here you will define your flags and configuration settings.

	// Cobra supports Persistent Flags which will work for this command
	// and all subcommands, e.g.:
	// downCmd.PersistentFlags().String("foo", "", "A help for foo")

	// Cobra supports local flags which will only run when this command
	// is called directly, e.g.:
	// downCmd.Flags().BoolP("toggle", "t", false, "Help message for toggle")
}
