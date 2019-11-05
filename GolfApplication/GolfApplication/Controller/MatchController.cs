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

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
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
                    return StatusCode((int)HttpStatusCode.Forbidden, new { error = new { message = dt } });
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
            List<MatchList> matches = new List<MatchList>();
            try
            {
                DataTable dt = Data.Match.getMatchById(matchId);

                if (dt.Rows.Count > 0)
                {
                    MatchList match = new MatchList();
                    match.matchId = (dt.Rows[0]["matchId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["matchId"]);
                    match.matchCode = (dt.Rows[0]["matchCode"] == DBNull.Value ? "" : dt.Rows[0]["matchCode"].ToString());
                    match.matchName = (dt.Rows[0]["matchName"] == DBNull.Value ? "" : dt.Rows[0]["matchName"].ToString());
                    match.matchType = (dt.Rows[0]["matchType"] == DBNull.Value ? "" : dt.Rows[0]["matchType"].ToString());
                    match.matchRuleId = (dt.Rows[0]["matchRuleId"] == DBNull.Value ? "" : dt.Rows[0]["matchRuleId"].ToString());
                    match.ruleName = (dt.Rows[0]["ruleName"] == DBNull.Value ? "" : dt.Rows[0]["ruleName"].ToString());
                    match.matchStartDate = (dt.Rows[0]["matchStartDate"] == DBNull.Value ? "" : dt.Rows[0]["matchStartDate"].ToString());
                    match.matchEndDate = (dt.Rows[0]["matchEndDate"] == DBNull.Value ? "" : dt.Rows[0]["matchEndDate"].ToString());
                    match.matchFee = (dt.Rows[0]["matchFee"] == DBNull.Value ? 0 : (decimal)dt.Rows[0]["matchFee"]);
                    match.matchLocation = (dt.Rows[0]["matchLocation"] == DBNull.Value ? "" : dt.Rows[0]["matchLocation"].ToString());
                    match.createdBy = (dt.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[0]["createdBy"]);
                    match.createdDate = (dt.Rows[0]["createdDate"] == DBNull.Value ? "" : dt.Rows[0]["createdDate"].ToString());
                    match.matchStatus = (dt.Rows[0]["matchStatus"] == DBNull.Value ? "" : dt.Rows[0]["matchStatus"].ToString());
                    match.competitionTypeId = (dt.Rows[0]["competitionTypeId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["competitionTypeId"]);
                    matches.Add(match);
                   
                    return StatusCode((int)HttpStatusCode.OK, matches);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { });
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
    }
}