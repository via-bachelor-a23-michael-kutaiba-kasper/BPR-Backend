/*
Copyright © 2023 Michael Bui 293886@via.dk
*/

package cmd

import (
	"couch-cli/internal/service"

	"github.com/spf13/cobra"
)

var withDbUp bool

var (
	// serviceCmd represents the service command
	serviceCmd = &cobra.Command{
		Use:   "service",
		Short: "Creates a new .NET microservice based on the defined backend architecture for Couch Potatoes",
		Long: `Creates a new .NET microservice based on the defined backend architecture for Couch Potatoes.
This includes:
  - Creating new solution
  - Setting up Api, Application, Domain and Infrastructure projects.
  - Resolving project references
  - Adding MediatR and Automapper 
`,
		Args: cobra.ExactArgs(1),
		RunE: func(cmd *cobra.Command, args []string) error {
			serviceName := args[0]
			err := service.CreatNewService(serviceName, withDbUp)
			if err != nil {
				return err
			}

			return nil
		},
	}
)

func init() {
	newCmd.AddCommand(serviceCmd)

	// Here you will define your flags and configuration settings.

	// Cobra supports Persistent Flags which will work for this command
	// and all subcommands, e.g.:
	// serviceCmd.PersistentFlags().String("foo", "", "A help for foo")

	// Cobra supports local flags which will only run when this command
	// is called directly, e.g.:
	// serviceCmd.Flags().BoolP("toggle", "t", false, "Help message for toggle")
	serviceCmd.Flags().BoolVar(&withDbUp, "with-dbup", false, "Include DbUp project. Usage: couch-cli new service --with-dbup MyService")
}
