using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using System.Collections.Generic;
using System.Linq;

namespace Application.UseCases.AuthorCase
{
    public class GetAuthorsPaginationUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAuthorsPaginationUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public AuthorViewModel Execute(int page, int pageSize)
        {
            var authors = _unitOfWork.Authors.GetAll();
            int totalCount = authors.Count();

            var paginatedAuthors = authors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var authorModels = _mapper.Map<List<AuthorModel>>(paginatedAuthors);

            var viewModel = new AuthorViewModel
            {
                Authors = authorModels,
                CurrentPage = page,
                TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize)
            };

            return viewModel;
        }
    }
}
