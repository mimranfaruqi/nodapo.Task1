using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nodapo.Controllers;
using NUnit.Framework;

namespace Nodapo.UnitTests
{
    [TestFixture]
    public class BuyBook_ShopsControllerTests
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
        public void BuyBook_ShopIdNotInSystem_ReturnsNotFound()
        {
            var shopId = Guid.NewGuid();
            var isbn = _data.Shops.First().Books.First().ISBN13;
            var customerId = _data.Customers.First().Id;

            var result = _controller.BuyBook(shopId, isbn, customerId);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void BuyBook_IsbnNotInSystem_ReturnsNotFound()
        {
            var shopId = _data.Shops.First().Id;
            var isbn = "978-555555555";
            var customerId = _data.Customers.First().Id;

            var result = _controller.BuyBook(shopId, isbn, customerId);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
        
        [Test]
        public void BuyBook_CustomerIdNotInSystem_ReturnsNotFound()
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