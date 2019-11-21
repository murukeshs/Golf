using GolfApplication.Models;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Data
{
    public class User
    {

        public static DataTable GetUserType()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetUserType").Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        //public static string createUser(string firstName, string lastName, string email, string gender, string dob, string profileImage, string phoneNumber, string password, int countryId, int stateId, string city, string address, string pinCode, bool isEmailNotification, bool isSMSNotification, bool isPublicProfile, string userTypeId)//([FromBody]createUser userCreate)
        //{
        //    try
        //    {
        //        string ConnectionString = Common.GetConnectionString();

        //        List<SqlParameter> parameters = new List<SqlParameter>();
        //        parameters.Add(new SqlParameter("@firstName", firstName));
        //        parameters.Add(new SqlParameter("@lastName", lastName));
        //        parameters.Add(new SqlParameter("@email", email));
        //        parameters.Add(new SqlParameter("@gender", gender));
        //        parameters.Add(new SqlParameter("@dob", Convert.ToDateTime(dob)));
        //        parameters.Add(new SqlParameter("@profileImage", profileImage));
        //        parameters.Add(new SqlParameter("@phoneNumber", phoneNumber));
        //        parameters.Add(new SqlParameter("@password", password));
        //        parameters.Add(new SqlParameter("@countryId", countryId));
        //        parameters.Add(new SqlParameter("@stateId", stateId));
        //        parameters.Add(new SqlParameter("@city", city));
        //        parameters.Add(new SqlParameter("@address", address));
        //        parameters.Add(new SqlParameter("@pinCode", pinCode));
        //        parameters.Add(new SqlParameter("@isEmailNotification", isEmailNotification));
        //        parameters.Add(new SqlParameter("@isSMSNotification", isSMSNotification));
        //        parameters.Add(new SqlParameter("@isPublicProfile", isPublicProfile));
        //        parameters.Add(new SqlParameter("@userTypeId", userTypeId));

        //        string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spCreateUser", parameters.ToArray()).ToString();
        //        return rowsAffected;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}


        public static DataTable createUser([FromBody]createUser userCreate)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                var encryptPassword = Common.EncryptData(userCreate.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@firstName", userCreate.firstName));
                parameters.Add(new SqlParameter("@lastName", userCreate.lastName));
                parameters.Add(new SqlParameter("@email", userCreate.email));
                parameters.Add(new SqlParameter("@gender", userCreate.gender));
                parameters.Add(new SqlParameter("@dob", userCreate.dob.ToString()));
                parameters.Add(new SqlParameter("@profileImage", userCreate.profileImage));
                parameters.Add(new SqlParameter("@phoneNumber", userCreate.phoneNumber));
                parameters.Add(new SqlParameter("@password", encryptPassword));
                parameters.Add(new SqlParameter("@countryId", userCreate.countryId));
                parameters.Add(new SqlParameter("@stateId", userCreate.stateId));
                parameters.Add(new SqlParameter("@city", userCreate.city));
                parameters.Add(new SqlParameter("@address", userCreate.address));
                parameters.Add(new SqlParameter("@pinCode", userCreate.pinCode));
                parameters.Add(new SqlParameter("@isEmailNotification", userCreate.isEmailNotification));
                parameters.Add(new SqlParameter("@isSMSNotification", userCreate.isSMSNotification));
                parameters.Add(new SqlParameter("@isPublicProfile", userCreate.isPublicProfile));
                parameters.Add(new SqlParameter("@userTypeId", userCreate.userTypeId));
                
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCreateUser", parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string updateUser([FromBody]createUser userCreate)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //if (userCreate.password != null && userCreate.password != "")
                //{
                //    var encryptPassword = Common.EncryptData(userCreate.password);
                //}
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@userId", userCreate.userId));
                parameters.Add(new SqlParameter("@firstName", userCreate.firstName));
                parameters.Add(new SqlParameter("@lastName", userCreate.lastName));
                parameters.Add(new SqlParameter("@email", userCreate.email));
                parameters.Add(new SqlParameter("@gender", userCreate.gender));
                parameters.Add(new SqlParameter("@dob", userCreate.dob));
                parameters.Add(new SqlParameter("@profileImage", userCreate.profileImage));
                parameters.Add(new SqlParameter("@phoneNumber", userCreate.phoneNumber));
                parameters.Add(new SqlParameter("@password", ""));
                parameters.Add(new SqlParameter("@countryId", userCreate.countryId));
                parameters.Add(new SqlParameter("@stateId", userCreate.stateId));
                parameters.Add(new SqlParameter("@city", userCreate.city));
                parameters.Add(new SqlParameter("@address", userCreate.address));
                parameters.Add(new SqlParameter("@pinCode", userCreate.pinCode));
                parameters.Add(new SqlParameter("@isEmailNotification", userCreate.isEmailNotification));
                parameters.Add(new SqlParameter("@isSMSNotification", userCreate.isSMSNotification));
                parameters.Add(new SqlParameter("@isPublicProfile", userCreate.isPublicProfile));
                parameters.Add(new SqlParameter("@userTypeId", userCreate.userTypeId));

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spUpdateUser", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int deleteUser(int userID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userID", userID));

            try
            {
                string ConnectionString = Common.GetConnectionString();
                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spDeleteUser", parameters.ToArray());
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static DataTable selectUserById(int userID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userID", userID));

            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectUserById", parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static DataTable listUser(string Search, string userType)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search));
                parameters.Add(new SqlParameter("@userType", userType));
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListUser", parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DataSet login([FromBody]Login userlogin)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                var encryptPassword = Common.EncryptData(userlogin.password);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Email", userlogin.email));
                parameters.Add(new SqlParameter("@Password", encryptPassword)); 
                parameters.Add(new SqlParameter("@userTypeid", userlogin.userTypeid));

                DataSet ds = new DataSet();
                using (ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spLogin", parameters.ToArray()))
                {
                    return ds;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string updatePassword([FromBody]updatePassword updatePassword)
        {
            var encryptPassword = Common.EncryptData(updatePassword.password);

            List<SqlParameter> parameters = new List<SqlParameter>();            
            parameters.Add(new SqlParameter("@OTPValue", updatePassword.OTPValue));
            parameters.Add(new SqlParameter("@email", updatePassword.emailorPhone));
            parameters.Add(new SqlParameter("@password", encryptPassword));
            parameters.Add(new SqlParameter("@sourceType", updatePassword.sourceType));
            //parameters.Add(new SqlParameter("@source", updatePassword.source));

            try
            {
                string ConnectionString = Common.GetConnectionString();
                
                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spUpdatePassword", parameters.ToArray()).ToString();
                return rowsAffected;               
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static string GenerateSmsOTP(string OTPValue, [FromBody]GenerateSmsOTP otp)
        {
           
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", OTPValue));
            parameters.Add(new SqlParameter("@source", otp.phone));
            parameters.Add(new SqlParameter("@type", otp.type));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spGenerateOTP", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
        public static string generateEmailOTP(string OTPValue, [FromBody]generateEmailOTP otp)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", OTPValue));
            parameters.Add(new SqlParameter("@source", otp.email));
            parameters.Add(new SqlParameter("@type", otp.type));
            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spGenerateOTP", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
        public static string verifyOTP([FromBody]otpVerify otp)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OTPValue", otp.OTPValue));
            parameters.Add(new SqlParameter("@source", otp.emailorPhone));
            parameters.Add(new SqlParameter("@type", otp.type));
            parameters.Add(new SqlParameter("@sourceType", otp.sourceType));
            //parameters.Add(new SqlParameter("@source", otp.source));

            try
            {
                string ConnectionString = Common.GetConnectionString();

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spVerifyOTP", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static int updateUserCommunicationinfo([FromBody]updateUser user)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userId", user.userId));
            parameters.Add(new SqlParameter("@stateId", user.stateId));
            parameters.Add(new SqlParameter("@countryId", user.countryId));
            parameters.Add(new SqlParameter("@city", user.city));
            parameters.Add(new SqlParameter("@address", user.address));
            parameters.Add(new SqlParameter("@isEmailNotification", user.isEmailNotification));
            parameters.Add(new SqlParameter("@isSMSNotification", user.isSMSNotification));
            parameters.Add(new SqlParameter("@isPublicProfile", user.isPublicProfile));

            try
            {
                string ConnectionString = Common.GetConnectionString();
                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spUpdateUserCommunicationinfo", parameters.ToArray());
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
        
        public static DataTable getPlayerList(string Search)
        {
            try
            {
                if (Search=="" || Search == null)
                {
                    Search = "";
                }
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search));

                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetPlayerList", parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
