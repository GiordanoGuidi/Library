using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int AddressId { get; set; }
    [Column("Created_at")]
    public DateOnly CreatedAt { get; set; }

    public virtual Address Address { get; set; } = null!;
}
