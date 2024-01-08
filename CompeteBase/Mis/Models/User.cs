namespace Compete.Mis.Models
{
    public sealed class User : Entity
    {
        public Entity? Tenant { get; set; }

        public long RoleId { get; set; }

        public string? UserPassword { get; set; }
    }
}
