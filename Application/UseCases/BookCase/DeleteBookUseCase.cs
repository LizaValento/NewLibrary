using Domain.Interfaces.InterfacesForUOW;

namespace Application.UseCases.BookCase
{
    public class DeleteBookUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            _unitOfWork.Books.Remove(book);
            _unitOfWork.Complete();
        }
    }
}
