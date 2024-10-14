using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.UseCases.AuthorCase
{
    public class GetAuthorByIdForBooksUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAuthorByIdForBooksUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public AuthorViewModel Execute(int? id, int page, int pageSize)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var author = _unitOfWork.Authors.GetById(id.Value);
            if (author == null)
            {
                return null;
            }

            var booksQuery = _unitOfWork.Books.GetBooksByAuthorId(author.Id);
            var totalBooks = booksQuery.Count();

            var books = booksQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var authorModel = _mapper.Map<AuthorModel>(author);
            authorModel.Books = _mapper.Map<List<BookModel>>(books);

            var viewModel = new AuthorViewModel
            {
                Author = authorModel,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize)
            };

            return viewModel;
        }
    }
}
