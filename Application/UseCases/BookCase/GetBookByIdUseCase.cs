using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class GetBookByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBookByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BookModel Execute(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var book = _unitOfWork.Books.GetById(id.Value);
            return book == null ? null : _mapper.Map<BookModel>(book);
        }
    }
}
