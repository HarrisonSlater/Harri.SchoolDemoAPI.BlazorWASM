using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class StudentsPageSteps
    {
        private readonly StudentsPage _studentsPage;
        private IReadOnlyList<string> _page1Names;

        public StudentsPageSteps(StudentsPage studentsPage)
        {
            _studentsPage = studentsPage;
        }

        [Given("I am on the home page")]
        public async Task GivenIAmOnTheHomePage()
        {
            await _studentsPage.GoToHome();
            //Todo assert home page visible
        }

        [Given("I am on the students page")]
        public async Task GivenIAmOnTheStudentsPage()
        {
            await _studentsPage.GoTo();
            //Todo students page visible
        }

        [When("I navigate to the students page")]
        public async Task INavigateToTheStudentsPage()
        {
            await _studentsPage.NavigateTo();
        }

        [Given("I see a table full of students")]
        [Then("I see a table full of students")]
        [Then("I see a table full of students on page 1")]
        public async Task IShouldSeeATableFullOfStudents()
        {
            var names = _studentsPage.NameDataCellsLocator;
            var sIds = _studentsPage.IdDataCellsLocator;
            var rowsDisplayed = await _studentsPage.GetRowsDisplayed();

            var rows = _studentsPage.Page.GetByRole(AriaRole.Row);
            await Assertions.Expect(rows).ToHaveCountAsync(rowsDisplayed + 2); // + 2 includes header row and footer

            await _studentsPage.AssertRowsAndGetCellData(sIds);
            _page1Names = await _studentsPage.AssertRowsAndGetCellData(names);

            await Assertions.Expect(_studentsPage.Pagination.GoToFirstPageButton).ToBeDisabledAsync();
            await Assertions.Expect(_studentsPage.Pagination.PreviousPageButton).ToBeDisabledAsync();
        }
    }
}
