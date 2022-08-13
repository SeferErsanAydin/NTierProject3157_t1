using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Project.MCVUI.Tools
{
    public class ImageUploader
    {
        public static string UploadImage(string serverPath, HttpPostedFileBase file, string name)
        {
            if (file != null)
            {
                Guid uniqueName = Guid.NewGuid(); //isime eklenecek Guid kodu oluştur
                string[] fileArray = file.FileName.Split('.');// . lar ile dosya ismini ayırıp bunlları bir arraya  atıyoruz

                string extension = fileArray[fileArray.Length-1].ToLower(); // dogal olarak son . 'dan sonraki (fileArray içindeki son ) eleman dosyanın uzantısı oluyor

                string fileName = $"{uniqueName}.{name}.{extension}"; // burada kullanacagımız dosya ismini (fileName'i) uniqueName + name + extention şeklinde oluşturuyoruz

                if (extension =="jpg" || extension == "gif" || extension == "png")
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(serverPath + fileName)))
                    {
                        return "1"; //dosya var kodu, Guid kullandıgımızdan dolayı alamayacagız
                    }
                    else
                    {
                        string filePath = HttpContext.Current.Server.MapPath(serverPath + fileName);
                        file.SaveAs(filePath);
                        return serverPath + fileName;
                    }
                }
                else
                {
                    return "2"; //bu bir resim degildir kodu 
                }
            }
            else
            {
                return "3";//dosya bos kodu
            }
        }
    }
}