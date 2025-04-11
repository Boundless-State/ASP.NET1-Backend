using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class UserEntity : IdentityUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public ICollection<ProjectEntity> Projects { get; set; } = [];
    }
}
