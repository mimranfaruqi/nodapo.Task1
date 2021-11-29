using Microsoft.AspNetCore.Mvc;
using Nodapo.Controllers;
using NUnit.Framework;

namespace Nodapo.UnitTests
{
    public class CustomersControllerTests
    {
        private CustomersController _controller;
        private Data _data;
        
        [SetUp]
        public void Setup()
        {
            _data = new Data();
            _controller = new CustomersController(_data);
        }

        [Test]
        public void Customers_WhenCustomersPresent_ReturnsAllCustomersWithOk()
        {
            var result = _controller.Customers();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void Customers_WhenCustomersNotPresent_ReturnsNotFound()
        {
            _data.Customers.Clear();
  
            var result = _controller.Customers();
            
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}