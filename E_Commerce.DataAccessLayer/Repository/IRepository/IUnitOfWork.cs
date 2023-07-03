using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }

        IProductRepository ProductRepository { get; }

        IProductImageRepository ProductImageRepository { get; }

        ICompanyRepository CompanyRepository { get; }

        IShoppingCartRepository ShoppingCartRepository { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }

        IOrderDetailRepository OrderDetailRepository { get; }

        IOrderHeaderRepository OrderHeaderRepository { get; }

        void Save();
    }
}
