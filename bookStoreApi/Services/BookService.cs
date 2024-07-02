using bookStoreApi.Data;
using bookStoreApi.Entities;
using bookStoreApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net;
using bookStoreApi.DTOs;
using bookStoreApi.Handler;

namespace bookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<BookService> _logger;

        public BookService(DataContext dataContext, ILogger<BookService> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<List<Book>> GetBooks()
        {
            try
            {
                var books = await _dataContext.books.ToListAsync();
                if(books == null || books.Count == 0)
                {
                    throw new NotFoundException("Nenhum livro encontrado.");
                }
                return books;

            }
            catch (NotFoundException) { throw; }

        }


        public async Task<Book> GetBookById(int id)
        {
            try
            {   
                var book = await _dataContext.books.FindAsync(id);
                if (book == null)
                {
                    throw new NotFoundException("Livro não encontrado!");
                }
                return book;
            }
            catch (NotFoundException) { throw; }             
        }


        public async Task<List<Book>> GetBooksByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ServiceException("Argumento inválido ou nulo");
                }
                var books = await _dataContext.books.Where( b => b.Title.Contains(name)).ToListAsync();
                if (books == null || books.Count == 0 )
                {
                    throw new NotFoundException("Não foi encontrado nenhum livro com este nome: "+ name);
                }
                return books;
            }
            catch(ServiceException) { throw; }
            catch(NotFoundException)
            {
                throw;
            }
            catch (Exception ex) 
            {
                throw new Exception("Falha ao obter lista de livros", ex);
            }
        }

        public async Task<List<Book>> GetBooksByAuthor(string name)
        {
            try
            {
                var books = await _dataContext.books.Where(b=> b.Author.Contains(name)).ToListAsync();
                if(books == null || books.Count == 0)
                {
                    throw new NotFoundException("Livro não encontrado com este autor: "+name);
                }
                return books;
            }catch(NotFoundException) { throw; }
            catch (Exception ex)
            {
                
                throw new Exception("Falha ao obter lista de livros", ex);
            }
        }

        public async Task<Book> CreateBook(BookRequestDTO book)
        {
            try
            {
                if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
                {
                    throw new ServiceException("Titulo e ou Autor ausentes na requisição.");
                }
                if (await _dataContext.books.AnyAsync(b => b.Title == book.Title))
                {
                    throw new ServiceException("Este título ja foi cadastrado no sistema.");
                }


                var newBook = new Book
                {
                    Title = book.Title,
                    Url = book.Url,
                    Description = book.Description,
                    Pages = book.Pages.ToString(),
                    Author = book.Author,
                };


                await _dataContext.books.AddAsync(newBook);
                await _dataContext.SaveChangesAsync();

                return newBook;
            }
            catch (ServiceException) { throw; }
 
            
        }

        public async Task<Book> UpdateBook(int id,BookRequestDTO book) 
        {
            try
            {
                var uptBook = await this.GetBookById(id);

                if (!string.IsNullOrEmpty(book.Title))
                {
                    uptBook.Title = book.Title;
                }
                if (!string.IsNullOrEmpty(book.Author))
                {
                    uptBook.Author = book.Author;
                }
                if (!string.IsNullOrEmpty(book.Description))
                {
                    uptBook.Description = book.Description;
                }
                if (!string.IsNullOrEmpty(book.Pages.ToString()))
                {
                    uptBook.Pages = book.Pages.ToString();
                }
                if (!string.IsNullOrEmpty(book.Url))
                {
                    uptBook.Pages = book.Url;
                }

                await _dataContext.SaveChangesAsync();

                return uptBook;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar os dados do livro.",ex);
            }

            
        }


        public async Task DeleteBook(int id)
        {
            try
            {
                var uptBook = await this.GetBookById(id);
                _dataContext.Remove(uptBook);
                await _dataContext.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir livro indicado.", ex);
            }
        }
    }
}
