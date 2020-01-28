using Microsoft.ApplicationBlocks.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static GolfApplication.Models.CourseModel;

namespace GolfApplication.Data
{
    public class Course
    {
        #region createUser
        public static DataTable createCourse([FromBody]createCourse createCourse)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

               
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@courseName", createCourse.courseName));
                parameters.Add(new SqlParameter("@locationAddress", createCourse.locationAddress));
                parameters.Add(new SqlParameter("@contactName", createCourse.contactName));
                parameters.Add(new SqlParameter("@contactTitile", createCourse.contactTitile));
                parameters.Add(new SqlParameter("@email", createCourse.email));
                parameters.Add(new SqlParameter("@phoneNo", createCourse.phoneNo));
                parameters.Add(new SqlParameter("@ratings", createCourse.ratings));
                parameters.Add(new SqlParameter("@teeBoxMen", createCourse.teeBoxMen));
                parameters.Add(new SqlParameter("@teeBoxLadies", createCourse.teeBoxLadies));
                parameters.Add(new SqlParameter("@defaultHole", createCourse.defaultHole));
                parameters.Add(new SqlParameter("@noOfHoles", createCourse.noOfHoles));
                parameters.Add(new SqlParameter("@createdBy", createCourse.createdBy));
                parameters.Add(new SqlParameter("@Action", "Add")); 

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCreateCourse", parameters.ToArray()).Tables[0])
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

        #region updateCourse
        public static string updateCourse([FromBody]updateCourse updateCourse)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@courseId", updateCourse.courseId));
                parameters.Add(new SqlParameter("@courseName", updateCourse.courseName));
                parameters.Add(new SqlParameter("@locationAddress", updateCourse.locationAddress));
                parameters.Add(new SqlParameter("@contactName", updateCourse.contactName));
                parameters.Add(new SqlParameter("@contactTitile", updateCourse.contactTitile));
                parameters.Add(new SqlParameter("@email", updateCourse.email));
                parameters.Add(new SqlParameter("@phoneNo", updateCourse.phoneNo));
                parameters.Add(new SqlParameter("@ratings", updateCourse.ratings));
                parameters.Add(new SqlParameter("@teeBoxMen", updateCourse.teeBoxMen));
                parameters.Add(new SqlParameter("@teeBoxLadies", updateCourse.teeBoxLadies));
                parameters.Add(new SqlParameter("@defaultHole", updateCourse.defaultHole));
                parameters.Add(new SqlParameter("@noOfHoles", updateCourse.noOfHoles));
                parameters.Add(new SqlParameter("@createdBy", updateCourse.createdBy));
                parameters.Add(new SqlParameter("@Action", null));

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spCreateCourse", parameters.ToArray()).ToString();
                return rowsAffected;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region deleteCourse
        public static int deleteCourse(int courseId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@courseId", courseId));
            parameters.Add(new SqlParameter("@action", "delete"));


            try
            {
                string ConnectionString = Common.GetConnectionString();
                int rowsAffected = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteById", parameters.ToArray());
                return rowsAffected;
            }
            catch (Exception e)
            {
                //loggerErr.Error(e.Message + " - " + e.StackTrace);
                throw e;
            }
        }
        #endregion

        #region selectCourseById
        public static DataTable selectCourseById(int courseId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@courseId", courseId));

            try
            {
                string ConnectionString = Common.GetConnectionString();
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spSelectDeleteById", parameters.ToArray()).Tables[0])
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
        #endregion

        #region listCourse
        public static DataTable listCourse(string Search)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Search", Search == null ? "" : Search));
                //Execute the query
                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "splistCourse", parameters.ToArray()).Tables[0])
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

        #region createScorecardDetail
        public static DataTable createScorecardDetail(createScorecardDetail createScorecardDetail)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();


                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@courseId", createScorecardDetail.courseId));
                parameters.Add(new SqlParameter("@par", createScorecardDetail.par));
                parameters.Add(new SqlParameter("@mhcp", createScorecardDetail.mhcp));
                parameters.Add(new SqlParameter("@lhcp", createScorecardDetail.lhcp));
                parameters.Add(new SqlParameter("@hole", createScorecardDetail.hole));
                parameters.Add(new SqlParameter("@slopeJson", createScorecardDetail.slopeJson));
                parameters.Add(new SqlParameter("@Action", "Add"));

                using (DataTable dt = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "spCreateScorecardDetail", parameters.ToArray()).Tables[0])
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

        #region updateScorecardDetail
        public static string updateScorecardDetail([FromBody]createScorecardDetail updateCourse)
        {
            try
            {
                string ConnectionString = Common.GetConnectionString();

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@courseId", updateCourse.courseId));
                parameters.Add(new SqlParameter("@par", updateCourse.par));
                parameters.Add(new SqlParameter("@mhcp", updateCourse.mhcp));
                parameters.Add(new SqlParameter("@lhcp", updateCourse.lhcp));
                parameters.Add(new SqlParameter("@hole", updateCourse.hole));
                parameters.Add(new SqlParameter("@slopeJson", updateCourse.slopeJson));
                parameters.Add(new SqlParameter("@Action", null));

                string rowsAffected = SqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "spCreateScorecardDetail", parameters.ToArray()).ToString();
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
