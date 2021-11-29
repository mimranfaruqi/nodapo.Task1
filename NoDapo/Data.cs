using System;
using System.Collections.Generic;
using System.Linq;
using NoDapo.Domain;

namespace NoDapo
{
    public class Data
    {
        public List<Book> Books;
        public List<Shop> Shops;
        public List<Customer> Customers;

        public Data()
        {
            Books = new List<Book>();
            Shops = new List<Shop>();
            Customers = new List<Customer>();

            GenerateData();
        }

        public void GenerateData()
        {
            var random = new Random();

            for (var i = 1; i <= 100; i++)
            {
                Books.Add(new Book($"Title {i}",
                    random.Next(200, 400) * 50,
                    random.Next(100, 255),
                    $"$978-3442{random.Next(100, 700)}747",
                    (Genre) random.Next(1, 4)));
            }

            for (var i = 1; i <= 10; i++)
            {
                Customers.Add(new Customer
                {
                    Money = random.Next(100, 500) * 50,
                    Name = $"Customer {i}"
                });
            }

            for (var i = 1; i <= 5; i++)
            {
                Shops.Add(new Shop
                {
                    Name = $"Shop {i}",
                    Books = Books.GetRange(0, random.Next(Books.Count - 10, Books.Count))
                });
            }

            // Adding the shops that will contain exactly same books for testing only
            var range = random.Next(Books.Count - 10, Books.Count);
            Shops.AddRange(new List<Shop>
            {
                new Shop
                {
                    Name = "Shop 6",
                    Books = Books.GetRange(0, range)
                },
                new Shop
                {
                    Name = "Shop 7",
                    Books = Books.GetRange(0, range)
                }
            });
            // printing the shop IDs on console to know which shops are similar for testing only
            Console.WriteLine(Shops.FirstOrDefault(x => "Shop 7" == x.Name)?.Id);
            Console.WriteLine(Shops.FirstOrDefault(x => "Shop 6" == x.Name)?.Id);
        }
    }
}