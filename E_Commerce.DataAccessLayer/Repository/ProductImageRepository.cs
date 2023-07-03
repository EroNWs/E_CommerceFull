using E_Commerce.DataAccessLayer.Data;
using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
	{
		private ECommerceDbContext _context;

        public ProductImageRepository(ECommerceDbContext context):base(context)
        {
			_context = context;
        }


        public void Update(ProductImage pImage)
		{
			_context.ProductImages.Update(pImage);
		}
	}
}
