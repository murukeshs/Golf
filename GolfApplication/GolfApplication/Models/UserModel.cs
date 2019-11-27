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
        //public int userTypeid { get; set; }
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
        public int userWithTypeId { get; set; }
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
        [DefaultValue(false)]
        public bool? isEmailVerified { get; set; }
        [DefaultValue(false)]
        public bool? isPhoneVerified { get; set; }
        public bool? isModerator { get; set; }
    }

    public class userList 
    {
        public int userId { get; set; }
        public int userWithTypeId { get; set; }
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
        public string userType { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string userCreatedDate { get; set; }
        public string userUpdatedDate { get; set; }
        [DefaultValue(false)]
        public bool? isEmailVerified { get; set; }
        [DefaultValue(false)]
        public bool? isPhoneVerified { get; set; }
    }

    public class otpVerify
    {
       public string OTPValue { get; set; }
       public string emailorPhone { get; set; }
       public string type { get; set; }
       public string sourceType { get; set; }
       //public string source { get; set; }

    }

    public class updatePassword
    {
        public string OTPValue { get; set; }
        public string emailorPhone { get; set; }
        public string password { get; set; }
        public string sourceType { get; set; }
        //public string source { get; set; }
    }

    public class generateOTP
    {
        public string emailorphone { get; set; }
        public string type { get; set; }
        public string sourceType { get; set; }
    }

    public class updateUser
    {
        public int userId { get; set; }
        public string address { get; set; }
        public int stateId { get; set; }
        public int countryId { get; set; }
        public string city { get; set; }
        [DefaultValue(false)]
        public bool? isEmailNotification { get; set; }
        [DefaultValue(false)]
        public bool? isSMSNotification { get; set; }
        [DefaultValue(false)]
        public bool? isPublicProfile { get; set; }
    }

    class Global
    {
        public static string fileurl;
    }


    public class  matchRule
    {
       public string matchRules{ get; set; }
    }
}
