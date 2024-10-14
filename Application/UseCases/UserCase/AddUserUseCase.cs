using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;

public class AddUserUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddUserUseCase(IUnitOfWork unitOfWork, IMapper mapper)
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
        _unitOfWork.Users.Add(userEntity);
        _unitOfWork.Complete();
    }
}
