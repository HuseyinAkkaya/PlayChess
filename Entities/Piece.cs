using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Entities
{
    public class Piece
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }
        public SideColor Color { get; set; }
        public PieceStatus Status { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public int GameRoomId { get; set; }
        public virtual GameRoom GameRoom { get; set; }
    }
}