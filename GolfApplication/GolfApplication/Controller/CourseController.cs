using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GolfApplication.Data.Course;
using static GolfApplication.Models.CourseModel;

namespace GolfApplication.Controller
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CourseController : ControllerBase
    {

        #region createCourse
        [HttpPost, Route("createCourse")]
        [AllowAnonymous]
        public IActionResult createUser(createCourse createCourse)
        {
            try
            {
                if (String.IsNullOrEmpty(createCourse.courseName))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter courseName" });
                }
                else if (String.IsNullOrEmpty(createCourse.email))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter email" });
                }
                else if (String.IsNullOrEmpty(createCourse.phoneNo))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter phoneNo" });
                }
                else if (String.IsNullOrEmpty(createCourse.locationAddress))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter locationAddress" });
                }
                else if (createCourse.ratings == 0 || createCourse.ratings == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMessage = "Please enter ratings" });
                }

                    DataTable dt = Data.Course.createCourse(createCourse);
                    string Response = dt.Rows[0][0].ToString();
                    if (Response == "Success")
                    {
                        return StatusCode((int)HttpStatusCode.OK, new { courseId = dt.Rows[0][1].ToString() });
                    }
                    else
                    {
                            return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                    }

            }
            catch (Exception e)
            {
               
                string SaveErrorLog = Data.Common.SaveErrorLog("createCourse", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }

        #endregion

        #region updateCourse
        [HttpPut, Route("updateCourse")]
        public IActionResult updateCourse([FromBody]updateCourse updateCourse)
        {
            try
            {
                string row = Data.Course.updateCourse(updateCourse);

                if (row == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                   
                 return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = row });
                   
                }
                
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateCourse", e.Message);
                
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
               
            }
        }
        #endregion

        #region deleteCourse
        [HttpDelete, Route("deleteCourse")]
        public IActionResult deleteCourse([Required]int courseId)
        {
            try
            {
                int row = Data.Course.deleteCourse(courseId);

                if (row > 0)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Deleted Successfully");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = "Error while Deleting the record" });
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("deleteCourse", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region selectCourseById
        [HttpGet, Route("selectCourseById")]
        public IActionResult selectCourseById([Required]int courseId)
        {
            try
            {
                DataTable dt = Data.Course.selectCourseById(courseId);
                dynamic course = new System.Dynamic.ExpandoObject();

                if (dt.Rows.Count > 0)
                {

                    course.courseId = (int)dt.Rows[0]["courseId"];
                    course.courseName = (dt.Rows[0]["courseName"] == DBNull.Value ? "" : dt.Rows[0]["courseName"].ToString());
                    course.locationAddress = (dt.Rows[0]["locationAddress"] == DBNull.Value ? "" : dt.Rows[0]["locationAddress"].ToString());
                    course.contactName = (dt.Rows[0]["contactName"] == DBNull.Value ? "" : dt.Rows[0]["contactName"].ToString());
                    course.contactTitile = (dt.Rows[0]["contactTitile"] == DBNull.Value ? "" : dt.Rows[0]["contactTitile"].ToString());
                    course.email = (dt.Rows[0]["email"] == DBNull.Value ? "" : dt.Rows[0]["email"].ToString());
                    course.phoneNo = (dt.Rows[0]["phoneNo"] == DBNull.Value ? "" : dt.Rows[0]["phoneNo"].ToString());
                    course.ratings = (dt.Rows[0]["ratings"] == DBNull.Value ? "" : dt.Rows[0]["ratings"].ToString());
                    course.teeBoxMen = (dt.Rows[0]["teeBoxMen"] == DBNull.Value ? "" : dt.Rows[0]["teeBoxMen"].ToString());
                    course.teeBoxLadies = (dt.Rows[0]["teeBoxLadies"] == DBNull.Value ? "" : dt.Rows[0]["teeBoxLadies"].ToString());
                    course.defaultHole = (dt.Rows[0]["defaultHole"] == DBNull.Value ? "" : dt.Rows[0]["defaultHole"].ToString());
                    course.noOfHoles = (dt.Rows[0]["noOfHoles"] == DBNull.Value ? "" : dt.Rows[0]["noOfHoles"].ToString());
                    course.createdDate = (dt.Rows[0]["createdDate"] == DBNull.Value ? "" : dt.Rows[0]["createdDate"].ToString());
                    course.createdBy = (dt.Rows[0]["createdBy"] == DBNull.Value ? 0: (int)dt.Rows[0]["createdBy"]);
                    course.createdByName = (dt.Rows[0]["createdByName"] == DBNull.Value ? "" : dt.Rows[0]["createdByName"].ToString());
                    course.par = (dt.Rows[0]["par"] == DBNull.Value ? "" : dt.Rows[0]["par"].ToString());
                    course.mhcp = (dt.Rows[0]["mhcp"] == DBNull.Value ? "" : dt.Rows[0]["mhcp"].ToString());
                    course.lhcp = (dt.Rows[0]["lhcp"] == DBNull.Value ? "" : dt.Rows[0]["lhcp"].ToString());
                    course.hole = (dt.Rows[0]["hole"] == DBNull.Value ? "" : dt.Rows[0]["hole"].ToString());
                    course.slopeDeatils = (dt.Rows[0]["slopeDeatils"] == DBNull.Value ? "" : dt.Rows[0]["slopeDeatils"].ToString());
                    

                    return StatusCode((int)HttpStatusCode.OK, course);
                }

                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("selectCourseById", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region listCourse
        [HttpGet, Route("listCourse")]
        public IActionResult listCourse(string Search)
        {
            List<dynamic> courseList = new List<dynamic>();
            try
            {
                DataTable dt = Data.Course.listCourse(Search);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dynamic course = new System.Dynamic.ExpandoObject();

                        course.courseId = (int)dt.Rows[i]["courseId"];
                        course.courseName = (dt.Rows[i]["courseName"] == DBNull.Value ? "" : dt.Rows[i]["courseName"].ToString());
                        course.locationAddress = (dt.Rows[i]["locationAddress"] == DBNull.Value ? "" : dt.Rows[i]["locationAddress"].ToString());
                        course.contactName = (dt.Rows[i]["contactName"] == DBNull.Value ? "" : dt.Rows[i]["contactName"].ToString());
                        course.contactTitile = (dt.Rows[i]["contactTitile"] == DBNull.Value ? "" : dt.Rows[i]["contactTitile"].ToString());
                        course.email = (dt.Rows[i]["email"] == DBNull.Value ? "" : dt.Rows[i]["email"].ToString());
                        course.phoneNo = (dt.Rows[i]["phoneNo"] == DBNull.Value ? "" : dt.Rows[i]["phoneNo"].ToString());
                        course.ratings = (dt.Rows[i]["ratings"] == DBNull.Value ? "" : dt.Rows[i]["ratings"].ToString());
                        course.teeBoxMen = (dt.Rows[i]["teeBoxMen"] == DBNull.Value ? "" : dt.Rows[i]["teeBoxMen"].ToString());
                        course.teeBoxLadies = (dt.Rows[i]["teeBoxLadies"] == DBNull.Value ? "" : dt.Rows[i]["teeBoxLadies"].ToString());
                        course.defaultHole = (dt.Rows[i]["defaultHole"] == DBNull.Value ? "" : dt.Rows[i]["defaultHole"].ToString());
                        course.noOfHoles = (dt.Rows[i]["noOfHoles"] == DBNull.Value ? "" : dt.Rows[i]["noOfHoles"].ToString());
                        course.createdDate = (dt.Rows[i]["createdDate"] == DBNull.Value ? "" : dt.Rows[i]["createdDate"].ToString());
                        course.createdBy = (dt.Rows[i]["createdBy"] == DBNull.Value ? 0 : (int)dt.Rows[i]["createdBy"]);
                        course.createdByName = (dt.Rows[i]["createdByName"] == DBNull.Value ? "" : dt.Rows[i]["createdByName"].ToString());
                        course.par = (dt.Rows[i]["par"] == DBNull.Value ? "" : dt.Rows[i]["par"].ToString());
                        course.mhcp = (dt.Rows[i]["mhcp"] == DBNull.Value ? "" : dt.Rows[i]["mhcp"].ToString());
                        course.lhcp = (dt.Rows[i]["lhcp"] == DBNull.Value ? "" : dt.Rows[i]["lhcp"].ToString());
                        course.hole = (dt.Rows[i]["hole"] == DBNull.Value ? "" : dt.Rows[i]["hole"].ToString());
                        course.slopeDeatils = (dt.Rows[i]["slopeDeatils"] == DBNull.Value ? "" : dt.Rows[i]["slopeDeatils"].ToString());
                        courseList.Add(course);

                    }

                    return StatusCode((int)HttpStatusCode.OK, courseList);
                }
                else
                {
                    string[] data = new string[0];
                    return StatusCode((int)HttpStatusCode.OK, data);
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("listCourse", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });
            }
        }
        #endregion

        #region createScorecardDetail
        [HttpPost, Route("createScorecardDetail")]
        [AllowAnonymous]
        public IActionResult createScorecardDetail(createScorecardDetail createScorecardDetail)
        {
            try
            {
                

                DataTable dt = Data.Course.createScorecardDetail(createScorecardDetail);
                string Response = dt.Rows[0][0].ToString();
                if (Response == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, new { scoreCardId = dt.Rows[0][0].ToString() , slopeId  = dt.Rows[0][1].ToString() });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = Response });
                }

            }
            catch (Exception e)
            {

                string SaveErrorLog = Data.Common.SaveErrorLog("createScorecardDetail", e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }

        #endregion

        #region updateScorecardDetail
        [HttpPut, Route("updateScorecardDetail")]
        public IActionResult updateScorecardDetail([FromBody]createScorecardDetail updateScoreDetails)
        {
            try
            {
                string row = Data.Course.updateScorecardDetail(updateScoreDetails);

                if (row == "Success")
                {
                    return StatusCode((int)HttpStatusCode.OK, "Updated Successfully");
                }
                else
                {
                    
                        return StatusCode((int)HttpStatusCode.Forbidden, new { ErrorMessage = row });
                   
                }

            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("updateScorecardDetail", e.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message });

            }
        }
        #endregion
    }
}