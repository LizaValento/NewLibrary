using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.UserCase
{
    public class GetUsersUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<UserModel> Execute()
        {
            var users = _unitOfWork.Users.GetAll();
            return _mapper.Map<List<UserModel>>(users);
        }
    }

}
