using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MapBuilder;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/query/{type}", (HttpContext httpContext, string type) =>
            {
                string apiUrl = "https://overpass-api.de/api/interpreter";
                string query = $"[maxsize:1073741824][timeout:900];{type}(51.15,7.12,51.16,7.13);out;";
                
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;
                    return response.Content.ReadAsStringAsync().Result;
                }
            })
            .WithName("Query")
            .WithOpenApi();

        app.Run();
    }
}