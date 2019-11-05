using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GolfApplication.Data;
using GolfApplication.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class JWTAuthenticationController : ControllerBase
    {

        private IConfiguration _config;

        public JWTAuthenticationController(IConfiguration config)
        {
            _config = config;
        }


        private string GenerateJSONWebToken()  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var JwtKey = configuration.GetSection("Jwt").GetSection("Key").Value;

            var JwtIssuer = configuration.GetSection("Jwt").GetSection("Issuer").Value;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(JwtIssuer,
            JwtIssuer,
            null,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        #region GetUserLogin     
        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]Login login)
        {
            //string GetConnectionString = UsersController.GetConnectionString();
            IActionResult response = Unauthorized();
            //var user = AuthenticateUser(login);

            List<getUser> userList = new List<getUser>();
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(login.email);

                if (match.Success)
                {
                    if(login.userTypeid > 0)
                    {
                        DataSet ds = Data.User.login(login);
                        DataTable dt0 = ds.Tables[0];

                        if (dt0.Rows[0]["ErrorMessage"].ToString() == "Success")
                        {
                            DataTable dt = ds.Tables[1];

                            getUser user = new getUser();
                            user.userId = (int)dt.Rows[0]["userId"];
                            user.firstName = (dt.Rows[0]["firstName"] == DBNull.Value ? "" : dt.Rows[0]["firstName"].ToString());
                            user.lastName = (dt.Rows[0]["lastName"] == DBNull.Value ? "" : dt.Rows[0]["lastName"].ToString());
                            user.gender = (dt.Rows[0]["gender"] == DBNull.Value ? "" : dt.Rows[0]["gender"].ToString());
                            user.userType = (dt.Rows[0]["userType"] == DBNull.Value ? "" : dt.Rows[0]["userType"].ToString());
                            user.email = (dt.Rows[0]["email"] == DBNull.Value ? "" : dt.Rows[0]["email"].ToString());
                            //user.password = (dt.Rows[0]["password"] == DBNull.Value ? "" : dt.Rows[0]["password"].ToString());
                            user.phoneNumber = (dt.Rows[0]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[0]["phoneNumber"].ToString());
                            user.countryId = (dt.Rows[0]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["countryId"]);
                            user.stateId = (dt.Rows[0]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["stateId"]);
                            user.city = (dt.Rows[0]["city"] == DBNull.Value ? "" : dt.Rows[0]["city"].ToString());
                            user.address = (dt.Rows[0]["address"] == DBNull.Value ? "" : dt.Rows[0]["address"].ToString());
                            user.pinCode = (dt.Rows[0]["pinCode"] == DBNull.Value ? "" : dt.Rows[0]["pinCode"].ToString());
                            user.profileImage = (dt.Rows[0]["profileImage"] == DBNull.Value ? "" : dt.Rows[0]["profileImage"].ToString());
                            user.isEmailNotification = (dt.Rows[0]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailNotification"]);
                            user.isEmailVerified = (dt.Rows[0]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailVerified"]);
                            user.isSMSNotification = (dt.Rows[0]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isSMSNotification"]);
                            user.userCreatedDate = (dt.Rows[0]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userCreatedDate"].ToString());
                            user.isPublicProfile = (dt.Rows[0]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[0]["isPublicProfile"]);
                            user.userUpdatedDate = (dt.Rows[0]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userUpdatedDate"].ToString());
                            user.passwordUpdatedDate = (dt.Rows[0]["passwordUpdatedDate"] == DBNull.Value ? "" : dt.Rows[0]["passwordUpdatedDate"].ToString());
                            userList.Add(user);

                            var token = GenerateJSONWebToken();
                            //var encrypt = Common.EncryptData(token);
                            return StatusCode((int)HttpStatusCode.OK, new { user, token });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = dt0.Rows[0]["ErrorMessage"].ToString() } });
                        }
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a userTypeId" } });
                    }
                }

                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }

            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("Login", e.Message);

                //return StatusCode((int)HttpStatusCode.InternalServerError, new { Data = e.Message.ToString() });
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion


    }

}