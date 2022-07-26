using System;

namespace EasyPro.ViewModels
{
    public class SmsVm
    {
        public string Content { get; set; }
        public string TelNo { get; set; }
        public DateTime PeriodEnding { get; set; }
        public string Location { get; set; }
        public SMSRecipient Recipient { get; set; }
    }

    public enum SMSRecipient
    {
        ALLFarmars = 1,
        ActiveFarmers = 2,
        SpecificLocation = 3,
        Individual = 4,
    };
}
