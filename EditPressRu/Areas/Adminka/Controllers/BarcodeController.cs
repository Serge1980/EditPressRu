using EditPressRu.Areas.Adminka.Models;
using EditPressRu.Models.DB;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Areas.Adminka.Controllers
{
    public class BarcodeController : Controller
    {
        private EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Adminka/Barcode
        [HttpGet]
        public ActionResult Index()
        {
            BarcodeViewModel model = new BarcodeViewModel();
            model.NumberCode = 212040000022;
            model.CountCode = 50;
            model.PathToFile = "";

            model.BarcodeType = new List<SelectListItem>();

            SelectListItem item1 = new SelectListItem { Text = "ean13", Value = "1" };
            SelectListItem item2 = new SelectListItem { Text = "ean8", Value = "2"};
            SelectListItem item3 = new SelectListItem { Text = "itf14", Value = "3"};

            model.SelectedType = 1;

            model.BarcodeType.Add(item1);
            model.BarcodeType.Add(item2);
            model.BarcodeType.Add(item3);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(BarcodeViewModel model)
        {
            model.BarcodeType = new List<SelectListItem>();

            SelectListItem item1 = new SelectListItem { Text = "ean13", Value = "1" };
            SelectListItem item2 = new SelectListItem { Text = "ean8", Value = "2" };
            SelectListItem item3 = new SelectListItem { Text = "itf14", Value = "3" };
            model.BarcodeType.Add(item1);
            model.BarcodeType.Add(item2);
            model.BarcodeType.Add(item3);

            string val = model.SelectedType.ToString();
            string typeBar=model.BarcodeType.FirstOrDefault(x => x.Value==val).Text;
           
            //Db.Database.ExecuteSqlCommand(String.Format("exec spaa_shtrih {0}, {1}, {2}", typeBar, model.NumberCode,model.CountCode));

            var data = Db.Database.SqlQuery<BarcodeSPrezult>(String.Format("exec spaa_shtrih {0}, {1}, {2}", typeBar, model.NumberCode, model.CountCode)).ToList();
            // Set the file name and get the output directory
            var fileName = "Штрихкоды-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xlsx";
            var outputDir = Server.MapPath("/files/barcodes/");

            model.PathToFile = String.Format("/files/barcodes/{0}", fileName);
            model.SpListRez = new List<BarcodeSPrezult>();
            model.SpListRez = data;

            // Create the file using the FileInfo object
            var file = new FileInfo(outputDir + fileName);
            using (var package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet ws = CreateSheet(package, "BarCode1");
                ws.Cells[1, 1].Value = "Номер по порядку";
                ws.Cells[1, 2].Value = "Номер штрихкода";
                ws.Cells[1, 3].Value = "Контрольное число";
                ws.Cells[1, 4].Value = "Штрихкод Ean13";
                ws.Cells[1, 5].Value = "Штрихкод Ean8";
                ws.Cells[1, 6].Value = "Штрихкод Itf14";

                ws.Cells[2, 1].LoadFromCollection(data, true);

                ws.Column(1).AutoFit();
                ws.Column(2).AutoFit();
                ws.Column(3).AutoFit();
                ws.Column(4).AutoFit();

                int rowCnt = model.CountCode + 3;
                String rangeCol1 = String.Format("B2:B{0}", rowCnt);
                String rangeCol2 = String.Format("D2:D{0}", rowCnt);

                ws.Cells[rangeCol1].Style.Numberformat.Format = "0";
                ws.Cells[rangeCol2].Style.Numberformat.Format = "0";

                package.Save();

            }
            return View(model);
        }

        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            return ws;
        }

        //public void ExportExcel_EmployeeData()
        //{
        //    var sb = new StringBuilder();
        //   var data= Db.Database.SqlQuery<BarcodeSPrezult>("exec spaa_shtrih 'itf14', 2120400000225, 20");

        //    var list = data.ToList();
        //    var grid = new System.Web.UI.WebControls.GridView();
        //    grid.DataSource = list;
        //    grid.DataBind();
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment; filename=Emp.xlsx");
        //    Response.ContentType = "application/vnd.ms-excel";
        //    StringWriter sw = new StringWriter();
        //    System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
        //    grid.RenderControl(htw);
        //    Response.Write(sw.ToString());
        //    Response.End();
        //}
    }
}