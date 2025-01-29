using System;
using System.Collections.Generic;

namespace Library.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
