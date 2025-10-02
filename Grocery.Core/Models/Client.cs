
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public enum Roles
        {
            None,
            Admin
        }
        public Roles MyRole { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Client(int id, string name, string emailAddress, string password, Roles role) : base(id, name)
        {
            EmailAddress=emailAddress;
            Password=password;
            MyRole = role;
        }
    }
}
