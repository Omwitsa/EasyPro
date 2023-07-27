﻿using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class Usergroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Registration { get; set; }
        public bool Flmd { get; set; }
        public bool Activity { get; set; }
        public bool Reports { get; set; }
        public bool Setup { get; set; }
        public bool Files { get; set; }
        public bool Accounts { get; set; }
        public bool Deductions { get; set; }
        public bool Staff { get; set; }
        public bool Store { get; set; }
        public bool Coffee { get; set; }
        public bool Products { get; set; }
        public bool ProdSupplier { get; set; }
        public bool ProdSales { get; set; }
        public bool SalesReturn { get; set; }
        public bool ProdDispatch { get; set; }
        public bool SaccoReports { get; set; }
        public bool ProdIntake { get; set; }
        public bool IntakeCorrection { get; set; }
        public bool ImportIntake { get; set; }
        public bool MilkTest { get; set; }
        public bool VarBalancing { get; set; }
        public bool Dispatch { get; set; }
        public bool SendSms { get; set; }
        public bool MilkControl { get; set; }
        public bool BranchMilkEnquiry { get; set; }
        public bool SupplierStatement { get; set; }
        public bool TransporterStatement { get; set; }
        public string SaccoCode { get; set; }
        public bool RFarmers { get; set; }
        public bool RTransporter { get; set; }
        public bool RImportS { get; set; }
        public bool RVendor { get; set; }
        public bool RCustomer { get; set; }
        public bool StandingOrder { get; set; }
        public bool CashShares { get; set; }
        public bool DefaultDed { get; set; }
        public bool DedFarmer { get; set; }
        public bool DedTransport { get; set; }
        public bool DedStaff { get; set; }
        public bool TransporterAssign { get; set; }
        public bool Millers { get; set; }
        public bool Marketers { get; set; }
        public bool Pulping { get; set; }
        public bool Milling { get; set; }
        public bool Marketing { get; set; }
        public bool CofPricing { get; set; }
        public bool CofPayroll { get; set; }
        public bool SetProducts { get; set; }
        public bool SetPrice { get; set; }
        public bool SetFarmersDif { get; set; }
        public bool SetOrganization { get; set; }
        public bool SetOrgBranch { get; set; }
        public bool SetUsers { get; set; }
        public bool SetUserGroups { get; set; }
        public bool SetCounties { get; set; }
        public bool SetSubCounties { get; set; }
        public bool SetWards { get; set; }
        public bool SetLocation { get; set; }
        public bool SetDedTypes { get; set; }
        public bool SetBanks { get; set; }
        public bool SetBanksBranch { get; set; }
        public bool SetZones { get; set; }
        public bool SetDebtors { get; set; }
        public bool SetTaxes { get; set; }
        public bool SetSharesCat { get; set; }
        public bool SetRoutes { get; set; }
        public bool ChartsofAcc { get; set; }
        public bool JournalPosting { get; set; }
        public bool Glinquiry { get; set; }
        public bool Budgettings { get; set; }
        public bool JournalListing { get; set; }
        public bool TrialBalance { get; set; }
        public bool IncomeStatement { get; set; }
        public bool BalanceSheet { get; set; }
        public bool Payroll { get; set; }
        public bool Bills { get; set; }
        public bool Refunds { get; set; }
        public bool CustomerInvoices { get; set; }
        public bool CreditNotes { get; set; }
        public bool VendorProducts { get; set; }
        public bool CustomerProducts { get; set; }
    }
}
