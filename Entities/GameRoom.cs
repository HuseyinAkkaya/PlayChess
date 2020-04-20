using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Entities
{
    public class GameRoom
    {
        public int Id { get; set; }
        public String WhiteUserId { get; set; }
        public String BlackUserId { get; set; }
        public SideColor Turn { get; set; } 
        public GameStatus GameStatus { get; set; }
        public virtual List<Piece> Pieces { get; set; }
    }
}