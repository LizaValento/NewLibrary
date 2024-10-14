using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IAuthorRepository
    {
        Author GetById(int id);
        IEnumerable<Author> GetAll();
        void Add(Author Author);
        void Update(Author Author);
        void Remove(Author Author);
        Task<int> GetOrCreateAuthorAsync(string firstName, string lastName);
    }
}
