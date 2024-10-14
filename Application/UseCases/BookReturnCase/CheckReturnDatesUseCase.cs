using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookReturnCase
{
    public class CheckReturnDatesUseCase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public CheckReturnDatesUseCase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void Execute()
        {
            using var unitOfWork = _unitOfWorkFactory.Create();
            var overdueBooks = unitOfWork.Books.GetAll()
                .Where(b => b.ReturnDate < DateTime.Now && b.UserId != null)
                .ToList();

            foreach (var book in overdueBooks)
            {
                book.UserId = null; 
                unitOfWork.Books.Update(book);
            }

            unitOfWork.Complete();
        }
    }
}


