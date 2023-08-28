Console.WriteLine("Console Application with EF Core and SQL Server\r\n");

await using var dbContext = new HandsOnDbContext();

await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("HandsOnDb database created");

var authors = new List<Author>
{
    new() { Name = "John", Surname = "Doe" },
    new() { Name = "Jane", Surname = "Doe" },
    new() { Name = "John", Surname = "Smith" }
};

var categories = new List<Category>
{
    new() { Name = "Fiction" },
    new() { Name = "Non-fiction" },
    new() { Name = "Science" }
};

var books = new List<Book>
{
    new()
    {
        Title = "Book 1",
        Isbn = "1234567890123",
        Description = "Description of Book 1",
        Pages = 100,
        PublishedOn = new DateTime(2021, 1, 1),
        Authors = new List<Author> { authors[0], authors[1] },
        Categories = new List<Category> { categories[0], categories[1] }
    },
    new()
    {
        Title = "Book 2",
        Isbn = "1234567890124",
        Description = "Description of Book 2",
        Pages = 200,
        PublishedOn = new DateTime(2021, 2, 2),
        Authors = new List<Author> { authors[1], authors[2] },
        Categories = new List<Category> { categories[1], categories[2] }
    },
    new()
    {
        Title = "Book 3",
        Isbn = "1234567890125",
        Description = "Description of Book 3",
        Pages = 300,
        PublishedOn = new DateTime(2021, 3, 3),
        Authors = new List<Author> { authors[2], authors[0] },
        Categories = new List<Category> { categories[2], categories[0] }
    }
};

await dbContext.Books.AddRangeAsync(books);
await dbContext.SaveChangesAsync();

Console.WriteLine("Books added to database");

var booksDto = dbContext.Books
    .Select(b => new
    {
        b.Title,
        b.Isbn,
        b.Description,
        b.Pages,
        b.PublishedOn,
        Authors = b.Authors.Select(a => new { a.Name, a.Surname }),
        Categories = b.Categories.Select(c => c.Name)
    });

Console.WriteLine("Books retrieved from database");
Console.WriteLine($"\r\n{booksDto.ToQueryString()}\r\n");
Console.WriteLine("Books:");

foreach (var bookDto in booksDto)
{
    Console.WriteLine($"Title: {bookDto.Title}");
    Console.WriteLine($"Isbn: {bookDto.Isbn}");
    Console.WriteLine($"Description: {bookDto.Description}");
    Console.WriteLine($"Pages: {bookDto.Pages}");
    Console.WriteLine($"PublishedOn: {bookDto.PublishedOn:yyyy-MM-dd}");
    Console.WriteLine($"Authors: {string.Join(", ", authors.Select(a => $"{a.Name} {a.Surname}"))}");
    Console.WriteLine($"Categories: {string.Join(", ", categories.Select(c => c.Name))}");
    Console.WriteLine();
}

var review = new Review
{
    Rating = 5,
    BookId = books[0].BookId
};

await dbContext.Reviews.AddAsync(review);
await dbContext.SaveChangesAsync();

Console.WriteLine("Review added to database");

Console.WriteLine($"Book: {books[0].Title}");
Console.WriteLine($"Reviews: {string.Join(", ", books[0].Reviews.Select(r => r.Rating))}");
Console.WriteLine($"Avg: {string.Join(", ", books[0].Reviews.Average(r => r.Rating))}");
Console.WriteLine();

Console.WriteLine("Book reviews added to tracked entity without loading all reviews from database");
