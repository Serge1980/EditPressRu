using EditPressRu.Models.DB;
using System.Linq;
using System.Collections.Generic;

namespace EditPressRu.Helpers
{
    public class FlatFileAccess
    {
        
        public static List<Redirect> PreparedKeyParLink()
        {
            EditPressRuEntities db = new EditPressRuEntities();
            List<Redirect> redirectList = db.Redirect.ToList();

            return redirectList;
        }

        
        //public static Dictionary<string, string> Read301CSV()
        //{
        //    string file = "301.csv";
        //    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), file);

        //    if (File.Exists(path))
        //    {
        //        using (TextReader sr = new StreamReader(path))
        //        {
        //            Dictionary<string, string> redirect_dict = new Dictionary<string, string>();
        //            string line = "";
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                string[] columns = line.Split(',');
        //                redirect_dict.Add(columns[0], columns[1]);
        //            }
        //            return redirect_dict;
        //        }
        //    }
        //    else
        //        return null;

        //}

    }
}