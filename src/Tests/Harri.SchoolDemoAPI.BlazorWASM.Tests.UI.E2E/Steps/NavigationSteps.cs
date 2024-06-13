using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        }
    }
}
