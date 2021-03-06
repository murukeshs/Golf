﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class roundRules
    {
        public int roundRuleId { get; set; }
        public string ruleName { get; set; }
    }
    public class createRound
    {
        public string roundName { get; set; }
        public string roundCode { get; set; }
        public string roundRuleId { get; set; }
        public string roundStartDate { get; set; }
        public string roundEndDate { get; set; }
        public decimal roundFee { get; set; }
        public int createdBy { get; set; }
        public int competitionTypeId { get; set; }
        public int roundId { get; set; }
        public string roundStatus { get; set; }
        public bool isSaveAndNotify { get; set; }
    }

    public class roundPlayer
    {
        public string type { get; set; }
        public int eventId { get; set; }
        public string teamId { get; set; }
       // public string playerId { get; set; }
        //public bool isInvitationSent { get; set; }
        //public bool isInvitationAccept { get; set; }
        //public bool isPaymentMade { get; set; }
        //public string createdDate { get; set; }
    }

    //public class MatchList
    //{
    //    public int matchId { get; set; }
    //    public string matchCode { get; set; }
    //    public string matchName { get; set; }
    //    //public string matchType { get; set; }
    //    public string matchRuleId { get; set; }
    //    public string matchStartDate { get; set; }
    //    public string matchEndDate { get; set; }
    //    public decimal matchFee { get; set; }
    //    public string matchLocation { get; set; }
    //    public int createdBy { get; set; }
    //    public string createdDate { get; set; }
    //    public string matchStatus { get; set; }
    //    public int competitionTypeId { get; set; }
    //    public string ruleName { get; set; }
    //    public string competitionName { get; set; }
    //}
    public class RoundList
    {
        public int roundId { get; set; }
        public string roundCode { get; set; }
        public string roundName { get; set; }
        //public string matchType { get; set; }
        public string roundRuleId { get; set; }
        public string roundStartDate { get; set; }
        public string roundEndDate { get; set; }
        public decimal roundFee { get; set; }
        public string roundLocation { get; set; }
        public int createdBy { get; set; }
        public string createdDate { get; set; }
        public string roundStatus { get; set; }
        public int competitionTypeId { get; set; }
        public string ruleName { get; set; }
        public string competitionName { get; set; }
    }

    public class MatchRulesList
    {
        public int roundRuleId { get; set; }
        public string ruleName { get; set; }
    }

    public class CompetitionType
    {
        public int competitionTypeId { get; set; }
        public string CompetitionName { get; set; }
    }

    public class acceptMatchInvitation
    {
        public int roundId { get; set; }
        public string Type { get; set; }
        public int playerId { get; set; }
    }

    public class addParticipants
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string phoneNumber { get; set; }
        public int teamId { get; set; }
        public string userTypeId { get; set; }
    }
    public class SaveRoundPlayer
    {
        public string userId { get; set; }
        public int roundId { get; set; }
    }
}