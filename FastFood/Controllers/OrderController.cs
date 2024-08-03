using FastFood.Models.Order;
using FastFood.Rules;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRule _orderRule;

        public OrderController(OrderRule orderRule)
        {
            _orderRule = orderRule;
        }

        public IActionResult CreateOrder(Order order)
        {
            _orderRule.InsertOrder(order);
            return RedirectToAction("OrderConfirmation");
        }
    }
}