using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class GetFreeBooksUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetFreeBooksUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public (List<BookModel> Books, int TotalCount) Execute(int page, int pageSize)
        {
            var books = _unitOfWork.Books.GetFreeBooks();
            int totalCount = books.Count();

            var paginatedBooks = books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (Books: _mapper.Map<List<BookModel>>(paginatedBooks), TotalCount: totalCount);
        }
    }
}
