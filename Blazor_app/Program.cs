using Blazor_app.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PRAM_lib.Machine;

namespace Blazor_app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //dependency injection for the PRAM machine
            builder.Services.AddSingleton<PramMachine>();

            //dependency injection for the code editor
            builder.Services.AddSingleton<CodeEditorService>();

            //dependency injection for the global service
            builder.Services.AddSingleton<GlobalService>();

            //dependency injection for the pram code view service
            builder.Services.AddSingleton<PramCodeViewService>();

            //dependency injection for the history memory service
            builder.Services.AddSingleton<HistoryMemoryService>();

            //dependency injection for the execution service
            builder.Services.AddSingleton<ExecutionService>(sp =>
            {
                var pramMachine = sp.GetRequiredService<PramMachine>();
                var codeEditorService = sp.GetRequiredService<CodeEditorService>();
                var pramCodeViewService = sp.GetRequiredService<PramCodeViewService>();
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                var globalService = sp.GetRequiredService<GlobalService>();
                var historyMemoryService = sp.GetRequiredService<HistoryMemoryService>();
                return new ExecutionService(pramMachine, 
                    codeEditorService, 
                    pramCodeViewService, 
                    navigationManager, 
                    globalService, 
                    historyMemoryService);
            });

            await builder.Build().RunAsync();
        }
    }
}
