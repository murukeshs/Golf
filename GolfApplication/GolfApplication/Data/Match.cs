using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using GolfApplication.Data;
using GolfApplication.Models;

namespace GolfApplication.Data
{
    public class Match
    {
        #region Upsert MatchRules
        public static int createMatchRules(string matchRules)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@ruleName", matchRules));

                int rowsAffected = SqlHelper.ExecuteNonQuery(connectionstring, CommandType.StoredProcedure, "spCreateRule", parameters.ToArray());
                return rowsAffected;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static int updateMatchRules([FromBody]MatchRules matchRules)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@matchRuleId", matchRules.matchRuleId));
                parameters.Add(new SqlParameter("@ruleName", matchRules.ruleName));

                int rowsAffected = SqlHelper.ExecuteNonQuery(connectionstring, CommandType.StoredProcedure, "spUpdateRule", parameters.ToArray());
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DataTable getMatchRulesList()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListRule").Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region Upsert Match
        public static DataTable createMatch([FromBody]createMatch createMatch)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@matchName", createMatch.matchName));
                parameters.Add(new SqlParameter("@matchType", createMatch.matchType));
                parameters.Add(new SqlParameter("@matchRuleId", createMatch.matchRuleId));
                parameters.Add(new SqlParameter("@matchStartDate",createMatch.matchStartDate));
                parameters.Add(new SqlParameter("@matchEndDate", createMatch.matchEndDate));
                parameters.Add(new SqlParameter("@matchFee", createMatch.matchFee));
                parameters.Add(new SqlParameter("@matchLocation", createMatch.matchLocation));
                parameters.Add(new SqlParameter("@createdBy", createMatch.createdBy));
                parameters.Add(new SqlParameter("@competitionTypeId", createMatch.competitionTypeId));

                using (DataTable dt = SqlHelper.ExecuteDataset(connectionstring, CommandType.StoredProcedure, "spCreateMatch", parameters.ToArray()).Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string updateMatch([FromBody]createMatch updateMatch)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@matchId", updateMatch.matchId));
                parameters.Add(new SqlParameter("@matchName", updateMatch.matchName));
                parameters.Add(new SqlParameter("@matchRuleId", updateMatch.matchRuleId));
                parameters.Add(new SqlParameter("@matchEndDate", updateMatch.matchEndDate));
                parameters.Add(new SqlParameter("@matchFee", updateMatch.matchFee));
                parameters.Add(new SqlParameter("@matchLocation", updateMatch.matchLocation));
                parameters.Add(new SqlParameter("@matchStatus", updateMatch.matchStatus));
                parameters.Add(new SqlParameter("@competitionTypeId", updateMatch.competitionTypeId));
                
                string rowsAffected = SqlHelper.ExecuteScalar(connectionstring, CommandType.StoredProcedure, "spUpdateMatch", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Create  Matchplayer
        public static string createMatchplayer([FromBody]matchPlayer matchPlayer)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Type", matchPlayer.type));
                parameters.Add(new SqlParameter("@matchId", matchPlayer.matchId));
                parameters.Add(new SqlParameter("@userId", matchPlayer.userId));
                
                string rowsAffected = SqlHelper.ExecuteScalar(connectionstring, CommandType.StoredProcedure, "spCreateMatchPlayer", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region getMatchById
        public static DataSet getMatchById(int matchId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@matchId", matchId));
                //Execute the query
                using (DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectMatchById", parameters.ToArray()))
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region MatchList
        public static DataTable getMatchList()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListMatch").Tables[0])
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        public static DataTable getCompetitionType()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spGetCompetitionType").Tables[0])
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
