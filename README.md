# OWIN Microservice

A library containing a small set of classes for adding actuator style endpoints to either an ASP.NET 5 application or an OWIN application hosted under either IIS or Self-hosted

Code is licensed under the ISC license to use as you wish.

## Installation

Run the following from the package manager console to install the package

### ASP.NET 5

```
PM> Install-Package Dotnet.Microservice.Dnx
```

### For OWIN

```
PM> Install-Package Dotnet.Microservice.Owin
```

## Documentation

Documentation is available on the wiki pages and there are working samples in the src/samples folder

## Sample

This repo contains two sample projects one for running in DNX / ASP.NET 5 and the other for running in an Owin self-host.

Both sample apps listen on localhost:5000 so after starting either sample app you can browse the following endpoints:

- [http://localhost:5000/info](http://localhost:5000/info)
- [http://localhost:5000/health](http://localhost:5000/health)
- [http://localhost:5000/env](http://localhost:5000/env)

To run the DNX / ASP.NET 5 sample you need to install ASP.NET 5 as documented [here](https://docs.asp.net/en/latest/getting-started/index.html)

NOTE: At the moment you'll also need DNX to run the OWIN sample as this had to be a DNX Console Application project since you cannot yet refer
to xproj projects from csproj projects. A regular csproj project will work perfectly fine if you are pulling the library in via Nuget.