using System;

namespace ContestSystem.DbStructure.Models.Auth
{
    public class Session: BaseEntityWithoutId
    {
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public Guid RefreshToken { get; set; }
        public DateTime StartTimeUTC { get; set; }
        public int ExpiresInHours { get; set; }
        public string Fingerprint { get; set; }
    }
}
