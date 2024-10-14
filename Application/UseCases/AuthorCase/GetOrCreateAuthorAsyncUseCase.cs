using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.AuthorCase
{
    public class GetOrCreateAuthorAsyncUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrCreateAuthorAsyncUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Execute(string firstName, string lastName)
        {
            return await _unitOfWork.Authors.GetOrCreateAuthorAsync(firstName, lastName);
        }
    }
}


