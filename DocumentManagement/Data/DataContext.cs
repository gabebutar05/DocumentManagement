using DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<ParentFolder> ParentFolders { get; set; }
        public DbSet<SubFolder> SubFolders { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relasi ParentFolder dengan SubFolder
            modelBuilder.Entity<ParentFolder>()
                .HasMany(p => p.SubFolders)
                .WithOne(s => s.ParentFolder)
                .HasForeignKey(s => s.ParentFolderId)
                .OnDelete(DeleteBehavior.Cascade); // Hapus semua subfolder jika parent dihapus.

            // Relasi SubFolder dengan Folder
            modelBuilder.Entity<SubFolder>()
                .HasMany(s => s.Folders)
                .WithOne(f => f.SubFolder)
                .HasForeignKey(f => f.SubFolderId)
                .OnDelete(DeleteBehavior.Cascade); // Hapus semua folder jika subfolder dihapus.

            // Relasi Folder dengan Document
            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Documents)
                .WithOne(d => d.Folder)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.Cascade); // Hapus semua dokumen jika folder dihapus.

            // Properti unik atau aturan tambahan
            modelBuilder.Entity<ParentFolder>().Property(p => p.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<SubFolder>().Property(s => s.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Folder>().Property(f => f.Name).IsRequired().HasMaxLength(255);

            base.OnModelCreating(modelBuilder);
        }
    }
}
