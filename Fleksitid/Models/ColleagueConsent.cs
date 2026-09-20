namespace Fleksitid.Models
{
    /// <summary>
    /// Samtykke som lar en kollega (Viewer) se timene til en annen konsulent (Owner).
    /// Owner genererer en delingskode/QR; Viewer godtar den for å få tilgang.
    /// </summary>
    public class ColleagueConsent
    {
        public int Id { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        /// <summary>Satt først når en kollega har godtatt invitasjonen.</summary>
        public string? ViewerId { get; set; }
        public ApplicationUser? Viewer { get; set; }

        public string ShareToken { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public bool IsPending => AcceptedAt is null && RevokedAt is null && DateTime.UtcNow < ExpiresAt;
        public bool IsActive => AcceptedAt is not null && RevokedAt is null;
    }
}
