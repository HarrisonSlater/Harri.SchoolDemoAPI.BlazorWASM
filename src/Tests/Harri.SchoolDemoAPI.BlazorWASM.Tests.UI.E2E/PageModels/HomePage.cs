using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class HomePage : BasePage
    {
        public ILocator ViewStudentsButton => _page.Locator("#home-shortcut-students");
        public ILocator CreateNewStudentButton => _page.Locator("#home-shortcut-students-new");
        public ILocator EditStudentIdInput => _page.Locator("#home-shortcut-edit-student-id");
        public ILocator EditStudentButton => _page.Locator("#home-shortcut-edit-student");

        public ILocator InvalidStudentErrorAlert => _page.Locator("#student-error-alert");

        public ILocator InputContainersWithError => _page.Locator(".mud-input-control.mud-input-error");
        public ILocator ErrorText => _page.Locator(".mud-input-helper-text.mud-input-error");

        public AlertActions Alerts { get; set; }

        public HomePage(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config) : base(page, baseUrlSetting, config)
        {
            Alerts = new AlertActions(page);
        }

        public async Task ClickViewStudentsButton()
        {
            await ViewStudentsButton.ClickAsync();
        }

        public async Task ClickCreateNewStudentButton()
        {
            await CreateNewStudentButton.ClickAsync();
        }

        public async Task ClickEditStudentButton()
        {
            await EditStudentButton.ClickAsync();
        }

        public async Task EnterEditStudentId(string id)
        {
            await EditStudentIdInput.FillAsync(id);
        }

        public async Task ShouldHaveValidationErrorForStudentID()
        {
            var errorContainerName = InputContainersWithError.Filter(new LocatorFilterOptions() { Has = EditStudentIdInput });

            await Assertions.Expect(errorContainerName.Locator(ErrorText)).ToBeVisibleAsync();
            await Assertions.Expect(errorContainerName.Locator(ErrorText)).ToContainTextAsync(new Regex("Student ID"));
        }

        //TODO refactor, copied from StudentsPage.cs
        public async Task<string?> GetErrorAlert()
        {
            await Assertions.Expect(InvalidStudentErrorAlert).ToBeVisibleAsync();
            return await InvalidStudentErrorAlert.TextContentAsync();
        }

        public async Task<string> GetErrorAlertId()
        {
            return Alerts.ExtractIdFromAlert(await GetErrorAlert());
        }

        public async Task AssertHomePageIsVisible()
        {
            await Assertions.Expect(ViewStudentsButton).ToBeVisibleAsync();
            await Assertions.Expect(CreateNewStudentButton).ToBeVisibleAsync();
            await Assertions.Expect(EditStudentIdInput).ToBeVisibleAsync();
            await Assertions.Expect(EditStudentIdInput).ToBeEmptyAsync();
            await Assertions.Expect(EditStudentButton).ToBeVisibleAsync();
        }
    }
}
