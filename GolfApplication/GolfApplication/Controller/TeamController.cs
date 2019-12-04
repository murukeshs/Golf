using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GolfApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TeamController : ControllerBase
    {
        #region createTeam
        [HttpPost, Route("createTeam")]
        public IActionResult createTeam(TeamModel team)
        {
            try
            {
                if (team.teamName == "" || team.teamName == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {  ErrorMessage = "Please enter teamName" });
                }
                else if (team.createdBy <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter createdBy" });
                }
                else if (team.roundId <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter roundId" });
                }
                else
                {
                    DataSet ds = Data.Team.createTeam(team);

                    string Status = ds.Tables[0].Rows[0]["ErrorMessage"].ToString();

                    if (Status == "Success")
                    {
                        string TeamID = ds.Tables[1].Rows[0]["teamId"].ToString();
                        // return StatusCode((int)HttpStatusCode.OK, "TeamCreated Successfully");
                        return StatusCode((int)HttpStatusCode.OK, new { TeamID, message = "Team Created successfully" });
                    }
                    else
                    {
                        if (Status.Contains("UNIQUE KEY constraint") == true)
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Team name / Team Icon is already exists" });
                        }
                        else
                        {
                            //return "Invalid user";
                            return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = "Error while creating the Team" });
                        }
                    }
                   
                }
                
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createTeam", e.Message);
                if (e.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Team name / Team Icon is already exists" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message } );
                }
            }
        }
        #endregion

        #region updateTeam
        [HttpPut, Route("updateTeam")]
        public IActionResult updateTeam(updateTeam updateteam)
        {
            updateTeam team = new updateTeam();
            try
            {
                if (updateteam.teamId <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter teamId" });
                }
                else
                {
                    int row = Data.Team.UpdateTeam(updateteam);
                    if (row > 0)
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Error while Updating the team" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateTeam", e.Message);
                if (e.Message.Contains("UNIQUE KEY constraint") == true)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Team name / Team Icon is already exists" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
                }
            }
        }
        #endregion

        #region deleteTeam
        [HttpDelete, Route("deleteTeam")]
        public IActionResult deleteTeam(int teamId)
        {
            try
            {
                 if (teamId <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter teamId" });
                }                
                else
                {
                    string row = Data.Team.deleteTeam(teamId);

                    if (row == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Team is already enrolled" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteTeam", e.Message.ToString());

                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion

        #region selectTeamById
        [HttpGet, Route("selectTeamById")]
        public IActionResult selectTeamById(int teamId)
        {
            //List<getTeam> teamList = new List<getTeam>();
            List<dynamic> teamList = new List<dynamic>();
            try
            {
                DataSet ds = Data.Team.selectTeamById(teamId);
                string dt0 = ds.Tables[0].Rows[0]["status"].ToString();
                //List<updateTeam> teamDetailsList = new List<updateTeam>();
                dynamic teamDetails = new System.Dynamic.ExpandoObject();
                if (dt0 == "TeamPlayers")
                {
                    DataTable dt2 = ds.Tables[2];
                    //updateTeam teamDetails = new updateTeam();
                    
                    teamDetails.teamId = (dt2.Rows[0]["teamId"] == DBNull.Value ? 0 : (int)dt2.Rows[0]["teamId"]);
                    //team.scoreKeeperID = (dt2.Rows[0]["scoreKeeperID"] == DBNull.Value ? 0 : (int)dt.Rows[0]["scoreKeeperID"]);
                    teamDetails.teamName = (dt2.Rows[0]["teamName"] == DBNull.Value ? "" : dt2.Rows[0]["teamName"].ToString());
                    teamDetails.teamIcon = (dt2.Rows[0]["teamIcon"] == DBNull.Value ? "" : dt2.Rows[0]["teamIcon"].ToString());
                    teamDetails.CreatedOn = (dt2.Rows[0]["createdOn"] == DBNull.Value ? "" : dt2.Rows[0]["createdOn"].ToString());
                    teamDetails.createdBy = (dt2.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt2.Rows[0]["createdBy"]);
                    teamDetails.createdByName = (dt2.Rows[0]["createdByName"] == DBNull.Value ? "" : dt2.Rows[0]["createdByName"]);
                    teamDetails.startingHole = (dt2.Rows[0]["startingHole"] == DBNull.Value ? 0 : (int)dt2.Rows[0]["startingHole"]);
                    //teamList.Add(teamDetails);

                    DataTable dt = ds.Tables[1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //getTeam team = new getTeam();
                        dynamic team = new System.Dynamic.ExpandoObject();
                        team.teamPlayerListId = (int)dt.Rows[i]["teamPlayerListId"];
                        team.teamId = (int)dt.Rows[i]["teamId"];
                        team.playerName = (dt.Rows[i]["playerName"] == DBNull.Value ? "" : dt.Rows[i]["playerName"].ToString());
                        team.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                        team.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                        team.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        team.RoleType = (dt.Rows[i]["RoleType"] == DBNull.Value ? "" : dt.Rows[i]["RoleType"].ToString());
                        team.nickName = (dt.Rows[i]["nickName"] == DBNull.Value ? "" : dt.Rows[i]["nickName"].ToString());
                        teamList.Add(team);
                    }
                    teamDetails.TeamPlayerDetails = teamList;
                    return StatusCode((int)HttpStatusCode.OK, teamDetails);
                }
                else if (dt0 == "TeamDetails")
                {
                    //    DataTable dt = ds.Tables[1];
                    //    updateTeam team = new updateTeam();
                    //        team.teamId = (dt.Rows[0]["teamId"] == DBNull.Value ? 0 : (int)dt.Rows[0]["teamId"]);
                    //        team.scoreKeeperID = (dt.Rows[0]["scoreKeeperID"] == DBNull.Value ? 0 : (int)dt.Rows[0]["scoreKeeperID"]);
                    //        team.teamName = (dt.Rows[0]["teamName"] == DBNull.Value ? "" : dt.Rows[0]["teamName"].ToString());
                    //        team.teamIcon = (dt.Rows[0]["teamIcon"] == DBNull.Value ? "" : dt.Rows[0]["teamIcon"].ToString());
                    //        team.CreatedOn = (dt.Rows[0]["createdOn"] == DBNull.Value ? "" : dt.Rows[0]["createdOn"].ToString());
                    //        team.createdBy = (dt.Rows[0]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[0]["createdBy"]);
                    //        team.startingHole = (dt.Rows[0]["startingHole"] == DBNull.Value ? 0 : (int)dt.Rows[0]["startingHole"]);
                    //        teamDetailsList.Add(team);
                    // return StatusCode((int)HttpStatusCode.OK, team);

                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = "Team not available" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectTeamById", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }

        #endregion

        #region listTeam
        [HttpGet, Route("listTeam")]
        public IActionResult listTeam()
        {
            List<listTeam> teamList = new List<listTeam>();
            try
            {
                DataTable dt = Data.Team.listTeam();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        listTeam team = new listTeam();

                        team.teamId = (dt.Rows[i]["teamId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["teamId"]);
                        team.scoreKeeperID = (dt.Rows[i]["scoreKeeperID"] == DBNull.Value ? 0 : (int)dt.Rows[i]["scoreKeeperID"]); 
                        team.teamName = (dt.Rows[i]["teamName"] == DBNull.Value ? "" : dt.Rows[i]["teamName"].ToString());
                        team.teamIcon = (dt.Rows[i]["teamIcon"] == DBNull.Value ? "" : dt.Rows[i]["teamIcon"].ToString());
                        team.CreatedOn = (dt.Rows[i]["CreatedOn"] == DBNull.Value ? "" : dt.Rows[i]["CreatedOn"].ToString());
                        team.createdName = (dt.Rows[i]["createdName"] == DBNull.Value ? "" : dt.Rows[i]["createdName"].ToString());
                        team.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        team.startingHole = (dt.Rows[i]["startingHole"] == DBNull.Value ? 0 : (int)dt.Rows[i]["startingHole"]);
                        team.noOfPlayers = (dt.Rows[i]["noOfPlayers"] == DBNull.Value ? 0 : (int)dt.Rows[i]["noOfPlayers"]);
                        team.TeamplayerList= (dt.Rows[i]["playerList"] == DBNull.Value ? "" : dt.Rows[i]["playerList"].ToString());
                        //team.roundId= (dt.Rows[i]["roundId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["roundId"]);
                        teamList.Add(team);
                    }
                    return StatusCode((int)HttpStatusCode.OK, teamList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, teamList);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listTeam", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region createTeamPlayers
        [HttpPut, Route("createTeamPlayers")]
        public IActionResult createTeamPlayers(TeamPlayer teamPlayer)
        {
            try
            {
                //TeamPlayer teamPlayer
                DataTable dt = Data.Team.createTeamPlayers(teamPlayer);
                string row = dt.Rows[0]["ErrorMessage"].ToString();

                if (dt.Rows[0]["ErrorMessage"].ToString() == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Team Players created Successfully");
                }
                else
                {
                    //return "Invalid user";
                    return StatusCode((int)HttpStatusCode.Forbidden, new {ErrorMessage = dt.Rows[0]["ErrorMessage"].ToString() });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("createTeamPlayers", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region deleteTeamPlayers
        [HttpDelete, Route("deleteTeamPlayers")]
        public IActionResult deleteTeamPlayers(int teamPlayerListId)
        {
            List<dynamic> teamList = new List<dynamic>();
            try
            {
                if (teamPlayerListId <= 0 )
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new {ErrorMessage = "Please enter proper teamPlayerListId" });
                }
                else
                {
                    DataSet ds  = Data.Team.deleteTeamPlayers(teamPlayerListId);

                    if (ds.Tables[0].Rows[0][0].ToString() == "Success")
                    {
                        DataTable dt = ds.Tables[1];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dynamic team = new System.Dynamic.ExpandoObject();
                            team.teamPlayerListId = (int)dt.Rows[i]["teamPlayerListId"];
                            team.teamId = (int)dt.Rows[i]["teamId"];
                            team.playerName = (dt.Rows[i]["playerName"] == DBNull.Value ? "" : dt.Rows[i]["playerName"].ToString());
                            team.profileImage = (dt.Rows[i]["profileImage"] == DBNull.Value ? "" : dt.Rows[i]["profileImage"].ToString());
                            team.gender = (dt.Rows[i]["gender"] == DBNull.Value ? "" : dt.Rows[i]["gender"].ToString());
                            team.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                            team.RoleType = (dt.Rows[i]["RoleType"] == DBNull.Value ? "" : dt.Rows[i]["RoleType"].ToString());
                            team.nickName = (dt.Rows[i]["nickName"] == DBNull.Value ? "" : dt.Rows[i]["nickName"].ToString());
                            teamList.Add(team);
                        }
                        return StatusCode((int)HttpStatusCode.OK, teamList);
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = "Team player is already Paid Fee" });
                    }
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteTeamPlayers", e.Message.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion
    }
}