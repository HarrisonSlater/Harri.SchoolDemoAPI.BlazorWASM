using FluentAssertions;
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

            _page1Names = await AssertFullTableAndGetNames();

            await Assertions.Expect(_studentsPage.Pagination.GoToFirstPageButton).ToBeDisabledAsync();
            await Assertions.Expect(_studentsPage.Pagination.PreviousPageButton).ToBeDisabledAsync();
        }

        [Then("I see a table full of students on page 2")]
        public async Task IShouldSeeATableFullOfStudentsOnPage2()
        {
            await AssertFullTableAndGetNames();
        }

        [Then("I see a table with at least one student")]
        public async Task IShouldSeeATableWithAtLeastOneStudent()
        {
            var sIds = (await _studentsPage.GetCellData(_studentsPage.IdDataCellsLocator));
            sIds.Should().HaveCountGreaterThan(0);

            var names = (await _studentsPage.GetCellData(_studentsPage.NameDataCellsLocator));
            names.Should().HaveCountGreaterThan(0);
        }

        private async Task<IReadOnlyList<string>> AssertFullTableAndGetNames()
        {
            var names = _studentsPage.NameDataCellsLocator;
            var sIds = _studentsPage.IdDataCellsLocator;
            var rowsDisplayed = await _studentsPage.GetRowsDisplayed();

            var rows = _studentsPage.Page.GetByRole(AriaRole.Row);
            await Assertions.Expect(rows).ToHaveCountAsync(rowsDisplayed + 2); // + 2 includes header row and footer

            await _studentsPage.AssertRowsAndGetCellData(sIds);

            return await _studentsPage.AssertRowsAndGetCellData(names);
        }

        [Then("I see page 1 again")]
        public async Task ISeePage1Again()
        {
            var page1NamesAgain = await AssertFullTableAndGetNames();

            page1NamesAgain.Should().BeEquivalentTo(_page1Names);
        }

        // Pagination steps, could be separate files
        [When("I click next page")]
        public async Task IClickNext()
        {
            await _studentsPage.Pagination.NextPageButton.ClickAsync();
        }

        [When("I click previous page")]
        public async Task IClickPrevious()
        {
            await _studentsPage.Pagination.PreviousPageButton.ClickAsync();

        }

        [When("I click first page")]
        public async Task IClickFirst()
        {
            await _studentsPage.Pagination.GoToFirstPageButton.ClickAsync();

        }

        [When("I click last page")]
        public async Task IClickLast()
        {
            await _studentsPage.Pagination.GoToLastPageButton.ClickAsync();

        }
    }
}
