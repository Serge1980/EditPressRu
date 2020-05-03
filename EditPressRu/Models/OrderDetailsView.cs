using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class OrderDetailsView
    {
        public List<OrderDetailsKucha> OrderDetail { get; set; }
        public int TotalItems { get; set; }
        public string PriznakZakaz { get; set; }
        public string  Message { get; set; }
        public bool ModelIsEmpty { get; set; }
        public string DayCnt { get; set; }
    }

    public class OrderDetailsKucha
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public string Articul { get; set; }
        public string Img { get; set; }
        public int Cnt { get; set; }
        public int OrderDetailsId { get; set; }
        public int ProductId { get; set; }
        public string ProdName { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ItemSumm { get; set; }
        public decimal TotalSummOrder { get; set; }
        public DateTime DateOrder { get; set; }
        public int MoskShipmnt { get; set; }
        public string AdrShipmnt { get; set; }

    }
}