using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nodapo.Domain;
using Nodapo.Utilities;

namespace Nodapo.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ShopsController : Controller
    {
        private readonly Data _data;

        public ShopsController(Data data)
        {
            _data = data;
        }

        [HttpGet]
        public IActionResult Shops()
        {
            return Ok(_data.Shops.ToList());
        }
        
        [HttpGet]
        public IActionResult Genres()
        {
            return Ok(Enum.GetNames<Genre>().Select(x => new
            {
                GenreId = x.ToGenre(),
                Genre = x
            }));
        }

        [HttpGet("{shopId:guid}")]
        public IActionResult Books([FromRoute] Guid shopId)
        {
            var shop = _data.Shops.FirstOrDefault(x => x.Id == shopId);

            if (shop is null) return NotFound("Invalid Shop selected");

            return Ok(shop.GetAllBooks());
        }

        [HttpGet("{shopId:guid}/{genre}")]
        public IActionResult BooksByGenre([FromRoute] Guid shopId, [FromRoute] string genre)
        {
            var shop = _data.Shops.FirstOrDefault(x => x.Id == shopId);

            if (shop is null) return NotFound("Unable to find the shop you are looking for!");

            var gen = genre.ToGenre();

            if (gen == Genre.Empty) return NotFound("No books of such a genre available");

            var books = shop.GetBookByGenre(gen);

            if (books.Count == 0) return NotFound("No books of this genre exists");

            return Ok(books);
        }

        [HttpPost("{shopId:guid}/{isbn}/{customerId:guid}")]
        public IActionResult BuyBook([FromRoute] Guid shopId, [FromRoute] string isbn, [FromRoute] Guid customerId)
        {
            var shop = _data.Shops.FirstOrDefault(x => x.Id == shopId);
            var customer = _data.Customers.FirstOrDefault(x => x.Id == customerId);

            if (shop is null || customer is null) return NotFound("Invalid information provided!");

            var book = shop.Books.FirstOrDefault(x => x.ISBN13 == isbn);

            if (book is null)
                return NotFound(
                    "Unable to find the book. Please verify whether you entered the ISBN in the correct format? e.g. 978-3841335180");

            var (success, message) = customer.BuyBook(book);

            if (!success) return BadRequest(message);

            shop.SaleBook(book);

            return Ok($"Thank you for adding {book.Title} to your remarkable collection!");
        }

        [HttpGet("{shopId1:guid}/{shopId2:guid}")]
        public IActionResult CompareBookStores([FromRoute] Guid shopId1, [FromRoute] Guid shopId2)
        {
            var firstShop = _data.Shops.FirstOrDefault(x => x.Id == shopId1);
            var secondShop = _data.Shops.FirstOrDefault(x => x.Id == shopId2);

            if (firstShop is null || secondShop is null)
                return NotFound("Either or both of the stores are not available in the system!");

            var firstNotSecond = firstShop.Books.Distinct().Except(secondShop.Books).ToList();
            var secondNotFirst = secondShop.Books.Distinct().Except(firstShop.Books).ToList();

            var response = !firstNotSecond.Any() && !secondNotFirst.Any()
                ? "The selected stores do have the exact same Books in their collection."
                : "The selected stores do not have similar collection of books.";

            return Ok(response);
        }

        [HttpPost("{shopId:guid}/{copies:int}")]
        public IActionResult AddBook([FromRoute] Guid shopId, [FromBody] Book book, int copies)
        {
            if (!Regex.IsMatch(book.ISBN13, "978-([0-9]{10})$")) return BadRequest("ISBN-13 must match the format: 978-3442267747");
            
            var shop = _data.Shops.FirstOrDefault(s => s.Id == shopId);

            if (shop is null) return NotFound("Specified shop does not exist.");

            for (var i = 0; i < copies; i++)
            {
                shop.Books.Add(book);
            }

            return Ok($"{copies} copies of book: {book.Title} with ISBN: {book.ISBN13} are registered to your store");
        }
    }
}