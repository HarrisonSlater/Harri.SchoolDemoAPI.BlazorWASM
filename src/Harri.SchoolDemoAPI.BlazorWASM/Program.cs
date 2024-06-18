using Harri.SchoolDemoAPI.BlazorWASM;
using Harri.SchoolDemoApi.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var url = builder.Configuration["StudentDemoAPIUrl"];

if (url is null)
{
    throw new ArgumentException("StudentDemoAPIUrl must be set in appsettings.json");
}

var uri = new Uri(url);
builder.Services.AddSingleton<IStudentApiClient>(sp => new StudentApiClient(new HttpClient() { BaseAddress = uri }));
builder.Services.AddMudServices();

await builder.Build().RunAsync();
