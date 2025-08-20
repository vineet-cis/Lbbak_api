namespace DataCommunication
{
    public class UserType
    {
        public int Id { get; set; } // 1 = Individual, 2 = Company, 3 = Designer
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }

}
