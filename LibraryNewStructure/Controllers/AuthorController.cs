using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.UseCases.AuthorCase;

namespace Presentation.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AddAuthorUseCase _addAuthorUseCase;
        private readonly UpdateAuthorUseCase _updateAuthorUseCase;
        private readonly DeleteAuthorUseCase _deleteAuthorUseCase;
        private readonly GetAuthorByIdUseCase _getAuthorByIdUseCase;
        private readonly GetAuthorsPaginationUseCase _getAuthorsPaginationUseCase;
        private readonly GetAuthorByIdForBooksUseCase _getAuthorByIdForBooksUseCase;

        public AuthorController(
            AddAuthorUseCase addAuthorUseCase,
            UpdateAuthorUseCase updateAuthorUseCase,
            DeleteAuthorUseCase deleteAuthorUseCase,
            GetAuthorByIdUseCase getAuthorByIdUseCase,
            GetAuthorsPaginationUseCase getAuthorsPaginationUseCase,
            GetAuthorByIdForBooksUseCase getAuthorByIdForBooksUseCase)
        {
            _addAuthorUseCase = addAuthorUseCase;
            _updateAuthorUseCase = updateAuthorUseCase;
            _deleteAuthorUseCase = deleteAuthorUseCase;
            _getAuthorByIdUseCase = getAuthorByIdUseCase;
            _getAuthorsPaginationUseCase = getAuthorsPaginationUseCase;
            _getAuthorByIdForBooksUseCase = getAuthorByIdForBooksUseCase;
        }

        [HttpGet("Author/Authors/")]
        public ViewResult Authors(int page = 1, int pageSize = 5)
        {
            var model = _getAuthorsPaginationUseCase.Execute(page, pageSize);

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute<AuthorModel>))]
        public async Task<ActionResult> AddAuthor(AuthorModel author)
        {
            try
            {
                _addAuthorUseCase.Execute(author);

                return RedirectToAction("Authors", "Author");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(author);
            }
        }


        [HttpGet("Author/AddAuthor")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddAuthor()
        {
            return View();
        }

        [HttpGet("Author/ViewAuthor/{AuthorId}")]
        public ActionResult ViewAuthor(int? authorId, int page = 1, int pageSize = 5)
        {
            try
            {
                var viewModel = _getAuthorByIdForBooksUseCase.Execute(authorId, page, pageSize);

                if (viewModel == null)
                {
                    return NotFound();
                }

                return View(viewModel);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Author ID is required.");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute<AuthorModel>))]
        public ActionResult UpdateAuthor(AuthorModel author)
        {
            try
            {
                _updateAuthorUseCase.Execute(author);
                return RedirectToAction("ViewAuthor", "Author", new { id = author.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("DateOfBirth", ex.Message);
                return View(author);
            }
        }

        [HttpGet("Author/UpdateAuthor/{AuthorId}")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult UpdateAuthor(int authorId)
        {
            var author = _getAuthorByIdUseCase.Execute(authorId);
            return View(author);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult DeleteAuthor(int id)
        {
            _deleteAuthorUseCase.Execute(id);
            return RedirectToAction("Authors", "Author");
        }
    }
}

