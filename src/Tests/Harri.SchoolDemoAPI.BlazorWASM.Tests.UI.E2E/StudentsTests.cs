using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
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
        private StudentsPage StudentsPage { get; set; }

        [SetUp]
        public void SetUp()
        {
            StudentsPage = new StudentsPage(base.Page, SchoolDemoAdminUrl);
        }

        [Test]
        public async Task CanViewAllStudents_MultiplePages()
        {
            await Page.GotoAsync(SchoolDemoAdminUrl);

            await StudentsPage.NavigateTo();

            var names = StudentsPage.NameDataCellsLocator;
            var sIds = StudentsPage.IdDataCellsLocator;
            var rowsDisplayed = await StudentsPage.GetRowsDisplayed();

            var rows = Page.GetByRole(AriaRole.Row);
            await Expect(rows).ToHaveCountAsync(rowsDisplayed + 2); // + 2 includes header row and footer

            await StudentsPage.AssertRowsAndGetCellData(sIds);
            var page1Names = await StudentsPage.AssertRowsAndGetCellData(names);

            await Expect(StudentsPage.Pagination.GoToFirstPageButton).ToBeDisabledAsync();
            await Expect(StudentsPage.Pagination.PreviousPageButton).ToBeDisabledAsync();

            await StudentsPage.Pagination.NextPageButton.ClickAsync();

            var page2Names = await StudentsPage.AssertRowsAndGetCellData(names);
            page2Names.Should().NotBeEquivalentTo(page1Names); // Assert content different from page 1

            await StudentsPage.Pagination.PreviousPageButton.ClickAsync();

            var page1AgainNames = await StudentsPage.AssertRowsAndGetCellData(names);
            page1AgainNames.Should().BeEquivalentTo(page1Names); // Assert content same from page 1

            await StudentsPage.Pagination.GoToLastPageButton.ClickAsync();

            var lastPageNames = await names.AllTextContentsAsync();
            lastPageNames.Should().HaveCountGreaterThan(0);
            lastPageNames.Should().NotBeEquivalentTo(page1Names); // Assert content different from page 1


            await StudentsPage.Pagination.GoToFirstPageButton.ClickAsync();

            var page1AgainNames2 = await StudentsPage.AssertRowsAndGetCellData(names);
            page1AgainNames2.Should().BeEquivalentTo(page1Names); // Assert content same from page 1
        }
    }
}
