using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext;
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
        [Given("I see a table full of students on page 1")]
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

        [Then("The students table should be empty")]
        public async Task TheStudentsTableShouldBeEmpty()
        {
            await _studentsPage.AssertNoStudentRowsExist();
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

        [Then("(I )see a success alert for an updated student")]
        public async Task ThenISeeASuccessAlertForAnUpdatedStudent()
        {
            var studentId = await _studentsPage.GetEditSuccessAlertId();
            _createdTestStudent.StudentId = studentId;
        }

        [Then("(I )see a success alert for a deleted student")]
        public async void ThenSeeASuccessAlertForADeletedStudent()
        {
            var deletedStudentId = await _studentsPage.GetDeleteAlertId();
            deletedStudentId.Should().Be(_createdTestStudent.StudentId);
        }

        [Then("(I )should not see a success alert")]
        public async Task ThenIShouldNotSeeASuccessAlert()
        {
            await Assertions.Expect(_studentsPage.StudentSuccessAlert).Not.ToBeVisibleAsync();
        }

        [When("I search for student using the success alert ID")]
        public async Task WhenISearchForStudentUsingID()
        {
            await _studentsPage.SearchForStudentBySId(_createdTestStudent.StudentId);
        }

        [When("I search for student id {string}")]
        public async Task WhenISearchForStudent(string searchString)
        {
            await _studentsPage.SearchForStudentBySId(searchString);
        }

        [When("I search for student with name {string}")]
        public async Task WhenISearchForStudentName(string searchString)
        {
            await _studentsPage.SearchForStudentByName(searchString);
        }

        [When("I search for student with GPA {string}")]
        public async Task WhenISearchForStudentByGPA(string searchString)
        {
            await _studentsPage.SearchForStudentByGPA(searchString);
        }

        [When("I search for student with GPA greater than {string}")]
        public async Task WhenISearchForStudentByGPAGreaterThan(string searchString)
        {
            await _studentsPage.SearchForStudentByGPAGreaterThan(searchString);
        }

        [When("I search for student with GPA less than {string}")]
        public async Task WhenISearchForStudentByGPALessThan(string searchString)
        {
            await _studentsPage.SearchForStudentByGPALessThan(searchString);
        }

        [When("I search for the new student by name")]
        public async Task WhenISearchForTheNewStudentByName()
        {
            await _studentsPage.SearchForStudentByName(_createdTestStudent.StudentName);
        }

        [When("I search for the new student by id")]
        public async Task WhenISearchForTheNewStudentById()
        {
            await _studentsPage.SearchForStudentBySId(_createdTestStudent.StudentId);
        }

        [When("I search for the new student by GPA")]

        public async Task WhenISearchForTheNewStudentByGPA()
        {
            await _studentsPage.SearchForStudentByGPA(_createdTestStudent.StudentGPA);
        }

        [When("I clear the student name filter")]
        public async Task WhenIClearTheStudentNameFilter()
        {
            await _studentsPage.ClearNameSearch();
        }

        [When("I clear the student id filter")]
        public async Task WhenIClearTheStudentIdFilter()
        {
            await _studentsPage.ClearIdSearch();
        }

        [When("I clear the student GPA filter")]
        public async Task WhenIClearTheStudentGPAFilter()
        {
            await _studentsPage.ClearGPASearch();
        }

        [When("I set the student GPA filter to 'is empty'")]
        public async Task WhenISetTheStudentGPAFiltertoIsEmpty()
        {
            await _studentsPage.SearchForStudentByGPAIsEmpty();
        }

        [When("I set the student GPA filter to 'is not empty'")]
        public async Task WhenISetTheStudentGPAFiltertoIsNotEmpty()
        {
            await _studentsPage.SearchForStudentByGPAIsNotEmpty();
        }

        [Then("I should see the updated/same/new student with name {string}")]
        public async Task ThenIShouldSeeTheCorrectStudent(string studentName)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, studentName, null);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should see the updated/same/new student with name {string} and GPA {string}")]

        public async Task ThenIShouldSeeTheCorrectStudentWithNameAndGPA(string studentName, string gpa)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, studentName, gpa);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }

        [Then("I should see only the updated/same/new student")]
        public async Task ThenIShouldSeeTheCorrectStudent()
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();
            
            var rowTuple = new Tuple<string?, string?, string?>(_createdTestStudent.StudentId, _createdTestStudent.StudentName, _createdTestStudent.StudentGPA);

            rowData.Should().ContainSingle().And.ContainEquivalentOf(rowTuple);
        }

        [Then("I should see only students with the name {string}")]
        public async Task ThenIShouldSeeOnlyStudentsWithTheName(string name)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            rowData.Should().AllSatisfy(x => x.Item2.Should().Contain(name));
        }

        [Then("I should see only students containing the id {string}")]
        public async Task ThenIShouldSeeOnlyStudentsContainingTheId(string id)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            rowData.Should().AllSatisfy(x => x.Item1.Should().Contain(id));
        }

        [Then("I should see only students with the GPA {string}")]
        public async Task ThenIShouldSeeOnlyStudentsWithTheGPA(string gpa)
        {
            await AssertAllGPAs(gpa, (actual, expectation) => actual.Should().Be(expectation));
        }

        [Then("I should see only students with a GPA greater than {string}")]
        public async Task ThenIShouldSeeOnlyStudentsWithAGPAGreaterThan(string gpa)
        {
            await AssertAllGPAs(gpa, (actual, expectation) => actual.Should().BeGreaterThan(expectation));
        }

        [Then("I should see only students with a GPA less than {string}")]
        public async Task ThenIShouldSeeOnlyStudentsWithAGPALessThan(string gpa)
        {
            await AssertAllGPAs(gpa, (actual, expectation) => actual.Should().BeLessThan(expectation));
        }

        private async Task AssertAllGPAs(string gpa, Action<decimal, decimal> assert)
        {
            await IShouldSeeATableWithAtLeastOneStudent();
            var rowData = await _studentsPage.GetAllRowData();

            rowData.Should().AllSatisfy(x => {
                var parsedActual = decimal.Parse(x.Item3!);
                var parsedExpectation = decimal.Parse(gpa);

                assert.Invoke(parsedActual, parsedExpectation);
            });
        }

        [Then("I should see a filter input validation error")]
        public async Task IShouldSeeAFilterInputValidationMessage()
        {
            var errorMessage = await _studentsPage.GetErrorAlert();

            errorMessage.Should().NotBeNullOrEmpty();
        }

        [Then("I should not see any filter input validation errors")]
        public async Task IShouldNotSeeAFilterInputValidationMessage()
        {
            await _studentsPage.AssertNoErrorAlert();
        }
    }
}
