public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Authorization Header: {authHeader}");

            // Extract and log the token part
            var token = authHeader.Replace("Bearer ", "").Trim();
            Console.WriteLine($"Extracted Token: {token}");
        }

        await _next(context);
    }
}
