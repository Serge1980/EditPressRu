using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EditPressRu.Areas.Adminka.Models
{
    //class Tovar
    //**********************************************************************


    //Класс для десериализации
    public class OasisTovarJson
    {
        public string id { get; set; }
        public List<int> full_categories { get; set; }
        public string article { get; set; }
        public string parent_color_id { get; set; }
        public string parent_size_id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string size { get; set; }
        public string price { get; set; }
        public string discount_group_id { get; set; }
        public List<int> categories { get; set; }
        public string brand_id { get; set; }
        public string kit_id { get; set; }
        public bool is_virtual { get; set; }
        public int? rating { get; set; }
        public string description { get; set; }
        public List<string> materials { get; set; }
        public string brand { get; set; }
        public string branding { get; set; }
        public string cdr { get; set; }
        public string group_id { get; set; }
        public string video_id { get; set; }
        public bool is_export_allowed { get; set; }
        public bool is_visible { get; set; }
        public string color_group_id { get; set; }
        public string parent_volume_id { get; set; }
        public string parent_gender_id { get; set; }
        public bool is_on_order { get; set; }
        public string old_price { get; set; }
        public bool is_high { get; set; }
        public int? size_sort { get; set; }
        public string parent_id { get; set; }
        public int? type_id { get; set; }
        //public string branding_option { get; set; }
        public string article_base { get; set; }
        public bool is_vip { get; set; }
        public int? provider_type_id { get; set; }
        public bool? is_stopped { get; set; }
        public string main_category { get; set; }
        public string discount_price { get; set; }
        public List<OasisAttributes> attributes { get; set; }
        public List<OasisPacks> package { get; set; }
        public List<OasisColor> colors { get; set; }
        public List<OasisImage> images { get; set; }

    }

    public class OasisImage
    {
        public string big { get; set; }
        public string small { get; set; }
        public string superbig { get; set; }
        public string thumbnail { get; set; }
        public int? updated_at { get; set; }
    }

    public class OasisColor
    {
        public string name { get; set; }
        public int sort { get; set; }
        public string pantone { get; set; }
    }


    public class OasisPacks
    {
        public string id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string size { get; set; }
        public string weight { get; set; }
        public string amount { get; set; }
        public string volume { get; set; }
        public bool is_main { get; set; }
               
    }

    public class OasisAttributes
    {
        public string dim { get; set; }
        public string name { get; set; }
        public string value { get; set; }               
    }
}