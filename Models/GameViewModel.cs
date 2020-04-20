using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess.Entities;
using Chess.Enums;

namespace Chess.Models
{
    public class GameViewModel
    {
        public int RoomId { get; set; }
        public List<PieceModel> Pieces { get; set; }
        public SideColor Turn { get; set; } //Sıra Hangi Renkte
        public SideColor Color { get; set; } // Oyuncu Rengi
        public bool IsMoveCorrect { get; set; }
        public string fromCell { get; set; }
        public string toCell { get; set; }
        public string WhiteUserId { get; set; }
        public string BlackUserId { get; set; }
        public GameStatus GameStatus { get; set; }

        public string MyHashCode()
        {
            string hash = "";
            Pieces.OrderBy(e=> e.Row).ThenBy(e=>e.Col).ToList().ForEach(e => {

                hash += e.Color.ToString().Substring(0,1);
                switch (e.Name)
                {
                    case "Rook":
                        {
                            hash += "R";
                            break;
                        }
                    case "Queen":
                        {
                            hash += "Q";
                            break;
                        }
                    case "Knight":
                        {
                            hash += "N";
                            break;
                        }
                    case "King":
                        {
                            hash += "K";
                            break;
                        }
                    case "Pawn":
                        {
                            hash += "P";
                            break;
                        }
                    case "Bishop":
                        {
                            hash += "B";
                            break;
                        }
                }
                hash += e.Row.ToString() + e.Col.ToString()+"~";
              });
            hash = hash.Remove(hash.Length - 1, 1);

            return hash;
        }
    }
}

