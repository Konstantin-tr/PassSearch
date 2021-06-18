using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;
using MudBlazor.Services;
using PasswordQueryTool.ParseWebApp.Client.Services;

namespace PasswordQueryTool.ParseWebApp.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:444/") });

            builder.Services.AddScoped<IImportProvider, ApiImportProvider>();

            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
