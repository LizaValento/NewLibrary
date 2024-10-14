using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.UserCase
{
    public class GetUserByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserModel> ExecuteAsync(int? id) // Изменена сигнатура на асинхронную
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(id.Value); // Предполагается, что метод асинхронный
            return user == null ? null : _mapper.Map<UserModel>(user);
        }
        public UserModel Execute(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var user = _unitOfWork.Users.GetById(id.Value);
            return user == null ? null : _mapper.Map<UserModel>(user);
        }
    }
}
