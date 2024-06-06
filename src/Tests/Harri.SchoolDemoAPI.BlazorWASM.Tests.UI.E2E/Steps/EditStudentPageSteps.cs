using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class EditStudentPageSteps
    {
        private readonly EditStudentPage _editStudentPage;

        public EditStudentPageSteps(EditStudentPage editStudentPage)
        {
            _editStudentPage = editStudentPage;
        }

        [Then("I see the create new student form")]
        public async Task ISeeTheCreateNewStudentPage()
        {
            await _editStudentPage.AssertCurrentPage();
        }

        [Then("I should be on the create new student page")]
        public async Task IShouldBeOnTheCreateNewStudentPage()
        {
            await _editStudentPage.Navigation.AssertCreateNewStudentPageUrlIsCorrect();
            await ISeeTheCreateNewStudentPage();
        }

        [When("I enter a student name {}")]
        public async Task IEnterAStudentName(string studentName)
        {
            await _editStudentPage.EnterStudentName(studentName);
        }

        [When("(I )click save")]
        public async Task IClickSave()
        {
            await _editStudentPage.ClickSave();
        }
    }
}
