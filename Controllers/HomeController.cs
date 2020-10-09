using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageUploadToDBWithDotnetCore.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ImageUploadToDBWithDotnetCore.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private IHostingEnvironment _environment;
        private MySql.Data.MySqlClient.MySqlConnection conn;
        private string _connString;
        public HomeController(IConfiguration configuration, IHostingEnvironment hostEnvironment)
        {
            _environment = hostEnvironment;
            _configuration = configuration;
            _connString = configuration.GetSection("ConnectionString").Value;

            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = _connString;
        }

        public IActionResult Index()
        {
            ViewData["apiResponse"] = "";
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            long fileSize;
            byte[] imageByte = null;
            var fileContent = user.Image;
            DateTime time = DateTime.Now;
            string fileTime = time.ToString("yyyyMMddHHmm");
            try
            {
                string origFileName = user.Image.FileName;
                string ext = Path.GetExtension(user.Image.FileName);
                string fileName = "IMAGE" + fileTime + ext;
                string uploadsFolder = System.IO.Path.Combine("wwwroot/images/");
                //string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

                string fileFQN = Path.Combine(uploadsFolder, fileName);
                FileStream fStream = new FileStream(fileFQN, FileMode.Create);
                user.Image.CopyTo(fStream);
                fStream.Close();
                FileStream fStream4db = new FileStream(fileFQN, FileMode.Open);
                BinaryReader binReader = new BinaryReader(fStream4db);
                imageByte = binReader.ReadBytes((int)fStream4db.Length);
                fileSize = fStream4db.Length;

                long id = insertFile(user.Name, fileName, imageByte, fileSize, fileFQN, origFileName);
                ViewData["apiResponse"] = origFileName + " was uploaded successfully.";
                return View();
            }
            catch (Exception)
            {
                //do 
                throw;
            }
        }
        
        public long insertFile(string refnum, string fileName, Array byteArray, long fileSize, string fileLocation, string origFileName)
        {
            string sqlInsert = "INSERT INTO IMAGE_DETAILS(REF_ID, FILENAME, IMAGE, FILE_LOCATION, SIZE, ORIG_FILENAAME) " +
                "VALUES('" + refnum + "', '" + fileName + "', @IMAGE, '" + fileLocation + "', '" + fileSize + "', '" + origFileName + "')";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlInsert, conn);

            conn.Open();
            try
            {
                cmd.Parameters.AddWithValue("@IMAGE", byteArray);
                cmd.ExecuteNonQuery();
                long id = cmd.LastInsertedId;
                return id;
            }
            catch (Exception ex)
            {
                new Exception("Unable to insertFile: ", ex);
            }
            return 0;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
