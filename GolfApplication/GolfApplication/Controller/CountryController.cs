﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using GolfApplication.Models;
using GolfApplication.Data;
using System.Net;
using System.Data;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        #region GetCountryList
        [HttpGet, Route("GetCountryList")]
        public IActionResult GetCountryList()
        {
            List<CountryModel> countryList = new List<CountryModel>();
            try
            {
                DataTable dt = Data.CountryState.GetCountryList();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CountryModel countries = new CountryModel();
                        countries.countryId = (dt.Rows[i]["countryId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["countryId"]);
                        countries.countryName = dt.Rows[i]["countryName"].ToString();
                        countryList.Add(countries);
                    }
                    return StatusCode((int)HttpStatusCode.OK, countryList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("GetCountryList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion

        #region GetStateList
        [HttpGet, Route("GetStateList/{CountryId}")]
        public IActionResult GetStateList(int CountryId)
        {
            List<StateModel> StateList = new List<StateModel>();
            try
            {
                DataTable dt = Data.CountryState.GetStateList(CountryId);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StateModel states = new StateModel();
                        states.StateId = (dt.Rows[i]["StateId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["StateId"]);
                        states.CountryId = (dt.Rows[i]["CountryId"] == DBNull.Value ? 0 : (int)dt.Rows[i]["CountryId"]);
                        states.StateName = dt.Rows[i]["StateName"].ToString();
                        StateList.Add(states);
                    }
                    return StatusCode((int)HttpStatusCode.OK, StateList);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.OK, new { });
                }
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("GetStateList", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new {ErrorMessage = e.Message });
            }
        }
        #endregion
    }
}