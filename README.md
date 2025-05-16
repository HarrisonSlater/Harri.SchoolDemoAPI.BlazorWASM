# Harri.SchoolDemoAPI.BlazorWASM - Blazor Admin UI

**Harri.SchoolDemoAPI.BlazorWASM** is a standalone Blazor WebAssembly Admin UI built with MudBlazor for managing students, schools, and applications, featuring comprehensive automation tests and CI/CD integration. 

The backend .NET 8 REST API is a separate project here: [Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)

## 🚧 Work In Progress - UI
So far the students pages are complete with in-depth Bunit and Playwright tests covering the implemented functionality




# Running the app
To point to a local Harri.SchoolDemoAPI instance:

Set `StudentDemoAPIUrl` in [appsettings.json](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI.BlazorWASM/blob/main/src/Harri.SchoolDemoAPI.BlazorWASM/wwwroot/appsettings.json)

 By default `StudentDemoAPIUrl` points to http://localhost:8080 

To set up and run the REST API and SQL DB locally see the [running from container instructions](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI#running-from-container)
# Nuget packages used
- [MudBlazor](https://mudblazor.com/)
- [Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)
- 
## Tests
- [NUnit](https://github.com/nunit/nunit)
- [FluentAssertions](https://github.com/fluentassertions/fluentassertions)

### Unit
- [bUnit](https://github.com/bUnit-dev/bUnit)
- [Moq](https://github.com/devlooped/moq)

### UI E2E
- [Playwright](https://github.com/microsoft/playwright)
- [Specflow 3.9](https://github.com/SpecFlowOSS)
- [CucumberExpressions.SpecFlow.3-9](https://github.com/gasparnagy/CucumberExpressions.SpecFlow)
- SpecFlow.Actions.Playwright

See the UI Test README [here](src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E/README.md)
