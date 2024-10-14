using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.AuthorCase
{
    public class DeleteAuthorUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAuthorUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(int id)
        {
            var author = _unitOfWork.Authors.GetById(id);
            if (author != null)
            {
                var books = _unitOfWork.Books.GetBooksByAuthorId(id);
                foreach (var book in books)
                {
                    _unitOfWork.Books.Remove(book);
                }

                _unitOfWork.Authors.Remove(author);
                _unitOfWork.Complete();
            }
        }
    }
}

    



