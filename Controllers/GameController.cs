using Chess.Enums;
using Chess.Models;
using Chess.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Chess.Controllers
{
    public class GameController : Controller
    {

        public ActionResult Chat()
        {
            return View();
        }



        GameRoomDbService roomService = new GameRoomDbService();
        // GET: Game
        public ActionResult Index()
        {
            roomService.CreateGame("a", "b");
            return View();
        }

        public ActionResult Room(int? roomid)
        {
            if (roomid == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            GameViewModel room = roomService.GetGame(roomid.Value);
            if (room == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (room.WhiteUserId == User.Identity.GetUserId())
                room.Color = SideColor.White;
            else
                room.Color = SideColor.Black;


            return View(room);
        }

        [HttpGet]
        public JsonResult Room2(string fromCell, string toCell, int roomid)
        {


            GameViewModel room = new GameViewModel();

            room = roomService.GetGame(roomid);

            if (room.WhiteUserId == User.Identity.GetUserId())
                room.Color = SideColor.White;
            else
                room.Color = SideColor.Black;

            if (room.Color == room.Turn)

                if (!string.IsNullOrWhiteSpace(fromCell) && !string.IsNullOrWhiteSpace(toCell) && fromCell != toCell)
                {
                    if (room.GameStatus == GameStatus.Promotion)
                    {
                        var pawn = room.Pieces.Find(e => e.Name == "Pawn" && (e.Row == 8 || e.Row == 1));
                        fromCell = "cell" + pawn.Row.ToString() + pawn.Col.ToString();
                        toCell = "cell" + toCell;
                    }

                    fromCell = fromCell.Remove(0, 4);
                    toCell = toCell.Remove(0, 4);
                    var aPiece = room.Pieces.Find(e => e.Row == Convert.ToInt32(fromCell.Substring(0, 1)) && e.Col == Convert.ToInt32(fromCell.Substring(1, 1)));

                    GameControlService gs = new GameControlService();
                    if (aPiece != null)
                    {

                        gs.ControlPiece(room, fromCell, toCell);



                    }
                    roomService.SetGame(room);
                }




            return Json(room, JsonRequestBehavior.AllowGet);
        }
    }
}