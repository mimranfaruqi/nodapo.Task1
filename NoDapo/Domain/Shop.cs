using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NoDapo.Domain
{
    public class Shop
    {
        public Shop()
        {
            Books = new List<Book>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        
        [Required, MinLength(3), MaxLength(255)]
        public string Name { get; set; }
        public double Sales { get; private set; }
        public List<Book> Books { get; set; }
        public void SaleBook(Book book)
        {
            if (book is null) throw new ArgumentNullException(nameof(book));

            Sales += book.Price;
            Books.Remove(book);
        }

        public IList<Book> GetBookByGenre(Genre genre)
        {
            return Books.Where(b => b.Genre == genre).Distinct().ToList();
        }

        public IList<Book> GetAllBooks()
        {
            return Books.Distinct().ToList();
        }
    }
}