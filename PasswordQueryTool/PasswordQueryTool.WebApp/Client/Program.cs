using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PasswordQueryTool.WebApp.Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PasswordQueryTool.WebApp.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IQueryService, ApiQueryService>();
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}