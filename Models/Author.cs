using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Author
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
