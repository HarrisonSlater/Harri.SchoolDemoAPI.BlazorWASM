# enable debug and run tests in browser

$env:PWDEBUG=1
dotnet test
These tests are written using BDD (cucumber via specflow) and the page object model pattern

'Action' classes encapsulate common UI actions across multiple pages

Any tests that create a new student will use a @cleanupNewStudent tag 
which will be deleted after a test run in CleanupNewStudentHook.cs

The tests have been written with the goal of many instances of the tests being run 
simultaneously against a single database as might be the case in a shared CI environment
and should not depend on any pre-existing data other than the fact that multiple pages of students should be present

When debugging the UI.E2E tests in visual studio choose debug.runsettings in the project directory under Test > Configure Run Settings which configures the above environment variable

Running the Playwright UI Tests
 By default SchoolDemoBaseUrl points to https://localhost:7144
