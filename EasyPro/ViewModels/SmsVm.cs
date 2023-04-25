using System;

namespace EasyPro.ViewModels
{
    public class SmsVm
    {
        public string Content { get; set; }
        public string TelNo { get; set; }
        public DateTime PeriodEnding { get; set; }
        public string Location { get; set; }
        public string Transporter { get; set; }
        public string SharesCategory { get; set; }
        public string Branch { get; set; }
        public SMSRecipient Recipient { get; set; }
    }

    public enum SMSRecipient
    {
        ALLFarmars = 1,
        ActiveFarmers = 2,
        SpecificLocation = 3,
        SpecificTransporter = 4,
        Individual = 5,
        SharesCat = 6,
    };
}
