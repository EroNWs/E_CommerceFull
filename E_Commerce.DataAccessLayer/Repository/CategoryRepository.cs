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
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private ECommerceDbContext _context;
        public CategoryRepository(ECommerceDbContext context) :base(context)
        {
			_context = context;   
        }    

		public void Update(Category category)
		{
			_context.Categories.Update(category);
		}
	}
}
