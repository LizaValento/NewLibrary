using Application.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.UseCases.BookCase
{
    public class GetPagedBooksUseCase
    {
        public BooksViewModel Execute(string booksJson, int page, int pageSize)
        {
            List<BookModel> foundBooks;

            if (!string.IsNullOrEmpty(booksJson))
            {
                foundBooks = JsonConvert.DeserializeObject<List<BookModel>>(booksJson);
            }
            else
            {
                return new BooksViewModel();
            }

            var totalCount = foundBooks.Count;
            var pagedBooks = foundBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new BooksViewModel
            {
                Books = pagedBooks,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}
