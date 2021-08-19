using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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

        public ActionResult Test1(RegistersCriteria inject, CashRelateVM injectmoney, DateTime? Date123 )
        {
            #region 日期
            
            int a;
            if (Date123!=null)
            {
                int sd = 0;
                DateTime Date = Convert.ToDateTime(Date123);
                string SDate = Date.ToString("yyyy-MM-dd");
                List<stockprice> tempd = db.stockprices.Where(x => x.年月日 == SDate).ToList();//問助教好了
                stockprice StartDate = tempd[0];
                sd = StartDate.id;
                inject.StartDate = sd;
            }
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
            List<int> Id = dball.Where(x => x.id <= inject.StartDate + a && x.id>= inject.StartDate).Select(x => x.id).ToList();
            //List<DateTime?> time = dball.Where(x => x.id <= a).Select(x => x.年月日).ToList();
            List<string> test = dball.Where(x => x.id <= inject.StartDate + a && x.id >= inject.StartDate).Select(x => x.年月日).ToList();
            List<DateTime> time2 = dball.Where(x => x.id <= inject.StartDate + a && x.id >= inject.StartDate).Select(x => Convert.ToDateTime(x.年月日)).ToList();//Datetime?沒有string多載來做日期格式，所以強制轉型
            List<string> time3 = time2.Select(x => x.ToString("M/dd")).ToList();
            List<double?> price = dball.Where(x => x.id <= inject.StartDate + a && x.id >= inject.StartDate).Select(x => x.收盤價_元_).ToList();

            ViewBag.Time3 = time3;
            ViewBag.Total = a;
            ViewBag.StartDate = inject.StartDate;

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
            #endregion
            customer customer;
            #region 錢錢
            if (injectmoney.Cash > 0)//有資料
            {
                customer = db.customers.Find(injectmoney.Cid);
                if(customer.Position>0)//檢查部位
                {
                    customer.Profit = Convert.ToInt32(price[price.Count() - 1]*1000) - customer.BuyCost;
                }
                else if(customer.Position < 0)
                {
                    customer.Profit = customer.BuyCost-Convert.ToInt32(price[price.Count() - 1] * 1000) ;

                }
                else
                {
                    customer.Profit = 0;
                }
                if (injectmoney.Status == 1)//檢查策略
                {
                    if (customer.Position >= 0)//做多開倉
                    {
                        
                        customer.Position += 1;
                        customer.Status = 0;
                        customer.BuyCost = Convert.ToInt32(price[price.Count() - 1]*1000);//索引是從0開始所以要-1
                        customer.Cash -= customer.BuyCost;
                    }
                    else//放空平倉
                    {
                        customer.Position += 1;
                        customer.Status = 0;
                        customer.BuyCost = null;
                        customer.Cash += injectmoney.BuyCost + customer.Profit;
                        //customer.Profit = null;
                    }

                }
                else if(injectmoney.Status == -1)
                {
                    if(customer.Position <= 0)//做空開倉
                    {
                        
                        customer.Position -= 1;
                        customer.Status = 0;
                        customer.BuyCost = Convert.ToInt32(price[price.Count() - 1]*1000);
                        customer.Cash -= customer.BuyCost;
                    }
                    else//做多平倉
                    {
                        customer.Position -= 1;
                        customer.Status = 0;
                        customer.BuyCost = null;
                        customer.Cash += Convert.ToInt32(price[price.Count() - 1] * 1000);
                        //customer.Profit = null;
                    }

                }
               
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
            else//沒資料
            {
                db.customers.Add(new customer { Cash = 100000,Position=0,Profit=0,Status = 0});
                db.SaveChanges();
                int id = db.customers.Select(x=>x.id).Max();
                customer = db.customers.Find(id);

            }

            ViewBag.Cid = customer.id;
            ViewBag.Cash = customer.Cash;
            ViewBag.Position = customer.Position;
            ViewBag.Profit = customer.Profit;
            ViewBag.Status = customer.Status;
            ViewBag.BuyCost = customer.BuyCost;

            #endregion
            return View(combine);

        }
        
        public ActionResult Test2()
        {
            return View();
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
