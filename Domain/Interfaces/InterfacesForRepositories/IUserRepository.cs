using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByNickname(string nickname);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Remove(User user);
        Task<User> GetByIdAsync(int id);
        (List<Book> Books, int TotalCount) GetUserBooks(int userId, int page, int pageSize);
    }
}
