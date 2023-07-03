using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace E_CommerceFull.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
	[Authorize(Roles = StaticDetails_SD.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

		public IActionResult Index()
		{
            var productList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
			return View(productList);
		}



		public IActionResult Upsert(int? id)
        {

			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};

			if (id == null || id == 0)
			{
				//create
				return View(productVM);
			}
			else
			{
				//update
				productVM.Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id,includeProperties: "ProductImages");
				return View(productVM);
			}

		}

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files) 
        {

            if (ModelState.IsValid)
            {


				if (productVM.Product.Id == 0)
				{
					_unitOfWork.ProductRepository.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.ProductRepository.Update(productVM.Product);
				}

				_unitOfWork.Save();



				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null)
				{

					foreach (IFormFile file in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\products\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);

						if (!Directory.Exists(finalPath))
							Directory.CreateDirectory(finalPath);

						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}

						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = productVM.Product.Id,
						};

						if (productVM.Product.ProductImages == null)
							productVM.Product.ProductImages = new List<ProductImage>();

						productVM.Product.ProductImages.Add(productImage);

					}

					_unitOfWork.ProductRepository.Update(productVM.Product);
					_unitOfWork.Save();




				}
				TempData["success"] = "Product Created/Updated Successfully";
                return RedirectToAction("Index");
            
            }
            else 
            {
                productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()

                });
				return View(productVM);

			}                     
                    
        }

		//Existing edit and delete functionality changed update - upsert combine with update - create 
		//delete in apıcalls
		//public IActionResult Edit(int? id)
		//{
		//    if (id == null || id == 0)
		//    {
		//        return NotFound();
		//    }

		//    Product? product = _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == id);

		//    if (product == null)
		//    {
		//        return NotFound();
		//    }

		//    return View(product);

		//}

		//[HttpPost]
		//public IActionResult Edit(Product product)
		//{
		//    if (ModelState.IsValid)
		//    {

		//        _unitOfWork.ProductRepository.Update(product);
		//        _unitOfWork.Save();
		//        TempData["success"] = "Product Updatede Successfully";
		//        return RedirectToAction("Index");

		//    }

		//    return View();


		//}



		//public IActionResult Delete(int? id)
		//{
		//    if (id == null || id == 0)
		//    {
		//        return NotFound();

		//    }

		//    Product? product = _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == id);

		//    if (product == null)
		//    {
		//        return NotFound();
		//    }

		//    return View(product);


		//}


		//      [HttpPost, ActionName("Delete")]
		//      public IActionResult DeletePost(int? id) 
		//      {
		//	Product? product = _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == id);

		//	if (product == null)
		//	{
		//		return NotFound();
		//	}

		//		_unitOfWork.ProductRepository.Delete(product);
		//		_unitOfWork.Save();
		//		TempData["success"] = "Product Deleted Successfully";
		//		return RedirectToAction("Index");


		//}




		#region API CALLS	
		[HttpGet]
		public IActionResult GetAll() 
		{
			var productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();

			return Json(new { data = productList });

		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}

				Directory.Delete(finalPath);
			}


			_unitOfWork.ProductRepository.Delete(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}


		#endregion




	}
}
