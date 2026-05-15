using System.ComponentModel.DataAnnotations;
using Practice1.Models;
using Xunit;

namespace Practice1.Tests
{
    public class BookValidationTests
    {
        private List<ValidationResult> ValidateModel(Book book)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(book);
            Validator.TryValidateObject(book, context, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Book_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var book = new Book
            {
                Title = "Война и мир",
                Author = "Лев Толстой",
                Year = 1869,
                PageCount = 1225
            };

            // Act
            var results = ValidateModel(book);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Book_WithEmptyTitle_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "",
                Author = "Толстой",
                Year = 1869,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("Название книги обязательно"));
        }

        [Fact]
        public void Book_WithTitleTooLong_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = new string('A', 201),
                Author = "Толстой",
                Year = 1869,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("от 1 до 200 символов"));
        }

        [Fact]
        public void Book_WithEmptyAuthor_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "Война и мир",
                Author = "",
                Year = 1869,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("Имя автора обязательно"));
        }

        [Fact]
        public void Book_WithAuthorTooLong_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "Война и мир",
                Author = new string('A', 101),
                Year = 1869,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("не может быть длиннее 100 символов"));
        }

        [Fact]
        public void Book_WithNegativeYear_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "Война и мир",
                Author = "Толстой",
                Year = -100,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("Год издания должен быть между 0 и 2100"));
        }

        [Fact]
        public void Book_WithYearOver2100_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "Война и мир",
                Author = "Толстой",
                Year = 2200,
                PageCount = 1225
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("Год издания должен быть между 0 и 2100"));
        }

        [Fact]
        public void Book_WithNegativePageCount_ShouldFailValidation()
        {
            var book = new Book
            {
                Title = "Война и мир",
                Author = "Толстой",
                Year = 1869,
                PageCount = -10
            };

            var results = ValidateModel(book);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("не может быть отрицательным"));
        }
    }
}