using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using System.Security.Claims;

namespace Application.UseCases.UserCase
{
    public class GetUserBooksUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserBooksUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BooksViewModel Execute(ClaimsPrincipal user, int page, int pageSize)
        {
            int userId = Convert.ToInt32(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var (books, totalCount) = _unitOfWork.Users.GetUserBooks(userId, page, pageSize);

            var bookModels = _mapper.Map<List<BookModel>>(books);

            var model = new BooksViewModel
            {
                Books = bookModels,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return model;
        }
    }
}
