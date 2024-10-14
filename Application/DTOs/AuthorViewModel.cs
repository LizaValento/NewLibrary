using System.ComponentModel;
using Domain.Entities;

namespace Application.DTOs
{
    public class AuthorViewModel
    {
        public List<AuthorModel> Authors { get; set; }
        public AuthorModel Author { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
