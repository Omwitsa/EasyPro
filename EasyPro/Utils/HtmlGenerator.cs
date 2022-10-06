using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPro.Utils
{
    public class HtmlGenerator
    {
        public static string GenerateIntakeReceiptHtml(ProductIntakeVm intake)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h3>Intake</h3></div><hr/>
                                ");

            sb.AppendFormat(@"<table>
                                <tr>
                                   <td>Sacco Code:</td>
                                   <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>Supplier No.:</td>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>Supplier Name:</td>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td col-span='2'>.....................................................................</td>
                                </tr>
                                <tr>
                                   <td>Product Type:</td>
                                   <td>{3}</td>
                                </tr>
                                <tr>
                                   <td>Quantity:</td>
                                   <td>{4}</td>
                                </tr>
                                <tr>
                                   <td>Cumlative:</td>
                                   <td>{5}</td>
                                </tr>
                                <tr>
                                   <td>Phone No.:</td>
                                   <td>{6}</td>
                                </tr>
                                <tr>
                                   <td>Trans Date:</td>
                                   <td>{7}</td>
                                </tr>
                                <tr>
                                   <td>Served By:</td>
                                   <td>{8}</td>
                                </tr>
                                   <td>Powered By:</td>
                                   <td>Amtech Technologies LTD</td>
                                </tr>
                              </table>",
                              intake.SaccoCode, intake.Sno, intake.SupName,
                              intake.ProductType, intake.Qsupplied, intake.Cumlative,
                              intake.PhoneNo, intake.TransDate, intake.AuditId);
            sb.Append(@"
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateIntakesHtml(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, IQueryable<DSupplier> suppliers)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0} List</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>SNo</th>
                                                <th>Name</th>
                                                <th>Date</th>
                                                <th>Product Type</th>
                                                <th>Qsupplied</th>
                                                <th>Price</th>
                                                <th>Description</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var intake in productIntakeobj)
            {
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToString() == intake.Sno);
                var supplierName = supplier?.Names ?? "";
                long.TryParse(intake.Sno, out long sno);
                var checkifexist = suppliers.Where(u => u.Sno == sno);
                if (checkifexist.Any())
                {
                    sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                            </tr>
                            ",
                              intake.Sno, supplierName, intake.TransDate, intake.ProductType,
                              intake.Qsupplied, intake.Ppu, intake.Description);
                }
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateSuppliersPayrollHtml(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title, IQueryable<DSupplier> suppliers)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);
            sb.AppendFormat(@"
                                <div class='header'><h3>{0}</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>SNo</th>
                                                <th>Name</th>
                                                <th>IdNo</th>
                                                <th>Transport</th>
                                                <th>Agrovet</th>
                                                <th>Bonus</th>
                                                <th>Shares</th>
                                                <th>Advance</th>
                                                <th>Mid Month</th>
                                                <th>Others</th>
                                                <th>TDeductions</th>
                                                <th>KgsSupplied</th>
                                                <th>GPay</th>
                                                <th>NPay</th>
                                                <th>Bank</th>
                                                <th>AccountNumber</th>
                                                <th>BBranch</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var payroll in dpayrollobj)
            {
                var supplier = suppliers.FirstOrDefault(s => s.Sno == payroll.Sno);
                var supplierName = supplier?.Names ?? "";

                sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                                <td>{8}</td>
                                <td>{9}</td>
                                <td>{10}</td>
                                <td>{11}</td>
                                <td>{12}</td>
                                <td>{13}</td>
                                <td>{14}</td>
                                <td>{15}</td>
                                <td>{16}</td>
                            </tr>
                            ",
                              payroll.Sno, supplierName, payroll.IdNo, payroll.Transport,
                              payroll.Agrovet, payroll.Bonus, payroll.Hshares, payroll.Advance,
                              payroll.Midmonth, payroll.Others, payroll.Tdeductions, payroll.KgsSupplied,
                              payroll.Gpay, payroll.Npay, payroll.Bank, payroll.AccountNumber, payroll.Bbranch);
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateAgSalesReceiptHtml(List<AgReceipt> receipts, DSupplier supplier)
        {
            var receipt = receipts.FirstOrDefault();
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h3>Sales</h3></div><hr/>
                                ");
            if(supplier != null)
            {
                sb.AppendFormat(@"<table>
                                <tr>
                                   <td>Sacco Code:</td>
                                   <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>Supplier No.:</td>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>Supplier Name:</td>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td col-span='2'>.....................................................................</td>
                                </tr>
                                <tr>
                                   <td>Phone No.:</td>
                                   <td>{3}</td>
                                </tr>
                                <tr>
                                   <td>Trans Date:</td>
                                   <td>{4}</td>
                                </tr>
                              ",
                             supplier.Scode, supplier.Sno, supplier.Names,
                             supplier.PhoneNo, receipt.TDate);
            }

            foreach (var agReceipt in receipts)
            {
                sb.AppendFormat(@"
                    <tr>
                        <td>{0}</td>
                        <td>{1}</td>
                    </tr>
                    ",
                agReceipt.Remarks, agReceipt.Amount);
            }
            sb.AppendFormat(@"
                    <tr>
                        <td>Served By:</td>
                        <td>{0}</td>
                    </tr>
                        <td>Powered By:</td>
                        <td>Amtech Technologies LTD</td>
                    </tr>
                    ",
                 receipt.UserId);
            sb.Append(@"
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateTIntakesHtml(IEnumerable<DTransporter> transporterobj, DCompany company, string title, IQueryable<ProductIntake> intakes)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0}</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>TransCode</th>
                                                <th>Name</th>
                                                <th>Date</th>
                                                <th>Product Type</th>
                                                <th>SNO</th>
                                                <th>Qsupplied</th>
                                                <th>Price</th>
                                                <th>Description</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var transporter in transporterobj)
            {
                if (intakes.Any())
                {
                    var transpoterIntakes = intakes.Where(i => i.Sno == transporter.TransCode);
                    foreach(var intake in transpoterIntakes)
                    {
                        var intake1 = intakes.FirstOrDefault(i => i.TransDate == intake.TransDate && i.TransTime == intake.TransTime && i.Sno != intake.Sno);
                        sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                            </tr>
                            ",
                              transporter.TransCode, transporter.TransName, intake.TransDate, intake.ProductType,
                              intake1.Sno, intake.Qsupplied, intake.Ppu, intake.Description);
                    }
                }
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GeneratetransportersHtml(IEnumerable<DTransporter> transporterobj, DCompany company, string title)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0}</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>TransCode</th>
                                                <th>Name</th>
                                                <th>RegDate</th>
                                                <th>IDNo</th>
                                                <th>Phone</th>
                                                <th>Bank</th>
                                                <th>AccNo</th>
                                                <th>Branch</th>
                                                <th>Active</th>
                                                <th>Payment Mode</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var transporter in transporterobj)
            {
                sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                                <td>{8}</td>
                                <td>{9}</td>
                            </tr>
                            ",
                              transporter.TransCode, transporter.TransName, transporter.TregDate, transporter.CertNo,
                              transporter.Phoneno, transporter.Bcode, transporter.Accno, transporter.Bbranch,
                              transporter.Active, transporter.PaymenMode);
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateTransportersPayrollHtml(IEnumerable<DTransportersPayRoll> transporterpayrollobj, DCompany company, string title, IQueryable<DTransporter> transporters)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);
            sb.AppendFormat(@"
                                <div class='header'><h3>{0}</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>


                                                <th>Code</th>
                                                <th>Name</th>
                                                <th>IdNo</th>
                                                <th>Kgs</th>
                                                <th>Amount</th>
                                                <th>Subsidy</th>
                                                <th>GrossPay</th>
                                                <th>Agrovet</th>
                                                <th>Shares</th>
                                                <th>Advance</th>
                                                <th>Others</th>
                                                <th>TDeductions</th>
                                                <th>NPay</th>
                                                <th>Bank</th>
                                                <th>AccountNumber</th>
                                                <th>BBranch</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var payroll in transporterpayrollobj)
            {
                var transporter = transporters.FirstOrDefault(s => s.TransCode == payroll.Code);
                var transName = transporter?.TransName ?? "";

                sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                                <td>{8}</td>
                                <td>{9}</td>
                                <td>{10}</td>
                                <td>{11}</td>
                                <td>{12}</td>
                                <td>{13}</td>
                                <td>{14}</td>
                                <td>{15}</td>
                            </tr>
                            ",
                              payroll.Code, transName, transporter.CertNo, payroll.QntySup,
                              payroll.Amnt, payroll.Subsidy, payroll.GrossPay, payroll.Agrovet,
                              payroll.Hshares, payroll.Advance, payroll.Others, payroll.Totaldeductions,
                              payroll.NetPay, payroll.BankName, payroll.AccNo, payroll.Branch);
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateBranchIntakeHtml(IEnumerable<DBranch> branchobj, DCompany company, string title, IQueryable<ProductIntake> intakes, IQueryable<DSupplier> suppliers)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0} List</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>SNo</th>
                                                <th>Name</th>
                                                <th>Date</th>
                                                <th>Product Type</th>
                                                <th>Qsupplied</th>
                                                <th>Price</th>
                                                <th>Description</th>
                                                <th>Branch</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var branch in branchobj)
            {
                intakes = intakes.Where(i => i.Branch == branch.Bname).OrderBy(i => i.Sno);
                foreach(var intake in intakes)
                {
                    var supplier = suppliers.FirstOrDefault(s => s.Sno.ToString() == intake.Sno);
                    var supplierName = supplier?.Names ?? "";
                    long.TryParse(intake.Sno, out long sno);
                    var checkifexist = suppliers.Where(u => u.Sno == sno);
                    if (checkifexist.Any())
                    {
                        sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                            </tr>
                            ",
                                  intake.Sno, supplierName, intake.TransDate, intake.ProductType,
                                  intake.Qsupplied, intake.Ppu, intake.Description, intake.Branch);
                    }
                }
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateSuppliersDeductionsHtml(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, IQueryable<DSupplier> suppliers)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0} List</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>SNo</th>
                                                <th>Name</th>
                                                <th>Date</th>
                                                <th>Product Type</th>
                                                <th>Amount</th>
                                                <th>Remarks</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);

            foreach (var intake in productIntakeobj)
            {
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToString() == intake.Sno);
                var supplierName = supplier?.Names ?? "";
                long.TryParse(intake.Sno, out long sno);
                var checkifexist = suppliers.Where(u => u.Sno == sno);
                if (checkifexist.Any())
                {
                    sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                            </tr>
                            ",
                              intake.Sno, supplierName, intake.TransDate, intake.ProductType,
                              intake.DR, intake.Remarks);
                }
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }

        public static string GenerateSuppliersHtml(IEnumerable<DSupplier> suppliersobj, DCompany company, string title)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>");

            sb.AppendFormat(@"<table>
                                <tr>
                                    <td>{0}</td>
                                </tr>
                                <tr>
                                   <td>{1}</td>
                                </tr>
                                <tr>
                                   <td>{2}</td>
                                </tr>
                                <tr>
                                   <td>{3}</td>
                                </tr>
                              </table>",
                              company.Name, company.Adress, company.Town, company.Email);

            sb.AppendFormat(@"
                                <div class='header'><h3>{0} List</h3></div><hr/>
                                    <table>
                                        <thead>
                                            <tr>
                                                <th>SNo</th>
                                                <th>Name</th>
                                                <th>RegDate</th>
                                                <th>IDNo</th>
                                                <th>Phone</th>
                                                <th>Bank</th>
                                                <th>AccNo</th>
                                                <th>Branch</th>
                                                <th>Gender</th>
                                                <th>Village</th>
                                                <th>Location</th>
                                                <th>Ward</th>
                                                <th>Sub-County</th>
                                                <th>County</th>
                                            </tr>
                                        </thead>
                                        <tbody>
            ", title);
             
            foreach (var Supplier in suppliersobj)
            {
                sb.AppendFormat(@"
                            <tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
                                <td>{8}</td>
                                <td>{9}</td>
                                <td>{10}</td>
                                <td>{11}</td>
                                <td>{12}</td>
                                <td>{13}</td>
                            </tr>
                            ",
                              Supplier.Sno, Supplier.Names, Supplier.Regdate, Supplier.IdNo,
                              Supplier.PhoneNo, Supplier.Bcode, Supplier.AccNo, Supplier.Bbranch,
                              Supplier.Type, Supplier.Village, Supplier.Location, Supplier.Division,
                              Supplier.District, Supplier.County);
            }

            sb.Append(@"
                                    </tbody>
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }
    }
}
