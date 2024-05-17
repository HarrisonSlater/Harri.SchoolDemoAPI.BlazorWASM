using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        //[Test]
        public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
        {
            await Page.GotoAsync("https://playwright.dev");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

            // create a locator
            var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

            // Click the get started link.
            await getStarted.ClickAsync();

            // Expects the URL to contain intro.
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }
    }
}