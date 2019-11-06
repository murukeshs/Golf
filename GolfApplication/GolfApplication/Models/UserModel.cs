using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class Login
    {
        public string email { get; set; }
        public string password { get; set; }
        public int userTypeid { get; set; }
    }

    public class UserType
    {
        public int userTypeId { get; set; }
        public string userType { get; set; }
        public string description { get; set; }
    }

    public class createUser
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string profileImage { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public int countryId { get; set; }
        public int stateId { get; set; }        
        public string city { get; set; }
        public string address { get; set; }
        public string pinCode { get; set; }
        [DefaultValue(false)]
        public bool? isEmailNotification { get; set; }
        [DefaultValue(false)]
        public bool? isSMSNotification { get; set; }
        [DefaultValue(false)]
        public bool? isPublicProfile { get; set; }
        public string userTypeId { get; set; }
    }

    public class getUser : createUser
    {
        public string userType { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string userCreatedDate { get; set; }
        public string userUpdatedDate { get; set; }
        public string passwordUpdatedDate { get; set; }
        [DefaultValue(false)]
        public bool? isEmailVerified { get; set; }
    }

    public class userList : getUser
    {
        public string userType { get; set; }
    }

    public class otpVerify
    {
       public int OTPValue { get; set; }
       public string email { get; set; }
       public string type { get; set; }
    }

    public class updatePassword
    {
        public int OTPValue { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    public class GenOTP
    { 
        public string email { get; set; }
        public string type { get; set; }
    }


    class Global
    {
        public static string fileurl;
    }
}
