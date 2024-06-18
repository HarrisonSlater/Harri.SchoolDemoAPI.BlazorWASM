using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
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

        [Then("I should be redirected (back )to the students page")]
        [Then("I should be on the students page")]
        public async Task IShouldBeOnTheStudentsPage()
        {
            await _studentsPage.AssertStudentPageIsVisible();
        }

        [Given("I see a table full of students")]
        [Then("I see a table full of students")]
        [Then("I see a table full of students on page 1")]
        public async Task IShouldSeeATableFullOfStudents()
        {
            _page1Names = await _studentsPage.AssertFullTableAndGetNames();

            await Assertions.Expect(_studentsPage.Pagination.GoToFirstPageButton).ToBeDisabledAsync();
            await Assertions.Expect(_studentsPage.Pagination.PreviousPageButton).ToBeDisabledAsync();
        }

        [Then("I see a table full of students on page 2")]
        public async Task IShouldSeeATableFullOfStudentsOnPage2()
        {
            await _studentsPage.AssertFullTableAndGetNames();
        }

        [Then("I see a table with at least one student")]
        public async Task IShouldSeeATableWithAtLeastOneStudent()
        {
            await _studentsPage.AssertAtLeastOneStudentRowExists();
        }

        [When("I click edit on the first student")]
        public async Task IClickEditOnTheFirstStudent()
        {
            await _studentsPage.StudentEditButton.First.ClickAsync();
        }

        //TODO move to pagination steps
        [Then("I see page 1 again")]
        public async Task ISeePage1Again()
        {
            var page1NamesAgain = await _studentsPage.AssertFullTableAndGetNames();

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

        private int? _lastPage;
        [When("I click last page")]
        public async Task IClickLast()
        {
            await _studentsPage.Pagination.GoToLastPageButton.ClickAsync();
            //await _studentsPage.Page.WaitForURLAsync(new Regex(".*"));

            _lastPage = _studentsPage.Pagination.GetPage();
        }

        [Then("(I )see page {int} in the url")]
        public async Task ThenISeePageInTheUrl(int pageNumber)
        {
            await _studentsPage.Page.WaitForURLAsync(new Regex(".*"));
            await Assertions.Expect(_studentsPage.Page).ToHaveURLAsync(new Regex($".*/page/{pageNumber}"));
        }

        [When("I click back")]
        public async Task WhenIClickBack()
        {
            await _studentsPage.Page.GoBackAsync();
        }

        [Then("I see the last page in the url")]
        public async Task ThenISeePageTheLastPageInTheUrl()
        {
            if (_lastPage is null) throw new ArgumentException("_lastPage has not been set by a previous step");

            await Assertions.Expect(_studentsPage.Page).ToHaveURLAsync(new Regex($".*/page/{_lastPage}"));
        }

        [Then("I see the home url")]
        public async Task ThenISeeTheHomeUrl()
        {
            await Assertions.Expect(_studentsPage.Page).ToHaveURLAsync(_studentsPage.Navigation.BaseUrl);
        }

        private string? _successAlertExtractedId;
        [Then("(I )see a success alert for a new student")]
        public async Task ThenISeeASuccessAlertForANewStudent()
        {
            _successAlertExtractedId = await _studentsPage.GetSuccessAlertId();
        }

        [Then("(I )should not see a success alert")]
        public async Task ThenIShouldNotSeeASuccessAlert()
        {
            await Assertions.Expect(_studentsPage.StudentSuccessAlert).Not.ToBeVisibleAsync();
        }

        [When("I search for student using the success alert id")]
        public async Task WhenISearchForStudent()
        {
            await _studentsPage.SearchForStudent(_successAlertExtractedId);
        }

        [Then("I should see the correct student with name {string}")]
        public async Task ThenIShouldSeeTheCorrectStudent(string studentName)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_successAlertExtractedId, studentName, null);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should see the correct student with name {string} and GPA {string}")]
        public async Task ThenIShouldSeeTheCorrectStudentWithNameAndGPA(string studentName, string gpa)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_successAlertExtractedId, studentName, gpa);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should not see a student with name {string} and GPA {string}")]
        public async Task ThenIShouldNotSeeAStudentWithNameAndGPA(string studentName, string gpa)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_successAlertExtractedId, studentName, gpa);

            rowData.Should().NotContainEquivalentOf(rowTuple);
        }
    }
}
