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
        public DbSet<TeamMember> TeamMembers { get; set; }


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
                    ImageUrl = "\\images\\product\\80f2b999-1a13-4915-92be-f03e44c7ff83.png"
                }
                );
            modelBuilder.Entity<Store>().HasData(
                   new Store { Id = 1, Name = "竹北中華店", Address = "新竹縣竹北市中華路391號", City = "新竹縣", PhoneNumber = "0987654321"},
                   new Store { Id = 2, Name = "光復餐廳店", Address = "台北市大安區忠孝東路四段341號2樓", City = "台北市", PhoneNumber = "0911111111"}
               );
            modelBuilder.Entity<TeamMember>().HasData(
                new TeamMember 
                {
                    Id = 1,
                    Name = "王曉明",
                    Position = "主廚",
                    Introduction = "對披薩烹飪充滿熱情，樂於嘗試新事物、接受顧客的建議並持續改進。他認為好的披薩應該不僅滿足顧客的味蕾，更能帶給顧客幸福感。",
                    ImageUrl = "/images/team/21b2d8dd-7877-4c4e-81ea-51a4673b7110.jpg"
                },
                new TeamMember 
                { 
                    Id = 2,
                    Name = "陳大鵬",
                    Position = "副主廚",
                    Introduction = "熱愛披薩料理，致力於創新和優化口味。他在工作中注重細節並保持積極學習的態度，並相信每一款披薩都應該展現出料理人的用心和對品質的追求。",
                    ImageUrl = "/images/team/c778129f-12f4-4646-a127-22bf43137f15.jpg"
                }
                );
        }
    }

}
