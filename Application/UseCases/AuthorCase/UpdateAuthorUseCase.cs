using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.AuthorCase
{
    public class UpdateAuthorUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAuthorUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                    throw new InvalidOperationException("Некорректная дата.");
                }
            }

            var authorEntity = _mapper.Map<Author>(authorModel);
            authorEntity.DateOfBirth = dateOfBirth;

            _unitOfWork.Authors.Update(authorEntity);
            _unitOfWork.Complete();
        }
    }
}