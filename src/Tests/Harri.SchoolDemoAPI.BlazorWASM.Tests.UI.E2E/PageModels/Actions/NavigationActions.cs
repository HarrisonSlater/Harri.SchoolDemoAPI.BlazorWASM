using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    /// <summary>
    /// Actions for interacting with the main nav sidebar and direct navigation via URL
    /// </summary>
    public class NavigationActions
    {
        private readonly SchoolDemoBaseUrlSetting _baseUrlSetting;
        private readonly PlaywrightConfiguration _config;
        public string BaseUrl => _baseUrlSetting.Value;

        private readonly IPage _page;

        private ILocator PageHeader => _page.Locator("header.mud-appbar .mud-toolbar");

        public NavigationActions(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config)
        {
            _page = page;
            _baseUrlSetting = baseUrlSetting;
            _config = config;
        }

        public async Task GoToHome()
        {
            await _page.GotoAsync(BaseUrl);
        }

        public async Task AssertPageLoaded()
        {
            await Assertions.Expect(PageHeader).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions() { Timeout = _config.DefaultTimeout * 1000 });
        }

        public async Task GoToStudentsPage()
        {
            await _page.GotoAsync(BaseUrl + "students");

            await AssertPageLoaded();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task GoToStudentsPage(int pageNumber)
        {
            await _page.GotoAsync(BaseUrl + $"students/page/{pageNumber}");

            await AssertPageLoaded();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task GoToCreateNewStudentPage()
        {
            await _page.GotoAsync(BaseUrl + "students/new");

            await AssertPageLoaded();

            await AssertCreateNewStudentPageUrlIsCorrect();
        }

        public async Task NavigateToStudentsPage()
        {
            var studentsNavLink = _page.GetByRole(AriaRole.Link, new() { Name = "Students" });

            await studentsNavLink.ClickAsync();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task NavigateToCreateNewStudentPage()
        {
            var createNewStudentNavLink = _page.GetByRole(AriaRole.Link, new() { Name = "Create new student" }); //TODO not select by text

            await createNewStudentNavLink.ClickAsync();

            await AssertCreateNewStudentPageUrlIsCorrect();
        }

        public async Task AssertStudentsPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/page/(\\d+)"), new PageAssertionsToHaveURLOptions());
        }

        public async Task AssertCreateNewStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/new"), new PageAssertionsToHaveURLOptions());
        }

        public async Task AssertEditStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/(\\d+)"), new PageAssertionsToHaveURLOptions());
        }
    }
}
