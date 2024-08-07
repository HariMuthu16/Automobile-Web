﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Infrastructure.Common;
using TopSpeed.Domain.Models;
using TopSpeed.Application.ApplicationConstants;
using TopSpeed.Application.Contracts.Presistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopSpeed.Domain.ApplicationEnums;
using TopSpeed.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace TopSpeed.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CustomRole.MasterAdmin + "," + CustomRole.Admin)]

    public class PostController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _unitOfWork.Post.GetAllPost();

            return View(posts);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> engineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>()
            .Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            IEnumerable<SelectListItem> transmissionTypeList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>()
           .Select(x => new SelectListItem
           {
               Text = x.ToString().ToUpper(),
               Value = ((int)x).ToString()
           });

            PostVM postVM = new PostVM
            {
                Post = new Post(),
               BrandList = brandList,
               VehicleTypeList = vehicleTypeList,
               EngineAndFuelTypeList= engineAndFuelTypeList,
               TransmissionTypeList= transmissionTypeList

            };



            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file[0].FileName);
                var upload = Path.Combine(webRootPath, @"images\post");

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                post.VehicleImage = @"\images\post\" + newFileName + extension;
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Post.Create(post);
                await _unitOfWork.SaveAsync();

                TempData["success"] = CommonMessage.RecordCreate;
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> engineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>()
            .Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            IEnumerable<SelectListItem> transmissionTypeList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>()
           .Select(x => new SelectListItem
           {
               Text = x.ToString().ToUpper(),
               Value = ((int)x).ToString()
           });

            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelTypeList = engineAndFuelTypeList,
                TransmissionTypeList = transmissionTypeList

            };



            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file[0].FileName);
                var upload = Path.Combine(webRootPath, @"images\post");

                var objFromDb = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);
                if (objFromDb.VehicleImage != null)
                {
                    var oldImgPath = Path.Combine(webRootPath, objFromDb.VehicleImage.Trim('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }


                postVM.Post.VehicleImage = @"\images\post\" + newFileName + extension;
            }


            if (ModelState.IsValid)
            {
                await _unitOfWork.Post.Update(postVM.Post);
                await _unitOfWork.SaveAsync();
                TempData["warning"] = CommonMessage.RecordEdit;
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Post post = await _unitOfWork.Post.GetByIdAsync(id);
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });
            IEnumerable<SelectListItem> engineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>()
            .Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            IEnumerable<SelectListItem> transmissionTypeList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>()
           .Select(x => new SelectListItem
           {
               Text = x.ToString().ToUpper(),
               Value = ((int)x).ToString()
           });

            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelTypeList = engineAndFuelTypeList,
                TransmissionTypeList = transmissionTypeList

            };



            return View(postVM);
           
        }


        [HttpPost]
        public async Task<IActionResult> Delete(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            if (!string.IsNullOrEmpty(postVM.Post.VehicleImage))
            {
                var objFromDb = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);
                if (objFromDb.VehicleImage != null)
                {
                    var oldImgPath = Path.Combine(webRootPath, objFromDb.VehicleImage.Trim('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                }
            }

            await _unitOfWork.Post.Delete(postVM.Post);
            await _unitOfWork.SaveAsync();


            TempData["danger"] = CommonMessage.RecordDelete;
            return RedirectToAction("Index");
        }


    }
}
