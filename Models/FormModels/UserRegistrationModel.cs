using System;
using System.ComponentModel.DataAnnotations;

namespace ContestSystem.Models.FormModels
{
    public class UserRegistrationModel
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        public string Culture { get; set; }
        [Required] public string Fingerprint { get; set; }
    }
}