namespace Library.Models.DTO
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public decimal Price { get; set; }

        public DateOnly PublishedDate { get; set; }

        public int Stock { get; set; }
        public string? AuthorNationality { get; set; }
    }
}
