﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class TeamModel
    {
        public string teamName { get; set; }
        public string teamIcon { get; set; }
        public int createdBy { get; set; }
        public string CreatedOn { get; set; }
        public int startingHole { get; set; }
        public int roundId { get; set; }
    }

    public class updateTeam : TeamModel
    {
        public int teamId { get; set; }
        //public int scoreKeeperID { get; set; }
        //public int noOfPlayers { get; set; }  
        //public string createdName { get; set; }
        //public string TeamplayerList { get; set; }
    }
    public class listTeam
    {
        public string teamName { get; set; }
        public string teamIcon { get; set; }
        public int createdBy { get; set; }
        public string CreatedOn { get; set; }
        public int startingHole { get; set; }
        public int teamId { get; set; }
        public int scoreKeeperID { get; set; }
        public int noOfPlayers { get; set; }
        public string createdName { get; set; }
        public string TeamplayerList { get; set; }
        public int roundId { get; set; }
    }

    public class getTeam
    {
        public int teamPlayerListId { get; set; }
        public int teamId { get; set; }
        public string playerName { get; set; }
        public string gender { get; set; }
        public string RoleType { get; set; }
        public string profileImage { get; set; }
        public string email { get; set; }
        public int roundId { get; set; }
    }

    public class TeamPlayer
    {
        public int teamId { get; set; }
        public int scoreKeeperID { get; set; }
        public string playerId { get; set; }
    }

}
