using GolfApplication.Models;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Data
{
    public class Team
    {
        public static DataSet createTeam(TeamModel team)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@teamName", team.teamName));
            parameters.Add(new SqlParameter("@teamIcon", team.teamIcon));
            parameters.Add(new SqlParameter("@createdBy", team.createdBy));
            parameters.Add(new SqlParameter("@startingHole", team.startingHole));
            parameters.Add(new SqlParameter("@roundId", team.roundId));

            try
            {
                string ConnectionString = Common.GetConnectionString();

                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCreateTeam", parameters.ToArray()))
                {                 
                    return ds;                   
                }                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int UpdateTeam(updateTeam updateteam)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@teamId", updateteam.teamId));
                parameters.Add(new SqlParameter("@teamName", updateteam.teamName));
                parameters.Add(new SqlParameter("@teamIcon", updateteam.teamIcon));
                //parameters.Add(new SqlParameter("@scoreKeeperID", updateteam.scoreKeeperID));                
                parameters.Add(new SqlParameter("@startingHole", updateteam.startingHole));
                parameters.Add(new SqlParameter("@roundId", updateteam.roundId));

                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spUpdateTeam", parameters.ToArray());
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static string deleteTeam(int teamID)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@teamId", teamID));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spDeleteTeam", parameters.ToArray()).Tables[0])
                {
                    string rowsAffected = dt.Rows[0]["Status"].ToString();
                    return rowsAffected;
                }
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static DataSet selectTeamById(int teamId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@teamId", teamId));
            
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectTeamById", parameters.ToArray()))
                {
                    return ds;
                }
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }

        public static DataTable listTeam()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListTeam").Tables[0])
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

        public static DataTable createTeamPlayers(TeamPlayer teamPlayer)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@teamId", teamPlayer.teamId));
                parameters.Add(new SqlParameter("@scoreKeeperID", teamPlayer.scoreKeeperID));
                parameters.Add(new SqlParameter("@playerId", teamPlayer.playerId));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCreateTeamPlayers", parameters.ToArray()).Tables[0])
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

        public static string deleteTeamPlayers(int teamPlayerListId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@teamPlayerListId", teamPlayerListId));
                //parameters.Add(new SqlParameter("@updateBy", updateBy));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spDeleteTeamPlayers", parameters.ToArray()).Tables[0])
                {
                    string rowsAffected = dt.Rows[0]["Status"].ToString();
                    return rowsAffected;
                }
                //int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spDeleteTeamPlayers", parameters.ToArray());
                //return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
    }
}
