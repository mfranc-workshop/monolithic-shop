namespace TransferCheckService.Data
{
    public class Buyer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Buyer()
        {
        }

        public Buyer(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
