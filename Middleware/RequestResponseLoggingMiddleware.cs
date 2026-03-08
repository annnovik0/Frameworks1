namespace Practice1.Middleware
{
    /// <summary>
    /// Middleware для логирования запросов и ответов
    /// Пишет в журнал информацию о входящем запросе и исходящем ответе
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;
            var request = context.Request;

            // 🔵 ЛОГИРУЕМ ВХОДЯЩИЙ ЗАПРОС
            _logger.LogInformation("[{RequestId}] Входящий запрос: {Method} {Path}{QueryString}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString);

            // Перехватываем ответ, чтобы его тоже залогировать
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Передаем управление дальше по конвейеру
                await _next(context);

                // 🟢 ЛОГИРУЕМ ИСХОДЯЩИЙ ОТВЕТ
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation("[{RequestId}] Исходящий ответ: Статус {StatusCode}, Тело: {ResponseBody}",
                    requestId,
                    context.Response.StatusCode,
                    responseBodyText);

                // Копируем ответ обратно в исходный поток
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Восстанавливаем оригинальный поток ответа
                context.Response.Body = originalBodyStream;
            }
        }
    }
}