#pragma checksum "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fa9d820e7f4f5e61ee78ee7ffe721631f035cde3"
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
#line 1 "D:\EASY 2022\EasyPro\EasyPro\Views\_ViewImports.cshtml"
using EasyPro;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\EASY 2022\EasyPro\EasyPro\Views\_ViewImports.cshtml"
using EasyPro.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fa9d820e7f4f5e61ee78ee7ffe721631f035cde3", @"/Views/DSuppliers/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bba33f867efe45290b68e19f5c237a35334c4540", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_DSuppliers_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<EasyPro.Models.DSupplier>
    #nullable disable
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
#line 3 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
  
    ViewData["Title"] = "Details";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Details</h2>\r\n\r\n<div>\r\n    <h4>DSupplier</h4>\r\n    <hr />\r\n    <dl class=\"dl-horizontal\">\r\n        <dt>\r\n            ");
#nullable restore
#line 14 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.LocalId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 17 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.LocalId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 20 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Sno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 23 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Sno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 26 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Regdate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 29 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Regdate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 32 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.IdNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 35 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.IdNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 38 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Names));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 41 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Names));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 44 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.AccNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 47 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.AccNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 50 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 53 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 56 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 59 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 62 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 65 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Type));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 68 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Village));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 71 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Village));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 74 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Location));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 77 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Location));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 80 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Division));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 83 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Division));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 86 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.District));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 89 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.District));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 92 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Trader));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 95 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Trader));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 98 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 101 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 104 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Approval));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 107 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Approval));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 110 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Branch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 113 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Branch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 116 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.PhoneNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 119 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.PhoneNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 122 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Address));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 125 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Address));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 128 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Town));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 131 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Town));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 134 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 137 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 140 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 143 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 146 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Sign));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 149 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Sign));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 152 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Photo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 155 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Photo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 158 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.AuditId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 161 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.AuditId));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 164 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Auditdatetime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 167 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Auditdatetime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 170 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Scode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 173 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Scode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 176 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Loan));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 179 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Loan));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 182 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Compare));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 185 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Compare));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 188 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Isfrate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 191 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Isfrate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 194 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Frate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 197 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Frate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 200 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Rate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 203 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Rate));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 206 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Hast));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 209 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Hast));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 212 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Br));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 215 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Br));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 218 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Mno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 221 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Mno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 224 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Branchcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 227 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Branchcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 230 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.HasNursery));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 233 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.HasNursery));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 236 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Notrees));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 239 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Notrees));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 242 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Aarno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 245 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Aarno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 248 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Tmd));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 251 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Tmd));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 254 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Landsize));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 257 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Landsize));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 260 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Thcpactive));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 263 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Thcpactive));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 266 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Thcppremium));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 269 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Thcppremium));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 272 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 275 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 278 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status2));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 281 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status2));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 284 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status3));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 287 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status3));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 290 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status4));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 293 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status4));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 296 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status5));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 299 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status5));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 302 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status6));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 305 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status6));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 308 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Types));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 311 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Types));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 314 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Dob));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 317 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Dob));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 320 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Freezed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 323 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Freezed));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 326 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Mass));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 329 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Mass));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 332 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Status1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 335 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Status1));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt>\r\n            ");
#nullable restore
#line 338 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Run));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd>\r\n            ");
#nullable restore
#line 341 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
       Write(Html.DisplayFor(model => model.Run));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n    </dl>\r\n</div>\r\n<div>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa9d820e7f4f5e61ee78ee7ffe721631f035cde334834", async() => {
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
#line 346 "D:\EASY 2022\EasyPro\EasyPro\Views\DSuppliers\Details.cshtml"
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fa9d820e7f4f5e61ee78ee7ffe721631f035cde336959", async() => {
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
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<EasyPro.Models.DSupplier> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
