using System.Collections.Generic;
using GithubDashboard.Data;
using Microsoft.AspNetCore.Mvc;

namespace GithubDashboard.Controllers
{
    public class AdminController: Controller
    {
        [HttpGet]
        [Route("/initdb")]
        public bool Init()
        {
            var mockedListOfProducts = new List<Product>
            {
                new Product(10.0m, "Bike", 1),
                new Product(5.0m, "Scooter", 2),
                new Product(4.99m, "Ball", 3),
                new Product(89.99m, "Helmet", 4)
            };


            using (var context = new MainDatabaseContext())
            {
                foreach (var mockedListOfProduct in mockedListOfProducts)
                {
                    context.Products.Add(mockedListOfProduct);
                }

                context.SaveChanges();

                return true;
            }
        }
    }
}
