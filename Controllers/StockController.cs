using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FirstTrade_.Models.EFModels;
using FirstTrade_.Models.Services;

namespace FirstTrade_.Controllers
{
    public class StockController : Controller
    {
        private Model1 db = new Model1();

        // GET: Stock
        public ActionResult Index()
        {
            return View(db.stockprices.ToList());
        }

        public ActionResult Test1(RegistersCriteria inject)
        {
            int a;
            if (inject.Total > 0)
            {
                if (inject.Name > 0)
                {
                    a = inject.Total + inject.Name;
                }
                else
                {
                    a = inject.Total + 1;
                }
            }
            else
            {
                a = 10;
            }
            var dball = db.stockprices.ToList();
            List<int> Id = dball.Where(x => x.id <= a).Select(x => x.id).ToList();
            List<DateTime?> time = dball.Where(x => x.id <= a).Select(x => x.年月日).ToList();
            List<double?> price = dball.Where(x => x.id <= a).Select(x => x.收盤價_元_).ToList();


            //ViewBag.MonthLabel = time;
            ViewBag.MonthLabel = Id;
            ViewBag.Criteria = inject;
            ViewBag.Total = a;

            List<StockVM2> combine = new List<StockVM2>
            {
                new StockVM2
                {
                    dev = "台積電2330",
                    price = new List<double?>()
                },
            };
            foreach (double? item in price)
            {
                combine[0].price.Add(item);
            }

            combine[0].UpL = combine[0].price.Max();
            combine[0].DownL = combine[0].price.Min();
            combine[0].count = a;

            return View(combine);

        }

        // GET: Stock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stockprice stockprice = db.stockprices.Find(id);
            if (stockprice == null)
            {
                return HttpNotFound();
            }
            return View(stockprice);
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stock/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,年月日,收盤價_元_")] stockprice stockprice)
        {
            if (ModelState.IsValid)
            {
                db.stockprices.Add(stockprice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockprice);
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stockprice stockprice = db.stockprices.Find(id);
            if (stockprice == null)
            {
                return HttpNotFound();
            }
            return View(stockprice);
        }

        // POST: Stock/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,年月日,收盤價_元_")] stockprice stockprice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockprice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockprice);
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            stockprice stockprice = db.stockprices.Find(id);
            if (stockprice == null)
            {
                return HttpNotFound();
            }
            return View(stockprice);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            stockprice stockprice = db.stockprices.Find(id);
            db.stockprices.Remove(stockprice);
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
    }
}
