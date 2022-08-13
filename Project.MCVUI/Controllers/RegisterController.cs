using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MCVUI.Controllers
{
    public class RegisterController : Controller
    {
        AppUserRepository _apRep;
        ProfileRepository _proRep;
        public RegisterController()
        {
            _apRep = new AppUserRepository();
            _proRep= new ProfileRepository();
        }

        public string Dantexcrypt { get; private set; }

        public ActionResult RegisterNow()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterNow(AppUser appUser, AppUserProfile userProfile)
        {
            appUser.Password = DantexCrypt.Crypt(appUser.Password); //şifre kriptolandı

            if (_apRep.Any(x=>x.UserName == appUser.UserName))
            {
                ViewBag.ZatenVar = "Kullanici ismi önceden alınmış";
                return View();
            }
            else if (_apRep.Any(x=>x.Email == appUser.Email))
            {
                ViewBag.ZatenVar = "Email zaten kayıtlı";
                return View();
            }

            //register işlemi başarılıysa aktivasyon kodu gönder
            string gonderilecekEmail = "Tebrikler... Hesabınız Oluşturulmuştur... Hesabınızı aktive etmek için https://localhost:44363/Register/Activation/" + appUser.ActivationCode + " linkine tıklayınız";

            MailService.Send(appUser.Email, body: gonderilecekEmail, subject: "Hesap aktivasyonu!!!");
            _apRep.Add(appUser);
            if (!string.IsNullOrEmpty(userProfile.FirstName.Trim())|| !string.IsNullOrEmpty(userProfile.LastName.Trim()))
            {
                userProfile.ID = appUser.ID;
                _proRep.Add(userProfile);
            }
            return View("RegisterOK");
        }
        public ActionResult RegisterOK()
        {
            return View();
        }
        public ActionResult Activation(Guid id)
        {
            AppUser aktifEdilecek = _apRep.FirstOrDefault(x => x.ActivationCode == id);
            if (aktifEdilecek != null)
            {
                aktifEdilecek.Active = true;
                _apRep.Update(aktifEdilecek);
                TempData["HesapAktifmi"] = "Hesabınız aktif hale getirildi";
                return RedirectToAction("Login","Home");
            }
            //şüpheli aktivite
            TempData["HesapAktifmi"] = "Hesabınız bulunamadı";
            return RedirectToAction("Login", "Home");
        }
    }
}