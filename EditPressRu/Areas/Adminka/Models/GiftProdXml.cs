using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace EditPressRu.Areas.Adminka.Models
{

        [XmlRoot(ElementName = "small_image")]
        public class Small_image
        {
            [XmlAttribute(AttributeName = "src")]
            public string Src { get; set; }
        }

        [XmlRoot(ElementName = "big_image")]
        public class Big_image
        {
            [XmlAttribute(AttributeName = "src")]
            public string Src { get; set; }
            [XmlAttribute(AttributeName = "deprecated")]
            public string Deprecated { get; set; }
        }

        [XmlRoot(ElementName = "super_big_image")]
        public class Super_big_image
        {
            [XmlAttribute(AttributeName = "src")]
            public string Src { get; set; }
        }

        [XmlRoot(ElementName = "status")]
        public class Status
        {
            [XmlAttribute(AttributeName = "id")]
            public int Id { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "pack")]
        public class Pack
        {
            [XmlElement(ElementName = "amount")]
            public string Amount { get; set; }
            [XmlElement(ElementName = "weight")]
            public string Weight { get; set; }
            [XmlElement(ElementName = "volume")]
            public string Volume { get; set; }
            [XmlElement(ElementName = "sizex")]
            public string Sizex { get; set; }
            [XmlElement(ElementName = "sizey")]
            public string Sizey { get; set; }
            [XmlElement(ElementName = "sizez")]
            public string Sizez { get; set; }
        }

        [XmlRoot(ElementName = "price")]
        public class Price
        {
            [XmlElement(ElementName = "price")]
            public string price { get; set; }
            [XmlElement(ElementName = "currency")]
            public string Currency { get; set; }
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
            [XmlElement(ElementName = "product")]
            public string Product { get; set; }
            [XmlElement(ElementName = "value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "product")]
        public class Product
        {
            [XmlElement(ElementName = "product_id")]
            public int Product_id { get; set; }
            [XmlElement(ElementName = "main_product")]
            public string Main_product { get; set; }
            [XmlElement(ElementName = "code")]
            public string Code { get; set; }
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
            [XmlElement(ElementName = "size_code")]
            public string Size_code { get; set; }
            [XmlElement(ElementName = "weight")]
            public string Weight { get; set; }
            [XmlElement(ElementName = "price")]
            public Price Price { get; set; }
            [XmlElement(ElementName = "group")]
            public int Group { get; set; }
            [XmlElement(ElementName = "product_size")]
            public string Product_size { get; set; }
            [XmlElement(ElementName = "matherial")]
            public string Matherial { get; set; }
            [XmlElement(ElementName = "content")]
            public string Content { get; set; }
            [XmlElement(ElementName = "brand")]
            public string Brand { get; set; }
            [XmlElement(ElementName = "small_image")]
            public Small_image Small_image { get; set; }
            [XmlElement(ElementName = "big_image")]
            public Big_image Big_image { get; set; }
            [XmlElement(ElementName = "super_big_image")]
            public Super_big_image Super_big_image { get; set; }
            [XmlElement(ElementName = "status")]
            public Status Status { get; set; }
            [XmlElement(ElementName = "pack")]
            public Pack Pack { get; set; }
            [XmlElement(ElementName = "product")]
            public List<Product> product { get; set; }
            [XmlElement(ElementName = "print")]
            public List<Print> Print { get; set; }
            [XmlElement(ElementName = "product_attachment")]
            public List<Product_attachment> Product_attachment { get; set; }
            [XmlElement(ElementName = "filters")]
            public Filters Filters { get; set; }
        }

        [XmlRoot(ElementName = "print")]
        public class Print
        {
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
            [XmlElement(ElementName = "description")]
            public string Description { get; set; }
        }

        [XmlRoot(ElementName = "product_attachment")]
        public class Product_attachment
        {
            [XmlElement(ElementName = "image")]
            public string Image { get; set; }
            [XmlElement(ElementName = "meaning")]
            public string Meaning { get; set; }
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "filter")]
        public class Filter
        {
            [XmlElement(ElementName = "filtertypeid")]
            public string Filtertypeid { get; set; }
            [XmlElement(ElementName = "filterid")]
            public string Filterid { get; set; }
        }

        [XmlRoot(ElementName = "filters")]
        public class Filters
        {
            [XmlElement(ElementName = "filter")]
            public List<Filter> Filter { get; set; }
        }

        [XmlRoot(ElementName = "doct")]
        public class Doct
        {
            [XmlElement(ElementName = "product")]
            public List<Product> Product { get; set; }
            [XmlAttribute(AttributeName = "timestamp")]
            public string Timestamp { get; set; }
        }

}

    