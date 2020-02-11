using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Katz.Core;
using Katz.Web.Website.Extensions;
using Katz.Web.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Image = Katz.Core.Image;
using Rating = Katz.Core.Rating;

namespace Katz.Web.Website.Controllers
{
    [Route("books")]
    [Route("")]
    public class BooksController : Controller
    {
        private readonly IBookService _service;

        public BooksController(IBookService service) => _service = service;

        [Route("")]
        [Route("index")]
        public ViewResult Index() => View("Index");

        [Route("search")]
        [HttpGet]
        public ViewResult Search() => View("Search");

        [Route("search")]
        [ActionName("SearchResults")]
        [HttpPost]
        public async Task<IActionResult> Search(BookSearch model)
        {
            IActionResult result;
            if (ModelState.IsValid)
            {
                var results = await _service.FindAsync(new SearchArguments
                {
                    Author = model.Author,
                    Title = model.Title,
                    Rating = model.Rating.HasValue ? (Rating) model.Rating : default(Rating?),
                    Series = model.Series
                });

                var resultModel = results.Select(book => book.ToDisplayBook()).ToArray();
                result = View("Search", resultModel);
            }
            else
            {
                result = View("Search");
            }

            return result;
        }

        [Route("add")]
        [HttpGet]
        public ViewResult Add() => View("Add", new AddBook());

        [Route("add")]
        [ActionName("AddBook")]
        [HttpPost]
        public async Task<IActionResult> Add(AddBook model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);

                var image = new Image(model.Image.ContentType, ms.ToArray());
                var id = await _service.AddAsync(new AddArguments(
                    new BookDetail(model.Title, model.Author, model.Series),
                    new BookInformation((Rating) model.Rating, model.Description, image)));
                result = RedirectToAction("Get", new { id });
            }
            else
            {
                result = View("Add", model);
            }

            return result;
        }

        [Route("{id}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IActionResult result;
            var book = await _service.GetAsync(id);
            if (book != null)
            {
                var model = new UpdateBook
                {
                    Author = book.Detail.Author,
                    Title = book.Detail.Title,
                    Series = book.Detail.Series,
                    DisplayImage = new Models.Image
                    {
                        MimeType = book.Info.Image.MimeType,
                        Base64Value = Convert.ToBase64String(book.Info.Image.Value)
                    },
                    Description = book.Info.Description,
                    Rating = (Models.Rating) book.Info.Rating,
                    Id = book.Id
                };

                result = View("Edit", model);
            }
            else
            {
                result = NotFound();
            }

            return result;
        }

        [Route("edit")]
        [ActionName("EditBook")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBook model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                Image image = null;
                if (!(model.Image is null))
                {
                    await using var ms = new MemoryStream();
                    await model.Image.CopyToAsync(ms);
                    image = new Image(model.Image.ContentType, ms.ToArray());
                }

                var arguments = new UpdateArguments
                {
                    Author = model.Author,
                    Rating = (Rating?) model.Rating,
                    Image = image,
                    Description = model.Description,
                    Series = model.Series,
                    Title = model.Title
                };

                await _service.EditAsync(model.Id, arguments);
                result = RedirectToAction("Get", new { model.Id });
            }
            else
            {
                result = View("Add", model);
            }

            return result;
        }

        [Route("{id}/delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult result;
            var success = await _service.DeleteAsync(id);

            if (success)
            {
                result = RedirectToAction("Index");
            }
            else
            {
                result = NotFound();
            }

            return result;
        }

        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            IActionResult result;
            var book = await _service.GetAsync(id);
            if (book != null)
            {
                var relatedBooks = await _service.GetRelatedBooksByAuthorOrSeriesAsync(book);
                var model = book.ToDisplayBook(relatedBooks);
                result = View("Book", model);
            }
            else
            {
                result = NotFound();
            }

            return result;
        }
    }
}