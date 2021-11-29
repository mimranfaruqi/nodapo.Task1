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
        public void CompareBookStores_ShopId1NotInSystem_ReturnsNotFound()
        {
            var shopId1 = Guid.NewGuid();
            var shopId2 = _data.Shops.First().Id;

            var result = _controller.CompareBookStores(shopId1, shopId2);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void CompareBookStores_ShopId2NotInSystem_ReturnsNotFound()
        {
            var shopId1 = _data.Shops.First().Id;
            var shopId2 = Guid.NewGuid();

            var result = _controller.CompareBookStores(shopId1, shopId2);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void CompareBookStores_BooksMatchesOrNot_ReturnsOk()
        {
            var shopId1 = _data.Shops.ElementAt(0).Id;
            var shopId2 = _data.Shops.ElementAt(1).Id;

            var result = _controller.CompareBookStores(shopId1, shopId2);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void AddBook_ShopIdNotInSystem_ReturnsNotFound()
        {
            var shopId = Guid.NewGuid();

            var result = _controller.AddBook(shopId, new Book(
                "Test Title",
                new Random().Next(1000, 15000),
                255,
                "978-3608963762",
                Genre.Adventure
            ), 5);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void AddBook_EverythingIsGood_ReturnsOk()
        {
            var shopId = _data.Shops.First().Id;
            var book = new Book(
                "Test Title",
                new Random().Next(1000, 15000),
                255,
                "978-3608963762",
                Genre.Adventure
            );
            var copies = 5;

            var result = _controller.AddBook(shopId, book, copies);

            Assert.That(_data.Shops
                    .Single(x => x.Id == shopId)
                    .Books.Count(x => x.ISBN13 == book.ISBN13),
                Is.EqualTo(copies));
            
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}