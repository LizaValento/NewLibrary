using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class GetAllBooksByAuthorUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllBooksByAuthorUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<BookModel> Execute(string firstName, string lastName)
        {
            var books = _unitOfWork.Books.GetBooksByAuthorNameAndLastName(firstName, lastName).ToList();
            return _mapper.Map<List<BookModel>>(books);
        }
    }
}
