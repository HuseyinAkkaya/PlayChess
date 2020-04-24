using Chess.Contexts;
using Chess.Entities;
using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Services
{
    public class InvitationService
    {
        ChessContext db = new ChessContext();

        public List<Invitation> GetInvitations(string UId,InvitationType? invitationType)
        {
            var query = db.Invitations.Where(e => e.InvitationTo == UId).AsQueryable();
            if (invitationType != null)
            {
                query.Where(e => e.InvitationType == invitationType);
            }

            return query.ToList();
        }

        public bool SendInvitation(string senderUId ,string receiverUId,InvitationType invitationType)
        {

            if(db.Invitations.Any(e=> e.InvitationFrom==senderUId && e.InvitationTo == receiverUId && e.InvitationState == InvitationState.Pending && e.InvitationType == invitationType))
            {
                return false;
            }
            Invitation invitation = new Invitation() { 
            InvitationFrom = senderUId,
            InvitationTo = receiverUId,
            InvitationType=invitationType,
            InvitationState = InvitationState.Pending
            };
            
            db.Invitations.Add(invitation);
            db.SaveChanges();
            return true;
        }

        public void DeleteInvitation(Invitation invitation)
        {
            Invitation inv = db.Invitations.Find(invitation.Id);
            db.Entry(inv).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
        }

        public Invitation GetInvitation(int invitationId)
        {
            return db.Invitations.FirstOrDefault(e=> e.Id == invitationId);
        }

        internal void UpdateInvitation(Invitation invitation)
        {
            var inv = db.Invitations.FirstOrDefault(e=>e.Id == invitation.Id);
            inv.InvitationState = invitation.InvitationState;
            db.Entry(inv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}