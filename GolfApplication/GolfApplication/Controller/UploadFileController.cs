using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GolfApplication.Data;
using GolfApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GolfApplication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UploadFileController : ControllerBase
    {

        #region UploadFile
        [HttpPost, Route("UploadFile")]
        [AllowAnonymous]
        public async Task<string> UploadFile()
        {
            //byte[] imgdata = System.IO.File.ReadAllBytes(@"C:\\Users\\admin\\Desktop\\Images\\download.jpg");
            try
            {
                //var jsonModel = Request.Form.First(f => f.Key == "myJsonObject").Value;
                //var myJsonObject = JsonConvert.DeserializeObject<UploadModel>(jsonModel);

                IFormFile myFile = Request.Form.Files.First();
                string myFileName = null;
                byte[] myFileContent = null;
                if (myFile != null)
                {
                    myFileName = myFile.FileName;
                    using (var memoryStream = new MemoryStream())
                    {
                        await myFile.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        myFileContent = new byte[memoryStream.Length];
                        await memoryStream.ReadAsync(myFileContent, 0, myFileContent.Length);
                    }
                }
                Global.fileurl = Common.CreateMediaItem(myFileContent, myFileName);
                return Global.fileurl;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion
        
    }
}