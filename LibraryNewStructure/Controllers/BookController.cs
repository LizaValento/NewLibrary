using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.AuthorCase;
using Application.UseCases.BookCase;

namespace Presentation.Controllers
{
    public class BookController : Controller
    {
        private readonly AddBookUseCase _addBookUseCase;
        private readonly GetBookByIdUseCase _getBookByIdUseCase;
        private readonly GetBooksForPaginationUseCase _getBooksForMainPageUseCase;
        private readonly UpdateBookUseCase _updateBookUseCase;
        private readonly DeleteBookUseCase _deleteBookUseCase;
        private readonly TakeBookUseCase _takeBookUseCase;
        private readonly SearchBooksByTitleUseCase _searchBooksByTitleUseCase;
        private readonly SearchBookByISBNUseCase _searchBookByISBNUseCase;
        private readonly GetAllBooksByGenresUseCase _getAllBooksByGenresUseCase;
        private readonly GetAllBooksByAuthorUseCase _getAllBooksByAuthorUseCase;
        private readonly GetPagedBooksUseCase _getPagedBooksUseCase;

        public BookController(AddBookUseCase addBookUseCase,
                              GetBookByIdUseCase getBookByIdUseCase,
                              GetBooksForPaginationUseCase getBooksForMainPageUseCase,
                              UpdateBookUseCase updateBookUseCase,
                              DeleteBookUseCase deleteBookUseCase,
                              TakeBookUseCase takeBookUseCase,
                              SearchBooksByTitleUseCase searchBooksByTitleUseCase,
                              SearchBookByISBNUseCase searchBookByISBNUseCase,
                              GetAllBooksByGenresUseCase getAllBooksByGenresUseCase,
                              GetAllBooksByAuthorUseCase getAllBooksByAuthorUseCase,
                              GetPagedBooksUseCase getPagedBooksUseCase)
        {
            _addBookUseCase = addBookUseCase;
            _getBookByIdUseCase = getBookByIdUseCase; 
            _getBooksForMainPageUseCase = getBooksForMainPageUseCase;
            _updateBookUseCase = updateBookUseCase;
            _deleteBookUseCase = deleteBookUseCase;
            _takeBookUseCase = takeBookUseCase;
            _searchBooksByTitleUseCase = searchBooksByTitleUseCase;
            _searchBookByISBNUseCase = searchBookByISBNUseCase;
            _getAllBooksByGenresUseCase = getAllBooksByGenresUseCase;
            _getAllBooksByAuthorUseCase = getAllBooksByAuthorUseCase;
            _getPagedBooksUseCase = getPagedBooksUseCase;
        }

        public ViewResult Main(int page = 1, int pageSize = 5)
        {
            var model = _getBooksForMainPageUseCase.Execute(page, pageSize);

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ServiceFilter(typeof(ValidateModelAttribute<BookModel>))]
        public async Task<IActionResult> AddBook(BookModel model, IFormFile BookImage)
        {
            var (success, errors) = await _addBookUseCase.ExecuteAsync(model, BookImage);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return RedirectToAction("Main", "Book");
        }

        [HttpGet("Book/AddBook")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddBook()
        {
            return View();
        }

        [HttpGet("Book/ViewBook/{BookId}")]
        public ActionResult ViewBook(int? BookId)
        {
            var book = _getBookByIdUseCase.Execute(BookId);
            return View(book);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ServiceFilter(typeof(ValidateModelAttribute<BookModel>))]
        public async Task<ActionResult> UpdateBookAsync(BookModel model, IFormFile BookImage)
        {
            var (success, errors) = await _updateBookUseCase.ExecuteAsync(model, BookImage);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return RedirectToAction("ViewBook", "Book", new { id = model.Id });
        }


        [HttpGet("Book/UpdateBook/{BookId}")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult UpdateBook(int BookId)
        {
            var book = _getBookByIdUseCase.Execute(BookId);
            return View(book);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult DeleteBook(int Id)
        {
            _deleteBookUseCase.Execute(Id);
            return RedirectToAction("Main", "Book");
        }

        [HttpPost]
        public ActionResult TakeBook(int Id)
        {
            try
            {
                _takeBookUseCase.Execute(Id, User);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction("ViewBook", "Book", new { id = Id });
        }

        [HttpGet("Book/Search")]
        public IActionResult Search(int page = 1, int pageSize = 5)
        {
            var model = _getBooksForMainPageUseCase.Execute(page, pageSize);
            return View(model);
        }

        [HttpPost("Book/SearchByTitle")]
        public IActionResult SearchByTitle(BookModel model)
        {
            var books = _searchBooksByTitleUseCase.Execute(model.Name);

            TempData["BookViewModel"] = JsonConvert.SerializeObject(books);

            return RedirectToAction("SearchResults");
        }

        [HttpGet("Book/SearchResults")]
        public ActionResult SearchResults(int page = 1, int pageSize = 5)
        {
            var booksJson = HttpContext.Session.GetString("FilteredBooks");

            var resultModel = _getPagedBooksUseCase.Execute(booksJson, page, pageSize);

            return View(resultModel);
        }

        [HttpPost("Book/SearchByISBN")]
        public IActionResult SearchByISBN(BookModel model)
        {
            var book = _searchBookByISBNUseCase.Execute(model.ISBN);
            return RedirectToAction("ViewBook", "Book", new { BookId = book.Id });
        }

        [HttpPost("Book/GenreFilter")]
        public IActionResult GenreFilter(BookModel model)
        {
            _getAllBooksByGenresUseCase.Execute(model.Genre);

            return RedirectToAction("SearchResults");
        }

        [HttpPost("Book/AuthorFilter")]
        public IActionResult AuthorFilter(BookModel model)
        {
            var books = _getAllBooksByAuthorUseCase.Execute(model.AuthorName, model.AuthorLastName);
            TempData["FilteredBooks"] = JsonConvert.SerializeObject(books);

            return RedirectToAction("SearchResults");
        }
    }
}
