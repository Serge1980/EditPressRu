using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace EditPressRu.Areas.Adminka.Models
{
    public class GiftStockXml
    {
        ////////////****************Stock****************************///////

        [XmlRoot(ElementName = "stock")]
        public class Stock
        {
            [XmlElement(ElementName = "product_id")]
            public string Product_id { get; set; }
            [XmlElement(ElementName = "code")]
            public string Code { get; set; }
            [XmlElement(ElementName = "amount")]
            public string Amount { get; set; }
            [XmlElement(ElementName = "free")]
            public string Free { get; set; }
            [XmlElement(ElementName = "inwayamount")]
            public string Inwayamount { get; set; }
            [XmlElement(ElementName = "inwayfree")]
            public string Inwayfree { get; set; }
            [XmlElement(ElementName = "dealerprice")]
            public string Dealerprice { get; set; }
            [XmlElement(ElementName = "enduserprice")]
            public string Enduserprice { get; set; }
        }

        [XmlRoot(ElementName = "doct")]
        public class DoctStock
        {
            [XmlElement(ElementName = "stock")]
            public List<Stock> Stock { get; set; }
            [XmlAttribute(AttributeName = "timestamp")]
            public string Timestamp { get; set; }
        }
    }
}