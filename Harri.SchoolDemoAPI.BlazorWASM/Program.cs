using Harri.SchoolDemoAPI.BlazorWASM;
using Harri.SchoolDemoApi.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Configuration["StudentDemoAPIUrl"]
var uri = new Uri(builder.Configuration["StudentDemoAPIUrl"]);
builder.Services.AddSingleton<IStudentApiClient>(sp => new StudentApiClient(new HttpClient() { BaseAddress = uri }));
builder.Services.AddMudServices();



await builder.Build().RunAsync();
