using Chess.Contexts;
using Chess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Chess.Services
{
    public class UserConnectionService
    {
    
        public void AddConnection(String UserId, String ConnectionId)
        {
            using (var db = new ChessContext())
            {
                db.UserConnections.Add(new UserConnection() { Userid = UserId, Connectionid = ConnectionId });
                db.SaveChanges();
            }
        }
        public void RemoveConnection(String ConnectionId)
        {
            using (var db = new ChessContext())
            {
               var userConnection = db.UserConnections.Single(e=> e.Connectionid== ConnectionId );
                db.Entry(userConnection).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public List<UserConnection> GetConnections(String UserId)
        {
            List<UserConnection> userConnections;
            using (var db = new ChessContext())
            {
                userConnections = db.UserConnections.Where(e => e.Userid== UserId).ToList();
            }
            return userConnections;
        }
    }
}