using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class RefreshTokenForm
    {
        [Required] public string Fingerprint { get; set; }
        public string RefreshToken { get; set; }
    }
}
