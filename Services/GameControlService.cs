using Chess.Models;
using Chess.Classes;
using Chess.Entities;
using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Chess.Services
{
    public class GameControlService
    {

        Cell _cellFrom = new Cell();
        Cell _cellTo = new Cell();


        public void ControlPiece(GameViewModel model, string fromCell, string toCell)
        {

            if (model.GameStatus == GameStatus.BlackWon || model.GameStatus == GameStatus.WhiteWon || model.GameStatus == GameStatus.Scoreless)
            {
                return;
            }
            var aPiece = model.Pieces.Find(e => e.Row == Convert.ToInt32(fromCell.Substring(0, 1)) && e.Col == Convert.ToInt32(fromCell.Substring(1, 1)));
            if (model.GameStatus == GameStatus.Promotion)
            {
                aPiece.Name = toCell;
                if (IsCheck(model, true))
                {
                    model.GameStatus = model.Turn == SideColor.White ? GameStatus.BlackThreat : GameStatus.WhiteThreat;
                    //TODO Yaptığımız hamle sonrasında rakip şahı tehdit ediyorsak burası çalışır

                }
                else
                {
                    model.GameStatus = GameStatus.Continues;
                }

                if (CanYouAnyMove(model))
                {
                    Debug.WriteLine("Hamle Var");
                }
                else
                {
                    if (model.GameStatus == (model.Turn == SideColor.White ? GameStatus.BlackThreat : GameStatus.WhiteThreat))
                    {
                        model.GameStatus = model.GameStatus == GameStatus.BlackThreat ? GameStatus.WhiteWon : GameStatus.BlackWon;

                    }
                    else
                    {
                        model.GameStatus = GameStatus.Scoreless;
                    }
                    if (model.GameStatus == GameStatus.BlackWon || model.GameStatus == GameStatus.Scoreless || model.GameStatus == GameStatus.WhiteWon)
                    {
                        var userService = new UserService();
                        userService.SetCount(model.WhiteUserId, model.BlackUserId, model.GameStatus);
                    }




                    Debug.WriteLine("Hamle Yok");
                }
                Debug.WriteLine(model.GameStatus.ToString());
                model.Turn = model.Turn == SideColor.White ? SideColor.Black : SideColor.White;

                return;
            }

            if (aPiece != null && aPiece.Color == model.Turn)
            {

                model.fromCell = fromCell;
                model.toCell = toCell;
                if (IsCellCorrect(model))// hamle uygun mu
                {
                    if (IsCheck(model))
                    {
                        //Yaptığımız hamle sonrası şahımız tehlikeye düşüyorsa burası çalışır 
                        model.IsMoveCorrect = false;
                        return;
                    }
                    if (RivalPieceEatOrMoveToEmptyCell(model))
                    {
                        var a = model.Pieces.Where(e => e.Status == PieceStatus.PawnDoubleMove).ToList();

                        foreach (var b in a)
                        {
                            b.Status = PieceStatus.Moved;
                        }

                        //hamle uygunsa rakip taş yenir ve burası çalışır.
                        aPiece.Row = Convert.ToInt32(toCell.Substring(0, 1));
                        aPiece.Col = Convert.ToInt32(toCell.Substring(1, 1));

                        if (aPiece.Name == "Pawn" && Math.Abs(Convert.ToInt32(toCell.Substring(0, 1)) - Convert.ToInt32(fromCell.Substring(0, 1))) == 2)
                        {
                            aPiece.Status = PieceStatus.PawnDoubleMove;
                        }
                        else
                        {
                            aPiece.Status = PieceStatus.Moved;
                        }

                        if (aPiece.Name != "Pawn")
                        {
                            Debug.WriteLine(model.MyHashCode());
                        }
                    }
                    else
                    {
                        model.IsMoveCorrect = false;
                        //todo ceza infaz
                        return;
                    }
                    if (IsCheck(model, true))
                    {
                        model.GameStatus = model.Turn == SideColor.White ? GameStatus.BlackThreat : GameStatus.WhiteThreat;
                        //TODO Yaptığımız hamle sonrasında rakip şahı tehdit ediyorsak burası çalışır

                    }
                    else
                    {
                        model.GameStatus = GameStatus.Continues;
                    }

                    if (model.Pieces.Where(e => e.Col != 0).ToList().Count < 3 || (model.Pieces.Where(e => e.Col != 0).ToList().Count < 4 && model.Pieces.Any(e => e.Col != 0 && (e.Name == "Bishop" || e.Name == "Knight"))))
                    {
                        model.GameStatus = GameStatus.Scoreless;
                        var userService = new UserService();
                        userService.SetCount(model.WhiteUserId, model.BlackUserId, model.GameStatus);
                    }
                    if (CanYouAnyMove(model))
                    {
                        Debug.WriteLine("Hamle Var");
                    }
                    else
                    {
                        if (model.GameStatus == (model.Turn == SideColor.White ? GameStatus.BlackThreat : GameStatus.WhiteThreat))
                        {
                            model.GameStatus = model.GameStatus == GameStatus.BlackThreat ? GameStatus.WhiteWon : GameStatus.BlackWon;
                        }
                        else
                        {
                            model.GameStatus = GameStatus.Scoreless;
                        }
                        if (model.GameStatus == GameStatus.BlackWon || model.GameStatus == GameStatus.Scoreless || model.GameStatus == GameStatus.WhiteWon)
                        {
                            var userService = new UserService();
                            userService.SetCount(model.WhiteUserId, model.BlackUserId, model.GameStatus);
                        }


                        Debug.WriteLine("Hamle Yok");
                    }
                    Debug.WriteLine(model.GameStatus.ToString());
                    if (model.Pieces.Any(e => e.Name == "Pawn" && (e.Row == 8 || e.Row == 1)))
                    {
                        model.GameStatus = GameStatus.Promotion;
                        Debug.WriteLine("Piyonu değiştir");
                    }
                    else
                    {
                        model.Turn = model.Turn == SideColor.White ? SideColor.Black : SideColor.White;

                    }

                }
                else
                {
                    model.IsMoveCorrect = false;
                }

            }


        }


        private bool CanYouAnyMove(GameViewModel model)
        {
            Cell cellTo = new Cell();
            Cell cellFrom = new Cell();

            foreach (PieceModel aPiece in model.Pieces.Where(e => e.Status != PieceStatus.Eated && e.Color != model.Turn).ToList())
            {


                cellFrom = new Cell()
                {
                    Col = aPiece.Col,
                    Row = aPiece.Row
                };



                switch (aPiece.Name)
                {

                    case "Pawn":
                        {
                            if (aPiece.Color == SideColor.White)
                            {

                                if (aPiece.Row < 8 && aPiece.Col > 1)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col - 1,
                                        Row = aPiece.Row + 1
                                    };

                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue;
                                    if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }

                                }
                                if (aPiece.Row < 8 && aPiece.Col < 8)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col + 1,
                                        Row = aPiece.Row + 1
                                    };
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }

                                }
                                if (aPiece.Row < 8)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col,
                                        Row = aPiece.Row + 1
                                    };
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                if (aPiece.Row > 0 && aPiece.Col > 1)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col - 1,
                                        Row = aPiece.Row - 1
                                    };

                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (aPiece.Row > 1 && aPiece.Col < 8)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col + 1,
                                        Row = aPiece.Row - 1
                                    };
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }

                                }
                                if (aPiece.Row > 1)
                                {
                                    cellTo = new Cell()
                                    {
                                        Col = aPiece.Col,
                                        Row = aPiece.Row - 1
                                    };
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;

                        }


                    case "Rook":
                        {
                            cellTo.Row = cellFrom.Row;

                            for (int i = 1; i <= 8; i++)
                            {
                                cellTo.Col = i;

                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }


                            cellTo.Col = cellFrom.Col;

                            for (int i = 1; i <= 8; i++)
                            {
                                cellTo.Row = i;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }

                            break;
                        }

                    case "Knight":
                        {
                            if (cellFrom.Row <= 6)
                            {
                                cellTo.Row = cellFrom.Row + 2;

                                if (cellFrom.Col > 1)
                                {
                                    cellTo.Col = cellFrom.Col - 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col < 8)
                                {
                                    cellTo.Col = cellFrom.Col + 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }

                            if (cellFrom.Row <= 7)
                            {

                                cellTo.Row = cellFrom.Row + 1;

                                if (cellFrom.Col > 2)
                                {
                                    cellTo.Col = cellFrom.Col - 2;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col < 7)
                                {
                                    cellTo.Col = cellFrom.Col + 2;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }

                            if (cellFrom.Row >= 1)
                            {

                                cellTo.Row = cellFrom.Row - 1;

                                if (cellFrom.Col > 2)
                                {
                                    cellTo.Col = cellFrom.Col - 2;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col < 7)
                                {
                                    cellTo.Col = cellFrom.Col + 2;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }

                            if (cellFrom.Row >= 2)
                            {
                                cellTo.Row = cellFrom.Row - 2;

                                if (cellFrom.Col > 1)
                                {
                                    cellTo.Col = cellFrom.Col - 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col < 8)
                                {
                                    cellTo.Col = cellFrom.Col + 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }

                            break;
                        }

                    case "Bishop":
                        {

                            for (int i = cellFrom.Col, k = cellFrom.Row; i <= 8 && k <= 8; i++, k++)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i <= 8 && k > 0; i++, k--)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i > 0 && k <= 8; i--, k++)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i > 0 && k > 0; i--, k--)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }

                            break;
                        }
                    case "Queen":
                        {
                            for (int i = cellFrom.Col, k = cellFrom.Row; i <= 8 && k <= 8; i++, k++)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i <= 8 && k > 0; i++, k--)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i > 0 && k <= 8; i--, k++)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            for (int i = cellFrom.Col, k = cellFrom.Row; i > 0 && k > 0; i--, k--)
                            {
                                cellTo.Col = i;
                                cellTo.Row = k;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }



                            cellTo.Row = cellFrom.Row;

                            for (int i = 1; i <= 8; i++)
                            {
                                cellTo.Col = i;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }


                            cellTo.Col = cellFrom.Col;

                            for (int i = 1; i <= 8; i++)
                            {
                                cellTo.Row = i;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }
                            break;
                        }

                    case "King":
                        {

                            cellTo.Row = cellFrom.Row;
                            if (cellFrom.Col < 7)
                            {
                                cellTo.Col = cellFrom.Col + 1;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue;
                                if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }

                            }
                            if (cellFrom.Col > 1)
                            {

                                cellTo.Col = cellFrom.Col - 1;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue;
                                if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }
                            }


                            if (cellFrom.Row < 7)
                            {
                                cellTo.Row = cellFrom.Row + 1;

                                cellTo.Col = cellFrom.Col;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue;
                                if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }

                                if (cellFrom.Col < 7)
                                {

                                    cellTo.Col = cellFrom.Col + 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col > 1)
                                {

                                    cellTo.Col = cellFrom.Col - 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }
                            if (cellFrom.Row > 1)
                            {
                                cellTo.Row = cellFrom.Row - 1;

                                cellTo.Col = cellFrom.Col;
                                if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                {
                                    return true;
                                }

                                if (cellFrom.Col < 7)
                                {

                                    cellTo.Col = cellFrom.Col + 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }
                                if (cellFrom.Col > 1)
                                {

                                    cellTo.Col = cellFrom.Col - 1;
                                    if (aPiece.Col == cellTo.Col && aPiece.Row == cellTo.Row) continue; if (CanIAnyMoveHelper(model, aPiece, cellTo, cellFrom))
                                    {
                                        return true;
                                    }
                                }

                            }




                            break;
                        }
                    default:
                        {
                            break;
                        }


                }



            }



            return false;
        }

        private bool CanIAnyMoveHelper(GameViewModel model, PieceModel aPiece, Cell cellto, Cell cellfrom)
        {

            if (IsCellCorrect(model, cellfrom, cellto))
            {
                if (!model.Pieces.Any(e => e.Col == _cellTo.Col && e.Row == _cellTo.Row && e.Color == aPiece.Color))
                {
                    // şah çekilmişse hamle sonrası tehdit kalkıyorsa true
                    Cell cellto1 = cellto;
                    Cell cellFrom1 = new Cell() { Row = cellfrom.Row, Col = cellfrom.Col };

                    PieceModel yenecek = model.Pieces.Find(e => e.Col == cellto1.Col && e.Row == cellto1.Row);
                    if (yenecek != null)
                    {
                        yenecek.Row = 0;
                        yenecek.Col = 0;
                    }
                    PieceModel oynanacak = model.Pieces.Find(e => e.Col == cellFrom1.Col && e.Row == cellFrom1.Row);
                    oynanacak.Row = cellto1.Row;
                    oynanacak.Col = cellto1.Col;



                    if (IsCheck(model, true))
                    {
                        if (yenecek != null)
                        {
                            yenecek.Row = cellto1.Row;
                            yenecek.Col = cellto1.Col;
                        }
                        oynanacak.Row = cellFrom1.Row;
                        oynanacak.Col = cellFrom1.Col;

                        return false;
                    }
                    else
                    {
                        if (yenecek != null)
                        {
                            yenecek.Row = cellto1.Row;
                            yenecek.Col = cellto1.Col;
                        }
                        oynanacak.Row = cellFrom1.Row;
                        oynanacak.Col = cellFrom1.Col;
                        Debug.WriteLine(oynanacak.Color.ToString() + " " + oynanacak.Name);
                        Debug.WriteLine(cellto.Row + "-" + cellto.Col);
                        return true;
                    }





                }

            }

            return false;
        }

        private bool IsCheck(GameViewModel model, bool AfterMove = false) //After Move true olursa rakip şah kontrol edilir.
        {


            SideColor KingColor = model.Turn;
            if (AfterMove)
            {
                KingColor = KingColor == SideColor.White ? SideColor.Black : SideColor.White;
            }

            var King = model.Pieces.SingleOrDefault(e => e.Name == "King" && e.Color == KingColor);
            var CounterPieces = model.Pieces.Where(e => e.Color != KingColor && e.Col != 0 && e.Row != 0);

            Cell cellto1 = new Cell();
            Cell cellFrom1 = new Cell();
            var oynanacak = new PieceModel();
            var yenecek = new PieceModel();
            if (!AfterMove)
            {
                cellto1 = new Cell()
                {
                    Col = Convert.ToInt32(model.toCell.Substring(1, 1)),
                    Row = Convert.ToInt32(model.toCell.Substring(0, 1))
                };
                cellFrom1 = new Cell()
                {
                    Row = Convert.ToInt32(model.fromCell.Substring(0, 1)),
                    Col = Convert.ToInt32(model.fromCell.Substring(1, 1))
                };

                yenecek = model.Pieces.Find(e => e.Col == cellto1.Col && e.Row == cellto1.Row);
                if (yenecek != null)
                {
                    yenecek.Row = 0;
                    yenecek.Col = 0;
                }


                oynanacak = model.Pieces.Find(e => e.Col == cellFrom1.Col && e.Row == cellFrom1.Row);
                oynanacak.Row = cellto1.Row;
                oynanacak.Col = cellto1.Col;
            }


            foreach (var aPiece in CounterPieces)
            {
                var cellto = new Cell();
                var cellFrom = new Cell();

                cellto = new Cell
                {
                    Row = King.Row,
                    Col = King.Col
                };


                cellFrom = new Cell()
                {
                    Row = aPiece.Row,
                    Col = aPiece.Col
                };

                if (IsCellCorrect(model, cellFrom, cellto))
                {
                    if (AfterMove)
                        return true;

                    if (RivalPieceEatOrMoveToEmptyCell(model, cellFrom, cellto))
                    {

                        if (yenecek != null)
                        {
                            yenecek.Row = cellto1.Row;
                            yenecek.Col = cellto1.Col;
                        }
                        oynanacak.Row = cellFrom1.Row;
                        oynanacak.Col = cellFrom1.Col;

                        return true;
                    }
                }
            }
            if (!AfterMove)
            {
                if (yenecek != null)
                {
                    yenecek.Row = cellto1.Row;
                    yenecek.Col = cellto1.Col;
                }
                oynanacak.Row = cellFrom1.Row;
                oynanacak.Col = cellFrom1.Col;
            }

            return false;
        }


        /// <summary>
        /// Hedef hücrede rakip taş varsa ye ve harekete izin ver.
        /// Kendi taşı varsa harekete izin verme.
        /// Taş yoksa harekete izin ver.
        /// İşlemi MoveStatusa kaydet
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool RivalPieceEatOrMoveToEmptyCell(GameViewModel model, Cell cellFrom = null, Cell cellTo = null)
        {


            if (cellFrom != null)
                _cellFrom = cellFrom;
            else
                _cellFrom = new Cell()
                {
                    Row = Convert.ToInt32(model.fromCell.Substring(0, 1)),
                    Col = Convert.ToInt32(model.fromCell.Substring(1, 1))
                };


            if (cellTo != null)
                _cellTo = cellTo;
            else
                _cellTo = new Cell()
                {
                    Col = Convert.ToInt32(model.toCell.Substring(1, 1)),
                    Row = Convert.ToInt32(model.toCell.Substring(0, 1))
                };


            var destinationCell = model.Pieces.SingleOrDefault(e => e.Col == _cellTo.Col && e.Row == _cellTo.Row);
            SideColor sorgu = model.Turn;
            if (cellFrom != null)
                sorgu = sorgu == SideColor.White ? SideColor.Black : SideColor.White;

            if (destinationCell != null)
            {
                if (destinationCell.Color == sorgu)
                {
                    model.IsMoveCorrect = false;
                    return false;
                }
                if (cellFrom == null)
                {
                    model.IsMoveCorrect = true;
                    destinationCell.Row = 0;
                    destinationCell.Col = 0;
                    destinationCell.Status = PieceStatus.Eated;
                }
            }
            else
            {
                var aPiece = model.Pieces.Find(e => e.Col == _cellFrom.Col && e.Row == _cellFrom.Row);
                if (aPiece.Name == "Pawn")
                {
                    int yDifference = _cellTo.Row - _cellFrom.Row;
                    var specialPawn = model.Pieces.Find(e => e.Col == _cellTo.Col && e.Row == _cellTo.Row - yDifference && e.Status == PieceStatus.PawnDoubleMove);
                    if (specialPawn != null && cellFrom == null)
                    {
                        specialPawn.Col = 0;
                        specialPawn.Row = 0;
                        specialPawn.Status = PieceStatus.Eated;
                    }
                }
                model.IsMoveCorrect = true;
            }

            return true;


        }
        public bool IsCellCorrect(GameViewModel model, Cell cellFrom = null, Cell cellTo = null)
        {
            if (cellFrom != null)
                _cellFrom = cellFrom;
            else
                _cellFrom = new Cell()
                {
                    Row = Convert.ToInt32(model.fromCell.Substring(0, 1)),
                    Col = Convert.ToInt32(model.fromCell.Substring(1, 1))
                };


            if (cellTo != null)
                _cellTo = cellTo;
            else
                _cellTo = new Cell()
                {
                    Col = Convert.ToInt32(model.toCell.Substring(1, 1)),
                    Row = Convert.ToInt32(model.toCell.Substring(0, 1))
                };




            var aPiece = model.Pieces.Find(e => e.Row == _cellFrom.Row && e.Col == _cellFrom.Col);
            if (aPiece != null)
            {



                int xDifference = _cellTo.Col - _cellFrom.Col;
                int yDifference = _cellTo.Row - _cellFrom.Row;

                switch (aPiece.Name)
                {

                    case "Pawn":
                        {
                            if (xDifference == 0)
                            {
                                var destinationCell = model.Pieces.Find(e => e.Row == _cellTo.Row && e.Col == _cellTo.Col);
                                if (destinationCell != null)
                                    return false;

                                /*    if (cellFrom != null)
                                        return false;
                                        */
                                // ilk hamlede 2 kare ilerleme
                                if (((aPiece.Color == SideColor.White && aPiece.Row == 2) || (aPiece.Color == SideColor.Black && aPiece.Row == 7)) && Math.Abs(yDifference) == 2)
                                {

                                    break;
                                }
                                // tek kare ilerleme
                                if ((aPiece.Color == SideColor.White && yDifference == 1) || (aPiece.Color == SideColor.Black && yDifference == -1))
                                {
                                    break;
                                }

                                return false;

                            }
                            else if (Math.Abs(xDifference) == 1)
                            {
                                if ((aPiece.Color == SideColor.White && yDifference == 1) || (aPiece.Color == SideColor.Black && yDifference == -1))
                                {


                                    /*  if (cellFrom != null)
                                      {
                                          return true;

                                      }
                                      */



                                    if (model.Pieces.Any(e => e.Color != aPiece.Color && e.Col == _cellTo.Col && e.Row == _cellTo.Row))
                                        break;
                                    if (model.Pieces.Any(e => e.Color != aPiece.Color && e.Col == _cellTo.Col && e.Row == _cellTo.Row - yDifference && e.Status == PieceStatus.PawnDoubleMove))
                                        break;


                                    return false;
                                }

                                return false;


                            }
                            else
                            {
                                return false;
                            }
                        }

                    case "Knight":
                        {
                            xDifference = Math.Abs(xDifference);
                            yDifference = Math.Abs(yDifference);

                            if (!((xDifference == 2 && yDifference == 1) || (xDifference == 1 && yDifference == 2)))
                                return false;
                            break;
                        }
                    case "Bishop":
                        {
                            // Çapraz mı gidiyor?
                            if (Math.Abs(xDifference) != Math.Abs(yDifference))
                                return false;

                            int differance = Math.Abs(xDifference);
                            if (xDifference == yDifference && xDifference > 0)
                            {
                                for (int i = 1; i < differance; i++)
                                {

                                    if (model.Pieces.Any(e => (e.Col == aPiece.Col + i) && (e.Row == aPiece.Row + i)))
                                        return false;
                                }
                                break;
                            }
                            else if (xDifference == yDifference && xDifference < 0)
                            {
                                for (int i = 1; i < differance; i++)
                                {

                                    if (model.Pieces.Any(e => (e.Col == aPiece.Col - i) && (e.Row == aPiece.Row - i)))
                                        return false;

                                }
                                break;
                            }
                            else if (xDifference < yDifference)
                            {

                                for (int i = 1; i < differance; i++)
                                {

                                    if (model.Pieces.Any(e => (e.Col == aPiece.Col - i) && (e.Row == aPiece.Row + i)))
                                        return false;

                                }
                                break;
                            }
                            else if (xDifference > yDifference)
                            {
                                for (int i = 1; i < differance; i++)
                                {

                                    if (model.Pieces.Any(e => (e.Col == aPiece.Col + i) && (e.Row == aPiece.Row - i)))
                                        return false;

                                }
                                break;
                            }

                            break;
                        }
                    case "Queen":
                        {
                            if (xDifference == 0 || yDifference == 0)
                            {
                                bool hasAnyPieceOnWay = false;
                                if (xDifference == 0)
                                {
                                    //yatay hareket
                                    //arada taş var mı?
                                    hasAnyPieceOnWay = model.Pieces.Any(e => (e.Col == aPiece.Col) && (_cellFrom.Row > _cellTo.Row ? (e.Row < _cellFrom.Row && e.Row > _cellTo.Row) : (e.Row > _cellFrom.Row && e.Row < _cellTo.Row)));

                                }
                                else
                                {

                                    //dikey hareket
                                    //arada taş var mı?
                                    hasAnyPieceOnWay = model.Pieces.Any(e => (e.Row == aPiece.Row) && (_cellFrom.Col > _cellTo.Col ? (e.Col < _cellFrom.Col && e.Col > _cellTo.Col) : (e.Col > _cellFrom.Col && e.Col < _cellTo.Col)));

                                }
                                if (hasAnyPieceOnWay)
                                    return false;

                                break;
                            }
                            else
                            {

                                if (Math.Abs(xDifference) != Math.Abs(yDifference))
                                    return false;

                                int differance = Math.Abs(xDifference);
                                if (xDifference == yDifference && xDifference > 0)
                                {
                                    for (int i = 1; i < differance; i++)
                                    {

                                        if (model.Pieces.Any(e => (e.Col == aPiece.Col + i) && (e.Row == aPiece.Row + i)))
                                            return false;
                                    }
                                    break;
                                }
                                else if (xDifference == yDifference && xDifference < 0)
                                {
                                    for (int i = 1; i < differance; i++)
                                    {

                                        if (model.Pieces.Any(e => (e.Col == aPiece.Col - i) && (e.Row == aPiece.Row - i)))
                                            return false;

                                    }
                                    break;
                                }
                                else if (xDifference < yDifference)
                                {

                                    for (int i = 1; i < differance; i++)
                                    {

                                        if (model.Pieces.Any(e => (e.Col == aPiece.Col - i) && (e.Row == aPiece.Row + i)))
                                            return false;

                                    }
                                    break;
                                }
                                else if (xDifference > yDifference)
                                {
                                    for (int i = 1; i < differance; i++)
                                    {

                                        if (model.Pieces.Any(e => (e.Col == aPiece.Col + i) && (e.Row == aPiece.Row - i)))
                                            return false;

                                    }
                                    break;
                                }

                            }

                            return false;
                        }
                    case "King":
                        {
                            if (Math.Abs(xDifference) == 2 && yDifference == 0)
                            {
                                if (Rock(model, aPiece, xDifference))
                                    break;
                                else
                                    return false;
                            }

                            if (Math.Abs(xDifference) <= 1 && Math.Abs(yDifference) <= 1)
                                break;

                            return false;

                        }
                    case "Rook":
                        {
                            if (xDifference == 0 || yDifference == 0)
                            {
                                bool hasAnyPieceOnWay = false;
                                if (xDifference == 0)
                                {
                                    //yatay hareket
                                    //arada taş var mı?
                                    hasAnyPieceOnWay = model.Pieces.Any(e => (e.Col == aPiece.Col) && (_cellFrom.Row > _cellTo.Row ? (e.Row < _cellFrom.Row && e.Row > _cellTo.Row) : (e.Row > _cellFrom.Row && e.Row < _cellTo.Row)));

                                }
                                else
                                {

                                    //dikey hareket
                                    //arada taş var mı?
                                    hasAnyPieceOnWay = model.Pieces.Any(e => (e.Row == aPiece.Row) && (_cellFrom.Col > _cellTo.Col ? (e.Col < _cellFrom.Col && e.Col > _cellTo.Col) : (e.Col > _cellFrom.Col && e.Col < _cellTo.Col)));

                                }
                                if (hasAnyPieceOnWay)
                                    return false;
                                break;
                            }

                            return false;
                        }
                    default:
                        {
                            return false;
                        }


                }


                return true;


            }

            return false;

        }

        private bool Rock(GameViewModel model, PieceModel sah, int xDifference)
        {

            if (sah.Status == PieceStatus.Moved)
                return false;
            if (model.GameStatus == GameStatus.BlackThreat || model.GameStatus == GameStatus.WhiteThreat)
                return false;

            if (xDifference > 0)
            {
                var kale = model.Pieces.Find(e => e.Row == sah.Row && e.Col == 8 && e.Status == PieceStatus.DontMoveYet);
                if (kale == null)
                    return false;
                if (model.Pieces.Any(e => e.Row == sah.Row && (e.Col == sah.Col + 1 || e.Col == sah.Col + 2)))
                    return false;


                model.toCell = (sah.Row).ToString() + (sah.Col + 1).ToString();
                if (IsCheck(model))
                {
                    return false;
                }

                model.toCell = (sah.Row).ToString() + (sah.Col + 2).ToString();
                if (IsCheck(model))
                {
                    return false;
                }

                kale.Col = sah.Col + 1;

            }

            if (xDifference < 0)
            {
                var kale = model.Pieces.Find(e => e.Row == sah.Row && e.Col == 1 && e.Status == PieceStatus.DontMoveYet);
                if (kale == null)
                    return false;
                if (model.Pieces.Any(e => e.Row == sah.Row && (e.Col == sah.Col - 1 || e.Col == sah.Col - 2)))
                    return false;


                model.toCell = (sah.Row).ToString() + (sah.Col - 1).ToString();
                if (IsCheck(model))
                {
                    return false;
                }

                model.toCell = (sah.Row).ToString() + (sah.Col - 2).ToString();
                if (IsCheck(model))
                {
                    return false;
                }

                kale.Col = sah.Col - 1;

            }


            return true;
        }
    }
}