using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Application.UseCases.BookCase
{
    public class GetAllBooksByGenresUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllBooksByGenresUseCase(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Execute(string genre)
        {
            var books = _unitOfWork.Books.GetBooksByGenre(genre).ToList();
            var bookModels = _mapper.Map<List<BookModel>>(books);

            var booksJson = JsonConvert.SerializeObject(bookModels);
            _httpContextAccessor.HttpContext.Session.SetString("FilteredBooks", booksJson);
        }
    }
}
