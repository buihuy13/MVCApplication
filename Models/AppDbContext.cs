using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models.Product;
using MySql.Data.MySqlClient;


namespace MVCTest.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductCategoryProduct> ProductCategoryProducts { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductPhoto> productPhotos {  get; set; } 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public string create()
        {
            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder["port"] = "3306";
            stringBuilder["server"] = "localhost";
            stringBuilder["database"] = "MVC";
            stringBuilder["UID"] = "root";
            stringBuilder["PWD"] = "huydaica";
            return stringBuilder.ToString();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(create());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Xóa đi AspNet có trong tên của các table csdl Identity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.CategoryId });
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
            });

            modelBuilder.Entity<ProductCategoryProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryProductId });
            });

            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
            });
        }
    }
}
