using Bunit;
using NUnit.Framework;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    /// <summary>
    /// Test context wrapper for bUnit.
    /// Read more about using <see cref="BunitTestContext"/> <seealso href="https://bunit.dev/docs/getting-started/writing-tests.html#remove-boilerplate-code-from-tests">here</seealso>.
    /// </summary>
    public abstract class BunitTestContext : TestContextWrapper
    {
        [SetUp]
        public void BaseSetup() => TestContext = new Bunit.TestContext();

        [TearDown]
        public void BaseTearDown() => TestContext?.Dispose();
    }
}
