using System;
using System.Linq;
using Katz.Core;
using Katz.Web.Website.Models;
using Image = Katz.Web.Website.Models.Image;

namespace Katz.Web.Website.Extensions
{
    public static class BookExtensions
    {
        public static BookDisplay ToDisplayBook(this Book book, params Book[] relatedBooks) => new BookDisplay
        {
            Id = book.Id,
            Title = book.Detail.Title,
            Author = book.Detail.Author,
            Description = book.Info.Description,
            Rating = (int) book.Info.Rating,
            Image = new Image
            {
                MimeType = book.Info.Image.MimeType,
                Base64Value = Convert.ToBase64String(book.Info.Image.Value)
            },
            Series = book.Detail.Series,
            RelatedBooks = relatedBooks.Select(relatedBook => relatedBook.ToDisplayBook()).ToArray()
        };
    }
}