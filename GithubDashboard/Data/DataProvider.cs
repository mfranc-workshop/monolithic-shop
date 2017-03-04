using System.Collections.Generic;

namespace GithubDashboard.Data
{
    public class Buyer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(decimal price, string name, int id)
        {
            Id = id;
            Price = price;
            Name = name;
        }
    }

    public class DataProvider
    {
        
    }
}
