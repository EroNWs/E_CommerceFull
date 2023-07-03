using E_Commerce.DataAccessLayer.Data;
using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private ECommerceDbContext _context;
        public ProductRepository(ECommerceDbContext context) :base(context)
        {
			_context = context;   
        }    

		public void Update(Product product)
		{
			var productList = _context.Products.FirstOrDefault(p => p.Id==product.Id);
			if (productList != null)
			{
				productList.Title = product.Title;
				productList.ISBN = product.ISBN;
				productList.ListPrice = product.ListPrice;
				productList.Price = product.Price;
				productList.Price50 = product.Price50;
				productList.Price100 = product.Price100;
				productList.Description = product.Description;
				productList.CategoryId = product.CategoryId;
				productList.Author = product.Author;
				productList.ProductImages = product.ProductImages;

			}

			
		}
	}
}
