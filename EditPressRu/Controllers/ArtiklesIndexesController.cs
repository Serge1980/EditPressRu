using System.Linq;
using System.Net;
using System.Web.Mvc;
using EditPressRu.Models.DB;

namespace EditPressRu.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ArtiklesIndexesController : Controller
    {
        private EditPressRuEntities db = new EditPressRuEntities();

        // GET: ArtiklesIndexes
        public ActionResult Index()
        {
            return View(db.ArtiklesIndexes.ToList());
        }

        // GET: ArtiklesIndexes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtiklesIndexes artiklesIndex = db.ArtiklesIndexes.Find(id);
            if (artiklesIndex == null)
            {
                return HttpNotFound();
            }
            return View(artiklesIndex);
        }

        // GET: ArtiklesIndexes/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: ArtiklesIndexes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,HrefToArtikle,Publish")] ArtiklesIndexes artiklesIndex)
        {
            if (ModelState.IsValid)
            {
                db.ArtiklesIndexes.Add(artiklesIndex);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artiklesIndex);
        }

        // GET: ArtiklesIndexes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtiklesIndexes artiklesIndex = db.ArtiklesIndexes.Find(id);
            if (artiklesIndex == null)
            {
                return HttpNotFound();
            }
            return View(artiklesIndex);
        }

        // POST: ArtiklesIndexes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,HrefToArtikle,Publish")] ArtiklesIndexes artiklesIndex)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artiklesIndex).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artiklesIndex);
        }

        // GET: ArtiklesIndexes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtiklesIndexes artiklesIndex = db.ArtiklesIndexes.Find(id);
            if (artiklesIndex == null)
            {
                return HttpNotFound();
            }
            return View(artiklesIndex);
        }

        // POST: ArtiklesIndexes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArtiklesIndexes artiklesIndex = db.ArtiklesIndexes.Find(id);
            db.ArtiklesIndexes.Remove(artiklesIndex);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
