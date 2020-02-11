using System;
using System.Linq;
using Katz.Core;
using Katz.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookEntity = Katz.Data.Entities.Book;

namespace Katz.Web.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped(provider => new BookContext(new DbContextOptionsBuilder<BookContext>()
                                                           .UseSqlServer(Configuration["connectionStrings:default"])
                                                           .Options));
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                InitializeBooks(app);
            }

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Books}/{action=Index}/{id?}");
            });
        }

        private void InitializeBooks(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var random = new Random();
            var booksRelatedByAuthor = Enumerable.Range(1, 10)
                                                 .Select(i => new BookEntity
                                                 {
                                                     Author = "Related Author",
                                                     Description = "Related Author description",
                                                     Image = Images.Test,
                                                     Title = "Related Author title",
                                                     ImageMimeType = "image/png",
                                                     Rating = random.Next(1, 6),
                                                     Series = $"Related Author series {i}"
                                                 });

            var booksRelatedBySeries = Enumerable.Range(1, 10)
                                                 .Select(i => new BookEntity
                                                 {
                                                     Author = $"Related By Series Author {i}",
                                                     Description = "Related By Series description",
                                                     Image = Images.Test,
                                                     Title = "Related By Series title",
                                                     ImageMimeType = "image/png",
                                                     Rating = random.Next(1, 6),
                                                     Series = "Related By series"
                                                 });

            var booksRelatedByBoth= Enumerable.Range(1, 10)
                                                 .Select(i => new BookEntity
                                                 {
                                                     Author = "Related By Series And Author",
                                                     Description = "Related By Series and author description",
                                                     Image = Images.Test,
                                                     Title = "Related By Series and author title",
                                                     ImageMimeType = "image/png",
                                                     Rating = random.Next(1, 6),
                                                     Series = "Related By series And author"
                                                 });

            var booksNotRelated = Enumerable.Range(1, 10)
                                               .Select(i => new BookEntity
                                               {
                                                   Author = $"Non-Related Author {i}",
                                                   Description = "Non-Related description",
                                                   Image = Images.Test,
                                                   Title = "Non-Related title",
                                                   ImageMimeType = "image/png",
                                                   Rating = random.Next(1, 6),
                                                   Series = $"Non-Related series {i}"
                                               });

            context.Books.AddRange(booksNotRelated.Concat(booksRelatedByAuthor)
                                                  .Concat(booksRelatedBySeries)
                                                  .Concat(booksRelatedByBoth));
            context.SaveChanges();
        }
    }
}