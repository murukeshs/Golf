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
using System.IO;
using System.Text;

namespace GolfApplication.Data
{
    public class Match
    {
        #region createMatchRules
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

        public static int updateRoundRules([FromBody]MatchRules matchRules)
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

        public static DataTable getRoundRulesList()
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

        #region createMatch
        public static DataTable createRound([FromBody]createRound createRound)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@roundName", createRound.roundName));
                parameters.Add(new SqlParameter("@roundRuleId", createRound.roundRuleId));
                parameters.Add(new SqlParameter("@roundStartDate", createRound.roundStartDate.ToString()));
                parameters.Add(new SqlParameter("@roundEndDate", createRound.roundEndDate.ToString()));
                parameters.Add(new SqlParameter("@roundFee", createRound.roundFee));
                parameters.Add(new SqlParameter("@createdBy", createRound.createdBy));
                parameters.Add(new SqlParameter("@competitionTypeId", createRound.competitionTypeId));

                using (DataTable dt = SqlHelper.ExecuteDataset(connectionstring, CommandType.StoredProcedure, "spCreateRound", parameters.ToArray()).Tables[0])
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

        #region updateRound
        public static string updateRound([FromBody]createRound updateRound)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@roundId", updateRound.roundId));
                parameters.Add(new SqlParameter("@roundName", updateRound.roundName));
                parameters.Add(new SqlParameter("@roundRuleId", updateRound.roundRuleId));
                parameters.Add(new SqlParameter("@roundStartDate", updateRound.roundStartDate));
                parameters.Add(new SqlParameter("@roundEndDate", updateRound.roundEndDate));
                parameters.Add(new SqlParameter("@roundFee", updateRound.roundFee));
                parameters.Add(new SqlParameter("@roundStatus", updateRound.roundStatus));
                parameters.Add(new SqlParameter("@competitionTypeId", updateRound.competitionTypeId));

                string rowsAffected = SqlHelper.ExecuteScalar(connectionstring, CommandType.StoredProcedure, "spUpdateRound", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        
        #region getMatchById
        public static DataSet getRoundById(int roundId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@roundId", roundId));
                //Execute the query
                using (DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectRoundById", parameters.ToArray()))
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

        #region RoundList
        public static DataTable getRoundList()
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spListRound").Tables[0])
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

        #region acceptRoundInvitation
        public static string acceptRoundInvitation([FromBody]acceptMatchInvitation acceptMatchInvitation)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@matchId", acceptMatchInvitation.matchId));
                parameters.Add(new SqlParameter("@Type", acceptMatchInvitation.Type));
                parameters.Add(new SqlParameter("@playerId", acceptMatchInvitation.playerId));

                string rowsAffected = SqlHelper.ExecuteScalar(connectionstring, CommandType.StoredProcedure, "spAcceptMatch", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region GetCompetitionType
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
        #endregion

        #region inviteMatch && sendmatchnotification
        public static DataSet inviteMatch(int roundId)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@roundId", roundId));
                //Execute the query
                using (DataSet dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectRoundById", parameters.ToArray()))
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

        #region inviteMatch
        public static string inviteMatch(string emailId, int roundID, string Title, /*int UserID,*/ string roundCode, string roundDate, string competitionName, int NoOfPlayers, string roundLocation,string CurrentHostedUrl,StringBuilder TeamPlayerList,string roundRuleName,decimal roundFee,string filepath)
        {
            //string CurrentURL = CurrentHostedUrl;
            string link= CurrentHostedUrl + "/api/acceptRoundInvitation" + "?matchId=" + roundID /* "/Type="+typeOf+ "/playerId="+ UserID*/;
            try
            {
                string res = "";
                var result = "";
                #region Form Content Body
                String Body = string.Empty;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    Body = sr.ReadToEnd();
                }
                Body = Body.Replace("*Title*", Title);
                Body = Body.Replace("*RoundCode*", roundCode);
                Body = Body.Replace("*RoundDate*", roundDate);
                Body = Body.Replace("*Competitiontype*", competitionName);
                Body = Body.Replace("*Noofplayers*", NoOfPlayers.ToString());
                Body = Body.Replace("*roundLocation*", roundLocation);
                Body = Body.Replace("*Link*", link);
                Body = Body.Replace("*TeamsPlayers*", TeamPlayerList.ToString());
                Body = Body.Replace("*roundrules*", roundRuleName.ToString());
                Body = Body.Replace("*roundfee*", roundFee.ToString());



                #endregion
                res = EmailSendGrid.inviteMatchMail("chitrasubburaj30@gmail.com", emailId, "Round Invitation", Body).Result; //and it's expiry time is 5 minutes.
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

        #region sendmatchnotification
        public static string sendroundnotification(string emailId, string Title, string roundCode, string roundDate, string competitionName, int NoOfPlayers, string roundLocation, string filepath,string ruleName,decimal roundFee,StringBuilder TeamPlayers)
        {
            try
            {
                string res = "";
                var result = "";
                #region Form Content Body
                String Body = string.Empty;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    Body = sr.ReadToEnd();
                }
                Body = Body.Replace("*Title*", Title);
                Body = Body.Replace("*RoundCode*", roundCode);
                Body = Body.Replace("*RoundDate*", roundDate);
                Body = Body.Replace("*Competitiontype*", competitionName);
                //Body = Body.Replace("*Noofplayers*", NoOfPlayers.ToString());
                Body = Body.Replace("*roundrules*", ruleName.ToString());
                Body = Body.Replace("*roundfee*", roundFee.ToString());
                Body = Body.Replace("*TeamsPlayers*", TeamPlayers.ToString());
                #endregion
                res = EmailSendGrid.inviteMatchMail("chitrasubburaj30@gmail.com", emailId, "Round Invitation", Body).Result; //and it's expiry time is 5 minutes.
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

        #region getRoundJoinList
        public static DataTable getRoundJoinList(int roundId, int userId)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@roundId", roundId));
                parameters.Add(new SqlParameter("@userId", userId));

                using (DataTable dt = SqlHelper.ExecuteDataset(connectionstring, CommandType.StoredProcedure, "spGetRoundJoinList", parameters.ToArray()).Tables[0])
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

        #region addParticipants
        public static DataTable addParticipants([FromBody]addParticipants addParticipants)
        {
            try
            {
                string connectionstring = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@firstName", addParticipants.firstName));
                parameters.Add(new SqlParameter("@lastName", addParticipants.lastName));
                parameters.Add(new SqlParameter("@gender", addParticipants.gender));
                parameters.Add(new SqlParameter("@phoneNumber", addParticipants.phoneNumber));
                parameters.Add(new SqlParameter("@teamId", addParticipants.teamId));
                parameters.Add(new SqlParameter("@email", addParticipants.email));
                parameters.Add(new SqlParameter("@userTypeId", addParticipants.userTypeId));

                DataTable rowsAffected = SqlHelper.ExecuteDataset(connectionstring, CommandType.StoredProcedure, "spAddParticipants", parameters.ToArray()).Tables[0];
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
