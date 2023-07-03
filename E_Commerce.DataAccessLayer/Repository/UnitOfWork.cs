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
    public class UnitOfWork : IUnitOfWork
    {
        private ECommerceDbContext _context;
        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository ProductRepository { get; private set; }

        public IProductImageRepository ProductImageRepository { get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }

        public IShoppingCartRepository ShoppingCartRepository { get; private set; }

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }

        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }


        public UnitOfWork(ECommerceDbContext context)
        {
            _context = context;          
            CategoryRepository = new CategoryRepository(_context);
            ProductRepository = new ProductRepository(_context);
            ProductImageRepository = new ProductImageRepository(_context);
            CompanyRepository = new CompanyRepository(_context);
			ShoppingCartRepository = new ShoppingCartRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context);
            OrderDetailRepository = new OrderDetailRepository(_context);
            OrderHeaderRepository = new OrderHeaderRepository(_context);
		}


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
