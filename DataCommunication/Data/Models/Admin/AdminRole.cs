namespace DataCommunication
{
    public class AdminRole
    {
        public int Id { get; set; } // 1 = SuperAdmin, 2 = Manager, etc.
        public string Name { get; set; }

        public ICollection<Admin> Admins { get; set; }
    }
}
