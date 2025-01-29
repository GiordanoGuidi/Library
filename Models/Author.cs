using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Author
{
    public int Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Nationality { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
