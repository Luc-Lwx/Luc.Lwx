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
  - Cors configuration
  - Fake jwt issuer for testing
* Binnary compilation using the PublishReadyToRun mechanism

## Installation

To use this library, you need to add the following NuGet packages to your project:

```sh
dotnet add package Luc.Lwx
dotnet add package Luc.Lwx.Generator
```

## Usage

Ensure that you have the source generator and utility library referenced in your project file:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Luc.Lwx" Version="1.0.0" />
    <PackageReference Include="Luc.Lwx.Generator" Version="1.0.0" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
</Project>
```

After adding the packages, you can start using the functionalities provided by the LWX Framework in your project.

