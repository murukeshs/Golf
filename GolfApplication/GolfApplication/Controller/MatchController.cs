using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GolfApplication.Models;
using GolfApplication.Data;
using System.Net;
using System.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchController : ControllerBase
    {
        #region matchRules
        [HttpPost, Route("matchRules")]
        public IActionResult matchRules(string matchRules)
        {
            try
            {
                int dt = Data.Match.createMatchRules(matchRules);

                if (dt >=1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Saved Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "Failed" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("matchRules", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }

        [HttpGet, Route("getMatchRulesList")]
        public IActionResult getMatchRulesList()
        {
            List<MatchRulesList> ruleList = new List<MatchRulesList>();
            try
            {
                DataTable dt = Data.Match.getMatchRulesList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MatchRulesList Rules = new MatchRulesList();
                        Rules.matchRuleId = (dt.Rows[i]["matchRuleId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchRuleId"]);
                        Rules.ruleName = (dt.Rows[i]["ruleName"] == DBNull.Value ? "" : dt.Rows[i]["ruleName"].ToString());
                        ruleList.Add(Rules);
                    }
                    return StatusCode((int)HttpStatusCode.OK, ruleList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { });  // 412: Precondition Failed when "no data available"
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchRulesList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region updateMatchRules
        [HttpPut, Route("updateMatchRules")]
        public IActionResult updateMatchRules(MatchRules matchRules)
        {
            try
            {
                int dt = Data.Match.updateMatchRules(matchRules);

                if (dt>=1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "MatchRuleId Not Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateMatchRules", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region createMatch
        [HttpPost, Route("createMatch")]
        public IActionResult createMatch(createMatch createMatch)
        {
            List<dynamic> Matches = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.createMatch(createMatch);
                if(dt.Rows[0][0].ToString()=="Success")
                {
                    //dynamic Mth = new System.Dynamic.ExpandoObject();
                    var MatchID = (dt.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["matchId"]);
                    var MatchCode = (dt.Rows[0]["matchCode"] == DBNull.Value ? "" : dt.Rows[0]["matchCode"]);
                    var CompetitionTypeName = (dt.Rows[0]["competitionTypeName"] == DBNull.Value ? "" : dt.Rows[0]["competitionTypeName"]);
                    //Matches.Add(Mth);
                    return StatusCode((int)HttpStatusCode.OK, new { matchId = MatchID, matchCode= MatchCode, competitionTypeName= CompetitionTypeName });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "Failed to create match" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createMatch", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region updateMatch
        [HttpPut, Route("updateMatch")]
        public IActionResult updateMatch(createMatch updateMatch)
        {
            try
            {
                int dt =Convert.ToInt32(Data.Match.updateMatch(updateMatch));

                if (dt >=1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "MatchId Not Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateMatch", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region create MatchPlayer
        [HttpPost, Route("createMatchplayer")]
        public IActionResult createMatchplayer(matchPlayer matchPlayer)
        {

            try
            {
                int dt =Convert.ToInt32(Data.Match.createMatchplayer(matchPlayer));

                if (dt >=1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Saved Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "MatchId is already present" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createMatchplayer", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region GetMatchByID
        [HttpGet, Route("getMatchById/{matchId}")]
        public IActionResult getMatchById(int matchId)
        {
            List<dynamic> matches = new List<dynamic>();
            List<dynamic> TeamsPlayers = new List<dynamic>();
            try
            {
                DataSet ds = Data.Match.getMatchById(matchId);
                DataTable dt1 = ds.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    dynamic MatchList = new System.Dynamic.ExpandoObject();
                    MatchList.matchId = (dt1.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["matchId"]);
                    MatchList.matchCode = (dt1.Rows[0]["matchCode"] == DBNull.Value ? "" : dt1.Rows[0]["matchCode"].ToString());
                    MatchList.matchName = (dt1.Rows[0]["matchName"] == DBNull.Value ? "" : dt1.Rows[0]["matchName"].ToString());
                    MatchList.matchRuleId = (dt1.Rows[0]["matchRuleId"] == DBNull.Value ? "" : dt1.Rows[0]["matchRuleId"].ToString());
                    MatchList.ruleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    MatchList.matchStartDate = (dt1.Rows[0]["matchStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["matchStartDate"].ToString());
                    MatchList.matchEndDate = (dt1.Rows[0]["matchEndDate"] == DBNull.Value ? "" : dt1.Rows[0]["matchEndDate"].ToString());
                    MatchList.matchFee = (dt1.Rows[0]["matchFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["matchFee"]);
                    MatchList.matchLocation = (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    MatchList.createdBy = (dt1.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["createdBy"]);
                    MatchList.createdDate = (dt1.Rows[0]["createdDate"] == DBNull.Value ? "" : dt1.Rows[0]["createdDate"].ToString());
                    MatchList.matchStatus = (dt1.Rows[0]["matchStatus"] == DBNull.Value ? "" : dt1.Rows[0]["matchStatus"].ToString());
                    MatchList.competitionTypeId = (dt1.Rows[0]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["competitionTypeId"]);
                    MatchList.competitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());

                    matches.Add(MatchList);

                    return StatusCode((int)HttpStatusCode.OK, matches);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "MatchId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getMatchesDetailsById
        [HttpGet, Route("getMatchesDetailsById/{matchId}")]
        public IActionResult getMatchesDetailsById(int matchId)
        {
            List<dynamic> TeamsPlayers = new List<dynamic>();
            try
            {
                DataSet ds = Data.Match.getMatchById(matchId);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
                    
                    if (dt2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            dynamic Teams = new System.Dynamic.ExpandoObject();
                            Teams.teamId = (dt2.Rows[i]["teamId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["teamId"]);
                            Teams.teamName = (dt2.Rows[i]["teamName"] == DBNull.Value ? "" : dt2.Rows[i]["teamName"].ToString());
                            Teams.teamIcon = (dt2.Rows[i]["teamIcon"] == DBNull.Value ? "" : dt2.Rows[i]["teamIcon"].ToString());
                            Teams.scoreKeeperName = (dt2.Rows[i]["scoreKeeperName"] == DBNull.Value ? "" : dt2.Rows[i]["scoreKeeperName"].ToString());
                            Teams.matchPlayerList = (dt2.Rows[i]["playerList"] == DBNull.Value ? "" : dt2.Rows[i]["playerList"].ToString());

                            TeamsPlayers.Add(Teams);
                        }
                    }
                    return StatusCode((int)HttpStatusCode.OK, TeamsPlayers);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "MatchId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchesDetailsById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getMatchList
        [HttpGet, Route("getMatchList")]
        public IActionResult getMatchList()
        {
            List<dynamic> matchList = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getMatchList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Matches = new System.Dynamic.ExpandoObject();

                        Matches.matchId = (dt.Rows[i]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchId"]);
                        Matches.matchCode = (dt.Rows[i]["matchCode"] == DBNull.Value ? "" : dt.Rows[i]["matchCode"].ToString());
                        Matches.matchName = (dt.Rows[i]["matchName"] == DBNull.Value ? "" : dt.Rows[i]["matchName"].ToString());
                        Matches.matchStartDate = (dt.Rows[i]["matchStartDate"] == DBNull.Value ? "" : dt.Rows[i]["matchStartDate"].ToString());
                        Matches.matchFee = (dt.Rows[i]["matchFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["matchFee"]);
                        
                        //Matches.matchType = (dt.Rows[i]["matchType"] == DBNull.Value ? "" : dt.Rows[i]["matchType"].ToString());
                        //Matches.matchRuleId = (dt.Rows[i]["matchRuleId"] == DBNull.Value ? "" : dt.Rows[i]["matchRuleId"].ToString());
                        // Matches.ruleName = (dt.Rows[i]["ruleName"] == DBNull.Value ? "" : dt.Rows[i]["ruleName"].ToString());
                        // Matches.matchEndDate = (dt.Rows[i]["matchEndDate"] == DBNull.Value ? "" : dt.Rows[i]["matchEndDate"].ToString());
                        //Matches.matchLocation = (dt.Rows[i]["matchLocation"] == DBNull.Value ? "" : dt.Rows[i]["matchLocation"].ToString());
                        //Matches.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        // Matches.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        //Matches.matchStatus = (dt.Rows[i]["matchStatus"] == DBNull.Value ? "" : dt.Rows[i]["matchStatus"].ToString());
                        //Matches.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        //Matches.competitionName = (dt.Rows[i]["competitionName"] == DBNull.Value ? "" : dt.Rows[i]["competitionName"].ToString());
                        matchList.Add(Matches);
                    }
                    return StatusCode((int)HttpStatusCode.OK, matchList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { }); //412: Precondition Failed when "no data available"
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getMatchesList
        [HttpGet, Route("getMatches")]
        public IActionResult getMatches()
        {
            List<dynamic> matchList = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getMatchList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Matches = new System.Dynamic.ExpandoObject();

                        Matches.matchCode = (dt.Rows[i]["matchCode"] == DBNull.Value ? "" : dt.Rows[i]["matchCode"].ToString());
                        Matches.matchName = (dt.Rows[i]["matchName"] == DBNull.Value ? "" : dt.Rows[i]["matchName"].ToString());
                        Matches.matchStartDate = (dt.Rows[i]["matchStartDate"] == DBNull.Value ? "" : dt.Rows[i]["matchStartDate"].ToString());
                        Matches.matchFee = (dt.Rows[i]["matchFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["matchFee"]);
                        Matches.matchId = (dt.Rows[i]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchId"]);
                        //Matches.matchType = (dt.Rows[i]["matchType"] == DBNull.Value ? "" : dt.Rows[i]["matchType"].ToString());
                        Matches.matchRuleId = (dt.Rows[i]["matchRuleId"] == DBNull.Value ? "" : dt.Rows[i]["matchRuleId"].ToString());
                        Matches.ruleName = (dt.Rows[i]["ruleName"] == DBNull.Value ? "" : dt.Rows[i]["ruleName"].ToString());
                        Matches.matchEndDate = (dt.Rows[i]["matchEndDate"] == DBNull.Value ? "" : dt.Rows[i]["matchEndDate"].ToString());
                        Matches.matchLocation = (dt.Rows[i]["matchLocation"] == DBNull.Value ? "" : dt.Rows[i]["matchLocation"].ToString());
                        Matches.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        Matches.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        Matches.matchStatus = (dt.Rows[i]["matchStatus"] == DBNull.Value ? "" : dt.Rows[i]["matchStatus"].ToString());
                        Matches.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        //Matches.competitionName = (dt.Rows[i]["competitionName"] == DBNull.Value ? "" : dt.Rows[i]["competitionName"].ToString());
                        matchList.Add(Matches);
                    }
                    return StatusCode((int)HttpStatusCode.OK, matchList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { }); //412: Precondition Failed when "no data available"
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region acceptMatchInvitation
        [HttpPut, Route("acceptMatchInvitation")]
        public IActionResult acceptMatchInvitation(acceptMatchInvitation acceptMatchInvitation)
        {
            try
            {
                int dt = Convert.ToInt32(Data.Match.acceptMatchInvitation(acceptMatchInvitation));

                if (dt >= 1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "Failed to update" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("acceptMatchInvitation", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getCompetitionType
        [HttpGet, Route("getCompetitionType")]
        public IActionResult getCompetitionType()
        {
            List<CompetitionType> typeList = new List<CompetitionType>();
            try
            {
                DataTable dt = Data.Match.getCompetitionType();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CompetitionType type = new CompetitionType();

                        type.competitionTypeId = (dt.Rows[i]["CompetitionName"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        type.CompetitionName = (dt.Rows[i]["CompetitionName"] == DBNull.Value ? "" : dt.Rows[i]["CompetitionName"].ToString());

                        typeList.Add(type);
                    }
                    return StatusCode((int)HttpStatusCode.OK, typeList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { }); //412: Precondition Failed when "no data available"
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getCompetitionType", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region sendmatchnotification
        [HttpGet, Route("sendmatchnotification/{matchId}")]
        public IActionResult sendmatchnotification(int matchId)
        {
            try
            {
                DataSet ds = Data.Match.inviteMatch(matchId);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];

                if (dt1.Rows.Count > 0)
                {
                    int matchID = (dt1.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["matchId"]);
                    string matchCode = (dt1.Rows[0]["matchCode"] == DBNull.Value ? "" : dt1.Rows[0]["matchCode"].ToString());
                    string matchName = (dt1.Rows[0]["matchName"] == DBNull.Value ? "" : dt1.Rows[0]["matchName"].ToString());
                    string matchDate = (dt1.Rows[0]["matchStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["matchStartDate"].ToString());
                    string CompetitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;
                    string MatchLocations = (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    string EmailId = string.Empty;

                    //string emailsWithComma;
                    //string[] values = null;
                    //if (dt3.Rows.Count > 0)
                    //{
                    //    emailsWithComma = dt3.Rows[0][0].ToString();
                    //    emailsWithComma = emailsWithComma.TrimStart(',');
                    //    values = emailsWithComma.Split(',');
                    //}
                    string emails = dt3.Rows[0]["emailList"].ToString();
                    if (emails.Contains('@'))
                    {
                        emails = emails.TrimStart(',').TrimEnd(',');
                        string res = Match.sendmatchnotification(emails, matchName, matchCode, matchDate, CompetitionName, NoOfPlayers, MatchLocations);
                        if (res == "Mail sent successfully.")
                        {
                            return StatusCode((int)HttpStatusCode.OK, "Match Invitations Sent Successfully");
                        }
                    }
                     return StatusCode((int)HttpStatusCode.Forbidden, "Email's Not Found");
                    //if (dt2.Rows.Count > 0)
                    //    {
                    //        for (int i = 0; i < dt2.Rows.Count; i++)
                    //        {
                    //            //Comma Seperate email
                    //            if (i < values.Length)
                    //            {
                    //                EmailId = values[i];
                    //                //Sending Email to Individual Match Players's
                    //                Match.sendmatchnotification(EmailId, matchName, matchCode, matchDate, CompetitionName, NoOfPlayers, MatchLocations);
                    //            }
                    //        }
                    //    }
                    //return StatusCode((int)HttpStatusCode.OK, "Match Invitations Sent Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "MatchId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("sendmatchnotification", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region inviteMatch
        [HttpGet, Route("inviteMatch/{matchId}")]
        public IActionResult inviteMatch(int matchId)
        {
            try
            {
                DataSet ds = Data.Match.inviteMatch(matchId);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];

                if (dt1.Rows.Count > 0)
                {
                    int matchID= (dt1.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["matchId"]);
                    string matchCode= (dt1.Rows[0]["matchCode"] == DBNull.Value ? "" : dt1.Rows[0]["matchCode"].ToString());
                    string matchName= (dt1.Rows[0]["matchName"] == DBNull.Value ? "" : dt1.Rows[0]["matchName"].ToString());
                    string matchDate = (dt1.Rows[0]["matchStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["matchStartDate"].ToString());
                    string CompetitionName= (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;  
                    string MatchLocations= (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    string EmailId = string.Empty;
                    string CurrentHostedUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);

                    string emailsWithComma;
                    string[] values=null;
                    if (dt3.Rows.Count > 0)
                    {
                        emailsWithComma = dt3.Rows[0][0].ToString();
                        emailsWithComma = emailsWithComma.TrimStart(',');
                        values = emailsWithComma.Split(',');
                    }
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                int playerID= (dt2.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["userId"]);

                                //Comma Seperate email
                                if (i < values.Length )
                                {
                                    EmailId = values[i];
                                    //Sending Email to Individual Match Players's
                                    Match.inviteMatch(EmailId, matchID, matchName, playerID, matchCode, matchDate,CompetitionName, NoOfPlayers, MatchLocations, CurrentHostedUrl);
                                }
                            }
                        }
                   
                    return StatusCode((int)HttpStatusCode.OK, "Match Invitations Sent Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "MatchId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("inviteMatch", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getMatchJoinList
        [HttpGet, Route("getMatchJoinList")]
        public IActionResult getMatchJoinList(int matchId ,int userId)
        {
            List<dynamic> matchJoinlist = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getMatchJoinList(matchId, userId);
                if (dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic matchjoin = new System.Dynamic.ExpandoObject();
                        matchjoin.userId = (dt.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["userId"]);
                        matchjoin.ParticipantName = (dt.Rows[i]["ParticipantName"] == DBNull.Value ? "" : dt.Rows[i]["ParticipantName"].ToString());
                        matchjoin.ParticipantId = (dt.Rows[i]["ParticipantId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["ParticipantId"]);
                        matchjoin.matchName = (dt.Rows[i]["matchName"] == DBNull.Value ? "" : dt.Rows[i]["matchName"].ToString());
                        matchjoin.matchId = (dt.Rows[i]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchId"]);
                        matchjoin.Type = (dt.Rows[i]["Type"] == DBNull.Value ? "" : dt.Rows[i]["Type"].ToString());
                        matchjoin.matchFee = (dt.Rows[i]["matchFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["matchFee"]);
                        matchjoin.CompetitionName = (dt.Rows[i]["CompetitionName"] == DBNull.Value ? "" : dt.Rows[i]["CompetitionName"].ToString());
                        matchjoin.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        matchJoinlist.Add(matchjoin);
                    }
                    return StatusCode((int)HttpStatusCode.OK, new { matchJoinlist });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "No Data Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getMatchJoinList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion


        #region addParticipants
        [HttpPost, Route("addParticipants")]
        public IActionResult addParticipants(addParticipants addParticipants)
        {

            try
            {
                DataTable dt = Data.Match.addParticipants(addParticipants);

                if (dt.Rows[0][0].ToString()== "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Participants added successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Unable to add participant" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("addParticipants", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion
    }
}