using Domain.Interfaces.InterfacesForRepositories;

namespace Domain.Interfaces.InterfacesForUOW
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IBookRepository Books { get; } 
        IAuthorRepository Authors { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
