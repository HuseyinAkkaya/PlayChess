using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Enums
{

    public enum GameStatus
    {
        Continues,
        WhiteThreat,
        BlackThreat,
        WhiteWon,
        BlackWon,
        Scoreless,
        Promotion
    }
}