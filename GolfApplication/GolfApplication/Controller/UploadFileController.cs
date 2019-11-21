using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        #region UploadFileBase64
        [HttpPost, Route("UploadFileBase64")]
        [AllowAnonymous]
        public async Task<string> UploadFileBase64([FromBody] UploadModel uploadModel )
        {
            try
            {
                var imageDataByteArray = Convert.FromBase64String(uploadModel.file);

                string myFileName = uploadModel.fileName;
                byte[] myFileContent = imageDataByteArray;
                
                Global.fileurl = Common.CreateMediaItem(myFileContent, myFileName);
                return Global.fileurl;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region UploadFileBytes
        [HttpPost, Route("UploadFileBytes"), DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<string> UploadFileBytes()
        {
            //byte[] imgdata = System.IO.File.ReadAllBytes(@"C:\\Users\\admin\\Desktop\\Images\\Sunil.jpg");
            try
            {
                IFormFile myFile = Request.Form.Files.First();
                string myFileName = myFile.FileName;
                byte[] myFileContent;

                using (var memoryStream = new MemoryStream())
                {
                    await myFile.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    myFileContent = new byte[memoryStream.Length];
                    await memoryStream.ReadAsync(myFileContent, 0, myFileContent.Length);
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