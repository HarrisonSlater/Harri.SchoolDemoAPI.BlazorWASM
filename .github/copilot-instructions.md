# Copilot Instructions for Harri.SchoolDemoAPI.BlazorWASM

## Project Overview

This is a standalone **Blazor WebAssembly** Admin UI built with **.NET 8** and **MudBlazor** for managing students, schools, and applications. It consumes a separate .NET 8 REST API ([Harri.SchoolDemoAPI](https://github.com/HarrisonSlater/Harri.SchoolDemoAPI)) via the `Harri.SchoolDemoAPI.Client` NuGet package.

## Solution Structure

```
Harri.SchoolDemoAPI.BlazorWASM.sln
├── src/Harri.SchoolDemoAPI.BlazorWASM/              # Main Blazor WASM app
│   ├── Pages/          # Routable page components
│   ├── Components/     # Reusable components (e.g. EditStudentForm)
│   ├── Layout/         # MainLayout, NavMenu
│   ├── Filters/        # Search filter models and operator mappings
│   ├── Helpers/        # Utility helpers
│   ├── wwwroot/        # Static assets and appsettings.json
│   ├── Constants.cs    # Query string keys, filter operator names
│   ├── Text.cs         # All user-facing text/messages
│   └── Program.cs      # Service registration and bootstrap
├── src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit/       # Unit + bUnit component tests
│   └── BunitTests/     # Razor-based component tests
└── src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E/     # SpecFlow + Playwright E2E tests
    ├── Features/       # Gherkin .feature files
    ├── Steps/          # Step definitions
    └── PageModels/     # Page object models for Playwright
```

## Tech Stack

- **Framework**: .NET 8, Blazor WebAssembly (standalone)
- **UI Library**: MudBlazor
- **API Client**: `Harri.SchoolDemoAPI.Client` and `Harri.SchoolDemoAPI.Models` NuGet packages
- **Unit Testing**: NUnit, bUnit, Moq, FluentAssertions
- **E2E Testing**: SpecFlow 3.9, Playwright (via `Microsoft.Playwright.NUnit`)
- **CI/CD**: Azure Pipelines

## Build and Run

```bash
# Build
dotnet build

# Run the app (API URL configured in wwwroot/appsettings.json)
dotnet run --project src/Harri.SchoolDemoAPI.BlazorWASM/Harri.SchoolDemoAPI.BlazorWASM.csproj

# Run unit tests
dotnet test src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit/

# Run E2E tests (requires running API + database)
dotnet test src/Tests/Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E/
```

## Code Conventions

### Razor Components
- Use **inline code-behind** (`@code { }` blocks) rather than separate `.razor.cs` files.
- Inject services with `@inject` directives (e.g. `@inject IStudentApi StudentApi`).
- Use `[Parameter]` and `[SupplyParameterFromQuery]` attributes for component parameters.
- Use `NavigationManager.NavigateTo()` with query string parameters for page-to-page state (success messages, error states).
- Use MudBlazor components (`MudDataGrid`, `MudTextField`, `MudButton`, `MudAlert`, etc.) for all UI elements.
- Use `EditForm` with `DataAnnotationsValidator` for form validation.

### Constants and Text
- Define all query string parameter names and filter operator names in `Constants.cs` using nested static classes.
- Centralize all user-facing strings in `Text.cs` using nested static classes per page/component, with interpolated string methods for dynamic text.

### Naming
- PascalCase for all public members, types, and namespaces.
- Use `internal` access modifier on properties/methods that need to be accessed from unit tests.
- Prefix MudDataGrid CSS selectors in test classes as constants (e.g. `IdDataCellsSelector`).

### Filters
- Filter logic is encapsulated in classes under the `Filters/` folder.
- `StudentSearchFilters` wraps MudDataGrid `IFilterDefinition<T>` and provides parsed filter values for API calls.

## Testing Conventions

### Unit Tests (bUnit)
- Test files are `.razor` files in `BunitTests/` that inherit from `BunitTestContext`.
- Use `SetupAuthorization()` and register mock services in `Setup()`.
- Mock `IStudentApi` with Moq and set up expected return values.
- Assert with FluentAssertions (e.g. `.Should().Be()`, `.Should().Contain()`).
- Use `WaitForAssertion()` for async rendering assertions.
- Use `FindAndClickAsync()` extension method for button interactions.
- Organize tests with `// Arrange`, `// Act`, `// Assert` comments.

### E2E Tests (SpecFlow + Playwright)
- Feature files use Gherkin syntax in `Features/`.
- Step definitions are in `Steps/` and use Playwright for browser automation.
- Page object models in `PageModels/` encapsulate page interactions.
- Use `@cleanupNewStudent` and similar tags for test data cleanup hooks.
- Tests are designed for parallel execution with a shared database.

## Dependencies

When adding new NuGet packages, prefer packages already in use in the project. The API client (`Harri.SchoolDemoAPI.Client`) provides `IStudentApi` for all backend communication.

## Configuration

- The API base URL is set via `StudentDemoAPIUrl` in `wwwroot/appsettings.json`.
- Services are registered as singletons in `Program.cs`.
