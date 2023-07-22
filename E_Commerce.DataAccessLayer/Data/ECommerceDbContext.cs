using E_Commerce.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccessLayer.Data
{
    public class ECommerceDbContext: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<Microsoft.AspNetCore.Identity.IdentityUser>
    {
        public DbSet<Category> Categories { get; set; }

		public DbSet<Product> Products { get; set; }

		public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }


        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options):base(options)
        {
            
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); 

			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Action", DisplayOrder = 5 },
				new Category { Id = 2, Name = "Science Fiction", DisplayOrder = 6 },
				new Category { Id = 3, Name = "History", DisplayOrder = 15 },
				new Category { Id = 4, Name = "Finance", DisplayOrder = 1}

				);


			modelBuilder.Entity<Company>().HasData(
			new Company { Id = 1, CompanyName="Tesla", StreetAddress="USA", City="USA", State="USA", PhoneNumber="1231251251", PostalCode="123we"   },
			new Company { Id = 2, CompanyName = "BigChef", StreetAddress = "USA", City = "USA", State = "USA", PhoneNumber = "123125231251", PostalCode = "123wsde" },
			new Company { Id = 3, CompanyName = "GeneralMotor", StreetAddress = "USA", City = "USA", State = "USA", PhoneNumber = "12312513251", PostalCode = "123wsde" }
		

			);

			modelBuilder.Entity<Product>().HasData(
				new Product 
				{ 
					Id = 1, 
					Title = "The Psychology of Money", 
					Author = "Morgan HOUSEL", 
					ISBN = "9789390166268", 
					Description = "In the Psychology of Money, Morgan Housel teaches you how to have a better relationship with money and to make smarter financial decisions. Instead of pretending that humans are ROI-optimizing machines, he shows you how your psychology can work for and against you.",
					ListPrice = 10,
					Price = 9, 
					Price50 = 7, 
					Price100 = 6,
					CategoryId = 4
					
				},

				new Product 
				{ 
					Id = 2, 
					Title = "Sapiens A Brief History of Humankind", 
					Author = "Yuval Noah HARARI", 
					ISBN = "9780062316097", 
					Description = "Fire gave us power. Farming made us hungry for more. Money gave us purpose. Science made us deadly. This is the thrilling account of our extraordinary history – from insignificant apes to rulers of the world.\r\n\r\nEarth is 4.5 billion years old. In just a fraction of that time, one species among countless others has conquered it: us.\r\n\r\nIn this bold and provocative book, Yuval Noah Harari explores who we are, how we got here and where we’re going.\r\n\r\n‘I would recommend Sapiens to anyone who’s interested in the history and future of our species’ Bill Gates", 
					ListPrice = 20, 
					Price = 18, 
					Price50 = 17, 
					Price100 = 15,
					CategoryId = 3
					
				},
				new Product
				{
					Id = 3,
					Title = "Fortune of Time",
					Author = "Billy Spark",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "SWD9999001",
					ListPrice = 99,
					Price = 90,
					Price50 = 85,
					Price100 = 80, 
					CategoryId = 1
					
				},
				new Product
				{
					Id = 4,
					Title = "Dark Skies",
					Author = "Nancy Hoover",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "CAW777777701",
					ListPrice = 40,
					Price = 30,
					Price50 = 25,
					Price100 = 20,
					CategoryId = 2
					
				},
				new Product
				{
					Id = 5,
					Title = "Vanish in the Sunset",
					Author = "Julian Button",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "RITO5555501",
					ListPrice = 55,
					Price = 50,
					Price50 = 40,
					Price100 = 35,
					CategoryId = 4
					
				},
				new Product
				{
					Id = 6,
					Title = "Cotton Candy",
					Author = "Abby Muscles",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "WS3333333301",
					ListPrice = 70,
					Price = 65,
					Price50 = 60,
					Price100 = 55,
					CategoryId = 1
					
				},
				new Product
				{
					Id = 7,
					Title = "Rock in the Ocean",
					Author = "Ron Parker",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "SOTJ1111111101",
					ListPrice = 30,
					Price = 27,
					Price50 = 25,
					Price100 = 20,
					CategoryId = 2
				
				},
				new Product
				{
					Id = 8,
					Title = "Leaves and Wonders",
					Author = "Laura Phantom",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "FOT000000001",
					ListPrice = 25,
					Price = 23,
					Price50 = 22,
					Price100 = 20,
					CategoryId = 3
				
				}

				);





		}



	}
}
