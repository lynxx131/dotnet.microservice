# Dotnet Microservice

A library containing a small set of classes for adding actuator style endpoints to either an ASP.NET Core 1.0 application or an OWIN application hosted under either IIS or Self-hosted

Code is licensed under the ISC license to use as you wish.

## Installation

Run the following from the package manager console to install the package

NOTE: The pre-release (-pre) flag is currently required for Non-DNX projects as the core Dotnet.Microservice package pulls in Microsoft.Extensions.Configuration which is 
still an RC so without the pre-release flag you will get a dependency resolution failure for Microsoft.Extensions.Configuration.

### ASP.NET Core 1.0

```
PM> Install-Package Dotnet.Microservice.Dnx
```

### For OWIN

```
PM> Install-Package Dotnet.Microservice.Owin -pre
```

## Documentation

Documentation is available on the wiki pages and there are working samples in the src/samples folder

## Sample

This repo contains two sample projects one for running on ASP.NET Core 1.0 and the other for running in an Owin self-host.

Both sample apps listen on localhost:5000 so after starting either sample app you can browse the following endpoints:

- [http://localhost:5000/info](http://localhost:5000/info)
- [http://localhost:5000/health](http://localhost:5000/health)
- [http://localhost:5000/env](http://localhost:5000/env)

To run the ASP.NET Core 1.0 sample you need to install ASP.NET Core as documented [here](https://docs.asp.net/en/latest/getting-started/index.html)

NOTE: At the moment you'll also need ASP.NET Core to run the OWIN sample as this had to be a ASP.NET Core Console Application project since you cannot yet refer
to xproj projects from csproj projects. A regular csproj project will work perfectly fine if you are pulling the library in via Nuget.