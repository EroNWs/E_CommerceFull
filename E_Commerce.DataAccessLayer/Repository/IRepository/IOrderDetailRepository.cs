﻿using E_Commerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DataAccessLayer.Repository.IRepository
{
	public interface IOrderDetailRepository:IRepository<OrderDetail>
	{
		void Update(OrderDetail orderDetail);
	}
}
