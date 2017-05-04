using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using LaunchpadTest.Models;
using System.Web.Mvc;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace LaunchpadTest.Controllers {
    public class ImageController : Controller {
        // GET: Image

        public ActionResult ViewImage(int imageVal) {
            string[] dirs = Directory.GetFiles(Server.MapPath("~/Data/Face"), "*");
            ViewBag.Files = dirs;
            if (imageVal == 0) {
                ViewBag.LargeImage = Url.Content("~/Data/Large/" + new FileInfo(dirs[0]).Name);
                ViewBag.NextImage = 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[1]).Name);
                ViewBag.LastImage = dirs.Length - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[dirs.Length-1]).Name);
                ViewBag.imageValue = 0;
            }
            else {
                ViewBag.LargeImage = Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal]).Name);

                if (imageVal > dirs.Length - 2) {
                    ViewBag.NextImage = 0;//Url.Content("~/Data/Large/" + new FileInfo(dirs[0]).Name);
                }
                else {
                    ViewBag.NextImage = imageVal + 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal + 1]).Name);
                }
                if (imageVal == 0) {
                    ViewBag.LastImage = dirs.Length - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[dirs.Length - 1]).Name);
                }
                else {
                    ViewBag.LastImage = imageVal - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal - 1]).Name);
                }
                ViewBag.imageValue = imageVal;
            }
            return View();
        }

        public ActionResult ViewImageCarousel(int imageVal) {
            string[] dirs = Directory.GetFiles(Server.MapPath("~/Data/Face"), "*");
            ViewBag.Files = dirs;
            if (imageVal == 0) {
                ViewBag.LargeImage = Url.Content("~/Data/Large/" + new FileInfo(dirs[0]).Name);
                ViewBag.NextImage = 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[1]).Name);
                ViewBag.LastImage = dirs.Length - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[dirs.Length-1]).Name);
                ViewBag.imageValue = 0;
            }
            else {
                ViewBag.LargeImage = Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal]).Name);

                if (imageVal > dirs.Length - 2) {
                    ViewBag.NextImage = 0;//Url.Content("~/Data/Large/" + new FileInfo(dirs[0]).Name);
                }
                else {
                    ViewBag.NextImage = imageVal + 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal + 1]).Name);
                }
                if (imageVal == 0) {
                    ViewBag.LastImage = dirs.Length - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[dirs.Length - 1]).Name);
                }
                else {
                    ViewBag.LastImage = imageVal - 1;//Url.Content("~/Data/Large/" + new FileInfo(dirs[imageVal - 1]).Name);
                }
                ViewBag.imageValue = imageVal;
            }
            return View();
        }


        [HttpGet]
        public ActionResult UploadFile() {
            return View();
        }



        private bool IsImage(HttpPostedFileBase file) {
            if (file.ContentType.Contains("image")) {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg", ".Jpeg" };
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }



        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, string name) {

            try {
                //check if admin logged in
                if (Session["Admin"] != null) {
                    if ((bool)Session["Admin"]) {
                        //check for name entry
                        if (name == "" || name == "name" || name == null) {
                            ViewBag.Message = "please add name of model";
                            return View();
                        }
                        //check file is present
                        if (file.ContentLength > 0) {
                            if (IsImage(file)) {
                                //get absolute paths
                                string _FileName = Path.GetFileName(file.FileName);
                                string _path = Path.Combine(Server.MapPath("~/Data/Large"), _FileName);
                                var relativePath = "~/Data/Large/" + file.FileName;
                                var absolutePath = HttpContext.Server.MapPath(relativePath);

                                if (System.IO.File.Exists(absolutePath)) {
                                    ViewBag.Message = "File upload failed: file (of same name) already exists";
                                    return View();
                                }
                                //save as defined name rather than origional file name
                                string _path2 = Path.Combine(Server.MapPath("~/Data/Large"), name + Path.GetExtension(file.FileName));
                                file.SaveAs(_path2);

                                if (await GetDetectedFaces(_path2, file, name)) {
                                    //SUCCESSFULL location of face(s)

                                    String filenameFile = file.FileName;
                                    String pathFile = Server.MapPath("~Data/Large/" + filenameFile);
                                }
                                else {
                                    //UNSUCCSESSFULL
                                    ViewBag.Message = "File upload failed: face not detected";
                                    if (System.IO.File.Exists(_path2)) {
                                        System.IO.File.Delete(_path2);
                                    }
                                    return View();
                                }
                            }
                            else {
                                ViewBag.Message = "not valid file type";
                                return View();
                            }
                        }
                        ViewBag.Message = "File Uploaded Successfully!!";
                    }
                }
                return View();
            }
            catch {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }





        //create close up
        [HttpGet]
        public async Task<bool> GetDetectedFaces(string filePath, HttpPostedFileBase file, string name) {
            FaceRectangle[] faceRects = await UploadAndDetectFaces(filePath);
            //only use FIRST face, this assumes only one face in a picture
            if (faceRects.Length > 0) {
                Bitmap myBitmap = new Bitmap(filePath);
                int resizeFactor = 1;
                //derive rect
                Rectangle rect = new Rectangle(faceRects[0].Left * resizeFactor,
                                                faceRects[0].Top * resizeFactor,
                                                faceRects[0].Width * resizeFactor,
                                                faceRects[0].Height * resizeFactor
                                                );
                //crop using rect
                Bitmap croppedImage = myBitmap.Clone(rect, myBitmap.PixelFormat);
                //save as name
                string _path = Path.Combine(Server.MapPath("~/Data/Face"), name + Path.GetExtension(file.FileName));
                croppedImage.Save(_path, ImageFormat.Jpeg);
            }
            else {
                return (false);
            }
            return (true);
        }

        //!!! NECCASSARY ACCESS CODE FOR MICROSOFT FACE!!!
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("0f69d974cdd44617b9d4b68d5e30c50e");
        //connect to microsoft face and upload image, return detected faces as array
        private async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath) {
            try {
                using (Stream imageFileStream = System.IO.File.OpenRead(imageFilePath)) {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception) {
                return new FaceRectangle[0];
            }
        }


    }
}
