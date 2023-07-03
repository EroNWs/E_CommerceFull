using E_Commerce.DataAccessLayer.Repository.IRepository;
using E_Commerce.Models.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace E_CommerceFull.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = StaticDetails_SD.Role_Admin)]
    public class CompanyController : Controller
    {
     
            private readonly IUnitOfWork _unitOfWork;
         
            public CompanyController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
               
            }

            public IActionResult Index()
            {
                var companyList = _unitOfWork.CompanyRepository.GetAll().ToList();
                return View(companyList);
            }



            public IActionResult Upsert(int? id)
            {
;
                if (id == null || id == 0)
                {
                    //create
                    return View(new Company());
                }
                else
                {
                    //update
                    Company company = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id);
                    return View(company);
                }

            }

            [HttpPost]
            public IActionResult Upsert(Company Company)
            {

                if (ModelState.IsValid)
                {


                    if (Company.Id == 0)
                    {
                        _unitOfWork.CompanyRepository.Add(Company);
                    }
                    else
                    {
                        _unitOfWork.CompanyRepository.Update(Company);
                    }

                    _unitOfWork.Save();                 
                    TempData["success"] = "Company Created/Updated Successfully";
                    return RedirectToAction("Index");

                }
                else
                {                  
                    return View(Company);

                }

            }

         


            #region API CALLS	
            [HttpGet]
            public IActionResult GetAll()
            {
                var companyList = _unitOfWork.CompanyRepository.GetAll().ToList();

                return Json(new { data = companyList });

            }

            [HttpDelete]
            public IActionResult Delete(int? id)
            {
                var companyToBeDeleted = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id);
                if (companyToBeDeleted == null)
                {
                    return Json(new { success = false, message = "Error while deleting"});
                }


                _unitOfWork.CompanyRepository.Delete(companyToBeDeleted);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Delete Successful" });
            }


            #endregion




    
    }
}
