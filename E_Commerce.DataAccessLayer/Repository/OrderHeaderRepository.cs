using E_Commerce.DataAccessLayer.Data;
using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ECommerceDbContext _context;

        public OrderHeaderRepository(ECommerceDbContext context):base(context)
        {
			_context = context;
        }


        public void Update(OrderHeader orderHeader)
		{
			_context.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var order = _context.OrderHeaders.FirstOrDefault(x=>x.Id== id);
			if (order != null) 
			{
				order.OrderStatus = orderStatus;
				if(!string.IsNullOrEmpty(paymentStatus))
					order.PaymentStatus = paymentStatus;
			}

		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (!string.IsNullOrEmpty(sessionId)) 
			{
			order.SessionId = sessionId;
			
			}

			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				order.PaymentIntentId = paymentIntentId;
				order.PaymentDate = DateTime.Now;

			}


		}
	}
}
