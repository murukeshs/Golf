using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class CourseModel
    {

        public class createCourse
        {
            public string courseName { get; set; }
            public string locationAddress { get; set; }
            public string contactName { get; set; }
            public string contactTitile { get; set; }
            public string email { get; set; }
            public string phoneNo { get; set; }
            public int ratings { get; set; }
            public string teeBoxMen { get; set; }
            public string teeBoxLadies { get; set; }
            public int defaultHole { get; set; }
            public string noOfHoles { get; set; }
            public int createdBy { get; set; }
        }
        public class updateCourse : createCourse
        {
            [Required]
            public int courseId { get; set; }
        }
        public class createScorecardDetail 
        {
            [Required]
            public int courseId { get; set; }
            public int? scoreCardId { get; set; }
            public string par { get; set; }
            public string mhcp { get; set; }
            public string lhcp { get; set; }
            public string hole { get; set; }
            public string slopeJson { get; set; }
        }

    }
}
