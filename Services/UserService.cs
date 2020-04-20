using Chess.Contexts;
using Chess.Entities;
using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Chess.Services
{
    public class UserService
    {
        ChessContext db = new ChessContext();

       public User GetUser(string UId)
        {
            return db.Users.FirstOrDefault(e => e.UId == UId);
        }

        public void UpdateUser(User user)
        {
            User eUser = db.Users.FirstOrDefault(e => e.UId == user.UId);
            eUser.LoseCount = user.LoseCount;
            eUser.Name = user.Name;
            eUser.Surname= user.Surname;
            eUser.Title= user.Title;
            eUser.WonCount = user.WonCount;
            eUser.DrawCount = user.DrawCount;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void SetCount(string whiteUserId, string blackUserId, GameStatus gameStatus)
        {
            var whiteUser = GetUser(whiteUserId);
            var blackUser = GetUser(blackUserId);
            if (gameStatus == GameStatus.WhiteWon)
            {
                whiteUser.WonCount++;
                blackUser.LoseCount++;

            }
            else if (gameStatus == GameStatus.BlackWon)
            {
                whiteUser.LoseCount++;
                blackUser.WonCount++;

            }
            else
            {
                whiteUser.DrawCount++;
                blackUser.DrawCount++;
            }
            db.Entry(whiteUser).State = EntityState.Modified;
            db.Entry(blackUser).State = EntityState.Modified;
            db.SaveChanges();

        }
    }
}