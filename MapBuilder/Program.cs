using System.Net.Http;
using System.Text;
using System.Text.Json;
using Google.Common.Geometry;

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
        
        app.MapGet("/getcells/{level}/{latitude1}/{longitude1}/{latitude2}/{longitude2}", (HttpContext httpContext, int level, double latitude1, double longitude1,double latitude2, double longitude2) =>
            {
                S2RegionCoverer cov = new S2RegionCoverer();
                cov.MaxLevel=level; // Set the maximum level
                cov.MinLevel=level; // Set the minimum level
                ///getcells/15/37.7749/-122.4194/37.7858/-122.3864
                S2LatLngRect regionRect = new S2LatLngRect(
                    new S2LatLng(S1Angle.FromDegrees(latitude1), S1Angle.FromDegrees(longitude1)), // Bottom-left corner
                    new S2LatLng(S1Angle.FromDegrees(latitude2), S1Angle.FromDegrees(longitude2)) // Top-right corner
                );
                
                List<S2CellId> cells = new List<S2CellId>();
                
                cov.GetCovering(regionRect, cells);
                string ids = "";
                foreach (S2CellId cell in cells)
                {
                    ids += cell.ToToken()+", ";
                }
                return ids;
            })
            .WithName("Get Cells")
            .WithOpenApi();

        app.Run();
    }
}