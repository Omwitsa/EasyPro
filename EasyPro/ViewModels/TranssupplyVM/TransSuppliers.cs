﻿using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.TranssupplyVM
{
    public class TransSuppliers
    {
        public DTransport DTransport { get; set; }
        public AgReceipt AgReceipt { get; set; }
        public IEnumerable<DTransporter> DTransporter { get; set; }
        public IEnumerable<DSupplier> DSuppliers { get; set; }

    }
}
