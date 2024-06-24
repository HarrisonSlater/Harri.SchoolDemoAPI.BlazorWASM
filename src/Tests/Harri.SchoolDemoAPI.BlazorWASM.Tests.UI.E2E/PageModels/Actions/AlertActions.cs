using FluentAssertions;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions
{
    public class AlertActions
    {
        private readonly IPage _page;

        public AlertActions(IPage page)
        {
            _page = page;
        }

        public string ExtractIdFromAlert(string? alertText)
        {
            if (alertText is null) throw new ArgumentException($"{nameof(alertText)} cannot be null");

            var idMatch = Regex.Match(alertText, "'(\\d+)'");
            var id = idMatch.Groups[1].Value;
            id.Should().NotBeNull();

            return id;
        }
    }
}
