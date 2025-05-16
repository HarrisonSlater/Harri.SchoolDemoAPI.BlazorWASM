# Harri.SchoolDemoAPI.BlazorWASM UI E2E Tests

End-to-end UI tests for the Harri School Demo Blazor WebAssembly Admin UI, written with SpecFlow (Cucumber) and Playwright, following BDD and the Page Object Model pattern.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Running Tests](#running-tests)
## Prerequisites

- .NET 8 SDK
- Playwright CLI (`dotnet tool install --global Microsoft.Playwright.CLI` + `playwright install`)
- A running instance of the [Harri.SchoolDemoAPI.BlazorWASM](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI.BlazorWASM) (default URL: `https://localhost:7144`)

## Configuration

- **SchoolDemoBaseUrl** in `appsettings.json` points to the app base URL.
- **specflow.actions.json** (default in the test project root):
  ```json
  {
    "headless": true
  }
  ```
Set `"headless": false` for running the tests in a full browser

## Running Tests

```bash
# Run all UI E2E tests
dotnet test src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.csproj
```

### Debugging

#### Visual Studio

1. Under **Test > Configure Run Settings**, select `debug.runsettings` to enable `PWDEBUG`.
2. Run the UI E2E tests in **Debug** mode to launch full browser windows.

#### Command Line

1. For debugging step by step enable the playwright debug environment variable

```
$env:PWDEBUG=1
```

## Tags & Cleanup

- Scenarios tagged `@cleanupNewStudent` will delete new students created.
- `CleanupNewStudentHook.cs` removes these test records after each scenario.

## Test Data

Tests assume only that multiple pages of students exist; they do **not** depend on specific pre-populated records.

## Parallel Execution

Tests are designed for parallel execution against a single shared database, suitable for CI environments.
