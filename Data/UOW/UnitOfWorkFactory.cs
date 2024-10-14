using Domain.Interfaces.InterfacesForRepositories;
using Domain.Interfaces.InterfacesForUOW;
using Data.Data.LibraryContext;
using Microsoft.Extensions.DependencyInjection;

namespace Data.UOW
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var context = _serviceProvider.GetRequiredService<Context>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var bookRepository = _serviceProvider.GetRequiredService<IBookRepository>();
            var authorRepository = _serviceProvider.GetRequiredService<IAuthorRepository>();
            var refreshTokenRepository = _serviceProvider.GetRequiredService<IRefreshTokenRepository>();

            return new UnitOfWork(context, userRepository, bookRepository, authorRepository, refreshTokenRepository);
        }
    }

}
