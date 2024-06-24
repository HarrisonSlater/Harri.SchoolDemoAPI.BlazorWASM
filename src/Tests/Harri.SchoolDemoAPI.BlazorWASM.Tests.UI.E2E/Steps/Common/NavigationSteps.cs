using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common
{
    [Binding]
    public class NavigationSteps
    {
        private readonly IPage _page;
        private readonly EditStudentPage _editStudentPage;
        private readonly StudentsPage _studentsPage;
        private readonly CreatedTestStudent _createdTestStudent;
        private readonly NavigationActions _navigationActions;
        private readonly HomePage _homePage;

        public NavigationSteps(IPage page, NavigationActions navigationActions, HomePage homePage, EditStudentPage editStudentPage,
            StudentsPage studentsPage, CreatedTestStudent createdTestStudent)
        {
            _page = page;
            _editStudentPage = editStudentPage;
            _studentsPage = studentsPage;
            _createdTestStudent = createdTestStudent;
            _navigationActions = navigationActions;
            _homePage = homePage;
        }

        [When("I click back")]
        public async Task WhenIClickBack()
        {
            await _page.GoBackAsync();
        }

        [When("I refresh the page")]
        public async Task ThenIRefreshThePage()
        {
            await _page.ReloadAsync();
        }


        [Then("I see the home url")]
        public async Task ThenISeeTheHomeUrl()
        {
            await _navigationActions.AssertHomePageUrlIsCorrect();
        }

        [Given("I am on the home page")]
        public async Task GivenIAmOnTheHomePage()
        {
            await _navigationActions.GoToHome();
        }
        
        [Then("I should be on the home page")]
        public async Task ThenIShouldBeOnTheHomePage()
        {
            await _homePage.AssertHomePageIsVisible();
            await ThenISeeTheHomeUrl();
        }

        [Given("I am on the students page")]
        public async Task GivenIAmOnTheStudentsPage()
        {
            await _navigationActions.GoToStudentsPage();
        }

        [Given("I am on the students page {int}")]
        [Given("I go directly to the students page {int}")]
        public async Task GivenIAmOnTheStudentsPage(int pageNumber)
        {
            await _navigationActions.GoToStudentsPage(pageNumber);
        }

        [Given("I am on the create new student page")]
        public async Task GivenIAmOnTheCreateNewStudentPage()
        {
            await _navigationActions.GoToCreateNewStudentPage();
        }

        [When("I navigate to the home page")]
        public async Task INavigateToTheHomePage()
        {
            await _navigationActions.NavigateToHomePage();
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

        [Given("I am on the edit page for a student")]
        public async Task GivenIAmOnTheEditPageForAStudent()
        {
            await GivenIAmOnTheStudentsPage();

            await _studentsPage.ClickEditOnTheFirstStudent();

            await _editStudentPage.AssertEditStudentPageIsVisible();
            await _editStudentPage.AssertFormNotEmpty();
        }

        [When("I click edit on the new/updated student")]
        public async Task IClickEditOnTheNewStudent()
        {
            await _studentsPage.ClickEditOnStudent(_createdTestStudent.StudentId);
        }

        [When("(I )search for the new/updated student")]
        public async Task ISearchForTheNewStudent()
        {
            await _studentsPage.SearchForStudent(_createdTestStudent.StudentId);
        }
    }
}
