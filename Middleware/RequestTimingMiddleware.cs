using System.Diagnostics;

namespace Practice1.Middleware
{
    /// <summary>
    /// Middleware для замера времени выполнения запроса
    /// Добавляет заголовок X-Response-Time-Ms с временем в миллисекундах
    /// </summary>
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (!context.Response.HasStarted)
            {
                context.Response.Headers.TryAdd("X-Response-Time-Ms", elapsedMs.ToString());
            }

            // ⭐ ЭТА СТРОЧКА ДОЛЖНА БЫТЬ ДЛЯ ЛОГИРОВАНИЯ ВРЕМЕНИ
            _logger.LogInformation("Запрос {RequestId} выполнен за {ElapsedMs} мс",
                context.TraceIdentifier, elapsedMs);
        }
    }
}