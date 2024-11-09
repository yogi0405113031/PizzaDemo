using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PizzaDemo.DataAccess.Repository.IRepository;
using PizzaDemo.Models;
using PizzaDemo.Models.ViewModels;
using PizzaDemo.Utility;

namespace PizzaDemo.Areas.Customer.Controllers
{
    [Area("Customer")]
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

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;

            double total = 0;
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                total += (cart.Product.Price * cart.Count);
            }

            // 檢查是否有折扣
            int? discount = HttpContext.Session.GetInt32("DiscountAmount");
            if (discount.HasValue)
            {
                ShoppingCartVM.OrderHeader.OrderTotal = total - discount.Value;
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderTotal = total;
            }

            return View(ShoppingCartVM);
        }
		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST(ShoppingCartVM shoppingCartVM)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            double total = 0;
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                total += (cart.Product.Price * cart.Count);
            }

            // 檢查是否有未完成的訂單（包含折扣後的金額）
            var existingOrder = _unitOfWork.OrderHeader.Get(u => u.ApplicationUserId == userId && u.OrderStatus == null);
            if (existingOrder != null)
            {
                // 使用已經折扣後的金額
                ShoppingCartVM.OrderHeader.OrderTotal = existingOrder.OrderTotal;
            }
            else
            {
                // 如果沒有折扣，使用原始金額
                ShoppingCartVM.OrderHeader.OrderTotal = total;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Cheese = cart.Cheese,
                    Crust = cart.Crust,
                    Price = cart.Product.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            // 刪除未完成的訂單（如果有）
            if (existingOrder != null)
            {
                _unitOfWork.OrderHeader.Remove(existingOrder);
                _unitOfWork.Save();
            }


            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}
		public IActionResult OrderConfirmation(int id)
		{
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusPending);

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
		}
		public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult ApplyCoupon(string code)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (code == "SAVE100")
            {
                var cartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product").ToList();

                double total = cartItems.Sum(cart => cart.Product.Price * cart.Count);

                var orderHeader = _unitOfWork.OrderHeader.Get(u => u.ApplicationUserId == userId && u.OrderStatus == null);
                if (orderHeader == null)
                {
                    var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

                    orderHeader = new OrderHeader
                    {
                        ApplicationUserId = userId,
                        OrderTotal = total - 100,
                        OrderDate = DateTime.Now,
                        // 添加必要的用戶信息
                        Name = applicationUser.Name,
                        PhoneNumber = applicationUser.PhoneNumber,
                        Address = applicationUser.Address,
                        ApplicationUser = applicationUser
                    };
                    _unitOfWork.OrderHeader.Add(orderHeader);
                }
                else
                {
                    orderHeader.OrderTotal = total - 100;
                    _unitOfWork.OrderHeader.Update(orderHeader);
                }

                _unitOfWork.Save();
                HttpContext.Session.SetInt32("DiscountAmount", 100);

                return Json(new
                {
                    success = true,
                    discount = 100,
                    newTotal = total - 100,
                    message = "折扣碼套用成功！"
                });
            }

            return Json(new
            {
                success = false,
                message = "無效的折扣碼"
            });
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
