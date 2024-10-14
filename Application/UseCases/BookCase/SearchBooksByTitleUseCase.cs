using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class SearchBooksByTitleUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchBooksByTitleUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<BookModel> Execute(string title)
        {
            var books = _unitOfWork.Books.GetAllWithTitles(title);
            return _mapper.Map<List<BookModel>>(books);
        }
    }
}
