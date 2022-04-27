using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class B2cdisbursementResponse
    {
        public int Id { get; set; }
        public int? ResultCode { get; set; }
        public int? ResultType { get; set; }
        public string ResultDesc { get; set; }
        public string OriginatorConversationId { get; set; }
        public string ConversationId { get; set; }
        public string TransactionId { get; set; }
        public string TransacionReceipt { get; set; }
        public string Amount { get; set; }
        public string B2cutilityAccountAvailableFunds { get; set; }
        public string TransactionCompletedDateTime { get; set; }
        public string ReceiverPartyPublicName { get; set; }
        public long? Run { get; set; }
        public string PhoneNo2 { get; set; }
        public long? Run2 { get; set; }
    }
}
