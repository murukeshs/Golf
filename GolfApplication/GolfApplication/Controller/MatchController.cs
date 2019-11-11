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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = "Failed" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.OK, new { });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = "MatchRuleId Not Found" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                if(dt.Rows[0][2].ToString()=="Success")
                {
                    dynamic Mth = new System.Dynamic.ExpandoObject();
                    Mth.matchId = (dt.Rows[0][0] == DBNull.Value ? 0 : (int)dt.Rows[0][0]);
                    Mth.matchCode = (dt.Rows[0][1] == DBNull.Value ? "" : dt.Rows[0][1].ToString());
                    Matches.Add(Mth);

                    return StatusCode((int)HttpStatusCode.OK, Matches);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = "MatchId Not Found" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = "MatchId is already present" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                DataTable dt2 = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
                    dynamic MatchList = new System.Dynamic.ExpandoObject();
                    MatchList.matchId = (dt1.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["matchId"]);
                    MatchList.matchCode = (dt1.Rows[0]["matchCode"] == DBNull.Value ? "" : dt1.Rows[0]["matchCode"].ToString());
                    MatchList.matchName = (dt1.Rows[0]["matchName"] == DBNull.Value ? "" : dt1.Rows[0]["matchName"].ToString());
                    MatchList.matchType = (dt1.Rows[0]["matchType"] == DBNull.Value ? "" : dt1.Rows[0]["matchType"].ToString());
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
                    if (MatchList.matchType == "Teams")
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                dynamic Teams = new System.Dynamic.ExpandoObject();
                                Teams.teamId = (dt2.Rows[i]["teamId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["teamId"]);
                                Teams.teamName = (dt2.Rows[i]["teamName"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["teamName"]);
                                Teams.teamIcon = (dt2.Rows[i]["teamIcon"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["teamIcon"]);
                                Teams.scoreKeeperId = (dt2.Rows[i]["scoreKeeperId"] == DBNull.Value ? 0 : dt2.Rows[i]["scoreKeeperId"]);
                                Teams.userId = (dt2.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["userId"]);
                                Teams.firstName = (dt2.Rows[i]["firstName"] == DBNull.Value ? "" : dt2.Rows[i]["firstName"].ToString());
                                Teams.lastName = (dt2.Rows[i]["lastName"] == DBNull.Value ? "" : dt2.Rows[i]["lastName"].ToString());
                                Teams.email = (dt2.Rows[i]["email"] == DBNull.Value ? "" : dt2.Rows[i]["email"].ToString());
                                Teams.gender = (dt2.Rows[i]["gender"] == DBNull.Value ? "" : dt2.Rows[i]["gender"].ToString());
                                Teams.dob = (dt2.Rows[i]["dob"] == DBNull.Value ? "" : dt2.Rows[i]["dob"].ToString());
                                Teams.profileImage = (dt2.Rows[i]["profileImage"] == DBNull.Value ? "" : dt2.Rows[i]["profileImage"].ToString());
                                Teams.phoneNumber = (dt2.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt2.Rows[i]["phoneNumber"].ToString());
                                Teams.countryId = (dt2.Rows[i]["countryId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["countryId"]);
                                Teams.countryName = (dt2.Rows[i]["countryName"] == DBNull.Value ? "" : dt2.Rows[i]["countryName"].ToString());
                                Teams.stateId = (dt2.Rows[i]["stateId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["stateId"]);
                                Teams.stateName = (dt2.Rows[i]["stateName"] == DBNull.Value ? "" : dt2.Rows[i]["stateName"].ToString());
                                Teams.city = (dt2.Rows[i]["city"] == DBNull.Value ? "" : dt2.Rows[i]["city"].ToString());
                                Teams.address = (dt2.Rows[i]["address"] == DBNull.Value ? "" : dt2.Rows[i]["address"].ToString());
                                Teams.pinCode = (dt2.Rows[i]["pinCode"] == DBNull.Value ? "" : dt2.Rows[i]["pinCode"].ToString());

                                TeamsPlayers.Add(Teams);
                            }
                        }
                        MatchList.Teams = TeamsPlayers;
                    }
                    else
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                dynamic Players = new System.Dynamic.ExpandoObject();
                                Players.userId = (dt2.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["userId"]);
                                Players.firstName = (dt2.Rows[i]["firstName"] == DBNull.Value ? "" : dt2.Rows[i]["firstName"].ToString());
                                Players.lastName = (dt2.Rows[i]["lastName"] == DBNull.Value ? "" : dt2.Rows[i]["lastName"].ToString());
                                Players.email = (dt2.Rows[i]["email"] == DBNull.Value ? "" : dt2.Rows[i]["email"].ToString());
                                Players.gender = (dt2.Rows[i]["gender"] == DBNull.Value ? "" : dt2.Rows[i]["gender"].ToString());
                                Players.dob = (dt2.Rows[i]["dob"] == DBNull.Value ? "" : dt2.Rows[i]["dob"].ToString());
                                Players.profileImage = (dt2.Rows[i]["profileImage"] == DBNull.Value ? "" : dt2.Rows[i]["profileImage"].ToString());
                                Players.phoneNumber = (dt2.Rows[i]["phoneNumber"] == DBNull.Value ? "" : dt2.Rows[i]["phoneNumber"].ToString());
                                Players.countryId = (dt2.Rows[i]["countryId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["countryId"]);
                                Players.countryName = (dt2.Rows[i]["countryName"] == DBNull.Value ? "" : dt2.Rows[i]["countryName"].ToString());
                                Players.stateId = (dt2.Rows[i]["stateId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["stateId"]);
                                Players.stateName = (dt2.Rows[i]["stateName"] == DBNull.Value ? "" : dt2.Rows[i]["stateName"].ToString());
                                Players.city = (dt2.Rows[i]["city"] == DBNull.Value ? "" : dt2.Rows[i]["city"].ToString());
                                Players.address = (dt2.Rows[i]["address"] == DBNull.Value ? "" : dt2.Rows[i]["address"].ToString());
                                Players.pinCode = (dt2.Rows[i]["pinCode"] == DBNull.Value ? "" : dt2.Rows[i]["pinCode"].ToString());

                                TeamsPlayers.Add(Players);
                            }
                        }
                        MatchList.Players = TeamsPlayers;
                    }
                    matches.Add(MatchList);
                   
                    return StatusCode((int)HttpStatusCode.OK, matches);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { error = new { message = "MatchId not found" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetStateList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion

        #region getMatchList
        [HttpGet, Route("getMatchList")]
        public IActionResult getMatchList()
        {
            List<MatchList> matchList = new List<MatchList>();
            try
            {
                DataTable dt = Data.Match.getMatchList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MatchList Matches = new MatchList();
                        Matches.matchId = (dt.Rows[i]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchId"]);
                        Matches.matchCode = (dt.Rows[i]["matchCode"] == DBNull.Value ? "" : dt.Rows[i]["matchCode"].ToString());
                        Matches.matchName = (dt.Rows[i]["matchName"] == DBNull.Value ? "" : dt.Rows[i]["matchName"].ToString());
                        Matches.matchType = (dt.Rows[i]["matchType"] == DBNull.Value ? "" : dt.Rows[i]["matchType"].ToString());
                        Matches.matchRuleId = (dt.Rows[i]["matchRuleId"] == DBNull.Value ? "" : dt.Rows[i]["matchRuleId"].ToString());
                        Matches.ruleName = (dt.Rows[i]["ruleName"] == DBNull.Value ? "" : dt.Rows[i]["ruleName"].ToString());
                        Matches.matchStartDate = (dt.Rows[i]["matchStartDate"] == DBNull.Value ? "" : dt.Rows[i]["matchStartDate"].ToString());
                        Matches.matchEndDate = (dt.Rows[i]["matchEndDate"] == DBNull.Value ? "" : dt.Rows[i]["matchEndDate"].ToString());
                        Matches.matchFee = (dt.Rows[i]["matchFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["matchFee"]);
                        Matches.matchLocation = (dt.Rows[i]["matchLocation"] == DBNull.Value ? "" : dt.Rows[i]["matchLocation"].ToString());
                        Matches.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        Matches.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        Matches.matchStatus = (dt.Rows[i]["matchStatus"] == DBNull.Value ? "" : dt.Rows[i]["matchStatus"].ToString());
                        Matches.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        Matches.competitionName = (dt.Rows[i]["competitionName"] == DBNull.Value ? "" : dt.Rows[i]["competitionName"].ToString());
                        matchList.Add(Matches);
                    }
                    return StatusCode((int)HttpStatusCode.OK, matchList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = "Failed to update" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    return StatusCode((int)HttpStatusCode.OK, typeList);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("userType", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    string Typeof = (dt1.Rows[0]["matchType"] == DBNull.Value ? "" : dt1.Rows[0]["matchType"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;
                    string MatchLocations = (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    string EmailId = string.Empty;

                    if (Typeof == "Teams")
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                //Comma Seperate email
                                if (dt3.Rows.Count > 0)
                                {
                                    string s = dt3.Rows[0][0].ToString();
                                    s = s.TrimStart(',');
                                    string[] values = s.Split(',');
                                    //for (int j = 0; j < values.Length; j++)
                                    //{
                                    //    values[j] = values[j].Trim();
                                        EmailId = values[i];

                                        //Sending Email to Individual Match Players's
                                        Match.sendmatchnotification(EmailId, matchName, matchCode, matchDate, CompetitionName, Typeof, NoOfPlayers, MatchLocations);
                                   // }
                                }
                            }

                        }
                    }
                    else  //Players
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                //Comma Seperate email
                                if (dt3.Rows.Count > 0)
                                {
                                    string s = dt3.Rows[0][0].ToString();
                                    s = s.TrimStart(',');
                                    string[] values = s.Split(',');
                                    //for (int j = 0; j < values.Length; j++)
                                    //{
                                    //    values[j] = values[j].Trim();
                                        EmailId = values[i];

                                        //Sending Email to Individual Match Players's
                                        Match.sendmatchnotification(EmailId, matchName, matchCode, matchDate, CompetitionName, Typeof, NoOfPlayers, MatchLocations);
                                   // }
                                }
                            }
                        }
                    }

                    return StatusCode((int)HttpStatusCode.OK, "Match Invitations Sent Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { error = new { message = "MatchId not found" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetStateList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
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
                    string Typeof= (dt1.Rows[0]["matchType"] == DBNull.Value ? "" : dt1.Rows[0]["matchType"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;  
                    string MatchLocations= (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    string EmailId = string.Empty;

                    if (Typeof == "Teams")
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                int playerID= (dt2.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["userId"]);
                                //Comma Seperate email
                                if (dt3.Rows.Count > 0)
                                {
                                    string s = dt3.Rows[0][0].ToString();
                                    s = s.TrimStart(',');
                                    string[] values = s.Split(',');
                                    //for (int j = 0; j < values.Length; j++)
                                    //{
                                    //    values[j] = values[j].Trim();
                                        EmailId = values[i];

                                        //Sending Email to Individual Match Players's
                                        Match.inviteMatch(EmailId, matchID, matchName, playerID, matchCode, matchDate,CompetitionName, Typeof, NoOfPlayers, MatchLocations);
                                   // }
                                }
                            }
                        }
                    }
                    else  //Players
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                int playerID = (dt2.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt2.Rows[i]["userId"]);
                                

                                //Comma Seperate email
                                if (dt3.Rows.Count > 0)
                                {
                                    string s = dt3.Rows[0][0].ToString();
                                    s = s.TrimStart(',');
                                    string[] values = s.Split(',');
                                    //for (int j = 0; j < values.Length; j++)
                                    //{
                                    //    values[j] = values[j].Trim();
                                        EmailId = values[i];

                                        //Sending Email to Individual Match Players's
                                        Match.inviteMatch(EmailId, matchID, matchName, playerID, matchCode, matchDate, CompetitionName, Typeof, NoOfPlayers, MatchLocations);
                                    //}
                                }  
                            }
                        }
                    }

                    return StatusCode((int)HttpStatusCode.OK, "Match Invitations Sent Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { error = new { message = "MatchId not found" } });
                }
            }
            catch (Exception e)
            {
                //string SaveErrorLog = Data.Common.SaveErrorLog("GetStateList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = new { message = e.Message } });
            }
        }
        #endregion
    }
}