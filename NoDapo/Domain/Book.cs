using System;
using System.ComponentModel.DataAnnotations;

namespace Nodapo.Domain
{
    public class Book
    {
        public Book(string title, double price, int numberOfPages, string isbn13, Genre genre)
        {
            Title = title;
            Price = price;
            NumberOfPages = numberOfPages;
            ISBN13 = isbn13;
            Genre = genre;
        }
        
        [Required, MinLength(3), MaxLength(255)]
        public string Title { get; set; }
        
        [Required, DataType(DataType.Currency, ErrorMessage = "Invalid value for money!")]
        public double Price { get; set; }
        
        [Required]
        public int NumberOfPages { get; set; }
        
        [Required]
        public string ISBN13 { get; set; }
        
        [Required, Range(1,4, ErrorMessage = "Invalid Genre selected.")]
        public Genre Genre { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Book) return false;
            
            var other = (Book)obj;
            return string.Equals(ISBN13, other.ISBN13, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ISBN13.GetHashCode();
        }
    }
}