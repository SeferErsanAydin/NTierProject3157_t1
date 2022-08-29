using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project.MCVUI.AuthenticationClasses;

namespace Project.MCVUI.Controllers
{
    [MemberAuthentication]
    public class ShoppingController : Controller
    {
        OrderRepository _oRep;
        ProductRepository _pRep;
        CategoryRepository _cRep;
        OrderDetailRepository _odRep;

        public ShoppingController()
        {
            _odRep = new OrderDetailRepository();
            _cRep = new CategoryRepository();
            _pRep = new ProductRepository();
            _oRep = new OrderRepository();
            
        }
        public ActionResult ShoppingList(int? page, int? categoryID)
        {
            
            return View();
        }
    }
}