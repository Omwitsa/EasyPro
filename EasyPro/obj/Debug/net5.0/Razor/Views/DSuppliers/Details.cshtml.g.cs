#pragma checksum "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fa9d820e7f4f5e61ee78ee7ffe721631f035cde3"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_DSuppliers_Details), @"mvc.1.0.view", @"/Views/DSuppliers/Details.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\EASYPRO\EasyPro\EasyPro\Views\_ViewImports.cshtml"
using EasyPro;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\EASYPRO\EasyPro\EasyPro\Views\_ViewImports.cshtml"
using EasyPro.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fa9d820e7f4f5e61ee78ee7ffe721631f035cde3", @"/Views/DSuppliers/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bba33f867efe45290b68e19f5c237a35334c4540", @"/Views/_ViewImports.cshtml")]
    public class Views_DSuppliers_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<EasyPro.Models.DSupplier>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
  
    ViewData["Title"] = "Details";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Details</h2>\r\n\r\n<div>\r\n    <h4>DSupplier</h4>\r\n    <hr />\r\n    <dl class=\"dl-horizontal\">\r\n        <dt>\r\n            ");
#nullable restore
#line 14 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.LocalId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 17 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.LocalId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 20 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Sno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 23 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Sno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 26 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Regdate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 29 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Regdate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 32 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.IdNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 35 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.IdNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 38 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Names));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 41 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Names));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 44 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.AccNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 47 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.AccNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 50 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 53 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 56 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 59 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 62 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 65 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 68 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Village));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 71 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Village));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 74 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Location));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 77 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Location));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 80 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Division));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 83 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Division));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 86 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.District));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 89 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.District));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 92 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Trader));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 95 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Trader));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 98 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 101 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 104 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Approval));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 107 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Approval));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 110 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Branch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 113 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Branch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 116 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.PhoneNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 119 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.PhoneNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 122 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Address));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 125 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Address));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 128 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Town));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 131 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Town));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 134 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 137 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 140 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 143 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 146 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Sign));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 149 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Sign));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 152 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Photo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 155 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Photo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 158 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.AuditId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 161 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.AuditId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 164 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Auditdatetime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 167 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Auditdatetime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 170 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Scode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 173 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Scode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 176 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Loan));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 179 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Loan));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 182 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Compare));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 185 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Compare));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 188 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Isfrate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 191 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Isfrate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 194 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Frate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 197 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Frate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 200 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Rate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 203 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Rate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 206 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Hast));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 209 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Hast));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 212 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Br));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 215 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Br));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 218 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Mno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 221 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Mno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 224 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Branchcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 227 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Branchcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 230 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.HasNursery));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 233 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.HasNursery));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 236 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Notrees));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 239 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Notrees));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 242 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Aarno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 245 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Aarno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 248 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Tmd));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 251 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Tmd));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 254 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Landsize));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 257 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Landsize));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 260 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Thcpactive));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 263 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Thcpactive));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 266 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Thcppremium));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 269 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Thcppremium));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 272 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 275 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 278 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status2));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 281 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status2));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 284 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status3));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 287 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status3));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 290 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status4));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 293 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status4));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 296 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status5));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 299 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status5));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 302 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status6));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 305 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status6));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 308 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Types));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 311 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Types));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 314 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Dob));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 317 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Dob));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 320 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Freezed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 323 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Freezed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 326 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Mass));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 329 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Mass));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 332 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 335 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 338 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Run));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 341 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Run));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n    </dl>\r\n</div>\r\n<div>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa9d820e7f4f5e61ee78ee7ffe721631f035cde334560", async() => {
                WriteLiteral("Edit");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 346 "C:\EASYPRO\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
                           WriteLiteral(Model.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(" |\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa9d820e7f4f5e61ee78ee7ffe721631f035cde336683", async() => {
                WriteLiteral("Back to List");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<EasyPro.Models.DSupplier> Html { get; private set; }
    }
}
#pragma warning restore 1591
