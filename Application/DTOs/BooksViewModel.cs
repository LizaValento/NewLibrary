using Domain.Entities;

namespace Application.DTOs
{
    public class BooksViewModel
    {
        public List<BookModel> Books { get; set; }
        public BookModel? Book { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
