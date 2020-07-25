# GitHub API Playground

## Summary

This is a code playground to communicate with the GitHub API using a .NET Core 3.x console application. The chosen IDE was Visual Studio Code and the package management was done with the .NET Core CLI.

You can learn how to make some basic requests to the GitHub API using a personal token, including how to work with response pagination. Also, this project uses the following .NET Core concepts: Services dependency injection, Options pattern and the Secret Manager.

## How to Run the Application

1. You must configure the Secret Manager tool to securely store your GitHub credentials locally, as explained [here](https://docs.microsoft.com/pt-br/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux#how-the-secret-manager-tool-works). The JSON file should specify the following keys: `GitHubSettings:Username` and `GitHubSettings:Token`;
1. Edit the `Program.cs` file to write your desired logic, using the helper methods available in the `GitHubApiClient` class;
1. Run the .NET Core application.