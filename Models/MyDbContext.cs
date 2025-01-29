using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Library.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=BETACOM-PCHP03\\SQLEXPRESS;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Address");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(100);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .HasColumnName("Zip_code");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Firstname).HasMaxLength(100);
            entity.Property(e => e.Lastname).HasMaxLength(100);
            entity.Property(e => e.Nationality).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AuthorId).HasColumnName("Author_id");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.PublishedDate).HasColumnName("Published_date");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Authors");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AddressId).HasColumnName("Address_id");
            entity.Property(e => e.CreatedAt).HasColumnName("Created_at");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Firstname).HasMaxLength(100);
            entity.Property(e => e.Lastname).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.Address).WithMany(p => p.Users)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Address");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
