using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class ExcelDumpSupReg
    {
        [Key]
        public long Id { get; set; }
        public DateTime Reg_date { get; set; }
        public string SNo { get; set; }
        public string Names { get; set; }
        public string PhoneNo { get; set; }
        public string IdNo { get; set; }
        public DateTime DOB { get; set; }
        public string Acc_Number { get; set; }
        public string Bank_code { get; set; }
        public string Bank_Branch { get; set; }
        public string Gender { get; set; }
        public string PaymentMode { get; set; }
        public string Village { get; set; }
        public string LOCATION { get; set; }
        public string WARD { get; set; }
        public string SUB_COUNTY { get; set; }
        public string COUNTY { get; set; }
        public string LoggedInUser { get; set; }
        public string Branch { get; set; }
        public string SaccoCode { get; set; }
        public string CIGName { get; set; }
    }
}