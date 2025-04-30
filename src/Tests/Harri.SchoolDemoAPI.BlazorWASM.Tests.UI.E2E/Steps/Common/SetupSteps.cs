using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Tracing;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common
{
    [Binding]
    public class SetupSteps
    {
        private readonly IPage _page;
        private readonly EditStudentPage _editStudentPage;
        private readonly StudentsPage _studentsPage;
        private readonly CreatedTestStudent _createdTestStudent;
        private readonly NavigationActions _navigationActions;
        private readonly HomePage _homePage;

        public SetupSteps(IPage page, NavigationActions navigationActions, HomePage homePage, EditStudentPage editStudentPage,
            StudentsPage studentsPage, CreatedTestStudent createdTestStudent)
        {
            _page = page;
            _editStudentPage = editStudentPage;
            _studentsPage = studentsPage;
            _createdTestStudent = createdTestStudent;
            _navigationActions = navigationActions;
            _homePage = homePage;
        }

        [Given("A new student {string} exists")]
        public async Task GivenANewStudentExists(string name)
        {
            await AStudentExists(name);
        }

        [Given("A new student {string} with GPA {string} exists")]
        public async Task GivenANewStudentWithGPAExists(string name, string gpa)
        {
            await AStudentExists(name, gpa);
        }

        [Given("A new student with a unique name exists")]
        public async Task GivenANewStudentWithAUniqueNameExists()
        {
            var guidName = Guid.NewGuid().ToString();
            await AStudentExists(guidName);
        }

        [Given("A new student with a unique name and GPA {string} exists")]
        public async Task GivenANewStudentWithAUniqueNameAndGPAExists(string gpa)
        {
            var guidName = Guid.NewGuid().ToString();
            await AStudentExists(guidName, gpa);
        }

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
            await AStudentExists(name, gpa);

            await _studentsPage.SearchForStudentBySId(_createdTestStudent.StudentId);

            await _studentsPage.ClickEditOnStudent(_createdTestStudent.StudentId);

            await _editStudentPage.AssertEditStudentPageIsVisible();
            await _editStudentPage.AssertNameNotEmpty();

            if (gpa is null)
            {
                await _editStudentPage.AssertGPAEmpty();
            }
            else
            {
                await _editStudentPage.AssertGPANotEmpty();
            }
        }

        private async Task AStudentExists(string name, string? gpa = null)
        {
            await _navigationActions.GoToCreateNewStudentPage();

            await _editStudentPage.CreateNewStudent(name, gpa);

            _createdTestStudent.StudentId = await _studentsPage.GetSuccessAlertId();
            _createdTestStudent.StudentName = name;
            _createdTestStudent.StudentGPA = gpa;
        }

        public async Task DeleteAStudent(string id)
        {
            await _navigationActions.NavigateToHomePage();
            await _homePage.EnterEditStudentId(id);
            await _homePage.ClickEditStudentButton();

            await _editStudentPage.AssertEditStudentPageIsVisible();
            await _editStudentPage.ClickDelete();
            await _editStudentPage.ClickDeleteInDialog();

            await _studentsPage.AssertStudentPageIsVisible();
            var deletedStudent = await _studentsPage.GetDeleteAlertId();
            deletedStudent.Should().Be(id);
        }
    }
}
