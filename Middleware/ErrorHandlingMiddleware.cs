using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Practice1.Middleware
{
    /// <summary>
    /// Middleware для единой обработки ошибок
    /// Ловит все исключения и возвращает клиенту красивый JSON с RequestId
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Передаем управление следующему middleware
                // Весь конвейер будет внутри этого try-catch!
                await _next(context);
            }
            catch (Exception ex)
            {
                // Сюда попадают ВСЕ исключения из любых middleware и контроллеров
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Получаем уникальный ID запроса
            var requestId = context.TraceIdentifier;

            // Логируем ошибку с этим ID
            _logger.LogError(exception, "Ошибка при обработке запроса {RequestId}", requestId);

            // Формируем единый формат ответа об ошибке
            var errorResponse = new
            {
                Code = "InternalServerError",
                Message = "Произошла внутренняя ошибка сервера. Пожалуйста, попробуйте позже.",
                RequestId = requestId  // ← Тот же ID, что в логах!
            };

            // Устанавливаем тип ответа как JSON
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Отправляем ответ клиенту
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}