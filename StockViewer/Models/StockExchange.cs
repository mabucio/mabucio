using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace StockViewer.Models
{
    public class StockExchange
    {       
        public int ID { get; set; }   
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]              
        public DateTime Date { get;set; }
        public float StockValue { get; set; }
    }
    
    public class StockExchangeContext : DbContext
    {
        public DbSet<StockExchange> Stock { get; set; }        
    }
}