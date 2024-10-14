using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.UserCase
{
    public class UpdateUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateUserUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Execute(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "User model cannot be null.");
            }

            var userEntity = _mapper.Map<User>(userModel);
            _unitOfWork.Users.Update(userEntity);
            _unitOfWork.Complete();
        }
    }

}
