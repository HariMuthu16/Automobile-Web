using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Infrastructure.Common;
using TopSpeed.Domain.Models;
using TopSpeed.Application.ApplicationConstants;
using TopSpeed.Application.Contracts.Presistence;
using Microsoft.AspNetCore.Authorization;

namespace TopSpeed.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CustomRole.MasterAdmin + "," + CustomRole.Admin)]
    public class BrandController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Brand> brands = await _unitOfWork.Brand.GetAllAsync();

            return View(brands);
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file[0].FileName);
                var upload = Path.Combine(webRootPath, @"images\brand");

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                brand.BrandLogo = @"\images\brand\" + newFileName + extension;
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Brand.Create(brand);
                await _unitOfWork.SaveAsync();

                TempData["success"] = CommonMessage.RecordCreate;
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file[0].FileName);
                var upload = Path.Combine(webRootPath, @"images\brand");

                var objFromDb = await _unitOfWork.Brand.GetByIdAsync(brand.Id);
                if (objFromDb.BrandLogo != null)
                {
                    var oldImgPath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }


                brand.BrandLogo = @"\images\brand\" + newFileName + extension;
            }


            if (ModelState.IsValid)
            {
                await _unitOfWork.Brand.Update(brand);
                await _unitOfWork.SaveAsync();
                TempData["warning"] = CommonMessage.RecordEdit;
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            if (!string.IsNullOrEmpty(brand.BrandLogo))
            {
                var objFromDb = await _unitOfWork.Brand.GetByIdAsync(brand.Id);
                if (objFromDb.BrandLogo != null)
                {
                    var oldImgPath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                }
            }

            await _unitOfWork.Brand.Delete(brand);
            await _unitOfWork.SaveAsync();


            TempData["danger"] = CommonMessage.RecordDelete;
            return RedirectToAction("Index");
        }


    }
}
