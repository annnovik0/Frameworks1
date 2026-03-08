using Practice1.Models;
using System.Collections.Concurrent;

namespace Practice1.Repositories
{
    public class InMemoryBookRepository : IBookRepository
    {
        // ConcurrentDictionary обеспечивает потокобезопасность
        private readonly ConcurrentDictionary<int, Book> _books = new();
        private int _nextId = 1; // Простой генератор ID

        public Task<IEnumerable<Book>> GetAllAsync()
        {
            return Task.FromResult(_books.Values.AsEnumerable());
        }

        public Task<Book?> GetByIdAsync(int id)
        {
            _books.TryGetValue(id, out var book);
            return Task.FromResult(book);
        }

        public Task<Book> AddAsync(Book book)
        {
            // Атомарно получаем следующий ID и добавляем книгу
            // Interlocked.Increment гарантирует, что два параллельных запроса не получат одинаковый ID
            var id = Interlocked.Increment(ref _nextId);
            book.Id = id;

            if (!_books.TryAdd(id, book))
            {
                // Эта ситуация маловероятна при использовании Interlocked, но добавим проверку
                throw new InvalidOperationException("Не удалось добавить книгу из-за конфликта ID.");
            }

            return Task.FromResult(book);
        }
    }
}
