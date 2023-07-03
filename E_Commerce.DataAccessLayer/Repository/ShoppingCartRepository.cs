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
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private ECommerceDbContext _context;

        public ShoppingCartRepository(ECommerceDbContext context):base(context) 
        {
            _context= context;
        }



        public void Update(ShoppingCart shoppingCart)
		{
			_context.ShoppingCarts.Update(shoppingCart);
		}
	}
}
