using bookStoreApi.DTOs;
using bookStoreApi.Entities;
using bookStoreApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace bookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;


        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }


        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {

            var books = await _bookService.GetBooks();

            return Ok(books);
        }


        [HttpGet("/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {

            var books = await _bookService.GetBookById(id);
           

            return Ok(books);
        }

        [HttpGet("byName")]
        public async Task<IActionResult> GetBookByName([FromQuery]string name) {
            
            var books = await _bookService.GetBooksByName(name);
            return Ok(books);
        }

        [HttpGet("byAuthor")]
        public async Task<IActionResult> GetBookByAuthor([FromQuery] string name)
        {

            var books = await _bookService.GetBooksByAuthor(name);
            return Ok(books);
        }


        [HttpPost, Authorize]
        public async Task<IActionResult> CreateBook([FromBody]BookRequestDTO book)
        {
            if (ModelState.IsValid)
            {
                var bookResposne = await _bookService.CreateBook(book);
                return Ok(bookResposne);
            }
            return BadRequest();
        }



        [HttpPut("/{id}"),Authorize]
        public async Task<IActionResult> UpdateBook(int id,[FromBody] BookRequestDTO book)
        {
            if (id > 0) {
                if (ModelState.IsValid)
                {
                    var bookResposne = await _bookService.UpdateBook(id,book);

                    return Ok(bookResposne);
                }
                return BadRequest();
            }
            
            return BadRequest();
        }


        [HttpDelete,Authorize]
        public async Task<IActionResult> DeleteBook(int id) {
            if (id <= 0) 
            {
                await _bookService.DeleteBook(id);
                return Ok("Remoção realizada com sucesso!"); 
            }return BadRequest();
        }

    }
}
