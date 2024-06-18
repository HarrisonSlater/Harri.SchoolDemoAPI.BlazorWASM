# enable debug and run tests in browser

$env:PWDEBUG=1
dotnet test

When debugging the UI.E2E tests in visual studio choose debug.runsettings in the project directory under Test > Configure Run Settings which configures the above environment variable