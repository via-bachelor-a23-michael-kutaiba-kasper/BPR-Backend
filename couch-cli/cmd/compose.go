/*
Copyright © 2023 Michael Bui 293886@via.dk
*/
package cmd

import (
	"github.com/spf13/cobra"
)

// composeCmd represents the compose command
var composeCmd = &cobra.Command{
	Use:   "compose",
	Short: "Execute docker compose <COMMAND> on all docker compose files",
	Long: `Searches for all docker-compose.yaml & docker-compose.yml in the project
and runs the command on all the found files`,
}

func init() {
	rootCmd.AddCommand(composeCmd)

	// Here you will define your flags and configuration settings.

	// Cobra supports Persistent Flags which will work for this command
	// and all subcommands, e.g.:
	// composeCmd.PersistentFlags().String("foo", "", "A help for foo")

	// Cobra supports local flags which will only run when this command
	// is called directly, e.g.:
	// composeCmd.Flags().BoolP("toggle", "t", false, "Help message for toggle")
}
