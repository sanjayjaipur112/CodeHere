using System;

namespace WASender.Model
{
    public class WarmupTemplateModel
    {
        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string MessageText { get; set; }
        public int DelayMinSeconds { get; set; }
        public int DelayMaxSeconds { get; set; }
        public bool IsHumanLike { get; set; }
        public bool IsActive { get; set; }
    }
}
