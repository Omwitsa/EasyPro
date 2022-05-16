using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace EasyPro.Controllers
{
    public class DPricesController : Controller
    {
        private readonly MORINGAContext _context;

        public DPricesController(MORINGAContext context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;
        //public DPricesController(IConfiguration config)
        //{
        //    this.configuration = config;
        //}

        // GET: DPrices
        public async Task<IActionResult> Index()
        {
            return View(await _context.DPrices.ToListAsync());
        }
        //public IActionResult Index()
        //{
           
        //    //DPrice dPricemodel = new DPrice();
        //    //using (MORINGAContext db = new MORINGAContext())
        //    //{
        //    //    dPricemodel.Productcollection = db.DBranchProducts.ToList<DBranchProduct>();
        //    //}
           
        //    //return View(dPricemodel);
        //}
        private List<SelectListItem> Populateproducts()
        {
            List<SelectListItem> items = new();
           // string constr = ConfigurationManager.ConnectionStrings["MoringaDbConnection"].ConnectionString;
            //string constr2 = "Data Source = BII - PC; Initial Catalog = MORINGA; User ID = sasa; Password = 'sasa';";
            using (SqlConnection con = new SqlConnection("Data Source=BII-PC;Initial Catalog=MORINGA;Integrated Security=false;User ID=sasa;Password='sasa'"))
            {
                string query = "Select Bname from d_BranchProduct order by Bname";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["Bname"].ToString(),
                                //Value = sdr["FruitId"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        //private List<SelectListItem> Populateproducts()
        //{
        //    List<SelectListItem> items = new();
        //    string constr = "Data Source = BII - PC; Initial Catalog = MORINGA; User ID = sasa; Password = 'sasa';";
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        string query = "Select Bname from d_BranchProduct order by Bname";
        //        using (SqlCommand cmd = new SqlCommand(query))
        //        {
        //            cmd.Connection = con;
        //            con.Open();
        //            using (SqlDataReader sdr = cmd.ExecuteReader())
        //            {
        //                while (sdr.Read())
        //                {
        //                    items.Add(new SelectListItem
        //                    {
        //                        Text = sdr["Bname"].ToString(),
        //                        //Value = sdr["FruitId"].ToString()
        //                    });
        //                }
        //            }
        //            con.Close();
        //        }
        //    }

        //    return items;
        //}

        // GET: DPrices/Details/5
        public async Task<IActionResult> Details(string product)
        {
            if (product == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == product);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // GET: DPrices/Create
        public IActionResult Create()
        {
            DPrice products = new DPrice();
            products.Productt = Populateproducts();
            return View(products);
            //return View();
        }

        // POST: DPrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Edate,Price,Products")] DPrice dPrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice);
        }

        // GET: DPrices/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices.FindAsync(id);
            if (dPrice == null)
            {
                return NotFound();
            }
            return View(dPrice);
        }

        // POST: DPrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string product, [Bind("Id,Edate,Price,Products")] DPrice dPrice)
        {
            if (product != dPrice.Products)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dPrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPriceExists(dPrice.Products))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice);
        }

        // GET: DPrices/Delete/5
        public async Task<IActionResult> Delete(string product)
        {
            if (product == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == product);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // POST: DPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dPrice = await _context.DPrices.FindAsync(id);
            _context.DPrices.Remove(dPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPriceExists(string product)
        {
            return _context.DPrices.Any(e => e.Products == product);
        }
    }
}
