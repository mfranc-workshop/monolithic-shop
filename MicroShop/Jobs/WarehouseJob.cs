﻿using System.Data.Entity;
using System.Linq;
using MicroShop.Data;
using MicroShop.EmailHelpers;
using MicroShop.Services;
using Quartz;

namespace MicroShop.Jobs
{
    public class WarehouseJob : IJob
    {
        private readonly IEmailService _emailService;

        public WarehouseJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Execute(IJobExecutionContext jobContext)
        {
            using (var context = new MainDatabaseContext())
            {
                var orders = context.Orders
                    .Where(x => x.Status == OrderStatus.WaitingForWarehouse)
                    .Include(o => o.Buyer)
                    .Include(o => o.ProductOrders.Select(p => p.Product))
                    .ToList();

                foreach (var order in orders)
                {
                    var cannotFulfill = (from productOrder in order.ProductOrders
                        let productWarehouse = context.ProductsWarehouse.FirstOrDefault(pw => pw.Product.Id == productOrder.ProductId)
                        where productWarehouse.NumberAvailable < productOrder.Count
                        select productOrder).Any();

                    if (cannotFulfill)
                    {
                        break;
                    }
                    else
                    {
                        foreach (var productOrder in order.ProductOrders)
                        {
                            var productW = context.ProductsWarehouse
                                .Include(pw => pw.Product)
                                .FirstOrDefault(x => x.Product.Id == productOrder.ProductId);

                            productW.NumberAvailable = productW.NumberAvailable - productOrder.Count;
                        }

                        _emailService.SendEmail(order.Buyer.Email, EmailType.OrderSend);
                        order.Delivering();
                    }
                }

                context.SaveChanges();
            }
        }
    }
}