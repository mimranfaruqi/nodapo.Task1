using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace NoDapo.Domain
{
    public class Customer
    {
        public Customer()
        {
            Books = new Collection<Book>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public double Money { get; set; }
        
        public IList<Book> Books { get; set; }

        public Tuple<bool, string> BuyBook(Book book)
        {
            if (book.Price > Money)
                return new Tuple<bool, string>(false, "You do not have enough funds to buy this book!");

            Money -= book.Price;
            Books.Add(book);
            return new Tuple<bool, string>(true, "Book has been added to your collection.");
        }
    }
}