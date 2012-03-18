using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 象棋.Models;

namespace 象棋.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Reset()
        {
            Game.Instance = new Game();
            return Redirect("/");
        }

        public ActionResult Black()
        {
            ViewData["Role"] = "black";

            return View("Game");
        }

        public ActionResult Red()
        {
            ViewData["Role"] = "red";

            return View("Game");
        }

        public ActionResult Play()
        {
            string role = Request["role"];

            try
            {
                int step = int.Parse(Request["step"]);
                int fromX = int.Parse(Request["fromX"]);
                int fromY = int.Parse(Request["fromY"]);
                int toX = int.Parse(Request["toX"]);
                int toY = int.Parse(Request["toY"]);

                return Json(Game.Instance.Play(role, fromX, fromY, toX, toY));
            }
            catch
            {
                return Json(Game.Instance.GetGame(role));
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Revert()
        {
            string role = Request["role"];
            Game.Instance.Revert(role);

            if (role == "red")
                return this.RedirectToAction("Red");
            else
                return this.RedirectToAction("Black");
        }
    }
}
