﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaDemo.DataAccess.Data;
using PizzaDemo.DataAccess.Repository.IRepository;
using PizzaDemo.Models;
using PizzaDemo.Models.ViewModels;
using PizzaDemo.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PizzaDemo.Areas.Admin.Controllers

{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Manager)]
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
            List<Product> productlist = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(productlist);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                //編輯
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    try
                    {
                        using (var image = Image.Load(file.OpenReadStream()))
                        {
                            // 設定最大尺寸
                            int maxWidth = 600;   // 可以根據需求調整
                            int maxHeight = 800;  // 可以根據需求調整

                            // 如果圖片超過最大尺寸才進行縮放
                            if (image.Width > maxWidth || image.Height > maxHeight)
                            {
                                // 計算縮放比例
                                double ratioX = (double)maxWidth / image.Width;
                                double ratioY = (double)maxHeight / image.Height;
                                double ratio = Math.Min(ratioX, ratioY);

                                // 計算新的尺寸
                                int newWidth = (int)(image.Width * ratio);
                                int newHeight = (int)(image.Height * ratio);

                                // 調整圖片大小
                                image.Mutate(x => x.Resize(newWidth, newHeight));
                            }

                            // 保存圖片
                            image.Save(Path.Combine(productPath, fileName));
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "圖片處理過程中發生錯誤: " + ex.Message);
                        productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        });
                        return View(productVM);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "產品新增成功！";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "刪除失敗" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "刪除成功" });
        }
        #endregion
    }
}
