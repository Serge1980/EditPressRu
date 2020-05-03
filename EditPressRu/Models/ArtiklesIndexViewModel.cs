using EditPressRu.Helpers;
using EditPressRu.Models.DB;
using System.Collections.Generic;

namespace EditPressRu.Models
{
    public class ArtiklesIndexViewModel
    {
        //public CategoryesViewModel Cats { get; set; }
        //public IPagedList<ArtiklesIndex> ArtiklesIndexPL { get; set; }
        public PageInfo Paging { get; set; }
        public IEnumerable<ArtiklesIndexes> ArtiklesIndexPL { get; set; }
        public int Page { get; set; }
    }
}