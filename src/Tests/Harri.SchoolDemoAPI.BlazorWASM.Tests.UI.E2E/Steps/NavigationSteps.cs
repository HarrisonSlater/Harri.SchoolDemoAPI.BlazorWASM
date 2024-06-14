using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class NavigationSteps
    {
        private readonly IPage _page;
        private readonly EditStudentPage _editStudentPage;
        private readonly StudentsPage _studentsPage;
        private readonly NavigationActions _navigationActions;

        public NavigationSteps(IPage page, EditStudentPage editStudentPage, StudentsPage studentsPage)
        {
            _page = page;
            _editStudentPage = editStudentPage;
            _studentsPage = studentsPage;
            _navigationActions = new NavigationActions(page);
        }

        [Given("I am on the home page")]
        public async Task GivenIAmOnTheHomePage()
        {
            await _navigationActions.GoToHome();
        }

        [Given("I am on the students page")]
        public async Task GivenIAmOnTheStudentsPage()
        {
            await _navigationActions.GoToStudentsPage();
        }

        [Given("I am on the create new student page")]
        public async Task GivenIAmOnTheCreateNewStudentPage()
        {
            await _navigationActions.GoToCreateNewStudentPage();
        }

        [When("(I )navigate to the students page")]
        public async Task INavigateToTheStudentsPage()
        {
            await _navigationActions.NavigateToStudentsPage();
        }

        [When("I navigate to the create new student page")]
        public async Task INavigateToTheCreateNewStudentPage()
        {
            await _navigationActions.NavigateToCreateNewStudentPage();
        }

        //TODO move to separate step file for combined steps?
        private (string?, string?, string?) _studentRow;
        [Given("I am on the edit page for a student")]
        public async Task GivenIAmOnTheEditPageForAStudent()
        {
            await GivenIAmOnTheStudentsPage();

            _studentRow = await _studentsPage.ClickEditOnTheFirstStudent();

            await _editStudentPage.AssertCurrentPage();
            await _editStudentPage.AssertFormNotEmpty();
        }

        private string _newStudentId;
        [Given("I am on the edit page for a new student {string}")]

        public async Task GivenIAmOnTheEditPageForANewStudent(string name)
        {
            await IAmOnTheEditPageForANewStudent(name);
        }

        [Given("I am on the edit page for a new student {string} with GPA {string}")]
        public async Task GivenIAmOnTheEditPageForANewStudentWithGPA(string name, string gpa)
        {
            await IAmOnTheEditPageForANewStudent(name, gpa);
        }

        private async Task IAmOnTheEditPageForANewStudent(string name, string? gpa = null)
        {
            await _navigationActions.GoToCreateNewStudentPage();

            await _editStudentPage.CreateNewStudent(name, gpa);

            //TODO Use this id in cleanup tag
            _newStudentId = await _studentsPage.GetSuccessAlertId();

            await ISearchForTheNewStudent();

            await IClickEditOnTheNewStudent();

            await _editStudentPage.AssertCurrentPage();
            await _editStudentPage.AssertNameNotEmpty();

            if (gpa is null )
            {
                await _editStudentPage.AssertGPAEmpty();
            }
            else
            {
                await _editStudentPage.AssertGPANotEmpty();
            }
        }

        [When("I click edit on the new/updated student")]
        public async Task IClickEditOnTheNewStudent()
        {
            await _studentsPage.ClickEditOnStudent(_newStudentId);
        }

        [When("(I )search for the new/updated student")]
        public async Task ISearchForTheNewStudent()
        {
            await _studentsPage.SearchForStudent(_newStudentId);
        }

        //TODO this is copy pasted from StudentPageSteps, refactor required
        [Then("I should see the updated/same student with name {string}")]
        public async Task ThenIShouldSeeTheUpdatedStudent(string studentName)
        {
            await _studentsPage.AssertAtLeastOneStudentRowExists();

            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_newStudentId, studentName, null);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }        
        
        [Then("I should see the updated/same student with name {string} and GPA {string}")]
        public async Task ThenIShouldSeeTheUpdatedStudent(string studentName, string gpa)
        {
            await _studentsPage.AssertAtLeastOneStudentRowExists();

            var rowData = await _studentsPage.GetAllRowData();

            var rowTuple = new Tuple<string?, string?, string?>(_newStudentId, studentName, gpa);

            rowData.Should().ContainEquivalentOf(rowTuple);
        }
    }
}
