/*
Copyright © 2023 Michael Bui 293886@via.dk
*/

package cmd

import (
	"couch-cli/internal/docker"
	"github.com/spf13/cobra"
	"log"
)

var detached bool
var build bool

// upCmd represents the up command
var upCmd = &cobra.Command{
	Use:   "up",
	Short: "A brief description of your command",
	Long: `A longer description that spans multiple lines and likely contains examples
and usage of using your command. For example:

Cobra is a CLI library for Go that empowers applications.
This application is a tool to generate the needed files
to quickly create a Cobra application.`,
	RunE: func(cmd *cobra.Command, args []string) error {
		err := docker.CreateDockerNetwork("couch-potatoes-network", "bridge")
		if err != nil {
			// NOTE: (mibui 2023-05-20) This will throw an error if the network already exists
			//                           However, We don't want to log.Fatal here, since we don't care
			//                           if the network already exists, we just want to ensure that it actually
			//                           exists.
			log.Println(err)
		}

		err = docker.StartAllDockerComposes(detached, build)
		if err != nil {
			log.Fatal(err)
		}

		return nil
	},
}

func init() {
	composeCmd.AddCommand(upCmd)

	// Here you will define your flags and configuration settings.

	// Cobra supports Persistent Flags which will work for this command
	// and all subcommands, e.g.:
	// upCmd.PersistentFlags().String("foo", "", "A help for foo")

	// Cobra supports local flags which will only run when this command
	// is called directly, e.g.:
	// upCmd.Flags().BoolP("toggle", "t", false, "Help message for toggle")
	upCmd.Flags().BoolVarP(&detached, "detached", "d", false, "Run docker composes in detached mode?")
	upCmd.Flags().BoolVarP(&build, "build", "b", false, "Build images for all docker composes?")
}
