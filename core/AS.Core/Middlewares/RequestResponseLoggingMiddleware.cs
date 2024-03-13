using Microsoft.AspNetCore.Http;
using Serilog;

namespace AS.Core.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    Log.Information($"Request: {context.Request.Method} {context.Request.Path}");

                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();
                    Log.Information($"Response: {context.Response.StatusCode} {responseBodyContent}");

                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Request: {context.Request.Method} {context.Request.Path}");
                Log.Error($"Exception: {ex.Message}");
            }
        }
    }
}
