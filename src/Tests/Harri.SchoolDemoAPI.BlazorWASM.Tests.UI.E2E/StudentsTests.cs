using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class StudentsTests : PageTest
    {
        private const string SchoolDemoAdminUrl = "https://localhost:7144";

        [Test]
        public async Task CanViewAllStudents_MultiplePages()
        {
            await Page.GotoAsync(SchoolDemoAdminUrl);

            var studentsNavLink = Page.GetByRole(AriaRole.Link, new() { Name = "Students" });

            await studentsNavLink.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*students/page/1"));

            var defaultRows = await Page.Locator(".mud-table-pagination-select").TextContentAsync();

            defaultRows.Should().NotBeNull();
            var defaultRowsInt = int.Parse(defaultRows);

            var rows = Page.GetByRole(AriaRole.Row);
            await Expect(rows).ToHaveCountAsync(defaultRowsInt + 2); // + 2 includes header row and footer

            var sIds = Page.Locator("td[data-label=\"SId\"]");
            await Expect(sIds).ToHaveCountAsync(defaultRowsInt);
            var page1Ids = await sIds.AllTextContentsAsync();
            page1Ids.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            var names = Page.Locator("td[data-label=\"Name\"]");
            await Expect(names).ToHaveCountAsync(defaultRowsInt);
            var page1Names = await names.AllTextContentsAsync();
            page1Names.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            var paginationActions = Page.Locator(".mud-table-pagination-actions");

            var gotoFirstButton = paginationActions.Locator("button").Nth(0);
            var previousButton = paginationActions.Locator("button").Nth(1);
            var nextButton = paginationActions.Locator("button").Nth(2);
            var gotoLastButton = paginationActions.Locator("button").Nth(3);

            (await gotoFirstButton.IsDisabledAsync()).Should().BeTrue();
            (await previousButton.IsDisabledAsync()).Should().BeTrue();

            await nextButton.ClickAsync();

            await Expect(names).ToHaveCountAsync(defaultRowsInt);
            var page2Names = await names.AllTextContentsAsync();
            page2Names.Should().AllSatisfy(x => x.Should().NotBeEmpty());
            //Assert content different from page 1
            page2Names.Should().NotBeEquivalentTo(page1Names);

            await previousButton.ClickAsync();

            await Expect(names).ToHaveCountAsync(defaultRowsInt);
            var page1AgainNames = await names.AllTextContentsAsync();
            page1AgainNames.Should().AllSatisfy(x => x.Should().NotBeEmpty());
            //Assert content same from page 1
            page1AgainNames.Should().BeEquivalentTo(page1Names);

            await gotoLastButton.ClickAsync();
            //Assert content different from page 1
            var lastPageNames = await names.AllTextContentsAsync();
            lastPageNames.Should().HaveCountGreaterThan(0);
            lastPageNames.Should().NotBeEquivalentTo(page1Names);

            await gotoFirstButton.ClickAsync();

            await Expect(names).ToHaveCountAsync(defaultRowsInt);
            var page1AgainNames2 = await names.AllTextContentsAsync();
            page1AgainNames2.Should().AllSatisfy(x => x.Should().NotBeEmpty());
            //Assert content same from page 1
            page1AgainNames2.Should().BeEquivalentTo(page1Names);
        }
    }
}
