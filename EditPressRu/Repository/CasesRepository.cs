using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EditPressRu.Repository
{
    public partial class DataRepository
    {

        public virtual List<string> GetImgFiles()
        {
            List<string> listFiles = new List<string>();
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/Cases"); //Server.MapPath("~/App_Data/somedata.xml");
            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            //FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            //myImage.ImageUrl = ResolveUrl(this.ImageUrl);
            foreach (FileInfo file in Files)
            {
                listFiles.Add(String.Format("/Images/Cases/{0}", file.Name));
            }
            return listFiles;
        }
    }
}



