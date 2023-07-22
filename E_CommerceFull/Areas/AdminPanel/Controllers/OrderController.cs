using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace E_CommerceFull.Areas.AdminPanel.Controllers
{
	[Area("AdminPanel")]
	[Authorize]
	public class OrderController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
		  OrderVM = new()

			{
				OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
		return View(OrderVM);
		
		}

		[HttpPost]
		[Authorize(Roles =StaticDetails_SD.Role_Admin+","+StaticDetails_SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
			var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

			orderHeader.Name = OrderVM.OrderHeader.Name;
			orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;	
			orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeader.City = OrderVM.OrderHeader.City;
			orderHeader.State = OrderVM.OrderHeader.State;
			orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{ 
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			
			}
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{ 
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			
			}

			_unitOfWork.OrderHeaderRepository.Update(orderHeader);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });

        }


		[HttpPost]
		[Authorize(Roles = StaticDetails_SD.Role_Admin + "," + StaticDetails_SD.Role_Employee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails_SD.StatusInProcess);
			_unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }




		[HttpPost]
		[Authorize(Roles = StaticDetails_SD.Role_Admin + "," + StaticDetails_SD.Role_Employee)]
		public IActionResult ShipOrder()
		{

			var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = StaticDetails_SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			if (orderHeader.PaymentStatus == StaticDetails_SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
			}

			_unitOfWork.OrderHeaderRepository.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order Shipped Successfully.";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
		}





		[HttpPost]
		[Authorize(Roles = StaticDetails_SD.Role_Admin + "," + StaticDetails_SD.Role_Employee)]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);

			if (orderHeader.PaymentStatus == StaticDetails_SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{

					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails_SD.StatusCancelled, StaticDetails_SD.StatusRefunded);

			}
			else
			{
				_unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails_SD.StatusCancelled, StaticDetails_SD.StatusCancelled);

			}

			_unitOfWork.Save();
			TempData["Success"] = "Order Cancelled Successfully.";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}


		[ActionName("Details")]
		[HttpPost]
		public IActionResult Details_PAY_NOW() 
		{
			OrderVM.OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");

			OrderVM.OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(o => o.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

			//stripe logic
			var domain = "https://localhost:7025/";

			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",

			};
			foreach (var item in OrderVM.OrderDetail)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),
						Currency = "USD",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

		}


		public IActionResult PaymentConfirmation(int orderHeaderId)
		{

			OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderHeaderId);
			if (orderHeader.PaymentStatus == StaticDetails_SD.PaymentStatusDelayedPayment)
			{
				//this is an order by company 
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, StaticDetails_SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

		


			return View(orderHeaderId);

		}






		#region API CALLS	
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;


			if (User.IsInRole(StaticDetails_SD.Role_Admin) || User.IsInRole(StaticDetails_SD.Role_Employee))
			{

				orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();


			}
			else 
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			
			
			}




			switch (status) {

				case "pending":
					orderHeaders = orderHeaders.Where(x => x.PaymentStatus == StaticDetails_SD.PaymentStatusPending);
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == StaticDetails_SD.StatusInProcess);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == StaticDetails_SD.StatusShipped);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == StaticDetails_SD.StatusApproved);
					break;
				default:					
					break;


			}



			return Json(new { data = orderHeaders });


		}

	


		#endregion



	}
}
