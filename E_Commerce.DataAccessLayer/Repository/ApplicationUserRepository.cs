using E_Commerce.DataAccessLayer.Data;
using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository
{
	public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
	{

		private ECommerceDbContext _context;

        public ApplicationUserRepository(ECommerceDbContext context):base(context)
        {
            _context = context;
        }

    }
}
