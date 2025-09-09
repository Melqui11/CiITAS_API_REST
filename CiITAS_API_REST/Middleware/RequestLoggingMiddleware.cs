using CiITAS_API_REST.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class SafeRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public SafeRequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        string requestBody = null;

        try
        {
            context.Request.EnableBuffering();

            if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leyendo request body: {ex.Message}");
        }

        var originalResponseBody = context.Response.Body;
        using var newResponseBody = new MemoryStream();
        context.Response.Body = newResponseBody;

        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en pipeline: {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
        }

        string responseBody = null;
        try
        {
            newResponseBody.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(newResponseBody).ReadToEndAsync();
            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leyendo response body: {ex.Message}");
        }

        try
        {
            var log = new requestLog
            {
                StatusCode = context.Response.StatusCode,
                Payload = string.IsNullOrWhiteSpace(requestBody) ? null : requestBody,
                Response = string.IsNullOrWhiteSpace(responseBody) ? null : responseBody,
                ExecutionDate = DateTime.Now
            };

            dbContext.RequestLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"No se pudo guardar el log: {ex.Message}");
        }
    }
}

public static class SafeRequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseSafeRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SafeRequestLoggingMiddleware>();
    }
}
