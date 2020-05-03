using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;

namespace EditPressRu.Models
{
    public class KPlistViewModel
    {
        public bool Availability { get; set; }
        public List<KpStoreView> KPstoreList { get; set; }
    }
}

public class KpStoreView
{
    public int KpId { get; set; }
    public int TotalItem { get; set; }
    public string KpImg { get; set; }
    public DateTime DateDoc { get; set; }
    public string KpNumber { get; set; }

}