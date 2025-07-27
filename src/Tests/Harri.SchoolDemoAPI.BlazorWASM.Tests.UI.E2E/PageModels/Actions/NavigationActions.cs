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

        private ILocator PageHeader => _page.Locator("header.mud-appbar .mud-toolbar h5:has-text(\"Application\")");
        private ILocator BlazorLoadingProgress => _page.Locator(".loading-progress");

        public NavigationActions(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config)
        {
            _page = page;
            _baseUrlSetting = baseUrlSetting;
            _config = config;
        }

        public async Task GoToHome()
        {
            await _page.GotoAsync(BaseUrl);

            await AssertPageLoaded();

            await AssertHomePageUrlIsCorrect();
        }

        public async Task AssertPageLoaded()
        {
            await Assertions.Expect(BlazorLoadingProgress).Not.ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions() { Timeout = 30000 });
            await Assertions.Expect(PageHeader).ToBeVisibleAsync();
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

        public async Task NavigateToHomePage()
        {
            var homeNavLink = _page.Locator("#nav-home");

            await homeNavLink.ClickAsync();

            await AssertHomePageUrlIsCorrect();
        }

        public async Task NavigateToStudentsPage()
        {
            var studentsNavLink = _page.Locator("#nav-students");

            await studentsNavLink.ClickAsync();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task NavigateToCreateNewStudentPage()
        {
            var createNewStudentNavLink = _page.Locator("#nav-students-new");

            await createNewStudentNavLink.ClickAsync();

            await AssertCreateNewStudentPageUrlIsCorrect();
        }

        public async Task AssertHomePageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex($"^{BaseUrl}(\\?.*)?$"));
        }

        public async Task AssertStudentsPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex("^.*students/page/(\\d+)"));
        }

        public async Task AssertCreateNewStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex("^.*students/new$"));
        }

        public async Task AssertEditStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex("^.*students/(\\d+)$"));
        }

        public async Task AssertEditStudentPageUrlIsCorrect(string id)
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex($"^.*students/{id}$"));
        }
    }
}
