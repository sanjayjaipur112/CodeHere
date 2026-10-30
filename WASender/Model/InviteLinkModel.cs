using System;

namespace WASender.Model
{
    public class InviteLinkModel
    {
        public string Id { get; set; }
        public string GroupName { get; set; }
        public string GroupLink { get; set; }
        public string Source { get; set; }
        public bool IsValid { get; set; }
        public DateTime LastCheckedAtUtc { get; set; }
        public string Category { get; set; }
    }
}
