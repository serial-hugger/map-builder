using MapBuilder.Web.Components;

namespace MapBuilder.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddScoped<WebService>();

        var app = builder.Build();
        
        //Configure WebService
        var scope = app.Services.CreateScope();
        var MyService = scope.ServiceProvider.GetService<WebService>();

        // Middleware configuration
        app.Use(async (context, next) =>
        {
            if (context.Request.Query.ContainsKey("conditionKey") && context.Request.Query["conditionKey"] == "something")
            {
                MyService.LoadDefaults();
            }
            await next();
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}