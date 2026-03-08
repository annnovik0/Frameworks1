using System.ComponentModel.DataAnnotations;

namespace Practice1.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название книги обязательно.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Название должно быть от 1 до 200 символов.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Имя автора обязательно.")]
        [StringLength(100, ErrorMessage = "Имя автора не может быть длиннее 100 символов.")]
        public string? Author { get; set; }

        [Range(0, 2100, ErrorMessage = "Год издания должен быть между 0 и 2100.")]
        public int Year { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Количество страниц не может быть отрицательным.")]
        public int PageCount { get; set; }
    }
}


