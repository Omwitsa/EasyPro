using EasyPro.ViewModels;
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
    }
}
