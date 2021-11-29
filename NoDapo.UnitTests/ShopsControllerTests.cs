using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NoDapo.Controllers;
using NoDapo.Domain;
using NUnit.Framework;

namespace NoDapo.UnitTests
{
    [TestFixture]
    public class ShopsControllerTests
    {
        private Data _data;
        private ShopsController _controller;

        [SetUp]
        public void Setup()
        {
            _data = new Data();
            _controller = new ShopsController(_data);
        }

        [Test]
        public void Shops_WhenCalled_ReturnsOkWithObject()
        {
            var result = _controller.Shops();

            var value = (IList<Shop>) ((OkObjectResult) result).Value;
            
            Assert.That(value, Is.Not.Empty);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void Books_CorrectShopId_ReturnsAllBooksWithOkObject()
        {
            var shop = new Shop
            {
                Books = _data.Books,
                Id = Guid.NewGuid(),
                Name = "Test Shop"
            };
            _data.Shops.Add(shop);

            var result = _controller.Books(shop.Id);
            
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(((OkObjectResult) result).Value, Is.TypeOf<List<Book>>());
        }

        [Test]
        public void Books_IncorrectShopId_ReturnsNotFound()
        {
            var invalidId = Guid.NewGuid();

            var result = _controller.Books(invalidId);
            
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BooksByGenre_InvalidGenre_ReturnsNotFound()
        {
            const string invalidGenre = "test";
            var shopId = _data.Shops.First().Id;
            
            var result = _controller.BooksByGenre(shopId, invalidGenre);
            
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BooksByGenre_InvalidShopId_ReturnsNotFound()
        {
            var invalidShopId = Guid.NewGuid();

            var result = _controller.BooksByGenre(invalidShopId, nameof(Genre.Adventure));
            
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BooksByGenre_NoBooksAvailableForSuchGenre_ReturnsNotFound()
        {
            var shop = _data.Shops.First();
            const Genre genreToTest = Genre.Adventure;
            shop.Books = shop.Books.Where(x => x.Genre != genreToTest).ToList();
            
            var result = _controller.BooksByGenre(shop.Id, nameof(genreToTest));
            
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BooksByGenre_BooksAvailableOfSuchGenre_ReturnsOk()
        {
            var shopId = _data.Shops.First().Id;

            var result = _controller.BooksByGenre(shopId, nameof(Genre.Adventure));
            
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(((OkObjectResult) result).Value, Is.TypeOf<List<Book>>());
        }

        [Test]
        public void BuyBook_InvalidShopId_ReturnsNotFound()
        {
            var shopId = Guid.NewGuid();
            var isbn = _data.Shops.First().Books.First().ISBN13;
            var customerId = _data.Customers.First().Id;

            var result = _controller.BuyBook(shopId, isbn, customerId);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BuyBook_InvalidISBN_ReturnsNotFound()
        {
            var shopId = _data.Shops.First().Id;
            var isbn = "978-555555555";
            var customerId = _data.Customers.First().Id;

            var result = _controller.BuyBook(shopId, isbn, customerId);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
        
        [Test]
        public void BuyBook_InvalidCustomerId_ReturnsNotFound()
        {
            var shop = _data.Shops.First();
            var isbn = shop.Books.First().ISBN13;
            var customerId = Guid.NewGuid();

            var result = _controller.BuyBook(shop.Id, isbn, customerId);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BuyBook_InsufficientMoney_ReturnsBadRequest()
        {
            var shop = _data.Shops.First();
            var customer = _data.Customers.First();
            customer.Money = 0;
            var book = shop.Books.First();

            var result = _controller.BuyBook(shop.Id, book.ISBN13, customer.Id);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public void BuyBook_EverythingIsGood_ReturnsOk()
        {
            var shop = _data.Shops.First();
            var customer = _data.Customers.First();
            customer.Money = 1;
            var book = _data.Books.First();
            book.Price = 0;

            var result = _controller.BuyBook(shop.Id, book.ISBN13, customer.Id);
            
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}