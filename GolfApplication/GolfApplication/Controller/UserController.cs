using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GolfApplication.Data;
using GolfApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserController : ControllerBase
    {
       
        #region GetUserType
        [HttpGet, Route("userType")]
        [AllowAnonymous]
        public IActionResult getUserType()
        {
            List<UserType> userList = new List<UserType>();
            try
            {
                DataTable dt = Data.User.GetUserType();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UserType user = new UserType();

                        user.userTypeId = (int)dt.Rows[i]["userTypeId"];
                        user.userType = (dt.Rows[i]["userType"] == DBNull.Value ? "" : dt.Rows[i]["userType"].ToString());
                        user.description = (dt.Rows[i]["description"] == DBNull.Value ? "" : dt.Rows[i]["description"].ToString());

                        userList.Add(user);
                    }
                    return StatusCode((int)HttpStatusCode.OK, userList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, userList);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("userType", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region createUser
        [HttpPost, Route("createUser")]
        [AllowAnonymous]
        public IActionResult createUser(createUser userCreate)
        {
            try
            {                
                userCreate.profileImage = Global.fileurl;

                List<createUser> userList = new List<createUser>();

                if (userCreate.firstName == "" || userCreate.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter First Name" } });
                }
                else if (userCreate.password == "" || userCreate.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Password" } });
                }
                else if (userCreate.email == "" || userCreate.email == "string" || userCreate.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Email" } });
                }
                else if (userCreate.userTypeId == "" || userCreate.userTypeId == "string" || userCreate.userTypeId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter userTypeId" } });
                }

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(userCreate.email);

                if (match.Success)
                {
                    string row = Data.User.createUser(userCreate);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Saved Successfully");
                    }
                    else
                    {
                        if (row.Contains("UNIQUE KEY constraint") == true)
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = "Email Id is already exists" } });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = row.ToString() } });
                        }
                        
                    }

                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = "Email Id is already exists" } });
                }
                else
                {
                    string SaveErrorLog = Data.Common.SaveErrorLog("createUser", e.Message);
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
                }
            }
        }

        #endregion

        #region updateUser
        [HttpPut, Route("updateUser")]
        public IActionResult updateUser([FromBody]createUser userUpdate)
        {
            try
            {
            
                List<createUser> userList = new List<createUser>();
                if (userUpdate.userId <= 0 || userUpdate.userId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter First Name" } });
                }
                else if (userUpdate.firstName == "" || userUpdate.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter First Name" } });
                }
                else if (userUpdate.password == "" || userUpdate.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Password" } });
                }
                else if (userUpdate.email == "" || userUpdate.email == "string" || userUpdate.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Email" } });
                }

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(userUpdate.email);
                if (match.Success)
                {
                    string row = Data.User.updateUser(userUpdate);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        if (row.Contains("UNIQUE KEY constraint") == true)
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = "Email Id is already exists" } });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = row } });
                        }
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateUser", e.Message);
                if (e.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = "Email Id is already exists" } });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
                }
            }
        }
        #endregion

        #region DeleteUser
        [HttpDelete, Route("deleteUser/{userId}")]
        public IActionResult deleteUser(int userId)
        {
            try
            {
                int row = Data.User.deleteUser(userId);

                if (row > 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = "Error while Deleting the record" } });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteUser", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region selectUserById
        [HttpGet, Route("selectUserById/{userId}")]
        public IActionResult selectUserById(int userId)
        {
            List<getUser> userList = new List<getUser>();
            try
            {
                DataTable dt = Data.User.selectUserById(userId);
                getUser user = new getUser();

                if (dt.Rows.Count > 0)
                {

                    var DecryptPassword = Common.DecryptData(dt.Rows[0]["password"] == DBNull.Value ? "" : dt.Rows[0]["password"].ToString());

                    user.userId = (int)dt.Rows[0]["userId"];
                    user.firstName = (dt.Rows[0]["firstName"] == DBNull.Value ? "" : dt.Rows[0]["firstName"].ToString());
                    user.lastName = (dt.Rows[0]["lastName"] == DBNull.Value ? "" : dt.Rows[0]["lastName"].ToString());
                    user.gender = (dt.Rows[0]["gender"] == DBNull.Value ? "" : dt.Rows[0]["gender"].ToString());
                    user.dob = (dt.Rows[0]["dob"] == DBNull.Value ? "" : dt.Rows[0]["dob"].ToString());
                    user.email = (dt.Rows[0]["email"] == DBNull.Value ? "" : dt.Rows[0]["email"].ToString());
                    user.password = DecryptPassword;
                    user.phoneNumber = (dt.Rows[0]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[0]["phoneNumber"].ToString());
                    user.countryId = (dt.Rows[0]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["countryId"]);
                    user.stateId = (dt.Rows[0]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["stateId"]);
                    user.city = (dt.Rows[0]["city"] == DBNull.Value ? "" : dt.Rows[0]["city"].ToString());
                    user.address = (dt.Rows[0]["address"] == DBNull.Value ? "" : dt.Rows[0]["address"].ToString());
                    user.pinCode = (dt.Rows[0]["pinCode"] == DBNull.Value ? "" : dt.Rows[0]["pinCode"].ToString());
                    user.profileImage = (dt.Rows[0]["profileImage"] == DBNull.Value ? "" : dt.Rows[0]["profileImage"].ToString());
                    user.userType = (dt.Rows[0]["userType"] == DBNull.Value ? "" : dt.Rows[0]["userType"].ToString());
                    user.isEmailNotification = (dt.Rows[0]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailNotification"]);
                    user.isEmailVerified = (dt.Rows[0]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailVerified"]);
                    user.isSMSNotification = (dt.Rows[0]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isSMSNotification"]);
                    user.userCreatedDate = (dt.Rows[0]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userCreatedDate"].ToString());
                    user.isPublicProfile = (dt.Rows[0]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[0]["isPublicProfile"]);
                    user.userUpdatedDate = (dt.Rows[0]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userUpdatedDate"].ToString());
                    user.passwordUpdatedDate = (dt.Rows[0]["passwordUpdatedDate"] == DBNull.Value ? "" : dt.Rows[0]["passwordUpdatedDate"].ToString());
                    userList.Add(user);

                    return StatusCode((int)HttpStatusCode.OK, new { user });
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectUserById", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region listUser
        [HttpGet, Route("listUser")]
        public IActionResult listUser(string Search, string userType)
        {
            List<userList> userList = new List<userList>();
            try
            {
                DataTable dt = Data.User.listUser(Search, userType);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        userList user = new userList();
                        var DecryptPassword = Common.DecryptData(dt.Rows[i]["password"] == DBNull.Value ? "" : dt.Rows[i]["password"].ToString());

                        user.userId = (int)dt.Rows[i]["userId"];
                        user.firstName = (dt.Rows[i]["firstName"] == DBNull.Value ? "" : dt.Rows[i]["firstName"].ToString());
                        user.lastName = (dt.Rows[i]["lastName"] == DBNull.Value ? "" : dt.Rows[i]["lastName"].ToString());
                        user.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        user.dob = (dt.Rows[i]["dob"] == DBNull.Value ? "" : dt.Rows[i]["dob"].ToString());
                        user.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        user.password = DecryptPassword;
                        user.phoneNumber = (dt.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[i]["phoneNumber"].ToString());
                       // user.countryId = (dt.Rows[i]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["countryId"]);
                       // user.stateId = (dt.Rows[i]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["stateId"]);
                        user.city = (dt.Rows[i]["city"] == DBNull.Value ? "" : dt.Rows[i]["city"].ToString());
                        user.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        user.pinCode = (dt.Rows[i]["pinCode"] == DBNull.Value ? "" : dt.Rows[i]["pinCode"].ToString());
                        // user.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        user.isEmailNotification = (dt.Rows[i]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[i]["isEmailNotification"]);
                        user.isEmailVerified = (dt.Rows[i]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[i]["isEmailVerified"]);
                        user.isSMSNotification = (dt.Rows[i]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[i]["isSMSNotification"]);
                        user.userCreatedDate = (dt.Rows[i]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[i]["userCreatedDate"].ToString());
                        user.isPublicProfile = (dt.Rows[i]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPublicProfile"]);
                        user.userUpdatedDate = (dt.Rows[i]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[i]["userUpdatedDate"].ToString());
                        user.passwordUpdatedDate = (dt.Rows[i]["passwordUpdatedDate"] == DBNull.Value ? "" : dt.Rows[i]["passwordUpdatedDate"].ToString());
                        user.userTypeId = (dt.Rows[i]["userTypeId"] == DBNull.Value ? "" : dt.Rows[i]["userTypeId"].ToString());
                        user.userType = (dt.Rows[i]["userType"] == DBNull.Value ? "" : dt.Rows[i]["userType"].ToString());
                        userList.Add(user);

                    }

                    return StatusCode((int)HttpStatusCode.OK, userList);
                }
                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listUser", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion
            
        #region updatePassword
        [HttpPut, Route("updatePassword")]
        public IActionResult updatePassword([FromBody]updatePassword updatePassword)
        {
            try
            {
                if (updatePassword.password == "" || updatePassword.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Password" } });
                }
                else if (updatePassword.email == "" || updatePassword.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter Email" } });
                }
                else if (updatePassword.OTPValue <= 0 || updatePassword.OTPValue == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter OTPValue" } });
                }

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(updatePassword.email);
                if (match.Success)
                {
                    string row = Data.User.updatePassword(updatePassword);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        //return "Invalid user";
                        return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = row } });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("UpdatePassword", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region GenerateOTP
        [HttpPut, Route("generateOTP")]
        [AllowAnonymous]
        public IActionResult generateOTP([FromBody]GenOTP otp)
        {
            try
            {
                string res = "";
                //Random generator = new Random();
                //int OTPValue = generator.Next(0, 999999);

                int OTPValue = Common.GenerateOTP();

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(otp.email);
                if(otp.type == "" || otp.type == "string")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a type" } });
                }
                else if (match.Success)
                {
                    string row = Data.User.generateOTP(OTPValue, otp);

                    if (row == "Success")
                    {
                        res = Common.SendOTP(otp.email, otp.type, OTPValue);
                        if (res == "Mail sent successfully.")
                        {
                            return StatusCode((int)HttpStatusCode.OK, "OTP Generated and sent Successfully"); //result = "Mail sent successfully.";
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.OK, "OTP Generated, mail not sent Successfully");
                        }
                        
                    }
                    else
                    {
                        //return "Invalid user";
                        return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = row } });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("generateOTP", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region verifyOTP
        [HttpPut, Route("verifyOTP")]
        [AllowAnonymous]
        public IActionResult verifyOTP([FromBody]otpVerify otp)
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(otp.email);
                if (otp.OTPValue <= 0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter OTP Value" } });
                }
                else if (otp.type == "" || otp.type == "string")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a type" } });
                }
                else if (match.Success)
                {
                    string row = Data.User.verifyOTP(otp);

                    if (row == "OTP Verified")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "OTP Verified Successfully");
                    }
                    else
                    {
                        //return "Invalid user";
                        return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = row } });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("verifyOTP", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion


        //#region login
        //[HttpPost, Route("login")]
        //public IActionResult login(string email, string password)
        //{
        //    List<getUser> userList = new List<getUser>();
        //    try
        //    {
        //        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //        Match match = regex.Match(email);

        //        if (match.Success)
        //        {
        //            DataSet ds = Data.User.login(email, password);
        //            DataTable dt0 = ds.Tables[0];

        //            if (dt0.Rows[0]["ErrorMessage"].ToString() == "Success")
        //            {
        //                DataTable dt = ds.Tables[1];

        //                getUser user = new getUser();
        //                user.userId = (int)dt.Rows[1]["userId"];
        //                user.firstName = (dt.Rows[1]["firstName"] == DBNull.Value ? "" : dt.Rows[1]["firstName"].ToString());
        //                user.lastName = (dt.Rows[1]["lastName"] == DBNull.Value ? "" : dt.Rows[1]["lastName"].ToString());
        //                user.gender = (dt.Rows[1]["gender"] == DBNull.Value ? "" : dt.Rows[1]["gender"].ToString());
        //                user.dob = (dt.Rows[1]["dob"] == DBNull.Value ? "" : dt.Rows[1]["dob"].ToString());
        //                user.email = (dt.Rows[1]["email"] == DBNull.Value ? "" : dt.Rows[1]["email"].ToString());
        //                user.password = (dt.Rows[1]["password"] == DBNull.Value ? "" : dt.Rows[1]["password"].ToString());
        //                user.phoneNumber = (dt.Rows[1]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[1]["phoneNumber"].ToString());
        //                user.countryId = (dt.Rows[1]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[1]["countryId"]);
        //                user.stateId = (dt.Rows[1]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[1]["stateId"]);
        //                user.city = (dt.Rows[1]["city"] == DBNull.Value ? "" : dt.Rows[1]["city"].ToString());
        //                user.address = (dt.Rows[1]["address"] == DBNull.Value ? "" : dt.Rows[1]["address"].ToString());
        //                user.pinCode = (dt.Rows[1]["pinCode"] == DBNull.Value ? "" : dt.Rows[1]["pinCode"].ToString());
        //                // user.profileImage = (dt.Rows[1]["profileImage"] == DBNull.Value ? "" : dt.Rows[1]["profileImage"].ToString());
        //                user.isEmailNotification = (dt.Rows[1]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[1]["isEmailNotification"]);
        //                user.isEmailVerified = (dt.Rows[1]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[1]["isEmailVerified"]);
        //                user.isSMSNotification = (dt.Rows[1]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[1]["isSMSNotification"]);
        //                user.userCreatedDate = (dt.Rows[1]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[1]["userCreatedDate"].ToString());
        //                user.isPublicProfile = (dt.Rows[1]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[1]["isPublicProfile"]);
        //                user.userUpdatedDate = (dt.Rows[1]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[1]["userUpdatedDate"].ToString());
        //                user.userCreatedDate = (dt.Rows[1]["passwordUpdatedDate"] == DBNull.Value ? "" : dt.Rows[1]["passwordUpdatedDate"].ToString());
        //                userList.Add(user);
        //                return StatusCode((int)HttpStatusCode.OK, new { user });
        //            }
        //            else
        //            {
        //                return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = dt0.Rows[0]["ErrorMessage"].ToString() } });
        //            }
        //        }

        //        else
        //        {
        //            return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a valid Email" } });
        //        }
        //    }

        //    catch (Exception e)
        //    {
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message.ToString() } });
        //    }
        //}
        //#endregion

    }
}