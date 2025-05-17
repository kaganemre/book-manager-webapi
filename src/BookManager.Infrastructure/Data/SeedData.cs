using BookManager.Domain.Entities;
using BookManager.Infrastructure.Context;

namespace BookManager.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Books.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Yazılım Geliştirme" },
            new() { Name = "Kütüphanecilik" },
            new() { Name = "Arşivcilik" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var books = new List<Book>
        {
            new()
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                ISBN = "9780132350884",
                PublishedDate = new DateOnly(2008, 8, 1),
                StockQuantity = 15,
                Source = "Prentice Hall",
                PageCount = 464,
                Description = "Temiz ve sürdürülebilir kod yazma tekniklerini anlatan, yazılım geliştiriciler için temel bir rehber.",
                CategoryId = categories.First(c => c.Name == "Yazılım Geliştirme").Id
            },
            new()
            {
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt & David Thomas",
                ISBN = "9780201616224",
                PublishedDate = new DateOnly(1999, 10, 30),
                StockQuantity = 10,
                Source = "Addison-Wesley",
                PageCount = 352,
                Description = "Yazılım geliştiriciler için pragmatik yaklaşımlar sunan, meslek hayatına yön verebilecek kapsamlı bir kaynak.",
                CategoryId = categories.First(c => c.Name == "Yazılım Geliştirme").Id
            },
            new()
            {
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                ISBN = "9780134494166",
                PublishedDate = new DateOnly(2017, 9, 20),
                StockQuantity = 9,
                Source = "Pearson",
                PageCount = 432,
                Description = "Yazılım mimarisi konusunda modern ve sürdürülebilir tasarım ilkelerini ele alan detaylı bir kitap.",
                CategoryId = categories.First(c => c.Name == "Yazılım Geliştirme").Id
            },
            new()
            {
                Title = "Refactoring",
                Author = "Martin Fowler",
                ISBN = "9780201485677",
                PublishedDate = new DateOnly(1999, 7, 8),
                StockQuantity = 8,
                Source = "Addison-Wesley",
                PageCount = 418,
                Description = "Mevcut kodun performansını ve okunabilirliğini artırma tekniklerini anlatan yazılım geliştirme kitabı.",
                CategoryId = categories.First(c => c.Name == "Yazılım Geliştirme").Id
            },
            new()
            {
                Title = "Foundations of Library and Information Science",
                Author = "Richard E. Rubin",
                ISBN = "9780838913703",
                PublishedDate = new DateOnly(2016, 1, 15),
                StockQuantity = 6,
                Source = "ALA Editions",
                PageCount = 648,
                Description = "Kütüphane bilimi ve bilgi yönetimi üzerine temel prensipleri ve uygulamaları kapsayan akademik bir kaynak.",
                CategoryId = categories.First(c => c.Name == "Kütüphanecilik").Id
            },
            new()
            {
                Title = "Kitap ve Kütüphane Tarihine Giriş",
                Author = "Jale Baysal",
                ISBN = "978-605-1234567",
                PublishedDate = new DateOnly(2007, 3, 10),
                StockQuantity = 20,
                Source = "İstanbul Üniversitesi Yayınları",
                PageCount = 340,
                Description = "Kitap ve kütüphanelerin tarihi üzerine kapsamlı bir çalışma, geçmişten günümüze gelişim sürecini anlatıyor.",
                CategoryId = categories.First(c => c.Name == "Kütüphanecilik").Id
            },
            new()
            {
                Title = "Arşivcilik Terimleri Sözlüğü",
                Author = "Bekir Kemal Ataman",
                ISBN = "978-605-9876543",
                PublishedDate = new DateOnly(2015, 6, 22),
                StockQuantity = 15,
                Source = "Kültür ve Tarih Kitapları",
                PageCount = 280,
                Description = "Arşivcilik alanında kullanılan temel terimleri ve açıklamalarını içeren kapsamlı bir sözlük.",
                CategoryId = categories.First(c => c.Name == "Arşivcilik").Id
            }
        };


        context.Books.AddRange(books);
        await context.SaveChangesAsync();

    }

}