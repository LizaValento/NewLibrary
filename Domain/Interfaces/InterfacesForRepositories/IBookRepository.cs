﻿using Domain.Entities;

namespace Domain.Interfaces.InterfacesForRepositories
{
    public interface IBookRepository
    {
        Book GetById(int id);
        IEnumerable<Book> GetAll(); 
        void Add(Book Book);
        void Update(Book Book);
        void Remove(Book Book);
        IEnumerable<Book> GetBooksByAuthorId(int authorId);
        IEnumerable<Book> GetFreeBooks();
        IEnumerable<Book> GetBooksByGenre(string genre);
        IEnumerable<Book> GetAllWithTitles(string title);
        Book GetByISBN(string isbn);
        IEnumerable<Book> GetBooksByAuthorNameAndLastName(string firstName, string lastName);
    }
}
