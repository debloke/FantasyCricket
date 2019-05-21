namespace FantasyCricket.Models
{
    public class Points : Player
    {
        public int BattingPoints { get; set; }

        public int BowlingPoints { get; set; }

        public int FieldingPoints { get; set; }

        public int Bonus { get; set; }

        public string Team { get; set; }

        public BattingScore BattingScore { get; set; }

        public BowlingScore BowlingScore { get; set; }

        public FieldingScore FieldingScore { get; set; }

    }
}
