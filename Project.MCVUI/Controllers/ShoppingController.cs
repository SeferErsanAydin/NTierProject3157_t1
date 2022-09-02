using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project.MCVUI.AuthenticationClasses;
using Project.MCVUI.VMClasses;
using Project.MCVUI.Models.ShoppingTools;
using Project.ENTITIES.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Project.COMMON.Tools;

namespace Project.MCVUI.Controllers
{
    
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
            PaginationVM pavm = new PaginationVM
            {
                PagedProducts = categoryID == null ? _pRep.GetActives().ToPagedList(page ?? 1, 9) : _pRep.Where(x => x.CategoryID == categoryID).ToPagedList(page ?? 1, 9),
                Categories = _cRep.GetActives()
            };
            if (categoryID != null)
            {
                TempData["catID"] = categoryID;
            }

            return View(pavm);
        }
        public ActionResult AddToCart(int id)
        {
            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;
            Product addedItem = _pRep.Find(id);

            CartItem ci = new CartItem
            {
                ID = addedItem.ID,
                Name = addedItem.ProductName,
                Price = addedItem.UnitPrice,
                ImagePath = addedItem.ImagePath
            };

            c.SepeteEkle(ci);
            Session["scart"] = c;
            return RedirectToAction("ShoppingList");
        }

        public ActionResult CartPage()
        {
            if (Session["scart"] != null)
            {
                CartPageVM cpvm = new CartPageVM();
                Cart c = Session["scart"] as Cart;
                cpvm.Cart = c;
                return View(cpvm);
            }
            TempData["bos"] = "Sepetinizde ürün bulunmamaktadır";
            return RedirectToAction("ShoppingList");
        }

        public ActionResult DeleteFromCart(int id)
        {
            if (Session["scart"]!= null)
            {
                Cart c = Session["scart"] as Cart;
                c.SepettenCikar(id);
                if (c.Sepetim.Count == 0)
                {
                    Session.Remove("scart");
                    TempData["sepetBos"] = "Sepetinizdeki tüm ürünler çıkarılmıştır";
                    return RedirectToAction("ShoppingList");
                }
                return RedirectToAction("CartPage");
            }
            return RedirectToAction("ShoppingList");
        }
        public ActionResult ConfirmOrder()
        {
            AppUser currentUser;
            if (Session["member"] != null)
            {
                currentUser = Session["member"] as AppUser;
            }
            else TempData["anonim"] = "Kullanici üye değil";
            return View();
        }

        // https://localhost:44366/api/Payment/ReceivePayment

        [HttpPost]
        public ActionResult ConfirmOrder(OrderVM ovm)
        {
            bool result;
            Cart sepet = Session["scart"] as Cart;
            ovm.Order.TotalPrice = ovm.PaymentDTO.ShoppingPrice = sepet.TotalPrice;

            //api section
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44366/api/");
                Task<HttpResponseMessage> postTask = client.PostAsJsonAsync("Payment/ReceivePayment", ovm.PaymentDTO);
                HttpResponseMessage sonuc;

                try
                {
                    sonuc = postTask.Result;
                }
                catch (Exception)
                {
                    TempData["baglantiRed"] = "Banka bağlantiyi reddetti";
                    return RedirectToAction("ShoppingList");
                }

                if (sonuc.IsSuccessStatusCode) result = true;
                else result = false;

                if (result)
                {
                    if (Session["member"]!= null)
                    {
                        AppUser kullanici = Session["member"] as AppUser;
                        ovm.Order.AppUserID = kullanici.ID;
                        ovm.Order.UserName = kullanici.UserName;
                    }
                    else
                    {
                        ovm.Order.AppUserID = null;
                        ovm.Order.UserName = TempData["anonim"].ToString();
                    }

                    _oRep.Add(ovm.Order);

                    foreach (CartItem item in sepet.Sepetim)
                    {
                        OrderDetail od = new OrderDetail();
                        od.OrderID = ovm.Order.ID;
                        od.ProductID = item.ID;
                        od.Quantity = item.Amount;
                        _odRep.Add(od);

                        //stok eksiltme
                        Product stokDus = _pRep.Find(item.ID);
                        stokDus.UnitsInStock -= item.Amount;
                        _pRep.Update(stokDus);
                    }
                    TempData["odeme"] = "Siparişiniz bize ulaşmıştır, teşekkür ederiz";
                    MailService.Send(ovm.Order.Email, body: $"Siparişiniz alındı{ovm.Order.TotalPrice}");

                    Session.Remove("scart");
                    return RedirectToAction("ShoppingList");
                }
                else
                {
                    Task<string> s = sonuc.Content.ReadAsStringAsync();
                    TempData["sorun"] = s.Result;
                    return RedirectToAction("ShoppingList");
                }
            }
            //api section bitiş
        }
    }
}