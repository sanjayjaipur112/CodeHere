using System;

namespace WASender.Model
{
    public class GroupInsightModel
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int MessageCount { get; set; }
        public double AvgMessagesPerDay { get; set; }
        public string TopEngagedMember { get; set; }
        public double EngagementScore { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
    }
}
