using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.UserCase
{
    public class RegisterUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(RegisterModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "Register model cannot be null.");
            }

            var existingUser = _unitOfWork.Users.GetByNickname(userModel.Nickname);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            var userEntity = new User
            {
                FirstName = userModel.FirstName,
                Password = userModel.Password,
                LastName = userModel.LastName,
                Nickname = userModel.Nickname,
                Role = "User"
            };

            _unitOfWork.Users.Add(userEntity);
            _unitOfWork.Complete();
        }
    }

}
