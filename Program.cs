using Practice1.Repositories;
using Practice1.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрируем репозиторий
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

var app = builder.Build();

// Настройка Swagger для разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Подключаем middleware
app.UseMiddleware<ErrorHandlingMiddleware>();      // 1. Обработка ошибок
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // 2. Логирование
app.UseMiddleware<RequestTimingMiddleware>();      // 3. Замер времени


//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }