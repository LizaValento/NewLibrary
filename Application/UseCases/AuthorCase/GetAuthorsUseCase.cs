using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.AuthorCase
{
    public class GetAuthorsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAuthorsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<AuthorModel> Execute()
        {
            var authors = _unitOfWork.Authors.GetAll();
            return _mapper.Map<List<AuthorModel>>(authors);
        }
    }
}

