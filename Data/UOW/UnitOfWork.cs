using Domain.Interfaces.InterfacesForRepositories;
using Data.Data.Repositories;
using Domain.Interfaces.InterfacesForUOW;
using Data.Data.LibraryContext;

namespace Data.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        public IUserRepository Users { get; private set; }
        public IBookRepository Books { get; private set; }
        public IAuthorRepository Authors { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }

        public UnitOfWork(Context context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Books = new BookRepository(_context);
            Authors = new AuthorRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
        }

        public UnitOfWork(Context context, IUserRepository userRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            Users = userRepository;
            Books = bookRepository;
            Authors = authorRepository;
            RefreshTokens = refreshTokenRepository;
        }

        // Synchronous method
        public int Complete()
        {
            return _context.SaveChanges();
        }

        // Asynchronous method
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
