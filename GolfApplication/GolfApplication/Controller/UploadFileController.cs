using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GolfApplication.Data;
using GolfApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public string UploadFile(IFormFile profileImages)
        {
            try
            {
                Global.fileurl = Common.CreateMediaItem(profileImages);
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