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
        UserService userService = new UserService();
        InvitationService invitationService = new InvitationService();

        public ActionResult Chat()
        {
            return View();
        }



        GameRoomDbService roomService = new GameRoomDbService();
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindUsers()
        {
            var userlist = userService.GetOtherUsers(User.Identity.GetUserId());


            return View(userlist);
        }

        public ActionResult Invitations()
        {
            
            var invitations = invitationService.GetInvitations(User.Identity.GetUserId(), null);
            var model = invitations.Select(e => new InvitationViewModel() {
            Id=e.Id,
            InvitationFrom=e.InvitationFrom,
            InvitationState=e.InvitationState,
            InvitationType=e.InvitationType,
            InvitationFromEmail = userService.GetUser(e.InvitationFrom).EMail,
            }).ToList();


            return View(model);
        }
        
        public ActionResult MyGames()
        {
            List<GameViewModel> games = roomService.GetGames(User.Identity.GetUserId());
            return View(games);
        }
        public ActionResult Accept(int invitationId)
        {
            var invitation = invitationService.GetInvitation(invitationId);
            if(invitation==null || invitation.InvitationTo != User.Identity.GetUserId() || invitation.InvitationState != InvitationState.Pending)
            {
                return new HttpStatusCodeResult(404,"Fuck Off Exception!");
            }
            invitation.InvitationState = InvitationState.Accepted;
            invitationService.UpdateInvitation(invitation);

            if(invitation.InvitationType == InvitationType.Game)
            {
                Random r = new Random(100);
                int _roomid;
                if(r.Next() %2 == 0)
                {
                  _roomid=  roomService.CreateGame(User.Identity.GetUserId(), invitation.InvitationFrom);
                }
                else
                {
                   _roomid= roomService.CreateGame(invitation.InvitationFrom, User.Identity.GetUserId());
                }
                
                


                return RedirectToAction("Room",new { roomid = _roomid });

            }


            return RedirectToAction("");
        }

        
        public ActionResult Deny(int invitationId)
        {
            var invitation = invitationService.GetInvitation(invitationId);
            if (invitation == null || invitation.InvitationTo != User.Identity.GetUserId() || invitation.InvitationState != InvitationState.Pending)
            {
                return new HttpStatusCodeResult(404, "Fuck Off Exception!");
            }
            invitation.InvitationState = InvitationState.Denied;
            invitationService.UpdateInvitation(invitation);
            return RedirectToAction("Invitations");
        }

        public ActionResult Invite(string receiverUid, InvitationType invitationType)
        {
           if(invitationService.SendInvitation(User.Identity.GetUserId(), receiverUid, invitationType))
            {
                TempData["invitationInfo"] = "Your request has been sent.";
            }
            else
            {
                TempData["invitationInfo"] = "You have already a pending invitation.";
            }

            return RedirectToAction("FindUsers");
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