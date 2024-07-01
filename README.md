# Harri.SchoolDemoAPI.BlazorWASM

Admin UI Demo for managing CRUD operations on students, schools, and student's applications to schools.

Written in Blazor as a WASM standalone app using MudBlazor with a heavy focus on automation tests. 

The backend .NET 8 REST API is a separate project here: [Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)

## WIP
So far the students pages are complete with in-depth tests covering the implemented functionality

# Dependencies
- MudBlazor
- [Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)

# Running the app
To point to a local Harri.SchoolDemoAPI instance:

Set `StudentDemoAPIUrl` in [appsettings.json](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI.BlazorWASM/blob/main/src/Harri.SchoolDemoAPI.BlazorWASM/wwwroot/appsettings.json)

 By default `StudentDemoAPIUrl` points to http://localhost:8080 

To set up and run the REST API and SQL DB locally see the [running from container instructions](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI#running-from-container)

# Test Dependencies
- NUnit
- FluentAssertions

## Unit
- bUnit
- Moq

## UI E2E
- Playwright
- Specflow 3.9
- CucumberExpressions.SpecFlow.3-9
- SpecFlow.Actions.Playwright

See the UI Test README [here](src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E/README.md)
