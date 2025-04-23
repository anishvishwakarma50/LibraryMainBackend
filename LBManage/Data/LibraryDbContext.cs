using Microsoft.EntityFrameworkCore;
using LBManage.Models;

namespace LBManage.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<IssuedBook> IssuedBooks { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<SuggestedBook> SuggestedBooks { get; set; }
        public DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships and constraints

            modelBuilder.Entity<IssuedBook>()
                .HasOne(ib => ib.Book)
                .WithMany()
                .HasForeignKey(ib => ib.BookID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssuedBook>()
                .HasOne(ib => ib.Student)
                .WithMany()
                .HasForeignKey(ib => ib.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssuedBook>()
                .HasOne(ib => ib.IssuedByLibrarian)
                .WithMany()
                .HasForeignKey(ib => ib.IssuedByLibrarianID)
                .OnDelete(DeleteBehavior.SetNull); // ✅ This will now work


        modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany()
                .HasForeignKey(r => r.BookID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Student)
                .WithMany()
                .HasForeignKey(f => f.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.IssuedBook)
                .WithMany()
                .HasForeignKey(f => f.IssueID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
