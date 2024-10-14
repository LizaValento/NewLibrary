using System.Security.Claims;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class TakeBookUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TakeBookUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Execute(int bookId, ClaimsPrincipal user) 
        {
            var existingBook = _unitOfWork.Books.GetById(bookId);
            if (existingBook == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            int userId = Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier));

            existingBook.UserId = userId;
            existingBook.IssueDate = DateTime.Now;
            existingBook.ReturnDate = DateTime.Now.AddDays(7);

            _unitOfWork.Books.Update(existingBook);
            _unitOfWork.Complete();
        }
    }
}
