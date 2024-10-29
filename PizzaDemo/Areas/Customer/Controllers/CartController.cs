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

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
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

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
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
        public IActionResult ApplyCoupon(string code, int orderId)
        {
            if (code.ToUpper() == "SAVE100")
            {
                var order = _unitOfWork.OrderHeader.Get(o => o.Id == orderId);
                if (order != null)
                {
                    order.OrderTotal -= 100;
                    _unitOfWork.Save();

                    return Json(new
                    {
                        success = true,
                        discount = 100,
                        newTotal = order.OrderTotal,
                        message = "折扣碼套用成功！"
                    });
                }
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
