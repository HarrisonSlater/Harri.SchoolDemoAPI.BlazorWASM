using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class StudentsPageSteps
    {
        private readonly StudentsPage _studentsPage;
        private readonly CreatedTestStudent _createdTestStudent;

        private IReadOnlyList<string>? _page1Names = null;

        public StudentsPageSteps(StudentsPage studentsPage, CreatedTestStudent createdTestStudent)
        {
            _studentsPage = studentsPage;
            _createdTestStudent = createdTestStudent;
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

        [Then("I see page 1 again")]
        public async Task ISeePage1Again()
        {
            var page1NamesAgain = await _studentsPage.AssertFullTableAndGetNames();

            page1NamesAgain.Should().BeEquivalentTo(_page1Names);
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

        [Then("(I )see a success alert for a new student")]
        public async Task ThenISeeASuccessAlertForANewStudent()
        {
            var studentId = await _studentsPage.GetSuccessAlertId();
            _createdTestStudent.StudentId = studentId;
        }

        [Then("(I )should not see a success alert")]
        public async Task ThenIShouldNotSeeASuccessAlert()
        {
            await Assertions.Expect(_studentsPage.StudentSuccessAlert).Not.ToBeVisibleAsync();
        }

        [When("I search for student using the success alert id")]
        public async Task WhenISearchForStudent()
        {
            if (_createdTestStudent.StudentId is null) throw new ArgumentException("_createdTestStudent.StudentId has not been set by a previous step");

            await _studentsPage.SearchForStudent(_createdTestStudent.StudentId);
        }

        [Then("I should see the correct student with name {string}")]
        public async Task ThenIShouldSeeTheCorrectStudent(string studentName)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, studentName, null);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should see the correct student with name {string} and GPA {string}")]
        public async Task ThenIShouldSeeTheCorrectStudentWithNameAndGPA(string studentName, string gpa)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, studentName, gpa);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should not see a student with name {string} and GPA {string}")]
        public async Task ThenIShouldNotSeeAStudentWithNameAndGPA(string studentName, string gpa)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, studentName, gpa);

            rowData.Should().NotContainEquivalentOf(rowTuple);
        }
    }
}
