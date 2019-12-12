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
using static Nexmo.Api.SMS;


namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoundController : ControllerBase
    {
        #region HostingEnvironment
        private readonly IHostingEnvironment _env;

        public RoundController(IHostingEnvironment env)
        {
            _env = env;
        }
        #endregion

        #region matchRules
        [HttpPost, Route("roundRules")]
        public IActionResult roundRules(matchRule roundRule)
        {
            try
            {
                if(roundRule.roundRules == "" || roundRule.roundRules == null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Please Enter Roundrules" });
                }

                int dt = Data.Match.createMatchRules(roundRule);

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
                string SaveErrorLog = Data.Common.SaveErrorLog("roundRules", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getRoundRulesList
        [HttpGet, Route("getRoundRulesList")]
        public IActionResult getRoundRulesList()
        {
            List<MatchRulesList> ruleList = new List<MatchRulesList>();
            try
            {
                DataTable dt = Data.Match.getRoundRulesList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MatchRulesList Rules = new MatchRulesList();
                        Rules.roundRuleId = (dt.Rows[i]["matchRuleId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["matchRuleId"]);
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
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundRulesList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region updateMatchRules
        [HttpPut, Route("updateRoundRules")]
        public IActionResult updateRoundRules(MatchRules roundRules)
        {
            try
            {
                int dt = Data.Match.updateRoundRules(roundRules);

                if (dt>=1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "RoundRuleId Not Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateRoundRules", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region createRound
        [HttpPost, Route("createRound")]
        public IActionResult createRound(createRound createRound)
        {
            if (createRound.roundName == "" || createRound.roundName == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter  Round Name" });
            }
            else if (createRound.roundFee <= 0 )
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Round Fee" });
            }
            else if (createRound.createdBy <= 0 )
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Round createdBy" });
            }
            else if (createRound.roundStartDate == "" || createRound.roundStartDate == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter Round startdate" });
            }
            else if (createRound.competitionTypeId <= 0 )
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter CompetitionTypeID" });
            }
            List<dynamic> Matches = new List<dynamic>();
            try
            {
                DataTable dt = Data.Match.createRound(createRound);
                if (dt.Rows[0][0].ToString() == "Success")
                {
                    var roundId = (dt.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["roundId"]);
                    var roundCode = (dt.Rows[0]["roundCode"] == DBNull.Value ? "" : dt.Rows[0]["roundCode"]);
                    var CompetitionTypeName = (dt.Rows[0]["competitionTypeName"] == DBNull.Value ? "" : dt.Rows[0]["competitionTypeName"]);
                    return StatusCode((int)HttpStatusCode.OK, new { roundId = roundId, roundCode = roundCode, competitionTypeName = CompetitionTypeName });
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
                        IActionResult result = sendroundnotification(updateRound.roundId);
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
       
        #region GetRoundByID
        [HttpGet, Route("getRoundById/{roundId}")]
        public IActionResult getRoundById(int roundId)
        {
            try
            {
                //var myList = new List<KeyValuePair<string, dynamic>>();
                //myList.Add(new KeyValuePair<string, dynamic>("@roundId", roundId));

                //DataSet ds = Data.dbConnections.GetDataSetByID("spSelectRoundById", myList);
                DataSet ds = Data.Match.getRoundById(roundId);
                DataTable dt1 = ds.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    RoundList RoundList = new RoundList();
                    RoundList.roundId = (dt1.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["roundId"]);
                    RoundList.roundCode = (dt1.Rows[0]["roundCode"] == DBNull.Value ? "" : dt1.Rows[0]["roundCode"].ToString());
                    RoundList.roundName = (dt1.Rows[0]["roundName"] == DBNull.Value ? "" : dt1.Rows[0]["roundName"].ToString());
                    RoundList.roundRuleId = (dt1.Rows[0]["roundRuleId"] == DBNull.Value ? "" : dt1.Rows[0]["roundRuleId"].ToString());
                    RoundList.ruleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    RoundList.roundStartDate = (dt1.Rows[0]["roundStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundStartDate"].ToString());
                    RoundList.roundEndDate = (dt1.Rows[0]["roundEndDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundEndDate"].ToString());
                    RoundList.roundFee = (dt1.Rows[0]["roundFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["roundFee"]);
                    RoundList.roundLocation = (dt1.Rows[0]["roundLocation"] == DBNull.Value ? "" : dt1.Rows[0]["roundLocation"].ToString());
                    RoundList.createdBy = (dt1.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["createdBy"]);
                    RoundList.createdDate = (dt1.Rows[0]["createdDate"] == DBNull.Value ? "" : dt1.Rows[0]["createdDate"].ToString());
                    RoundList.roundStatus = (dt1.Rows[0]["roundStatus"] == DBNull.Value ? "" : dt1.Rows[0]["roundStatus"].ToString());
                    RoundList.competitionTypeId = (dt1.Rows[0]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["competitionTypeId"]);
                    RoundList.competitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    return StatusCode((int)HttpStatusCode.OK, RoundList);
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
                            Teams.roundPlayerList = (dt2.Rows[i]["playerList"] == DBNull.Value ? "" : dt2.Rows[i]["playerList"].ToString());

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
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getRounds
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

        #region acceptRoundInvitation
        [HttpPut, Route("acceptRoundInvitation")]
        public IActionResult acceptRoundInvitation(acceptMatchInvitation acceptMatchInvitation)
        {
            try
            {
                int dt = Convert.ToInt32(Data.Match.acceptRoundInvitation(acceptMatchInvitation));

                if (dt >= 1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage ="PlayerId Not Found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("acceptRoundInvitation", e.Message);
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
        
        #region sendroundnotification
        [HttpGet, Route("sendroundnotification/{roundId}")]
        public IActionResult sendroundnotification(int roundId)
        {
            SMSResponse results = new SMSResponse();
            try
            {
                DataSet ds = Data.Match.inviteMatch(roundId);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                DataTable dt4 = ds.Tables[3];

                var FilePath = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "EmailTemplates"
               + Path.DirectorySeparatorChar.ToString()
               + "email-template.html";

                if (dt1.Rows.Count > 0)
                {
                    int roundID = (dt1.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["roundId"]);
                    string roundCode = (dt1.Rows[0]["roundCode"] == DBNull.Value ? "" : dt1.Rows[0]["roundCode"].ToString());
                    string roundName = (dt1.Rows[0]["roundName"] == DBNull.Value ? "" : dt1.Rows[0]["roundName"].ToString());
                    string roundStartDate = (dt1.Rows[0]["roundStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundStartDate"].ToString());
                    string CompetitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    string RuleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    decimal roundFee = (dt1.Rows[0]["roundFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["roundFee"]);
                    int NoOfPlayers = dt2.Rows.Count;
                    string roundLocation = (dt1.Rows[0]["roundLocation"] == DBNull.Value ? "" : dt1.Rows[0]["roundLocation"].ToString());
                    string EmailId = string.Empty;
                    
                    string emails = dt3.Rows[0]["emailList"].ToString();
                    string phone = null;//dt4.Rows[0]["PhoneList"].ToString();

                    #region HtmlTemplate Code 
                    System.Text.StringBuilder sbody = new StringBuilder();
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        sbody.Append("<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'>");
                        sbody.Append("<tr>");
                        sbody.Append("<td valign='middle' class='' style='background: #fff;'>");
                        sbody.Append("<table width='88%' style='background: #f5f5f5;padding: 30px;'>");
                        sbody.Append("<tr>");
                        sbody.Append("<td style='width:20%'>");
                        string icon = dt2.Rows[i]["teamIcon"].ToString();
                        if (icon == null || icon == "")
                        {
                            sbody.Append("<img src='https://cdn.dribbble.com/users/2476222/screenshots/6768199/golf_logo_effect1_2x.png' width='50' height='50' style='border-radius: 50%;'>");
                        }
                        else
                        {
                            sbody.Append("<img src=" + icon + " width='50' height='50' style='border-radius: 50%;'>");
                        }
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
                    string res = string.Empty;
                    string SmsResult = string.Empty;
                    if (emails.Contains('@'))
                    {
                        emails = emails.TrimStart(',').TrimEnd(',');
                        res = Match.sendroundnotification(emails, roundName, roundCode, roundStartDate, CompetitionName, NoOfPlayers, roundLocation, FilePath, RuleName, roundFee, sbody);
                    }
                    if(phone !=null)
                    {
                        var SmsStatus = "";
                        results = SmsNotification.SendMessageNotification(phone, "Hi , your are invited to "+roundName+ " CompetitionName is " + CompetitionName+" and Location is "+roundLocation+"");
                        string status = results.messages[0].status.ToString();
                        if (status == "0")
                        {
                            SmsStatus = "Sms Message sent successfully.";
                        }
                        else
                        {
                            string err = results.messages[0].error_text.ToString();
                            SmsStatus = err;
                        }
                    }
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
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "RoundId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("sendroundnotification", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region inviteRound
        [HttpGet, Route("inviteRound/{roundId}")]
        public IActionResult inviteRound(int roundId)
        {
            SMSResponse results = new SMSResponse();
            try
            {
                DataSet ds = Data.Match.inviteMatch(roundId);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                DataTable dt4 = ds.Tables[3];

                var FilePath = _env.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "EmailTemplates"
               + Path.DirectorySeparatorChar.ToString()
               + "PlayersInviteMatch.html";

                if (dt1.Rows.Count > 0)
                {
                    int roundID = (dt1.Rows[0]["roundId"] == DBNull.Value ? 0 : (int)dt1.Rows[0]["roundId"]);
                    string roundCode = (dt1.Rows[0]["roundCode"] == DBNull.Value ? "" : dt1.Rows[0]["roundCode"].ToString());
                    string roundName = (dt1.Rows[0]["roundName"] == DBNull.Value ? "" : dt1.Rows[0]["roundName"].ToString());
                    string roundStartDate = (dt1.Rows[0]["roundStartDate"] == DBNull.Value ? "" : dt1.Rows[0]["roundStartDate"].ToString());
                    string competitionName = (dt1.Rows[0]["competitionName"] == DBNull.Value ? "" : dt1.Rows[0]["competitionName"].ToString());
                    int NoOfPlayers = dt2.Rows.Count;
                    string roundLocation = (dt1.Rows[0]["roundLocation"] == DBNull.Value ? "" : dt1.Rows[0]["roundLocation"].ToString());
                    string RuleName = (dt1.Rows[0]["ruleName"] == DBNull.Value ? "" : dt1.Rows[0]["ruleName"].ToString());
                    string type = (dt1.Rows[0]["type"] == DBNull.Value ? "" : dt1.Rows[0]["type"].ToString());
                    decimal roundFee = (dt1.Rows[0]["roundFee"] == DBNull.Value ? 0 : (decimal)dt1.Rows[0]["roundFee"]);
                    string EmailId = string.Empty;

                    string CurrentHostedUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);

                    string emails = dt3.Rows[0]["emailList"].ToString();
                    string phone = null; //dt4.Rows[0]["PhoneList"].ToString();

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
                            string icon = dt2.Rows[i]["teamIcon"].ToString();
                            if (icon==null || icon == "")
                            {
                            sbody.Append("<img src='https://cdn.dribbble.com/users/2476222/screenshots/6768199/golf_logo_effect1_2x.png' width='50' height='50' style='border-radius: 50%;'>");
                            }
                            else
                            {
                            sbody.Append("<img src="+icon+" width='50' height='50' style='border-radius: 50%;'>");
                            }
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
                        result = Match.inviteMatch(emails, roundID, roundName, roundCode, roundStartDate, competitionName, NoOfPlayers, roundLocation, CurrentHostedUrl, sbody,RuleName,roundFee, FilePath,type);
                    }
                    if (phone != null)
                    {
                        var SmsStatus = "";
                        results = SmsNotification.SendMessageNotification(phone, "Hi , your are invited to " + roundName + " CompetitionName is " + competitionName + " and Location is " + roundLocation + "");
                        string status = results.messages[0].status.ToString();
                        if (status == "0")
                        {
                            SmsStatus = "Sms Message sent successfully.";
                        }
                        else
                        {
                            string err = results.messages[0].error_text.ToString();
                            SmsStatus = err;
                        }
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
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new {ErrorMessage = "RoundId not found" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("inviteRound", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region getRoundJoinList
        [HttpGet, Route("getRoundJoinList")]
        public IActionResult getRoundJoinList(int roundId ,int userId)
        {
            if (roundId <=0 && userId <= 0)
            {
                return StatusCode((int)HttpStatusCode.OK, "Please Enter RoundId or userId");
            }
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
                        roundjoin.isAllowRound = (dt.Rows[i]["isAllowMatch"] == DBNull.Value ? "" : dt.Rows[i]["isAllowMatch"]);
                        roundjoin.teamName = (dt.Rows[i]["teamName"] == DBNull.Value ? "" : dt.Rows[i]["teamName"]);
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

        #region SaveRoundPlayer
        [HttpPost, Route("SaveRoundPlayer")]
        public IActionResult SaveRoundPlayer(SaveRoundPlayer roundPlayers)
        {
            try
            {
                if (roundPlayers.roundId <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Please Enter RoundID" });
                }
                else if(roundPlayers.userId == "")
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Please Enter UserID" });
                }
                int dt = Data.Match.SaveRoundPlayer(roundPlayers);

                if (dt >= 1)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Saved Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Failed" });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("SaveRoundPlayer", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region GetRoundPlayers
        [HttpGet, Route("GetRoundPlayers")]
        public IActionResult GetRoundPlayers(int roundId)
        {
            List<dynamic> RoundPlayersList = new List<dynamic>();
            try
            {
                DataSet ds = Data.Match.GetRoundPlayers(roundId);
                DataTable dt = ds.Tables[0];
                //DataTable dt1 = ds.Tables[1];
                if (dt.Rows.Count > 0)
                { 
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic RoundPlayers = new System.Dynamic.ExpandoObject();
                        RoundPlayers.userId = (int)dt.Rows[i]["userId"];
                        RoundPlayers.playerName = (dt.Rows[i]["playerName"] == DBNull.Value ? "" : dt.Rows[i]["playerName"].ToString());
                        RoundPlayers.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        RoundPlayers.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        RoundPlayers.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        RoundPlayers.nickName = (dt.Rows[i]["nickName"] == DBNull.Value ? "" : dt.Rows[i]["nickName"].ToString());
                        RoundPlayers.isPublicProfile = (dt.Rows[i]["isPublicProfile"] == DBNull.Value ? "" : dt.Rows[i]["isPublicProfile"].ToString());
                        RoundPlayers.isChecked = (dt.Rows[i]["isChecked"] == DBNull.Value ? false : dt.Rows[i]["isChecked"]);
                        RoundPlayersList.Add(RoundPlayers);
                    }
                    return StatusCode((int)HttpStatusCode.OK, RoundPlayersList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed, new { }); //412: Precondition Failed when "no data available"
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("getRoundList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region DeleteRoundPlayer
        [HttpDelete, Route("DeleteRoundPlayer")]
        public IActionResult DeleteRoundPlayer(int userId ,int roundId)
        {
            try
            {
                if (roundId <= 0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter RoundID" });
                }
                else if(userId <=0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter UserID" });
                }
                else
                {
                    DataTable dt = Data.Match.DeleteRoundPlayer(userId,roundId);

                    if (dt.Rows[0]["Status"].ToString() == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Player is already added in following teams"+"'"+ dt.Rows[0]["teamName"].ToString()+"',"+"So try deleting from teams first"});
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("DeleteRoundPlayer", e.Message.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion
    }
}