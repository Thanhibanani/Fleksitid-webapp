namespace Fleksitid.Models
{
    public class Arbeidstid
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public string Navn { get; set; } = string.Empty;
        public decimal TimerPerUke { get; set; }

        public DateTime FraDato { get; set; }
        public DateTime? TilDato { get; set; }
    }
}