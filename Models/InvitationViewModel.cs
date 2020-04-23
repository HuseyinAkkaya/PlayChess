using Chess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
    public class InvitationViewModel
    {
        public int Id { get; set; }
        public string InvitationFrom { get; set; }
        public string InvitationFromEmail { get; set; }
        public InvitationState InvitationState { get; set; }
        public InvitationType InvitationType { get; set; }
    }
}