# LWX Framework

This project is an experiment using the dotnet source code generator (plugin) to make cleaner projects with the following capabilities:

* Syntax Rules
  - classes in dir/files with the same name;
  - reserved namespace for endpoints
  - reserved namespace for authentication schemes
  - reserved namespace for authentication policies
  - block the old style controllers
* Code Generation
  - create extension functions on WebApplication to map endpoints
  - create extension functions on WebApplicationBuilder to map authentication schemes
  - create extension functions on WebApplicationBuilder to map authentication policies
* Utilities  
  - Activity Log Middleware
  - Development Configuration Loader
  - Health Check Fix
  - Swagger Gen (using Swashbuckle)
* Binnary compilation using the PublishReadyToRun mechanism

