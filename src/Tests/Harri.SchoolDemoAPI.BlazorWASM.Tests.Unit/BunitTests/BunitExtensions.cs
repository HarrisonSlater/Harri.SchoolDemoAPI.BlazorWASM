using AngleSharp.Dom;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    public static class BunitExtensions
    {
        public static async Task<IElement> FindAndClickAsync<T>(this IRenderedComponent<T> renderedComponent, string selector) where T : IComponent
        {
            var button = renderedComponent.Find(selector);
            button.IsDisabled().Should().BeFalse();
            await button.ClickAsync(new MouseEventArgs());

            return button;
        }
    }
}
