using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Domain.Entities;
using FluentValidation;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.BookCase
{
    public class AddBookUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<BookModel> _bookValidator;

        public AddBookUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<BookModel> bookValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookValidator = bookValidator;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ExecuteAsync(BookModel bookModel, IFormFile bookImage)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "Book model cannot be null.");
            }

            var validationResult = _bookValidator.Validate(bookModel);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return (false, errors);
            }

            if (bookImage != null && bookImage.Length > 0)
            {
                var fileName = Path.GetFileName(bookImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bookImage.CopyToAsync(stream);
                }

                bookModel.BookImage = $"/images/{fileName}";
            }
            else
            {
                bookModel.BookImage = "/images/DefaultBook.jpg";
            }

            bookModel.IssueDate = DateTime.Now;
            bookModel.ReturnDate = DateTime.Now;

            int authorId = await _unitOfWork.Authors.GetOrCreateAuthorAsync(bookModel.AuthorName, bookModel.AuthorLastName);

            var bookEntity = _mapper.Map<Book>(bookModel);
            bookEntity.AuthorId = authorId;

            _unitOfWork.Books.Add(bookEntity);

            _unitOfWork.Complete();

            return (true, Enumerable.Empty<string>());
        }
    }
}
