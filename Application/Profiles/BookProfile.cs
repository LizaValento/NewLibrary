using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Application.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookModel, Book>();
            CreateMap<Book, BookModel>();
        }
    }
}
