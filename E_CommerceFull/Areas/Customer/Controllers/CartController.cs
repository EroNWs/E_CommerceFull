using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace E_CommerceFull.Areas.Customer.Controllers
{
	[Area("customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;

		}

		[Route("cart")]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()

			};
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImageRepository.GetAll();

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}



			return View(ShoppingCartVM);
		}


		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()

			};
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImageRepository.GetAll();

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == userId);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}


			return View(ShoppingCartVM);
		}


		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == userId);


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it is a regular customer 
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails_SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails_SD.StatusPending;
			}
			else
			{
				//it is a company user
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails_SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails_SD.StatusApproved;
			}
			_unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetailRepository.Add(orderDetail);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it is a regular customer account and we need to capture payment
				//stripe logic
				var domain = "https://localhost:7025/";

				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + "customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",

				};
				foreach (var item in ShoppingCartVM.ShoppingCartList)
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
				_unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}


		public IActionResult OrderConfirmation(int id)
		{

			OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "ApplicationUser");
			if (orderHeader.PaymentStatus != StaticDetails_SD.PaymentStatusDelayedPayment) 
			{
			//this is an order by customer 
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeaderRepository.UpdateStatus(id, StaticDetails_SD.StatusApproved, StaticDetails_SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}			
			}

			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCartRepository.DeleteRange(shoppingCarts);
			_unitOfWork.Save();


			return View(id);

		}






		public IActionResult Plus(int cartId)
		{
			var cartPlus = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			cartPlus.Count += 1;
			_unitOfWork.ShoppingCartRepository.Update(cartPlus);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));

		}

		public IActionResult Minus(int cartId)
		{
			var cartPlus = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			if (cartPlus.Count <= 1)
			{
				//if cart less than or equal 1 remove that cart from db
				_unitOfWork.ShoppingCartRepository.Delete(cartPlus);

			}
			else
			{
				//if not less than or equal to 1 minus action continue

				cartPlus.Count -= 1;
				_unitOfWork.ShoppingCartRepository.Update(cartPlus);
			}

			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));

		}

		public IActionResult Remove(int cartId)
		{
			var cartPlus = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
			_unitOfWork.ShoppingCartRepository.Delete(cartPlus);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));

		}



		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
			{

				if (shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price50;
				}
				else
				{
					return shoppingCart.Product.Price100;
				}

			}

		}



	}
}
