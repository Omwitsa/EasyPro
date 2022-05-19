using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyPro.ViewModels.EnquiryVM
{
    public class EnquiryVM
    {
        public ProductIntakeVm intakk { get; set; }

    }
}
