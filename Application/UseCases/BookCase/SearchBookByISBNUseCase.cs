using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class SearchBookByISBNUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchBookByISBNUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BookModel Execute(string isbn)
        {
            var book = _unitOfWork.Books.GetByISBN(isbn);
            return _mapper.Map<BookModel>(book);
        }
    }
}
