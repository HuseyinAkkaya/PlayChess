using Chess.Entities;
using System.Data.Entity;

namespace Chess.Contexts
{
    public class ChessContext :DbContext
    {
        public ChessContext() : base("DefaultConnection")
        {
          //  Database.SetInitializer<ChessContext>(null);
        }
        public virtual DbSet<Piece> Pieces { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<GameRoom> GameRooms { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }
    }
}