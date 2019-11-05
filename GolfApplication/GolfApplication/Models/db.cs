using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class db
    {
        public static IConfiguration configuration
        {
            get;
            private set;
        }
        public db(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        public static string GetConnectionString()
        {
            return configuration.GetSection("ConnectionString").GetSection("DefaultConnection").Value;
        }

    }
}
