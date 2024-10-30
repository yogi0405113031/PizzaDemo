using Microsoft.AspNetCore.Authorization;
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
    public class TeamMemberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TeamMemberController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<TeamMember> teammemberlist = _unitOfWork.TeamMember.GetAll().ToList();
            return View(teammemberlist);
        }
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new TeamMember());
            }
            else
            {
                var teamMember = _unitOfWork.TeamMember.Get(u => u.Id == id);
                return View(teamMember);
            }
        }

        [HttpPost]
        public IActionResult Upsert(TeamMember teamMember, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string teamMemberPath = Path.Combine(wwwRootPath, "images", "team");

                    // 確保目錄存在
                    if (!Directory.Exists(teamMemberPath))
                    {
                        Directory.CreateDirectory(teamMemberPath);
                    }

                    // 刪除舊圖片
                    if (!string.IsNullOrEmpty(teamMember.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath,
                            teamMember.ImageUrl.TrimStart('/', '\\').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string filePath = Path.Combine(teamMemberPath, fileName);

                    try
                    {
                        // 使用 ImageSharp 處理圖片
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
                            image.Save(filePath);
                        }

                        teamMember.ImageUrl = "/images/team/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        // 處理圖片處理過程中可能發生的錯誤
                        ModelState.AddModelError("", "圖片處理過程中發生錯誤: " + ex.Message);
                        return View(teamMember);
                    }
                }

                // 新增或更新
                if (teamMember.Id == 0)
                {
                    _unitOfWork.TeamMember.Add(teamMember);
                    TempData["success"] = "團隊成員新增成功！";
                }
                else
                {
                    _unitOfWork.TeamMember.Update(teamMember);
                    TempData["success"] = "團隊成員更新成功！";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(teamMember);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var teamMembers = _unitOfWork.TeamMember.GetAll().ToList();
            return Json(new { data = teamMembers });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var teamMember = _unitOfWork.TeamMember.Get(u => u.Id == id);
            if (teamMember == null)
            {
                return Json(new { success = false, message = "刪除失敗" });
            }

            // 刪除圖片
            if (!string.IsNullOrEmpty(teamMember.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                    teamMember.ImageUrl.TrimStart('/', '\\').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.TeamMember.Remove(teamMember);
            _unitOfWork.Save();

            return Json(new { success = true, message = "刪除成功" });
        }

        #endregion
    }
}
