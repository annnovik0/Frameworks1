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
            // Запускаем секундомер ДО выполнения следующего middleware
            var stopwatch = Stopwatch.StartNew();

            // Передаем управление дальше по конвейеру
            await _next(context);

            // Останавливаем секундомер ПОСЛЕ того, как все middleware сработали
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Добавляем время выполнения в заголовки ответа
            // Клиент сможет увидеть, сколько времени занял запрос
            context.Response.Headers.TryAdd("X-Response-Time-Ms", elapsedMs.ToString());

            // Также пишем в лог (но это уже сделает следующий middleware)
            _logger.LogDebug("Запрос {RequestId} выполнен за {ElapsedMs} мс",
                context.TraceIdentifier, elapsedMs);
        }
    }
}