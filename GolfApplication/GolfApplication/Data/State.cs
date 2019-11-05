using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace GolfApplication.Data
{
    public class State
    {
            public static DataTable GetStateList(int CountryId)
            {
                try
                {
                    string ConnectionString = Common.GetConnectionString();
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@CountryId", CountryId));
                    //Execute the query
                    using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListState", parameters.ToArray()).Tables[0])
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

