using Domain.Entities;

namespace Application.DTOs
{
    public class BookSearchViewModel
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string SelectedGenre { get; set; }
        public int? SelectedAuthorId { get; set; }

        public List<string> Genres { get; set; }
        public List<AuthorModel> Authors { get; set; }
        public List<BookModel> FoundBooksByTitle { get; set; }
        public BookModel FoundBookByISBN { get; set; }
    }
}
