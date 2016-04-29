using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StockViewer.Models;
using System.IO;
using System.Data.SqlClient;

namespace StockViewer.Controllers
{
    public class StockExchangesController : Controller
    {
        private StockExchangeContext db = new StockExchangeContext();
        private static string startDate = "";
        private static string endDate = "";
        private static string dataRep = "";
        private static string message = "";
        private static bool compare = false;
        private static string _deposit ="10000";
        private static string _interest = "3";
        // GET: StockExchanges
        public ActionResult Index()
        {
            ViewBag.DataRep = dataRep;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Message = message;

            if (compare)
            {
                ViewBag.Interest = _interest;
                ViewBag.Deposit = _deposit;
                compare = false;                     
                return View("Compare",db.Stock.SqlQuery(SqlQueryBasedOnDates()));                     
            }
            else
            {
                return View(db.Stock.SqlQuery(SqlQueryBasedOnDates()));
            }
        }

        private string SqlQueryBasedOnDates()
        {
            if (startDate == string.Empty && endDate != string.Empty)
                return "SELECT * FROM dbo.StockExchanges WHERE Date <='" + endDate + "' order by Date ASC";
            else if (startDate != string.Empty && endDate == string.Empty)
                return "SELECT * FROM dbo.StockExchanges WHERE Date >='" + startDate + "'order by Date ASC";
            else if (startDate != string.Empty && endDate != string.Empty)
                return "SELECT * FROM dbo.StockExchanges WHERE Date >='" + startDate + "' AND DATE<='" + endDate + "'order by Date ASC";
            else
                return "SELECT * FROM dbo.StockExchanges order by Date ASC";
        }
        // GET: StockExchanges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockExchange stockExchange = db.Stock.Find(id);
            if (stockExchange == null)
            {
                return HttpNotFound();
            }
            return View(stockExchange);
        }

        // GET: StockExchanges/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Compare(string deposit, string interest)
        {
            if (deposit != string.Empty)
                _deposit = deposit;
            if(interest != string.Empty)
                _interest = interest;
            compare = true;
            return RedirectToAction("Index");
        }

        public ActionResult ShowChart()
        {           
            dataRep = "chart";
            return RedirectToAction("Index");
        }
        public ActionResult ShowTable()
        {
            dataRep= "table";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ResetDate()
        {
            startDate = "";
            endDate = "";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ResetDateCompare()
        {
            compare = true;
            startDate = "";
            endDate = "";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            //ViewBag.Message = "";
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
            file.SaveAs(path);
            if (file.ContentLength > 0 && Path.GetExtension(path) == ".csv")
            {
                var dataTable = new DataTable();
                try
                {
                    dataTable = GetDataTableFromFile(path);
                }
                catch (Exception exc)
                {
                    ViewBag.Error = exc.Message;
                    return View("Error");
                }
                try
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.StockExchanges");
                }
                catch (Exception exc)
                {
                    ViewBag.Error = exc.Message;
                    return View("Error");
                }

                using (SqlConnection conn = (SqlConnection)db.Database.Connection)
                {
                    var bulkCopy = new SqlBulkCopy(conn) { DestinationTableName = "dbo.StockExchanges" };
                    try
                    {
                        conn.Open();
                        bulkCopy.WriteToServer(dataTable);
                    }
                    catch (Exception exc)
                    {
                        ViewBag.Error = exc.Message;
                        return View("Error");
                    }
                    finally
                    {
                        bulkCopy.Close();
                        conn.Close();
                        conn.Dispose();
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                }
                startDate = "";
                endDate = "";
            }
            else
            {
                ViewBag.Error = "File extenstion is different than .csv or the file is empty.";
                return View("Error");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Refresh(string start, string end)
        {
            ValidateDate(start, end);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RefreshCompare(string start, string end)
        {
            ValidateDate(start, end);
            compare = true;
            return RedirectToAction("Index");
        }

        private bool CheckDateFormat(string date)
        {
            if (int.Parse(date.Substring(4, 2)) > 12 || int.Parse(date.Substring(6, 2)) > 31)
                return false;
            else
                return true;
        }

        public ActionResult UploadFile()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.StockExchanges");
            return RedirectToAction("Index");
        }

        // POST: StockExchanges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,StockValue")] StockExchange stockExchange)
        {
            if (ModelState.IsValid)
            {
                db.Stock.Add(stockExchange);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockExchange);
        }

        // GET: StockExchanges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockExchange stockExchange = db.Stock.Find(id);
            if (stockExchange == null)
            {
                return HttpNotFound();
            }
            return View(stockExchange);
        }

        // POST: StockExchanges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,StockValue")] StockExchange stockExchange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockExchange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockExchange);
        }

        // GET: StockExchanges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockExchange stockExchange = db.Stock.Find(id);
            if (stockExchange == null)
            {
                return HttpNotFound();
            }
            return View(stockExchange);
        }

        // POST: StockExchanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockExchange stockExchange = db.Stock.Find(id);
            db.Stock.Remove(stockExchange);
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

        private void ValidateDate(string start, string end)
        {
            if(start ==string.Empty && end == string.Empty)
            {
                return;
            }
            else
            {
                startDate = "";
                endDate = "";
                if(start != string.Empty)
                {
                    startDate = start.Replace("-", "");
                    if (!CheckDateFormat(startDate))
                    {
                        startDate = "";
                        message = "Wrong start date format. Date should be in format yyyy-mm-dd or yyyymmdd";                        
                    }
                    else
                    {
                        message = "";
                    }
                }
                if(end!=string.Empty)
                {
                    endDate = end.Replace("-", "");
                    if (!CheckDateFormat(endDate))
                    {
                        endDate = "";
                        message = "Wrong end date format. Date should be in format yyyy-mm-dd or yyyymmdd";
                        return;
                    }
                    else
                    {
                        message = "";
                    }
                }                
            }            
        }


        private DataTable GetDataTableFromFile(string filePath)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("StockValue", typeof(float));
            string[] fileLines;
            fileLines = System.IO.File.ReadAllLines(filePath);

            foreach (string line in fileLines)
            {
                string[] columns = line.Split(',');
                DataRow dtRow = dataTable.NewRow();
                dtRow["ID"] = columns[0];
                dtRow["Date"] = DateTime.ParseExact(columns[1], "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                dtRow["StockValue"] = float.Parse(columns[2], System.Globalization.CultureInfo.InvariantCulture);
                dataTable.Rows.Add(dtRow);
            }
            return dataTable;
        }
    }
}
