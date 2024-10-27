using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaDemo.Models;


namespace PizzaDemo.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicatioUsers {  get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "葷食"},
                new Category { Id = 2, Name = "奶素" }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "煙燻培根手撕豬比薩",
                    Description = "採用美式風味十足的煙燻醬，鋪上由楓糖調製而成的厚切培根、 火腿及香烤手撕豬，吸附醬汁精華同時滲出豬肉原汁肉香的美味。",
                    Price = 300,
                    CategoryId = 1,
                    ImageUrl = "\\images\\product\\39a67ce8-7c71-40b7-ae4f-46cff2e933b3.png"
                },
                new Product
                {
                    Id = 2,
                    Name = "夏威夷",
                    Description = "當甜中帶點微酸的鳳梨，遇上風味濃郁的火腿，絕妙味道，越吃越順口。",
                    Price = 280,
                    CategoryId = 1,
                    ImageUrl = "\\images\\product\\67acdf52-2357-4029-af6d-336723e8a1e5.png"
                },
                new Product
                {
                    Id = 3,
                    Name = "彩蔬鮮菇",
                    Description = "撒上鮮美的蘑菇與菠菜和番茄，再加上BBQ素醬，素食口味披薩讓你吃到蔬菜的清爽美味。",
                    Price = 260,
                    CategoryId = 2,
                    ImageUrl = "\\images\\product\\2376d0c5-785c-475c-9bdf-526670bb6640.png"
                }
                );
            modelBuilder.Entity<Store>().HasData(
               new Store { Id = 1, Name = "竹北中華店", Address = "新竹縣竹北市中華路391號", City = "新竹縣", PhoneNumber = "0987654321"},
               new Store { Id = 2, Name = "光復餐廳店", Address = "台北市大安區忠孝東路四段341號2樓", City = "台北市", PhoneNumber = "0911111111"}
               );
        }
    }

}
