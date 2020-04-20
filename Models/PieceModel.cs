using Chess.Enums;

namespace Chess.Models
{
    public class PieceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }
        public SideColor Color { get; set; }
        public PieceStatus Status { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public int GameRoomId { get; set; }
    }

}

