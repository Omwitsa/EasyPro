#pragma checksum "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "451a894417c6da31cacd213045c5dee992e6ed11"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_DTransporters_Index), @"mvc.1.0.view", @"/Views/DTransporters/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"451a894417c6da31cacd213045c5dee992e6ed11", @"/Views/DTransporters/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bba33f867efe45290b68e19f5c237a35334c4540", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_DTransporters_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<EasyPro.Models.DTransporter>>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Create", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn-warning btn-success"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 3 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
  
    ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""row"">
    <div class=""col-lg-12 col-md-12 col-sm-12 col-xs-12"">
        <div class=""normal-table-list mg-t-30"">
            <div class=""row"">
                <div class=""col-lg-2 col-md-2 col-sm-2 col-xs-2"">
                    <h3 class=""text-danger fa-bold"">Transporters</h3>
                </div>
                <div class=""col-lg-7 col-md-7 col-sm-7 col-xs-7"">

                </div>
                <div class=""col-lg-3 col-md-3 col-sm-3 col-xs-3"">
                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "451a894417c6da31cacd213045c5dee992e6ed115152", async() => {
                WriteLiteral("Create New");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                </div>
            </div>
            <div class=""bsc-tbl-hvr table-border-style"">
                <div class=""table-responsive"">
                    <table class=""table table-hover"">
                        <thead>
                            <tr>
                                <th>
                                    ");
#nullable restore
#line 27 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 30 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.TransName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 33 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.CertNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 36 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.Phoneno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 39 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 42 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.Accno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 45 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th>\r\n                                    ");
#nullable restore
#line 48 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                               Write(Html.DisplayNameFor(model => model.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                </th>\r\n                                <th></th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n");
#nullable restore
#line 54 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                             foreach (var item in Model)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <tr>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 58 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.TransCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 61 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.TransName));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 64 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.CertNo));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 67 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.Phoneno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 70 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.Bcode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 73 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.Accno));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 76 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.Bbranch));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td>\r\n                                        ");
#nullable restore
#line 79 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                   Write(Html.DisplayFor(modelItem => item.Active));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                    </td>\r\n                                    <td class=\"col-4\">\r\n                                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "451a894417c6da31cacd213045c5dee992e6ed1113405", async() => {
                WriteLiteral("Edit");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 82 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                                                                                               WriteLiteral(item.Id);

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
            WriteLiteral(" |\r\n");
            WriteLiteral("                                        <input type=\"button\" class=\"btn-danger btn-danger\" id=\"delete\"");
            BeginWriteAttribute("onClick", " onClick=\"", 4249, "\"", 4277, 3);
            WriteAttributeValue("", 4259, "Delete(\'", 4259, 8, true);
#nullable restore
#line 84 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
WriteAttributeValue("", 4267, item.Id, 4267, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 4275, "\')", 4275, 2, true);
            EndWriteAttribute();
            WriteLiteral(" value=\"Delete\"/>\r\n                                    </td>\r\n                                </tr>\r\n");
#nullable restore
#line 87 "D:\EASY 2022\EasyPro\EasyPro\Views\DTransporters\Index.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </tbody>\r\n                    </table>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n");
            DefineSection("scripts", async() => {
                WriteLiteral(@"
    <script>
        function Delete() {
            swal({
                
                title: ""Are you sure?"",
                text: ""You won't be able to revert this!"",
                icon: ""warning"",
                buttons: true,
                dangerMode: true
            }).then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: ""/Delete"",
                        data: { ""itemId"": item.Id },
                        type: ""POST"",
                        success: function (data) {
                            if (data.success) {
                                //success notification
                                toastr.success(data.message);
                                //dataTable.ajax.reload();
                            }
                            else {
                                //failsure notification
                                toastr.error(data.message);
                            }
      ");
                WriteLiteral("                  }\r\n                    });\r\n                }\r\n            });\r\n\r\n        }\r\n    </script>\r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<EasyPro.Models.DTransporter>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
