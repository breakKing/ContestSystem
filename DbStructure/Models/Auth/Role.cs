using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ContestSystem.DbStructure.Models.Auth
{
    public class Role : IdentityRole<long>
    {
        public Role(string name) : base(name)
        {
        }
        public string Description { get; set; }
        [JsonIgnore][InverseProperty("Roles")] public virtual List<User> Users { get; set; }
    }
}