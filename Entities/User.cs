using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UId { get; set; }
        public Title Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int WonCount { get; set; }
        public int DrawCount { get; set; }
        public int LoseCount { get; set; }

    }
}