using Application.DTOs;
using AutoMapper;
using Domain.Interfaces.InterfacesForUOW;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Application.UseCases.BookCase
{
    public class UpdateBookUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBookUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ExecuteAsync(BookModel bookModel, IFormFile bookImage)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "Book model cannot be null.");
            }

            var existingBook = _unitOfWork.Books.GetById(bookModel.Id);
            if (existingBook == null)
            {
                throw new InvalidOperationException("Book not found.");
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
                bookModel.BookImage = existingBook.BookImage; 
            }

            bookModel.IssueDate = DateTime.Now;
            bookModel.ReturnDate = DateTime.Now;

            existingBook.Name = bookModel.Name;
            existingBook.AuthorId = await _unitOfWork.Authors.GetOrCreateAuthorAsync(bookModel.AuthorName, bookModel.AuthorLastName);
            existingBook.BookImage = bookModel.BookImage;
            existingBook.ReturnDate = bookModel.ReturnDate;
            existingBook.IssueDate = bookModel.IssueDate;
            existingBook.Description = bookModel.Description;
            existingBook.ISBN = bookModel.ISBN;
            existingBook.Genre = bookModel.Genre;

            _unitOfWork.Books.Update(existingBook);
            _unitOfWork.Complete();

            return (true, Enumerable.Empty<string>());
        }
    }
}
