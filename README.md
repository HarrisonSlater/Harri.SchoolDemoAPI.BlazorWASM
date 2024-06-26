# Harri.SchoolDemoAPI.BlazorWASM
Blazor WASM Standalone Admin UI for .NET 8 REST API [Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)


# Dependencies
- MudBlazor
- https://github.com/HarrisonSlater/Harri.SchoolDemoAPI

Running the app
Pointing to local Harri.SchoolDemoAPI instance

Set StudentDemoAPIUrl in appsettings.json
https://github.com/HarrisonSlater/Harri.SchoolDemoAPI.BlazorWASM/blob/main/src/Harri.SchoolDemoAPI.BlazorWASM/wwwroot/appsettings.json
 By default StudentDemoAPIUrl points to http://localhost:8080

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
