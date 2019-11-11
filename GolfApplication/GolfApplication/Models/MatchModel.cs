using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Models
{
    public class MatchRules
    {
        public int matchRuleId { get; set; }
        public string ruleName { get; set; }
    }

    public class createMatch
    {
        public string matchName { get; set; }
        public string matchType { get; set; }
        public string matchCode { get; set; }
        public string matchRuleId { get; set; }
        public string matchStartDate { get; set; }
        public string matchEndDate { get; set; }
        public decimal matchFee { get; set; }
        public string matchLocation { get; set; }
        public int createdBy { get; set; }
        public int competitionTypeId { get; set; }
        public int matchId { get; set; }
        public string matchStatus { get; set; }
    }

    public class matchPlayer
    {
        public string type { get; set; }
        public int eventId { get; set; }
        public int teamId { get; set; }
        public int playerId { get; set; }
        public bool isInvitationSent { get; set; }
        public bool isInvitationAccept { get; set; }
        public bool isPaymentMade { get; set; }
        public string createdDate { get; set; }
    }

    public class MatchList
    {
        public int matchId { get; set; }
        public string matchCode { get; set; }
        public string matchName { get; set; }
        public string matchType { get; set; }
        public string matchRuleId { get; set; }
        public string matchStartDate { get; set; }
        public string matchEndDate { get; set; }
        public decimal matchFee { get; set; }
        public string matchLocation { get; set; }
        public int createdBy { get; set; }
        public string createdDate { get; set; }
        public string matchStatus { get; set; }
        public int competitionTypeId { get; set; }
        public string ruleName { get; set; }
        public string competitionName { get; set; }
    }

    public class MatchRulesList
    {
        public int matchRuleId { get; set; }
        public string ruleName { get; set; }
    }

    public class CompetitionType
    {
        public int competitionTypeId { get; set; }
        public string CompetitionName { get; set; }
    }

    public class acceptMatchInvitation
    {
        public int matchId { get; set; }
        public string Type { get; set; }
        public int playerId { get; set; }
    }

