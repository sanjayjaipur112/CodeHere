using System;

namespace WASender.Model
{
    public class CampaignLinkModel
    {
        public string Id { get; set; }
        public string CampaignName { get; set; }
        public string Source { get; set; }
        public string TargetPhoneNumber { get; set; }
        public string WaLink { get; set; }
        public string UTMSource { get; set; }
        public string UTMMedium { get; set; }
        public string UTMCampaign { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
