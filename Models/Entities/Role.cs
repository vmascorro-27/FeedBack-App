namespace FeedBack_APP.Models.Entities
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public byte Activo { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
