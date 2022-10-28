using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Homework_Assignment_6.Models;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace Homework_Assignment_6.Controllers
{
    public class productsController : Controller
    {
        private BikeStoresEntities1 db = new BikeStoresEntities1();

        // GET: products
        public ActionResult Index()
        {
          
            var products = db.products.Include(p => p.brand).Include(p => p.category);
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View(products.ToList());
        }
        public ActionResult Save(product product)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
               db.products.Add(product);
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }
        //test

        //end
        public JsonResult GetProducts()
        {
            var products = db.products.Include(p => p.brand).Include(p => p.category);
            products.ToList();
            //var jsonResult = Json(products, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult getProduct(int Id)
        {
            var products = db.products.Where(a => a.product_id == Id).FirstOrDefault();
            return Json(products,JsonRequestBehavior.AllowGet);
        }
        // GET: products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: products/Create
        public ActionResult Create()
        {
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
      
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Chart()
        {
            // const labels = Utils.months({ count: 7});
            var products = db.products.Where(a => a.category_id == 6).ToList();
            int products2016 = 0;
            int products2017 = 0;
            int products2018 = 0;
            int products2019 = 0;
            if (products.Count > 0)
            {
                string cData = "";
                string Views = "";
                string labels = "";
                cData += "<script>";
                foreach (var product in products)
                {
                    Views += product + ",";
                    labels += product.model_year + ",";
                    if (product.model_year == 2016)
                    {
                        products2016++;
                    }
                    if (product.model_year == 2017)
                    {
                        products2017++;
                    }
                    if (product.model_year == 2018)
                    {
                        products2018++;
                    }
                    if (product.model_year == 2019)
                    {
                        products2019++;
                    }
                }
                cData += "chartLables = ["+labels+ "]; chartData = ["+ Views + "];";
                cData += "</script>";
                string Datachart = "["+products2016+ ","+products2017+ ","+products2018+ ","+products2019+"]";
                ViewBag.Data = Datachart;
                ViewData["data"] = Datachart;
                ViewData["labels"] = @"[2016,2017,2018,2019]";
            }
           // ViewData["data"] = @"[{'count':42,'datetime':'2020-07-18T00:00:00'},{'count':49,'datetime':'2020-07-16T00:00:00'},{'count':90,'datetime':'2020-07-14T00:00:00'},{'count':85,'datetime':'2020-07-17T00:00:00'},{'count':100,'datetime':'2020-07-13T00:00:00'},{'count':38,'datetime':'2020-07-19T00:00:00'},{'count':53,'datetime':'2020-07-15T00:00:00'}]";
            return View();
        }
            [HttpPost]
        public JsonResult NewChart()
        {
            List<product> xy = new List<product>();
           
           var products =  db.products.ToList();
            //Looping and extracting each DataColumn to List<Object>  
            foreach (product dc in products)
            {
                List<product> x = new List<product>();
                x = db.products.Where(a => a.product_name == "Mountain Bikes").ToList();
                
                //x = (from product drr in dt.Rows select drr[dc.product_name]).ToList()

            }
            xy = db.products.Where(a => a.product_name == "Mountain Bikes").ToList();
            //Source data returned as JSON  

            return Json(xy, JsonRequestBehavior.AllowGet);
        }
    }
}
