using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int AuthorId { get; set; }

    public decimal Price { get; set; }
    [Column("published_date")]
    public DateOnly PublishedDate { get; set; }

    public int Stock { get; set; }

    public virtual Author Author { get; set; } = null!;
}
