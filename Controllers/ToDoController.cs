using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Infrastructure;
using ToDoList.Models;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using System.Text;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoContext context;

        public ToDoController(ToDoContext context)
        {
            this.context = context;
        }
        //Get/ == Get/todolist/all
        public async Task<ActionResult> Index()
        {
            IQueryable<TodoList> items = from i in context.ToDoLists orderby i.Id select i;

            List<TodoList> todoLists = await items.ToListAsync();

            return View(todoLists);
        }

        //Get //todolist/add
        public IActionResult Add() => View();

        //POST /todo/add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(TodoList item)
        {
            if (ModelState.IsValid)
            {
                context.Add(item);
                await context.SaveChangesAsync();

                TempData["Succes"] = "New item been addes";

                return RedirectToAction("Index");
            }

            return View(item);
        }

        //GET /todolist/id/
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "This item doasn't exist";
                return RedirectToAction("Index");
            }
            TodoList item = context.ToDoLists.Find(id);
            if (item == null)
            {
                TempData["Error"] = "This item doasn't exist";
                return RedirectToAction("Index");
            }
            return View(item);
        }


        //GET /todolist/remove
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "This item doasn't exist";
                return RedirectToAction("Index");
            }
            TodoList item = context.ToDoLists.Find(id);
            if (item == null)
            {
                TempData["Error"] = "This item doasn't exist";
                return RedirectToAction("Index");
            }

            context.ToDoLists.Remove(item);
            await context.SaveChangesAsync();
            TempData["Succes"] = "The item has been deleted";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> GenPDF()
        {
            IQueryable<TodoList> items = from i in context.ToDoLists orderby i.Id select i;

            List<TodoList> todoLists = await items.ToListAsync();
            string filename = "ToDoList.pdf";
            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XFont font1 = new XFont("Times", 12, (XFontStyle)FontStyle.Regular);
            int p = 10;

            foreach (TodoList item in items){
                    gfx.DrawString(item.Topic,font1, XBrushes.Black,10,p);
                    gfx.DrawString(item.Content, font1, XBrushes.Black, 10, p+20);
                    p += 55;
            }

            pdf.Save(filename);
            return RedirectToAction("Index");

        }

    }

}
