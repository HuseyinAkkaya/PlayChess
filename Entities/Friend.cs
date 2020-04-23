using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Entities
{
    public class Friend
    {
        public int Id { get; set; }
        public string FriendId { get; set; }
        public bool isAccept { get; set; }
        public User User { get; set; }
    }
}