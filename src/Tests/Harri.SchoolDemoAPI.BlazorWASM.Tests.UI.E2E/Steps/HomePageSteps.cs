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

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common
{
    [Binding]
    public class HomePageSteps
    {
        private readonly IPage _page;
        private readonly CreatedTestStudent _createdTestStudent;
        private readonly NavigationActions _navigationActions;
        private readonly HomePage _homePage;

        public HomePageSteps(IPage page, NavigationActions navigationActions, HomePage homePage, CreatedTestStudent createdTestStudent)
        {
            _page = page;
            _createdTestStudent = createdTestStudent;
            _navigationActions = navigationActions;
            _homePage = homePage;
        }

        [When("I click the view students shortcut")]
        public async Task WhenIClickTheViewStudentsShortcut()
        {
            await _homePage.ClickViewStudentsButton();
        }

        [When("I click the create new student shortcut")]
        public async Task WhenIClickTheCreatenewStudentShortcut()
        {
            await _homePage.ClickCreateNewStudentButton();
        }

        [When("(I )click the edit student shortcut")]
        public async Task WhenIClickTheEditStudentShortcut()
        {
            await _homePage.ClickEditStudentButton();
        }
        
        [When("(I )enter a student ID for an existing student")]
        public async Task WhenIEnterAStudentIDForAnExistingStudent()
        {
            await _homePage.EnterEditStudentId(_createdTestStudent.StudentId);
        }

        [When("(I )enter a student ID {string}")]
        public async Task WhenIEnterAStudentID(string id)
        {
            await _homePage.EnterEditStudentId(id);
        }

        [Then("I should see a validation error for the Student ID")]
        public async Task ThenIShouldSeeAValidationErrorForTheStudentID()
        {
            await _homePage.ShouldHaveValidationErrorForStudentID();
        }

        [Then("(I )see an error alert for a non existing student {string}")]
        public async Task ThenISeeAnErrorAlertForANewStudent(string id)
        {
            var invalidId = await _homePage.GetErrorAlertId();
            invalidId.Should().Be(id);
        }
    }
}
