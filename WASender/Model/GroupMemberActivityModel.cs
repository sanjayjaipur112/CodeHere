using System;

namespace WASender.Model
{
    public class GroupMemberActivityModel
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string MemberName { get; set; }
        public string MemberPhone { get; set; }
        public int MessageCount { get; set; }
        public DateTime LastSeenUtc { get; set; }
        public double EngagementScore { get; set; }
    }
}
