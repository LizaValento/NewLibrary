using Application.DTOs;
using Application.UseCases.AuthorCase;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.InterfacesForUOW;
using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Application.Tests.UseCases
{
    public class AuthorUseCasesTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<AuthorModel>> _authorValidatorMock;

        public AuthorUseCasesTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _authorValidatorMock = new Mock<IValidator<AuthorModel>>();
        }

        [Fact]
        public void AddAuthor_ShouldAddAuthor_WhenValidModel()
        {
            var authorModel = new AuthorModel { FirstName = "John", LastName = "Doe", Country = "Belarus", DateOfBirth = "1990-01-01" };
            var author = new Author();

            _authorValidatorMock.Setup(v => v.Validate(It.IsAny<AuthorModel>())).Returns(new FluentValidation.Results.ValidationResult());

            _unitOfWorkMock.Setup(u => u.Authors).Returns(new Mock<IAuthorRepository>().Object); 
            var useCase = new AddAuthorUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _authorValidatorMock.Object);

            useCase.Execute(authorModel);

            _unitOfWorkMock.Verify(u => u.Authors.Add(It.IsAny<Author>()), Times.Once); 
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once); 
        }

    [Fact]
        public void AddAuthor_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            var useCase = new AddAuthorUseCase(_unitOfWorkMock.Object, _mapperMock.Object, _authorValidatorMock.Object);

            var exception = Assert.Throws<ArgumentNullException>(() => useCase.Execute(null));
            Assert.Equal("Author model cannot be null. (Parameter 'authorModel')", exception.Message);
        }

        [Fact]
        public void DeleteAuthor_ShouldRemoveAuthor_WhenExists()
        {
            var authorId = 1;
            var existingAuthor = new Author { Id = authorId, FirstName = "John", LastName = "Doe" };

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns(existingAuthor);
            _unitOfWorkMock.Setup(u => u.Books.GetBooksByAuthorId(authorId)).Returns(new List<Book>());

            var useCase = new DeleteAuthorUseCase(_unitOfWorkMock.Object);
            useCase.Execute(authorId);

            _unitOfWorkMock.Verify(u => u.Authors.Remove(existingAuthor), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public void DeleteAuthor_ShouldNotRemoveAuthor_WhenDoesNotExist()
        {
            var authorId = 1;

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns((Author)null);

            var useCase = new DeleteAuthorUseCase(_unitOfWorkMock.Object);
            useCase.Execute(authorId);

            _unitOfWorkMock.Verify(u => u.Authors.Remove(It.IsAny<Author>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Never);
        }

        [Fact]
        public void GetAuthorByIdUseCase_ShouldReturnAuthorModel_WhenExists()
        {
            var authorId = 1;
            var existingAuthor = new Author { Id = authorId, FirstName = "John", LastName = "Doe" };
            var authorModel = new AuthorModel { Id = authorId, FirstName = "John", LastName = "Doe" };

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns(existingAuthor);
            _mapperMock.Setup(m => m.Map<AuthorModel>(existingAuthor)).Returns(authorModel);

            var useCase = new GetAuthorByIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = useCase.Execute(authorId);

            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public void GetAuthorByIdUseCase_ShouldReturnNull_WhenDoesNotExist()
        {
            var authorId = 1;

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns((Author)null);

            var useCase = new GetAuthorByIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = useCase.Execute(authorId);

            Assert.Null(result);
        }

        [Fact]
        public void GetAuthorByIdUseCase_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            var useCase = new GetAuthorByIdUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            var exception = Assert.Throws<ArgumentNullException>(() => useCase.Execute(null));

            Assert.Equal("Id cannot be null. (Parameter 'id')", exception.Message);
        }

        [Fact]
        public void GetAuthorByIdForBooksUseCase_ShouldReturnViewModel_WhenAuthorExists()
        {
            var authorId = 1;
            var existingAuthor = new Author { Id = authorId, FirstName = "John", LastName = "Doe" };
            var authorModel = new AuthorModel { Id = authorId, FirstName = "John", LastName = "Doe" };
            var books = new List<Book>
            {
                new Book { Id = 1, Name = "Book 1", AuthorId = authorId },
                new Book { Id = 2, Name = "Book 2", AuthorId = authorId }
            };

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns(existingAuthor);
            _unitOfWorkMock.Setup(u => u.Books.GetBooksByAuthorId(authorId)).Returns(books.AsQueryable());
            _mapperMock.Setup(m => m.Map<AuthorModel>(existingAuthor)).Returns(authorModel);
            _mapperMock.Setup(m => m.Map<List<BookModel>>(books)).Returns(new List<BookModel>
            {
                new BookModel { Id = 1, Name = "Book 1" },
                new BookModel { Id = 2, Name = "Book 2" }
            });

            var useCase = new GetAuthorByIdForBooksUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = useCase.Execute(authorId, 1, 10);

            Assert.NotNull(result);
            Assert.Equal(authorModel.FirstName, result.Author.FirstName);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(2, result.Author.Books.Count);
        }

        [Fact]
        public void GetAuthorByIdForBooksUseCase_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            var authorId = 1;

            _unitOfWorkMock.Setup(u => u.Authors.GetById(authorId)).Returns((Author)null);

            var useCase = new GetAuthorByIdForBooksUseCase(_unitOfWorkMock.Object, _mapperMock.Object);
            var result = useCase.Execute(authorId, 1, 10);

            Assert.Null(result);
        }

        [Fact]
        public void GetAuthorByIdForBooksUseCase_ShouldThrowArgumentNullException_WhenIdIsNull()
        {
            var useCase = new GetAuthorByIdForBooksUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            var exception = Assert.Throws<ArgumentNullException>(() => useCase.Execute(null, 1, 10));

            Assert.Equal("Id cannot be null. (Parameter 'id')", exception.Message);
        }

        [Fact]
        public void GetAuthorsPagination_ShouldReturnPaginatedAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "John", LastName = "Doe" },
                new Author { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };
            _unitOfWorkMock.Setup(u => u.Authors.GetAll()).Returns(authors.AsQueryable());

            var paginatedAuthors = new List<AuthorModel>
        {
            new AuthorModel { FirstName = "John", LastName = "Doe" }
        };
            _mapperMock.Setup(m => m.Map<List<AuthorModel>>(It.IsAny<List<Author>>())).Returns(paginatedAuthors);

            var useCase = new GetAuthorsPaginationUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = useCase.Execute(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Authors);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public void GetAuthors_ShouldReturnAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
    {
        new Author { Id = 1, FirstName = "John", LastName = "Doe" },
        new Author { Id = 2, FirstName = "Jane", LastName = "Doe" }
    };

            _unitOfWorkMock.Setup(u => u.Authors.GetAll()).Returns(authors.AsQueryable());

            var authorModels = new List<AuthorModel>
    {
        new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe" },
        new AuthorModel { Id = 2, FirstName = "Jane", LastName = "Doe" }
    };

            _mapperMock.Setup(m => m.Map<List<AuthorModel>>(It.IsAny<IEnumerable<Author>>())).Returns(authorModels);

            var useCase = new GetAuthorsUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = useCase.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Jane", result[1].FirstName);
        }


        [Fact]
        public async Task GetOrCreateAuthorAsync_ShouldReturnExistingOrNewAuthorId()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            _unitOfWorkMock.Setup(u => u.Authors.GetOrCreateAuthorAsync(firstName, lastName))
                .ReturnsAsync(1); // Предположим, что автор с такими данными уже существует и его ID = 1

            var useCase = new GetOrCreateAuthorAsyncUseCase(_unitOfWorkMock.Object);

            // Act
            var result = await useCase.Execute(firstName, lastName);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void UpdateAuthor_ShouldUpdateAuthor_WhenValidModel()
        {
            // Arrange
            var authorModel = new AuthorModel { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = "1990-01-01" };
            var authorEntity = new Author { Id = 1 };

            _mapperMock.Setup(m => m.Map<Author>(It.IsAny<AuthorModel>())).Returns(authorEntity);
            _unitOfWorkMock.Setup(u => u.Authors.Update(It.IsAny<Author>()));

            var useCase = new UpdateAuthorUseCase(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            useCase.Execute(authorModel);

            // Assert
            _unitOfWorkMock.Verify(u => u.Authors.Update(It.IsAny<Author>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }
    }
}
