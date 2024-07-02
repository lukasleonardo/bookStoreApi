using bookStoreApi.Data;
using bookStoreApi.DTOs;
using bookStoreApi.Entities;
using bookStoreApi.Handler;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using Assert = Xunit.Assert;

namespace bookStoreApi.Services.Tests
{
    
    public class BookServiceTests
    {
        private readonly BookService _bookService;
        private readonly Mock<ILogger<BookService>> _mockLogger;
        private readonly DataContext _dataContext;

        public BookServiceTests()
        {
            _dataContext = CreateInMemoryDbContext();
            _mockLogger = new Mock<ILogger<BookService>>();
            _bookService = new BookService(_dataContext, _mockLogger.Object);
        }

        private DataContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                          .UseInMemoryDatabase(Guid.NewGuid().ToString())
                          .Options;
            return new DataContext(options);
        }

        [Fact(DisplayName = "GetAll List item found")]
        public async Task GetBooksTestSuccessAsync()
        {

            await this.Setup();

            var result = await _bookService.GetBooks();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact(DisplayName ="GetAll List itens not found")]
        public async Task GetBooks_NotFoundException()
        {
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _bookService.GetBooks());
            Assert.NotNull(exception);
            Assert.Equal("Nenhum livro encontrado.", exception.Message);
        }

        [Fact(DisplayName ="GetById item found")]
        public async Task GetBookByIdTestSuccess()
        {
            int id = 1;
            var books = new List<Book>
            {
                new Book { Id=id, Title = "Book 1" , Author = "Autor 1"},
                new Book { Id=2, Title = "Book 2",  Author= "Autor 2" }
            };

            _dataContext.books.AddRange(books);
            await _dataContext.SaveChangesAsync();

            var result = await _bookService.GetBookById(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact(DisplayName = "GetbyId No item found")]
        public async Task GetBookById_NotFoundException()
        {
            var id = 1;

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _bookService.GetBookById(id));
            Assert.NotNull(exception);
            Assert.Equal("Livro não encontrado!", exception.Message);
        }

        
        [Fact(DisplayName = "GetbyName item Found")]
        public async Task GetBooksByNameTestSuccess()
        {
            var name = "Book";
            var books = await this.Setup();

            var result = await _bookService.GetBooksByName(name);
            
            Assert.NotNull(result);
            Assert.All(result, book => Assert.Contains( name, book.Title, StringComparison.OrdinalIgnoreCase));
         

            var expectedBooks = books.Where(b => b.Title.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            Assert.Equal(expectedBooks.Count, result.Count);
        }

        [Fact(DisplayName = "GetbyName No item Found")]
        public async Task GetBooksByName_NotFound()
        {
            var name = "Recipe";
            var books = await this.Setup();

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _bookService.GetBooksByName(name));
            

            Assert.NotNull(exception);
            Assert.Equal("Não foi encontrado nenhum livro com este nome: " + name, exception.Message);
        }

    

        [Fact(DisplayName ="GetAuthor items with name found")]
        public async Task GetBooksByAuthorTest()
        {
            var name = "Autor";
            var books = await this.Setup();

            var result = await _bookService.GetBooksByAuthor(name);

            Assert.NotNull(result);
            Assert.All(result, book => Assert.Contains(name, book.Author, StringComparison.OrdinalIgnoreCase));

            var expectedBooks = books.Where(b => b.Author.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            Assert.Equal(expectedBooks.Count, result.Count);
        }

        [Fact(DisplayName ="CreateBook sucessfully create a book")]
        public async Task CreateBookTestSucess()
        {
            var bookDto =  new BookRequestDTO { Title = "Book1", Author = "Autor 1", Description = "Generic Description", Pages = 100 };
            var book = new Book { Id=1, Title = "Book1", Author = "Autor 1", Description = "Generic Description", Pages = "100" };
            
        
            var expectedBook = await _bookService.CreateBook(bookDto);


            Assert.NotNull(expectedBook);
            Assert.True(HasProperty("Id",expectedBook));
            Assert.Equal(book.Title, expectedBook.Title);
            Assert.Equal(book.Author, expectedBook.Author);
            
        }

        [Fact(DisplayName = "CreateBook failed to create a book")]
        public async Task CreateBookTestFail() {
            await this.Setup();

            var bookDto = new BookRequestDTO { Title = "Book 1", Author = "Autor 1", Description = "Generic Description", Pages = 100 };
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _bookService.CreateBook(bookDto));

            Assert.NotNull(exception);
            Assert.Equal("Este título ja foi cadastrado no sistema.", exception.Message);
        }

        [Fact(DisplayName ="UpdateBook Successfuly")]
        public async Task UpdateBookTestSucess()
        {
            var id = 1;
            await this.Setup();
            var bookDto = new BookRequestDTO { Title = "Livro 1", Author = "Desconhecido", Pages = 100 };
            var expectedBook = await _bookService.UpdateBook(id, bookDto);

            Assert.NotNull(expectedBook);
            Assert.Equal(bookDto.Title, expectedBook.Title);
            Assert.Equal(bookDto.Author, expectedBook.Author);
            Assert.Equal("Something", expectedBook.Description);
            Assert.Equal(bookDto.Pages.ToString(), expectedBook.Pages);

        }

        [Fact(DisplayName ="UpdateBook Failed to update")]
        public async Task UpdateBookTest()
        {
            var id = 9;
            await this.Setup();
            var bookDto = new BookRequestDTO { Title = "Livro 1", Author = "Desconhecido", Pages = 100 };
            var exception = await Assert.ThrowsAsync<Exception>(() => _bookService.UpdateBook(id, bookDto));

            Assert.NotNull(exception);
            Assert.Equal("Falha ao atualizar os dados do livro.",exception.Message);
            
        }

        [Fact(DisplayName ="DeleteBook Sucessfuly delete a book")]
        public async Task DeleteBookTestSuccessfully()
        {
            var id = 1;
            await this.Setup();
            await _bookService.DeleteBook(id);
            var expectedBooks = await _bookService.GetBooks();

            Assert.NotNull(expectedBooks);
            Assert.Equal(2, expectedBooks.Count);

        }

        [Fact(DisplayName = "DeleteBook failed to delete a book")]
        public async Task DeleteBookTestFail()
        {
            var id = 10;
            await this.Setup();

            var exception = await Assert.ThrowsAsync<Exception>(() => _bookService.DeleteBook(id));
            Assert.NotNull(exception);
            Assert.Equal("Falha ao excluir livro indicado.", exception.Message);
        }
        
        public async Task<List<Book>> Setup()
        {
            var books = new List<Book>
            {
                new Book { Id=1, Title = "Book 1" ,Description= "Something" ,Author = "Autor 1"},
                new Book { Id=2, Title = "Book 2",  Author= "Autor 2" },
                new Book { Id=3, Title = "Manuscript 2",  Author= "noname 2" }
            };
            _dataContext.books.AddRange(books);
            await _dataContext.SaveChangesAsync();  
            return books;
        }

        public bool HasProperty(string prop, object obj)
        {
            return obj.GetType().GetProperty(prop) != null;
        }
    }
}