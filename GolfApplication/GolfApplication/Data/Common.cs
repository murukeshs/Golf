using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GolfApplication.Models;

namespace GolfApplication.Data
{
    public class Common
    {

        #region DBCon
        public static IConfiguration configuration
        {
            get;
            private set;
        }
        public Common(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public static string GetConnectionString()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var connstring = configuration.GetSection("ConnectionString").GetSection("DefaultConnection").Value;

            return connstring;
        }
        #endregion

        #region Get Current URL
        public static string MyMethod(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";
            return host;
        }
        #endregion

        #region ErrorLog
        public static string SaveErrorLog(string FunctionName, string ErrorMessage)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@FunctionName", FunctionName));
                parameters.Add(new SqlParameter("@ErrorMessage", ErrorMessage));

                string rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSaveErrorLog", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
        #endregion

        #region uploadFile       
        public static string CreateMediaItem(byte[] imageFile, string fileName)
        {
            try
            {

                string Exceptsymbols = Regex.Replace(fileName, @"[^.0-9a-zA-Z]+", "");
                string[] strFilename = Exceptsymbols.Split('.');

                string Filename = strFilename[0] + "_" + DateTime.Now.ToString("dd'-'MM'-'yyyy'-'HH'-'mm'-'ss") + "." + strFilename[1];

               var FileURL = AzureStorage.UploadImage(imageFile, Filename, "videosandimages").Result;  //+ "." + file.ContentType
                return FileURL;
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("CreateMediaItem", e.Message.ToString());
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }

        }
        #endregion

        #region Encryption_Decryption
        public static string EncryptData(string textToEncrypt)
        {
            try
            {
                string ToReturn = "";
                //string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                //string _iv = "abc@98797hjkas$&asd(*$%";
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
                IConfigurationRoot configuration = builder.Build();

                string _key = configuration.GetSection("EncryptDecrypt").GetSection("Key").Value;
                string _iv = configuration.GetSection("EncryptDecrypt").GetSection("iv").Value;

                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public static string DecryptData(string textToDecrypt)
        {
            try
            {
                string ToReturn = "";
                //string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                //string _iv = "abc@98797hjkas$&asd(*$%";
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
                IConfigurationRoot configuration = builder.Build();

                string _key = configuration.GetSection("EncryptDecrypt").GetSection("Key").Value;
                string _iv = configuration.GetSection("EncryptDecrypt").GetSection("iv").Value;

                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
        #endregion
        
        #region GenerateOTP
        public static string GenerateOTP()
        {
            try
            {
                Random generator = new Random();
                string OTPValue = generator.Next(0, 99999).ToString();
                return OTPValue;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region SendOTPViaEmail
        public static string SendOTP(string emailid, string Type, string OTPValue)
        {
            try
            {
                string res = "";
                var result = "";
                //#region Form Content Body
                //String Body = string.Empty;

                //string filename = @"UserInvitation.html";
                //string filePath = Directory.GetCurrentDirectory(); 
                //using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath+"//EmailTemplates//"+ filename))
                //{
                //    Body = sr.ReadToEnd();
                //}
                //Body = Body.Replace("*keycode*", OTPValue.ToString());
                //Body = Body.Replace("*Type*",Type );
                //Body = Body.Replace("*Product Name*", "Golf"); 
                //Body = Body.Replace("*invite_sender_name*", "Golf Team");
                //#endregion
                res = EmailSendGrid.Mail("chitrasubburaj30@gmail.com", emailid, "OTP Verification", "Hello, your OTP is " + OTPValue + " and for verify type is '" + Type + "' ").Result; //and it's expiry time is 5 minutes.
                if (res == "Accepted")
                {
                    result = "Mail sent successfully.";
                }
                else
                {
                    result = "Bad Request";
                }

                    return result;
                
            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("SendOTP", e.Message.ToString());

                throw e;
            }
        }
        #endregion


        //#region inviteMatch
        //public static string inviteMatch(string emailId,int matchID,string Title,int UserID, string matchCode, string matchDate, string competitionName, string typeOf, int NoOfPlayers, string MatchLocation)
        //{
           
        //    //var request = Microsoft.AspNetCore.Http.HttpContext;
        //    //string CurrentURL = context
        //    //string link= CurrentURL+ "?matchId="+matchID+ "/Type="+typeOf+ "/playerId="+ UserID;
        //    try
        //    {
        //        string res = "";
        //        var result = "";
        //        #region Form Content Body
        //        String Body = string.Empty;

        //        string filename = @"PlayersInviteMatch.html";
        //        string filePath = Directory.GetCurrentDirectory();
        //        using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath + "//EmailTemplates//" + filename))
        //        {
        //            Body = sr.ReadToEnd();
        //        }
        //        Body = Body.Replace("*Title*", competitionName);
        //        Body = Body.Replace("*MatchCode*", matchCode);
        //        Body = Body.Replace("*MatchDate*", matchDate);
        //        Body = Body.Replace("*Competitiontype*", competitionName);
        //        Body = Body.Replace("*Typeof*", typeOf);
        //        Body = Body.Replace("*Noofplayers*", NoOfPlayers.ToString());
        //        Body = Body.Replace("*matchLocation*", MatchLocation);
        //        Body = Body.Replace("*Link*", "Link");
        //        #endregion
        //        res = EmailSendGrid.inviteMatchMail("chitrasubburaj30@gmail.com", emailId, "Match Invitation", Body).Result; //and it's expiry time is 5 minutes.
        //        if (res == "Accepted")
        //        {
        //            result = "Mail sent successfully.";
        //        }
        //        else
        //        {
        //            result = "Bad Request";
        //        }

        //        return result;

        //    }

        //    catch (Exception e)
        //    {
        //        string SaveErrorLog = Data.Common.SaveErrorLog("SendOTP", e.Message.ToString());

        //        throw e;
        //    }
        //}
        //#endregion


        //#region inviteMatch
        //public static string sendmatchnotification(string emailId, string Title, string matchCode, string matchDate, string competitionName, string typeOf, int NoOfPlayers, string MatchLocation)
        //{
        //    try
        //    {
        //        string res = "";
        //        var result = "";
        //        #region Form Content Body
        //        String Body = string.Empty;

        //        string filename = @"email-template.html";
        //        string filePath = Directory.GetCurrentDirectory();
        //        using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath + "//EmailTemplates//" + filename))
        //        {
        //            Body = sr.ReadToEnd();
        //        }
        //        Body = Body.Replace("*Title*", competitionName);
        //        Body = Body.Replace("*MatchCode*", matchCode);
        //        Body = Body.Replace("*MatchDate*", matchDate);
        //        Body = Body.Replace("*Competitiontype*", competitionName);
        //        Body = Body.Replace("*Typeof*", typeOf);
        //        Body = Body.Replace("*Noofplayers*", NoOfPlayers.ToString());
        //        #endregion
        //        res = EmailSendGrid.inviteMatchMail("chitrasubburaj30@gmail.com", emailId, "Match Invitation", Body).Result; //and it's expiry time is 5 minutes.
        //        if (res == "Accepted")
        //        {
        //            result = "Mail sent successfully.";
        //        }
        //        else
        //        {
        //            result = "Bad Request";
        //        }

        //        return result;

        //    }

        //    catch (Exception e)
        //    {
        //        string SaveErrorLog = Data.Common.SaveErrorLog("SendOTP", e.Message.ToString());

        //        throw e;
        //    }
        //}
        //#endregion
    }
}
