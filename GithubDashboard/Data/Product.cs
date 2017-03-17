namespace GithubDashboard.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int NumberAvailable { get; set; }

        public Product()
        {
        }

        public Product(decimal price, string name, int id)
        {
            Id = id;
            Price = price;
            Name = name;
            NumberAvailable = 0;
        }
    }
}