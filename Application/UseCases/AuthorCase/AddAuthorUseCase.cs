using Application.DTOs;
using Domain.Entities;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using FluentValidation;
using System;

namespace Application.UseCases.AuthorCase
{
    public class AddAuthorUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<AuthorModel> _authorValidator;

        public AddAuthorUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AuthorModel> authorValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authorValidator = authorValidator;
        }

        public void Execute(AuthorModel authorModel)
        {
            if (authorModel == null)
            {
                throw new ArgumentNullException(nameof(authorModel), "Author model cannot be null.");
            }
            
            DateTime? dateOfBirth = null;
            if (!string.IsNullOrEmpty(authorModel.DateOfBirth))
            {
                if (DateTime.TryParse(authorModel.DateOfBirth, out var parsedDate))
                {
                    dateOfBirth = parsedDate;
                }
                else
                {
                    throw new InvalidOperationException("Некорректная дата рождения.");
                }
            }

            var newAuthor = new Author
            {
                FirstName = authorModel.FirstName,
                LastName = authorModel.LastName,
                Country = authorModel.Country,
                DateOfBirth = dateOfBirth
            };

            _unitOfWork.Authors.Add(newAuthor);
            _unitOfWork.Complete();
        }
    }
}
