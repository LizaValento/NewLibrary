using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class GetBooksForPaginationUseCase
    {
        private readonly GetFreeBooksUseCase _getFreeBooksUseCase;

        public GetBooksForPaginationUseCase(GetFreeBooksUseCase getFreeBooksUseCase)
        {
            _getFreeBooksUseCase = getFreeBooksUseCase;
        }

        public BooksViewModel Execute(int page, int pageSize)
        {
            var (books, totalCount) = _getFreeBooksUseCase.Execute(page, pageSize);

            var model = new BooksViewModel
            {
                Books = books,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return model;
        }
    }
}
