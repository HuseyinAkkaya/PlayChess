using Chess.Models;
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
    public class GameRoomDbService
    {
        ChessContext db = new ChessContext();
        public GameViewModel GetGame(int RoomId)
        {
            GameRoom gameRoom = db.GameRooms.FirstOrDefault(e => e.Id == RoomId);
            if (gameRoom == null)
                return null;
            GameViewModel model = new GameViewModel()
            {
                Pieces = gameRoom.Pieces.Select(e => new PieceModel()
                {
                    Col = e.Col,
                    Color = e.Color,
                    GameRoomId = e.GameRoomId,
                    Id = e.Id,
                    Name = e.Name,
                    Row = e.Row,
                    Status = e.Status,
                    Symbol = e.Symbol

                }).ToList(),
                WhiteUserId=gameRoom.WhiteUserId,
                BlackUserId = gameRoom.BlackUserId,
                Turn = gameRoom.Turn,
                GameStatus = gameRoom.GameStatus,
                RoomId = gameRoom.Id,

            };

            return model;
        }
        public void SetGame(GameViewModel model)
        {
            GameRoom gameRoom = db.GameRooms.FirstOrDefault(e => e.Id == model.RoomId);
            List<PieceModel> fark = new List<PieceModel>();
            gameRoom.Pieces.ToList().ForEach(g => {
                PieceModel aPiece = model.Pieces.FirstOrDefault(e => e.Id == g.Id && (e.Row != g.Row || e.Col != g.Col || e.Name != g.Name || e.Status != g.Status));
                if(aPiece!=null)
                fark.Add(aPiece);
            });
            fark.ForEach(e =>
            {
                var aPiece = gameRoom.Pieces.Single(k => k.Id == e.Id);
                if (e.Col==0 && e.Row == 0)
                {
                   
                    db.Entry(aPiece).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else
                {
                aPiece.Col = e.Col;
                aPiece.Row = e.Row;
                aPiece.Name = e.Name;
                aPiece.Status = e.Status;
                aPiece.Symbol = e.Symbol; 
                }
            });
            gameRoom.GameStatus = model.GameStatus;
            gameRoom.Turn = model.Turn;
            db.Entry(gameRoom).State = EntityState.Modified;
            db.SaveChanges();
        }
        public int CreateGame(String whiteUserId,String blackUserId)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Pieces = GetNewPieceList();
            gameRoom.GameStatus = GameStatus.Continues;
            gameRoom.Turn = SideColor.White;
            gameRoom.BlackUserId = blackUserId;
            gameRoom.WhiteUserId = whiteUserId;
            db.GameRooms.Add(gameRoom);
            db.SaveChanges();
            return gameRoom.Id;
        }

        private List<Piece> GetNewPieceList()
        {
            List<Piece> pieces = new List<Piece>()
            {
                new Piece(){Id=17,Status=PieceStatus.DontMoveYet, Name="Rook",  Color=SideColor.White,  Row=1, Col=1},
                new Piece(){Id=18,Status=PieceStatus.DontMoveYet, Name="Rook",  Color=SideColor.White,  Row=1, Col=8},
                new Piece(){Id=19,Status=PieceStatus.DontMoveYet, Name="Knight",  Color=SideColor.White,  Row=1, Col=2},
                new Piece(){Id=20,Status=PieceStatus.DontMoveYet, Name="Knight",  Color=SideColor.White,  Row=1, Col=7},
                new Piece(){Id=21,Status=PieceStatus.DontMoveYet, Name="Bishop",  Color=SideColor.White,  Row=1, Col=3},
                new Piece(){Id=22,Status=PieceStatus.DontMoveYet, Name="Bishop",  Color=SideColor.White,  Row=1, Col=6},
                new Piece(){Id=23,Status=PieceStatus.DontMoveYet, Name="Queen",  Color=SideColor.White,  Row=1, Col=4},
                new Piece(){Id=24,Status=PieceStatus.DontMoveYet, Name="King",  Color=SideColor.White,  Row=1, Col=5},
                new Piece(){Id=25,Status=PieceStatus.DontMoveYet, Name="Rook", Color=SideColor.Black,  Row=8, Col=1},
                new Piece(){Id=26,Status=PieceStatus.DontMoveYet, Name="Rook", Color=SideColor.Black,  Row=8, Col=8},
                new Piece(){Id=27,Status=PieceStatus.DontMoveYet, Name="Knight", Color=SideColor.Black,  Row=8, Col=2},
                new Piece(){Id=28,Status=PieceStatus.DontMoveYet, Name="Knight", Color=SideColor.Black,  Row=8, Col=7},
                new Piece(){Id=29,Status=PieceStatus.DontMoveYet, Name="Bishop", Color=SideColor.Black,  Row=8, Col=3},
                new Piece(){Id=30,Status=PieceStatus.DontMoveYet, Name="Bishop", Color=SideColor.Black,  Row=8, Col=6},
                new Piece(){Id=31,Status=PieceStatus.DontMoveYet, Name="Queen", Color=SideColor.Black,  Row=8, Col=4},
                new Piece(){Id=32,Status=PieceStatus.DontMoveYet, Name="King", Color=SideColor.Black,  Row=8, Col=5},

            };

            for (int i = 1; i <= 8; i++)
            {
                pieces.Add(new Piece() { Id = i, Name = "Pawn", Color = SideColor.White, Row = 2, Col = i });
                pieces.Add(new Piece() { Id = i + 8, Name = "Pawn", Color = SideColor.Black, Row = 7, Col = i });

            }

            return pieces;
        }

    }
}