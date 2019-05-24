namespace FantasyCricket.Models
{
    public class UserTeam
    {

        public int[] PlayerIds { get; set; }


        public int BattingCaptain { get; set; }
        public int BowlingCaptain { get; set; }
        public int FieldingCaptain { get; set; }

        public int RemSubs { get; set; }


    }
}
