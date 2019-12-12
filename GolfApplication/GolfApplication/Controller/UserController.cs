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
using static Nexmo.Api.SMS;

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
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
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
                //userCreate.profileImage = Global.fileurl;

                if (userCreate.firstName == "" || userCreate.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter First Name" });
                }
                else if (userCreate.password == "" || userCreate.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter Password" });
                }
                else if (userCreate.email == "" || userCreate.email == "string" || userCreate.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter Email" });
                }
                else if (userCreate.userTypeId == "" || userCreate.userTypeId == "string" || userCreate.userTypeId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter userTypeId" });
                }
                else if (userCreate.phoneNumber == "" ||  userCreate.phoneNumber == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter phonenumber" });
                }

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                System.Text.RegularExpressions.Match match = regex.Match(userCreate.email);

                if (match.Success)
                {
                    DataTable dt = Data.User.createUser(userCreate);
                    string Response = dt.Rows[0][0].ToString();
                    if (Response == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, new { userId= dt.Rows[0][1].ToString() });
                    }
                    else
                    {
                        if (Response.Contains("UQ__tblUser__AB6E61648296FE35"))  // Check Duplicate Key For Email
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Email Id  is already exists" });
                        }
                        if(Response.Contains("UQ__tblUser__4849DA0168C6999A"))   // Check Duplicate Key for Phone
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "PhoneNo  is already exists" });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = Response });
                        }
                        
                    }

                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter a valid Email" });
                }
            }
            catch (Exception e)
            {
                //if (e.Message.Contains("UNIQUE KEY constraint") == true)
                //{
                //    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Email Id is already exists" });
                //}
                //else
                //{
                    string SaveErrorLog = Data.Common.SaveErrorLog("createUser", e.Message);
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
                //}
            }
        }

        #endregion

        #region updateUser
        [HttpPut, Route("updateUser")]
        public IActionResult updateUser([FromBody]createUser userUpdate)
        {
            try
            {
                if (userUpdate.email != null && userUpdate.email != "")
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match match = regex.Match(userUpdate.email);
                    if (match.Success ==false)
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter a valid Email" });
                    }
                }
                //if (match.Success)
                //{
                    string row = Data.User.updateUser(userUpdate);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        if (row.Contains("UNIQUE KEY constraint") == true)
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Email Id is already exists" });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = row });
                        }
                    }
                //}
                //else
                //{
                //    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter a valid Email" });
                //}
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateUser", e.Message);
                if (e.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Email Id is already exists" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
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
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Error while Deleting the record" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteUser", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
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

                    var DecryptPassword = "";
                    if (dt.Rows[0]["password"].ToString() != "")
                    {
                        DecryptPassword = Common.DecryptData(dt.Rows[0]["password"] == DBNull.Value ? "" : dt.Rows[0]["password"].ToString());
                    }
                    else
                    {
                        DecryptPassword = "";
                    }

                    user.userId = (int)dt.Rows[0]["userId"];
                    user.firstName = (dt.Rows[0]["firstName"] == DBNull.Value ? "" : dt.Rows[0]["firstName"].ToString());
                    user.lastName = (dt.Rows[0]["lastName"] == DBNull.Value ? "" : dt.Rows[0]["lastName"].ToString());
                    user.gender = (dt.Rows[0]["gender"] == DBNull.Value ? "" : dt.Rows[0]["gender"].ToString());
                    user.dob = (dt.Rows[0]["dob"] == DBNull.Value ? "" : dt.Rows[0]["dob"].ToString());
                    user.email = (dt.Rows[0]["email"] == DBNull.Value ? "" : dt.Rows[0]["email"].ToString());
                    user.password = DecryptPassword;
                    user.phoneNumber = (dt.Rows[0]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[0]["phoneNumber"].ToString());
                    user.countryId = (dt.Rows[0]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["countryId"]);
                    user.countryName = (dt.Rows[0]["countryName"] == DBNull.Value ? "" : dt.Rows[0]["countryName"].ToString());
                    user.stateId = (dt.Rows[0]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["stateId"]);
                    user.stateName = (dt.Rows[0]["stateName"] == DBNull.Value ? "" : dt.Rows[0]["stateName"].ToString());
                    user.city = (dt.Rows[0]["city"] == DBNull.Value ? "" : dt.Rows[0]["city"].ToString());
                    user.address = (dt.Rows[0]["address"] == DBNull.Value ? "" : dt.Rows[0]["address"].ToString());
                    user.pinCode = (dt.Rows[0]["pinCode"] == DBNull.Value ? "" : dt.Rows[0]["pinCode"].ToString());
                    user.profileImage = (dt.Rows[0]["profileImage"] == DBNull.Value ? "" : dt.Rows[0]["profileImage"].ToString());
                    user.userType = (dt.Rows[0]["userType"] == DBNull.Value ? "" : dt.Rows[0]["userType"].ToString());
                    user.userTypeId = (dt.Rows[0]["userTypeId"] == DBNull.Value ? "" : dt.Rows[0]["userTypeId"].ToString());
                    user.isEmailNotification = (dt.Rows[0]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailNotification"]);
                    user.isEmailVerified = (dt.Rows[0]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[0]["isEmailVerified"]);
                    user.isSMSNotification = (dt.Rows[0]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[0]["isSMSNotification"]);
                    user.userCreatedDate = (dt.Rows[0]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userCreatedDate"].ToString());
                    user.isPublicProfile = (dt.Rows[0]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[0]["isPublicProfile"]);
                    user.userUpdatedDate = (dt.Rows[0]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[0]["userUpdatedDate"].ToString());
                    user.isPhoneVerified = (dt.Rows[0]["isPhoneVerified"] == DBNull.Value ? false : (bool)dt.Rows[0]["isPhoneVerified"]);
                    user.nickName = (dt.Rows[0]["nickName"] == DBNull.Value ? "" : dt.Rows[0]["nickName"].ToString());
                    userList.Add(user);

                    return StatusCode((int)HttpStatusCode.OK, user);
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

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
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
                        var DecryptPassword = "";
                        if (dt.Rows[i]["password"].ToString() != "" )
                        {
                            DecryptPassword = Common.DecryptData(dt.Rows[i]["password"] == DBNull.Value ? "" : dt.Rows[i]["password"].ToString());
                        }
                        else
                        {
                            DecryptPassword = "";
                        }

                        user.userId = (int)dt.Rows[i]["userId"];
                        user.firstName = (dt.Rows[i]["userName"] == DBNull.Value ? "" : dt.Rows[i]["userName"].ToString());
                        //user.lastName = (dt.Rows[i]["lastName"] == DBNull.Value ? "" : dt.Rows[i]["lastName"].ToString());
                        user.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        user.dob = (dt.Rows[i]["dob"] == DBNull.Value ? "" : dt.Rows[i]["dob"].ToString());
                        user.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        user.password = DecryptPassword;
                        user.phoneNumber = (dt.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt.Rows[i]["phoneNumber"].ToString());
                        user.countryId = (dt.Rows[i]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["countryId"]);
                        user.stateId = (dt.Rows[i]["stateId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["stateId"]);
                        user.countryName = (dt.Rows[i]["countryName"] == DBNull.Value ? "" : dt.Rows[i]["countryName"].ToString());
                        user.stateName = (dt.Rows[i]["stateName"] == DBNull.Value ? "" : dt.Rows[i]["stateName"].ToString());
                        user.city = (dt.Rows[i]["city"] == DBNull.Value ? "" : dt.Rows[i]["city"].ToString());
                        user.address = (dt.Rows[i]["address"] == DBNull.Value ? "" : dt.Rows[i]["address"].ToString());
                        user.pinCode = (dt.Rows[i]["pinCode"] == DBNull.Value ? "" : dt.Rows[i]["pinCode"].ToString());
                        user.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        user.isEmailNotification = (dt.Rows[i]["isEmailNotification"] == DBNull.Value ? false : (bool)dt.Rows[i]["isEmailNotification"]);
                        user.isEmailVerified = (dt.Rows[i]["isEmailVerified"] == DBNull.Value ? false : (bool)dt.Rows[i]["isEmailVerified"]);
                        user.isSMSNotification = (dt.Rows[i]["isSMSNotification"] == DBNull.Value ? false : (bool)dt.Rows[i]["isSMSNotification"]);
                        user.userCreatedDate = (dt.Rows[i]["userCreatedDate"] == DBNull.Value ? "" : dt.Rows[i]["userCreatedDate"].ToString());
                        user.isPublicProfile = (dt.Rows[i]["isPublicProfile"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPublicProfile"]);
                        user.userUpdatedDate = (dt.Rows[i]["userUpdatedDate"] == DBNull.Value ? "" : dt.Rows[i]["userUpdatedDate"].ToString());
                        user.isPhoneVerified = (dt.Rows[i]["isPhoneVerified"] == DBNull.Value ? false : (bool)dt.Rows[i]["isPhoneVerified"]);
                        user.userTypeId = (dt.Rows[i]["userTypeId"] == DBNull.Value ? "" : dt.Rows[i]["userTypeId"].ToString());
                        user.userType = (dt.Rows[i]["userType"] == DBNull.Value ? "" : dt.Rows[i]["userType"].ToString());
                        user.nickName = (dt.Rows[i]["nickName"] == DBNull.Value ? "" : dt.Rows[i]["nickName"].ToString());
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

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion
            
        #region updatePassword
        [HttpPut, Route("updatePassword")]
        [AllowAnonymous]
        public IActionResult updatePassword([FromBody]updatePassword updatePassword)
        {
            try
            {
                if (updatePassword.password == "" || updatePassword.password == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter Password" });
                }
                else if (updatePassword.emailorPhone == "" || updatePassword.emailorPhone == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter Email" });
                }
                else if (updatePassword.OTPValue == "" || updatePassword.OTPValue == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter OTPValue" });
                }
                else if (updatePassword.sourceType == "" || updatePassword.sourceType == "string")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "SourceType is required" });
                }
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                System.Text.RegularExpressions.Match match = regex.Match(updatePassword.emailorPhone);
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
                        return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = row });
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter a valid Email" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("UpdatePassword", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region GenerateOTP
        [HttpPut, Route("generateOTP")]
        [AllowAnonymous]
        public IActionResult generateOTP([FromBody]generateOTP otp)
        {
           
            if (otp.sourceType == "Email")
            {
                try
                {
                    string OTPValue = Common.GenerateOTP();
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match match = regex.Match(otp.emailorphone);
                    if (otp.type == "" || otp.type == "string")
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter a type" });
                    }
                    else if (match.Success)
                    {
                        string row = Data.User.GenerateOTP(OTPValue, otp);
                        if (row == "Success")
                        {
                            string res = Common.SendOTP(otp.emailorphone, otp.type, OTPValue);
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
                            return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = row });
                        }
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter a valid Email" });
                    }

                }
                catch (Exception e)
                {
                    string SaveErrorLog = Data.Common.SaveErrorLog("generateOTP", e.Message);
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
                }
            }
            else
            {
                try
                {
                    string OTPValue = Common.GenerateOTP();
                    SMSResponse results = new SMSResponse();
                    //otp.emailorPhone = "+14087224019";
                     //string SaveOtpValue = Data.Common.SaveOTP(PhoneNumber, OTPValue, "Phone");
                    string SaveOtpValue = Data.User.GenerateOTP(OTPValue, otp);
                    //var SmsStatus = "";
                    if (SaveOtpValue == "Success")
                    {
                        var SmsStatus = "";
                        results = SmsNotification.SendMessage(otp.emailorphone, "Hi User, your OTP is " + OTPValue + " and it's expiry time is 5 minutes.");
                        string status = results.messages[0].status.ToString();
                        if (status == "0")
                        {
                            SmsStatus = "Message sent successfully.";
                        }
                        else
                        {
                            string err = results.messages[0].error_text.ToString();
                            SmsStatus = err;
                        }
                        return StatusCode((int)HttpStatusCode.OK, new { SmsStatus });       //results.messages, 
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Phone number not available" });
                    }
                }
                catch (Exception e)
                {
                    string SaveErrorLog = Data.Common.SaveErrorLog("generateOTP", e.Message.ToString());
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
                }
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
                string row = "";

                if (otp.OTPValue == "")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter OTP Value" });
                }
                else if (otp.type == "" || otp.type == "string")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter a type" });
                }
                else if (otp.sourceType == "" || otp.sourceType == "string")
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "SourceType is required - 'Email / Phone'" });
                }
                //else if (otp.source == "" || otp.source == "string")
                //{
                //    return StatusCode((int)HttpStatusCode.BadRequest, new { error = new { message = "Please enter a source" } });
                //}

                if(otp.sourceType == "Email")
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match match = regex.Match(otp.emailorPhone);
                    if (match.Success)
                    {
                        row = Data.User.verifyOTP(otp);
                    }

                    else
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter a valid Email" });
                    }
                }

                else if (otp.sourceType == "Phone")
                {
                    row = Data.User.verifyOTP(otp);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Source Type is required - Enter 'Email / Phone'" });
                }


                if (row == "OTP Verified")
                {
                    return StatusCode((int)HttpStatusCode.OK, "OTP Verified Successfully");
                }
                else
                {
                    //return "Invalid user";
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = row });
                }


            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("verifyOTP", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        //#region SmsOTP
        //[HttpPut, Route("GenerateSmsOTP")]
        //[AllowAnonymous]
        //public IActionResult SmsOTP([FromBody]GenerateSmsOTP otp)
        //{
        //    try
        //    {
        //        string OTPValue = Common.GenerateOTP();

        //        SMSResponse results = new SMSResponse();

        //        var SmsStatus = "";

        //        //otp.emailorPhone = "+14087224019";

        //        // string SaveOtpValue = Data.Common.SaveOTP(PhoneNumber, OTPValue, "Phone");
        //        string SaveOtpValue = Data.User.GenerateSmsOTP(OTPValue, otp);

        //        if (SaveOtpValue == "Success")
        //        {
        //            results = SmsNotification.SendMessage(otp.phone, "Hi User, your OTP is " + OTPValue + " and it's expiry time is 5 minutes.");

        //            string status = results.messages[0].status.ToString();

        //            if (status == "0")
        //            {
        //                SmsStatus = "Message sent successfully.";
        //            }
        //            else
        //            {
        //                string err = results.messages[0].error_text.ToString();
        //                SmsStatus = err;
        //            }


        //            return StatusCode((int)HttpStatusCode.OK, new { SmsStatus });       //results.messages, 
        //        }

        //        else
        //        {
        //            return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "Phone number not available" });
        //        }

        //    }

        //    catch (Exception e)
        //    {
        //        string SaveErrorLog = Data.Common.SaveErrorLog("SmsOTP", e.Message.ToString());

        //        return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message.ToString() });
        //    }
        //}
        //#endregion

        #region updateUserCommunicationinfo
        [AllowAnonymous]
        [HttpPut, Route("updateUserCommunicationinfo")]
        public IActionResult updateUserCommunicationinfo([FromBody]updateUser user)
        {
            //updateUser user = new updateUser();
            try
            {
                if (user.userId <= 0 || user.userId == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter userId" });
                }
                else
                {
                    int row = Data.User.updateUserCommunicationinfo(user);

                    if (row > 0)
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Error while Updating the User Communication info" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateUserCommunicationinfo", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region getPlayerList
        [HttpGet, Route("getPlayerList")]
        [AllowAnonymous]
        public IActionResult getPlayerList(string SearchTerm)
        {
            List<dynamic> getPlayerList = new List<dynamic>();
            
            try
            {
                DataTable dt = Data.User.getPlayerList(SearchTerm);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic getPlayers = new System.Dynamic.ExpandoObject();
                        getPlayers.userId = (int)dt.Rows[i]["userId"];
                        getPlayers.playerName = (dt.Rows[i]["playerName"] == DBNull.Value ? "" : dt.Rows[i]["playerName"].ToString());
                        getPlayers.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        getPlayers.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        getPlayers.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        getPlayers.userType = (dt.Rows[i]["userType"] == DBNull.Value ? "" : dt.Rows[i]["userType"].ToString());
                        getPlayers.isScoreKeeper = (dt.Rows[i]["isScoreKeeper"] == DBNull.Value ? "" : dt.Rows[i]["isScoreKeeper"].ToString());
                        getPlayers.nickName = (dt.Rows[i]["nickName"] == DBNull.Value ? "" : dt.Rows[i]["nickName"].ToString());
                        getPlayers.isPublicProfile = (dt.Rows[i]["isPublicProfile"] == DBNull.Value ? "" : dt.Rows[i]["isPublicProfile"].ToString());
                        //string userType = (dt.Rows[i]["userType"] == DBNull.Value ? "" : dt.Rows[i]["userType"].ToString());
                        //string[] strArray = userType.Split(',');
                        //List<string> myList = strArray.ToList();
                        //getPlayers.UserType = myList;
                        getPlayerList.Add(getPlayers);
                    }
                    return StatusCode((int)HttpStatusCode.OK, getPlayerList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, getPlayerList);
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("searchPlayerList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region inviteParticipant
        [HttpPost, Route("inviteParticipant")]
        [AllowAnonymous]
        public IActionResult inviteParticipant(inviteParticipants inviteParticipant)
        {
            SMSResponse results = new SMSResponse();
            string res = string.Empty;
            var SmsStatus = "";
            try
            {
                if (inviteParticipant.firstName == "" || inviteParticipant.firstName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter First Name" });
                }
               
                else if (inviteParticipant.email == "" || inviteParticipant.email == "string" || inviteParticipant.email == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Email" });
                }
                else if (inviteParticipant.phoneNumber == "" || inviteParticipant.phoneNumber == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter phonenumber" });
                }
                else if (inviteParticipant.gender == "" || inviteParticipant.gender == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter gender" });
                }
                else if (inviteParticipant.isEmailNotification == false && inviteParticipant.isSMSNotification == false)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter mode of invitation" });
                }
                
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                System.Text.RegularExpressions.Match match = regex.Match(inviteParticipant.email);

                if (match.Success)
                {
                    DataTable dt = Data.User.inviteParticipant(inviteParticipant);
                    string Response = dt.Rows[0][0].ToString();
                    if (Response == "Success")
                    {
                        if (inviteParticipant.isSMSNotification == true)
                        {
                            
                            results = SmsNotification.SendMessage(inviteParticipant.phoneNumber, "Congratulations"+ inviteParticipant.firstName + ", your are invited for Golf Match");
                            string status = results.messages[0].status.ToString();
                            if (status == "0")
                            {
                                SmsStatus = "Sms sent successfully.";
                            }
                            else
                            {
                                string err = results.messages[0].error_text.ToString();
                                SmsStatus = err;
                            }
                        }
                        else if(inviteParticipant.isEmailNotification == true)
                        {
                            res = EmailSendGrid.Mail("chitrasubburaj30@gmail.com", inviteParticipant.email, "Invitation", "Congratulations" + inviteParticipant.firstName + ", your are invited for Golf Match").Result; //and it's expiry time is 5 minutes.
                            if (res == "Accepted")
                            {
                                SmsStatus = "Mail sent successfully.";
                            }
                            else
                            {
                                SmsStatus = "Bad Request";
                            }
                        }
                        return StatusCode((int)HttpStatusCode.OK, new { userId = dt.Rows[0][1].ToString(),Message= SmsStatus });
                    }
                    else
                    {
                        if (Response.Contains("UQ__tblUser__AB6E61648296FE35"))  // Check Duplicate Key For Email
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Email Id  is already exists" });
                        }
                        if (Response.Contains("UQ__tblUser__4849DA0168C6999A"))   // Check Duplicate Key for Phone
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "PhoneNo  is already exists" });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                        }

                    }

                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter a valid Email" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("inviteParticipant", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }

        #endregion
    }
}