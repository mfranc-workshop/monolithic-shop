using System.Collections.Generic;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class ShopController: Controller
    {
        public IActionResult Index()
        {
            var mockedListOfProducts = new List<Product>
            {
                new Product(10.0m, "Bike", 1),
                new Product(5.0m, "Scooter", 2),
                new Product(4.99m, "Ball", 3),
                new Product(89.99m, "Helmet", 4)
            };

            return View(mockedListOfProducts);
        }
    }
}
