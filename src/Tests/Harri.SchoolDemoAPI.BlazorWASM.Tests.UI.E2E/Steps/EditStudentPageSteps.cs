using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class EditStudentPageSteps
    {
        private readonly EditStudentPage _editStudentPage;
        private readonly StudentsPageSteps _studentsPageSteps;

        public EditStudentPageSteps(EditStudentPage editStudentPage, StudentsPageSteps studentsPageSteps)
        {
            _editStudentPage = editStudentPage;
            _studentsPageSteps = studentsPageSteps;
        }

        [Then("I see the create new student form")]
        public async Task ISeeTheCreateNewStudentForm()
        {
            await _editStudentPage.AssertCurrentPage();

            await _editStudentPage.AssertFormEmpty();
        }

        [Then("I see the edit student form")]
        public async Task ISeeTheEditStudentForm()
        {
            await _editStudentPage.AssertCurrentPage();

            await _editStudentPage.AssertNameNotEmpty();
        }

        [Then("I should be on the create new student page")]
        public async Task IShouldBeOnTheCreateNewStudentPage()
        {
            await _editStudentPage.Navigation.AssertCreateNewStudentPageUrlIsCorrect();
            await ISeeTheCreateNewStudentForm();
        }

        [Then("I should be on the edit student page")]
        public async Task IShouldBeOnTheCreateEditSudentPage()
        {
            await _editStudentPage.Navigation.AssertEditStudentPageUrlIsCorrect();
            await ISeeTheEditStudentForm();
        }

        [When("(I )enter a student name {string}")]
        public async Task IEnterAStudentName(string studentName)
        {
            await _editStudentPage.EnterStudentName(studentName);
        }

        [When("(I )enter a student GPA {string}")]
        public async Task IEnterStudentGPA(string gpa)
        {
            await _editStudentPage.EnterStudentGPA(gpa);
        }

        [When("(I )click save")]
        public async Task IClickSave()
        {
            await _editStudentPage.ClickSave();
        }

        [When("(I )create a new student")]
        public async Task WhenICreateANewStudent()
        {
            await _editStudentPage.CreateNewStudent("Tester StudentE2E-2");
        }

        [When("I remove the student's name")]
        [When("I enter a blank student name")]
        public async Task WhenIRemoveTheStudentsName()
        {
            await _editStudentPage.EnterStudentName("");
        }

        [Then("I should see a validation message for the Name")]
        public async Task ThenIShouldSeeAValidationMessageForTheName()
        {
            await _editStudentPage.ShouldHaveValidationErrorForName();
        }

        [Then("I should see a validation message for the GPA")]
        public async Task ThenIShouldSeeAValidationMessageForTheGPA()
        {
            await _editStudentPage.ShouldHaveValidationErrorForGPA();
        }

        [Then("I should see a validation message for the Name and GPA")]
        public async Task ThenIShouldSeeAValidationMessageForTheNameAndGPA()
        {
            await _editStudentPage.ShouldHaveValidationErrorForName();
            await _editStudentPage.ShouldHaveValidationErrorForGPA();
        }
    }
}
