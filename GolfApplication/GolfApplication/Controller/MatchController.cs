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
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchController : ControllerBase
    {
        #region HostingEnvironment
        private readonly IHostingEnvironment _env;

        public MatchController(IHostingEnvironment env)
        {
            _env = env;
        }
        #endregion

        #region matchRules
        [HttpPost, Route("matchRules")]
        public IActionResult matchRules(matchRule matchRule)
        {
            try
            {
                if(matchRule.matchRules == "" || matchRule.matchRules == null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Please Enter Matchrules" });
                }

                int dt = Data.Match.createMatchRules(matchRule.matchRules);

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

        #region createRound
        [HttpPost, Route("createRound")]
        public IActionResult createRound(createRound createRound)
        {
            if (createRound.roundStartDate == "" || createRound.roundEndDate == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter round startdate" });
            }
            List<dynamic> Matches = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.createRound(createRound);
                if (dt.Rows[0][0].ToString() == "Success")
                {
                    var MatchID = (dt.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["roundId"]);
                    var MatchCode = (dt.Rows[0]["roundCode"] == DBNull.Value ? "" : dt.Rows[0]["roundCode"]);
                    var CompetitionTypeName = (dt.Rows[0]["competitionTypeName"] == DBNull.Value ? "" : dt.Rows[0]["competitionTypeName"]);
                    return StatusCode((int)HttpStatusCode.OK, new { matchId = MatchID, matchCode = MatchCode, competitionTypeName = CompetitionTypeName });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Failed to create round" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createRound", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region updateRound
        [HttpPut, Route("updateRound")]
        public IActionResult updateRound(createRound updateRound)
        {
            try
            {
                int dt = Convert.ToInt32(Data.Match.updateRound(updateRound));

                if (dt >= 1)
                {
                    if (updateRound.isSaveAndNotify == true)
                    {
                        IActionResult result = sendmatchnotification(updateRound.roundId);
                    }
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "RoundId Not Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateRound", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
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

        #region GetRoundByID
        [HttpGet, Route("getMatchById/{matchId}")]
        public IActionResult getMatchById(int matchId)
        {
           // List<MatchList> matches = new List<MatchList>();
            //List<dynamic> TeamsPlayers = new List<dynamic>();
            try
            {
                DataSet ds = Data.Match.getMatchById(matchId);
                DataTable dt1 = ds.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    //dynamic MatchList = new System.Dynamic.ExpandoObject();
                    MatchList MatchList = new MatchList();
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
                    //matches.Add(MatchList);
                    return StatusCode((int)HttpStatusCode.OK, MatchList);
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

        [HttpGet, Route("getRoundById/{roundId}")]
        public IActionResult getRoundById(int roundId)
        {
            try
            {
                DataSet ds = Data.Match.getMatchById(roundId);
                DataTable dt1 = ds.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    RoundList MatchList = new RoundList();
                    MatchList.roundId = (dt1.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["roundId"]);
                    MatchList.roundCode = (dt1.Rows[0]["roundCode"] == DBNull.Value ? "" : dt1.Rows[0]["roundCode"].ToString());
                    MatchList.roundName = (dt1.Rows[0]["roundName"] == DBNull.Value ? "" : dt1.Rows[0]["roundName"].ToString());
                    MatchList.roundRuleId = (dt1.Rows[0]["roundRuleId"] == DBNull.Value ? "" : dt1.Rows[0]["roundRuleId"].ToString());
                    MatchList.ruleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    MatchList.roundStartDate = (dt1.Rows[0]["roundStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundStartDate"].ToString());
                    MatchList.roundEndDate = (dt1.Rows[0]["roundEndDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundEndDate"].ToString());
                    MatchList.roundFee = (dt1.Rows[0]["roundFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["roundFee"]);
                    MatchList.roundLocation = (dt1.Rows[0]["roundLocation"] == DBNull.Value ? "" : dt1.Rows[0]["roundLocation"].ToString());
                    MatchList.createdBy = (dt1.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["createdBy"]);
                    MatchList.createdDate = (dt1.Rows[0]["createdDate"] == DBNull.Value ? "" : dt1.Rows[0]["createdDate"].ToString());
                    MatchList.roundStatus = (dt1.Rows[0]["roundStatus"] == DBNull.Value ? "" : dt1.Rows[0]["roundStatus"].ToString());
                    MatchList.competitionTypeId = (dt1.Rows[0]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["competitionTypeId"]);
                    MatchList.competitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    return StatusCode((int)HttpStatusCode.OK, MatchList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "RoundId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getRoundsDetailsById
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
                            Teams.createdByName = (dt2.Rows[i]["createdByName"] == DBNull.Value ? "" : dt2.Rows[i]["createdByName"].ToString());
                            Teams.NoOfPlayers = (dt2.Rows[i]["NoOfPlayers"] == DBNull.Value ? "" : dt2.Rows[i]["NoOfPlayers"].ToString());
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



        [HttpGet, Route("getRoundDetailsById/{roundId}")]
        public IActionResult getRoundDetailsById(int roundId)
        {
            List<dynamic> TeamsPlayers = new List<dynamic>();
            try
            {
                DataSet ds = Data.Match.getRoundById(roundId);
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
                            Teams.createdByName = (dt2.Rows[i]["createdByName"] == DBNull.Value ? "" : dt2.Rows[i]["createdByName"].ToString());
                            Teams.NoOfPlayers = (dt2.Rows[i]["NoOfPlayers"] == DBNull.Value ? "" : dt2.Rows[i]["NoOfPlayers"].ToString());
                            Teams.scoreKeeperName = (dt2.Rows[i]["scoreKeeperName"] == DBNull.Value ? "" : dt2.Rows[i]["scoreKeeperName"].ToString());
                            Teams.matchPlayerList = (dt2.Rows[i]["playerList"] == DBNull.Value ? "" : dt2.Rows[i]["playerList"].ToString());

                            TeamsPlayers.Add(Teams);
                        }
                    }
                    return StatusCode((int)HttpStatusCode.OK, TeamsPlayers);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "RoundId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundDetailsById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getRoundList
        [HttpGet, Route("getRoundList")]
        public IActionResult getRoundList()
        {
            List<dynamic> roundList = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getRoundList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Rounds = new System.Dynamic.ExpandoObject();

                        Rounds.roundId = (dt.Rows[i]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["roundId"]);
                        Rounds.roundCode = (dt.Rows[i]["roundCode"] == DBNull.Value ? "" : dt.Rows[i]["roundCode"].ToString());
                        Rounds.roundName = (dt.Rows[i]["roundName"] == DBNull.Value ? "" : dt.Rows[i]["roundName"].ToString());
                        Rounds.roundStartDate = (dt.Rows[i]["roundStartDate"] == DBNull.Value ? "" : dt.Rows[i]["roundStartDate"].ToString());
                        Rounds.StartDate = (dt.Rows[i]["StartDate"] == DBNull.Value ? "" : dt.Rows[i]["StartDate"].ToString());
                        Rounds.StartTime = (dt.Rows[i]["StartTime"] == DBNull.Value ? "" : dt.Rows[i]["StartTime"].ToString());
                        Rounds.roundFee = (dt.Rows[i]["roundFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["roundFee"]);

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
                        roundList.Add(Rounds);
                    }
                    return StatusCode((int)HttpStatusCode.OK, roundList);
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
        [HttpGet, Route("getRounds")]
        public IActionResult getRounds()
        {
            List<dynamic> roundList = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getRoundList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic Rounds = new System.Dynamic.ExpandoObject();

                        Rounds.roundCode = (dt.Rows[i]["roundCode"] == DBNull.Value ? "" : dt.Rows[i]["roundCode"].ToString());
                        Rounds.roundName = (dt.Rows[i]["roundName"] == DBNull.Value ? "" : dt.Rows[i]["roundName"].ToString());
                        Rounds.roundStartDate = (dt.Rows[i]["roundStartDate"] == DBNull.Value ? "" : dt.Rows[i]["roundStartDate"].ToString());
                        Rounds.roundFee = (dt.Rows[i]["roundFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["roundFee"]);
                        Rounds.roundId = (dt.Rows[i]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["roundId"]);
                        Rounds.roundRuleId = (dt.Rows[i]["roundRuleId"] == DBNull.Value ? "" : dt.Rows[i]["roundRuleId"].ToString());
                        Rounds.ruleName = (dt.Rows[i]["ruleName"] == DBNull.Value ? "" : dt.Rows[i]["ruleName"].ToString());
                        Rounds.roundEndDate = (dt.Rows[i]["roundEndDate"] == DBNull.Value ? "" : dt.Rows[i]["roundEndDate"].ToString());
                        Rounds.roundLocation = (dt.Rows[i]["roundLocation"] == DBNull.Value ? "" : dt.Rows[i]["roundLocation"].ToString());
                        Rounds.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        Rounds.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        Rounds.roundStatus = (dt.Rows[i]["roundStatus"] == DBNull.Value ? "" : dt.Rows[i]["roundStatus"].ToString());
                        Rounds.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        roundList.Add(Rounds);
                    }
                    return StatusCode((int)HttpStatusCode.OK, roundList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { }); //412: Precondition Failed when "no data available"
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getRounds", e.Message);
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = /*"Failed to update"*/"PlayerId Not Found" });
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

                var FilePath = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "EmailTemplates"
               + Path.DirectorySeparatorChar.ToString()
               + "email-template.html";

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
                    
                    string emails = dt3.Rows[0]["emailList"].ToString();
                    if (emails.Contains('@'))
                    {
                        emails = emails.TrimStart(',').TrimEnd(',');
                    }
                    string res = Match.sendmatchnotification(emails, matchName, matchCode, matchDate, CompetitionName, NoOfPlayers, MatchLocations,FilePath);

                    if (res == "Mail sent successfully.")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Invitations Sent Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, "Mail Sending Failed");
                    }
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

                var FilePath = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "EmailTemplates"
               + Path.DirectorySeparatorChar.ToString()
               + "PlayersInviteMatch.html";

                if (dt1.Rows.Count > 0)
                {
                    int matchID = (dt1.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["matchId"]);
                    string matchCode = (dt1.Rows[0]["matchCode"] == DBNull.Value ? "" : dt1.Rows[0]["matchCode"].ToString());
                    string matchName = (dt1.Rows[0]["matchName"] == DBNull.Value ? "" : dt1.Rows[0]["matchName"].ToString());
                    string matchDate = (dt1.Rows[0]["matchStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["matchStartDate"].ToString());
                    string CompetitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;
                    string MatchLocations = (dt1.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt1.Rows[0]["matchLocation"].ToString());
                    string RuleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    decimal MatchFee = (dt1.Rows[0]["matchFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["matchFee"]);
                    string EmailId = string.Empty;

                    string CurrentHostedUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);

                    string emails = dt3.Rows[0]["emailList"].ToString();
                    string result = string.Empty;
                    #region HtmlTemplate Code 
                    System.Text.StringBuilder sbody = new StringBuilder();
                    for (int i = 0;i < dt2.Rows.Count;i++)
                    {
                            sbody.Append("<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>");
                            sbody.Append("<tr>");
                            sbody.Append("<td valign='middle' class='' style='background: #fff;'>");
                            sbody.Append("<table width='88%' style='background: #f5f5f5;padding: 30px;'>");
                            sbody.Append("<tr>");
                            sbody.Append("<td style='width:20%'>");
                            sbody.Append("<img src='https://cdn.dribbble.com/users/2476222/screenshots/6768199/golf_logo_effect1_2x.png' width='50' height='50' style='border-radius: 50%;'>");
                            sbody.Append("</td>");
                            sbody.Append("<td style='width: 40%'>" + dt2.Rows[i]["teamName"] + "</td>");
                            sbody.Append("<td style='width:40%'>No Of Players:" + dt2.Rows[i]["NoOfPlayers"] + "</td>");
                            sbody.Append("</tr>");
                            sbody.Append("</table>");
                            sbody.Append("</td>");
                            sbody.Append("</tr>");
                            sbody.Append("</table>");
                    }
                    #endregion
                    if (emails.Contains('@'))
                    {
                        emails = emails.TrimStart(',').TrimEnd(',');
                        result = Match.inviteMatch(emails, matchID, matchName, /*playerID,*/ matchCode, matchDate, CompetitionName, NoOfPlayers, MatchLocations, CurrentHostedUrl, sbody,RuleName,MatchFee, FilePath);
                    }
                    
                   
                    if (result == "Mail sent successfully.")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Invitations Sent Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, "Mail Sending Failed");
                    }
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
        [HttpGet, Route("getRoundJoinList")]
        public IActionResult getRoundJoinList(int roundId ,int userId)
        {
            List<dynamic> roundJoinlist = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.getRoundJoinList(roundId, userId);
                if (dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic roundjoin = new System.Dynamic.ExpandoObject();
                        roundjoin.userId = (dt.Rows[i]["userId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["userId"]);
                        roundjoin.ParticipantName = (dt.Rows[i]["ParticipantName"] == DBNull.Value ? "" : dt.Rows[i]["ParticipantName"].ToString());
                        roundjoin.ParticipantId = (dt.Rows[i]["ParticipantId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["ParticipantId"]);
                        roundjoin.roundName = (dt.Rows[i]["roundName"] == DBNull.Value ? "" : dt.Rows[i]["roundName"].ToString());
                        roundjoin.roundId = (dt.Rows[i]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["roundId"]);
                        roundjoin.Type = (dt.Rows[i]["Type"] == DBNull.Value ? "" : dt.Rows[i]["Type"].ToString());
                        roundjoin.roundFee = (dt.Rows[i]["roundFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[i]["roundFee"]);
                        roundjoin.CompetitionName = (dt.Rows[i]["CompetitionName"] == DBNull.Value ? "" : dt.Rows[i]["CompetitionName"].ToString());
                        roundjoin.competitionTypeId = (dt.Rows[i]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["competitionTypeId"]);
                        roundjoin.isAllowMatch = (dt.Rows[i]["isAllowMatch"] == DBNull.Value ? "" : dt.Rows[i]["isAllowMatch"]);
                        // matchjoin.isModerator = (dt.Rows[i]["isModerator"] == DBNull.Value ? "" : dt.Rows[i]["isModerator"]);
                        roundJoinlist.Add(roundjoin);
                    }
                    return StatusCode((int)HttpStatusCode.OK, roundJoinlist);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "No rounds available for this user" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundJoinList", e.Message);
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