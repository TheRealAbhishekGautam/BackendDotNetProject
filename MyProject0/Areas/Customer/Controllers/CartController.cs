﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using System.Security.Claims;

namespace MyProject0.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        internal readonly IUnitOfWork _unitOfWork;
        internal ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM() {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x=> x.ApplicationUserId == userId,"Product")
            };

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Count * cart.Price);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Plus(int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            ShoppingCartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(ShoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Minus (int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            if (ShoppingCartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(ShoppingCartFromDb);
            }
            else
            {
                ShoppingCartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(ShoppingCartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Remove (int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(ShoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if(shoppingCart.Count <= 100)
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
