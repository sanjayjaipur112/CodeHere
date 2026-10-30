using System;
namespace WASender.Model
{
    public class AccountProfileModel
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
        public bool IsBusinessAccount { get; set; }
        public string BrowserProfilePath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
