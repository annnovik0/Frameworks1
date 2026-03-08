using Microsoft.AspNetCore.Mvc;
using Practice1.Models;
using Practice1.Repositories;

namespace Practice1.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Маршрут будет /api/books
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        // 1. GET /api/books - возвращает список книг
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _repository.GetAllAsync();
            return Ok(books);
        }

        // 2. GET /api/books/{id} - возвращает одну книгу по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _repository.GetByIdAsync(id);

            // Если книга не найдена, вернется ответ 404, который будет обработан нашим middleware для ошибок
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // 3. POST /api/books - создает новую книгу
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            // Валидация данных происходит автоматически благодаря атрибутам [ApiController] и [Required]
            // Если модель невалидна, будет автоматически возвращен статус 400 Bad Request.
            // Это также будет обработано нашим middleware.

            var createdBook = await _repository.AddAsync(book);

            // Возвращаем статус 201 Created с ссылкой на новый ресурс
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
    }
}
