using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Enums
{
    public enum PieceStatus
    {
        DontMoveYet = 0,
        Eated = 1,
        Moved = 2,
        PawnDoubleMove
    }
}