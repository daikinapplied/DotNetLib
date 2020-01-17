# Introduction
<img src="Images/debugging.png" width="64" align="right" alt="Daikin.DotNetLib.Core.Tests Logo"/>
This project handles unit tests, which also serves as samples to access APIs in the libraries (other projects).

# Getting Started
This uses .NET Core to run the Unit Tests.

# Requirements
This supports the use of appsettings.json as well as [Safe Storage](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows) using App Secrets via `%APPDATA%\Microsoft\UserSecrets\7c94ff42-b17b-4a22-89ff-38d531f03402\secrets.json`.  This file has the same format as the included appsettings.json file, with the appropriate value information filled in for each data item.

This GUID (used in the path to the secrets.json file) is referenced in the Daikin.DotNetLib.Core.Tests.csproj file and in the building of Configuration.

# Documentation Overview
Check out the primary [README.md](../README.md) documentation.

~ end ~