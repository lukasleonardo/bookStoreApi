using bookStoreApi.DTOs;

namespace bookStoreApi.Entities
{
    public class Book
    {
       public int Id { get; set; }
       public required string Title { get; set; }
       public  string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
       public string Pages { get; set; } = string.Empty;
       public required string Author { get; set; }

        public Book() { }
        public Book(BookRequestDTO book)
        {
            Title = book.Title;
            Url = book.Url;
            Description = book.Description;
            Pages = book.Pages.ToString();
            Author = book.Author;
        }
    }
}
