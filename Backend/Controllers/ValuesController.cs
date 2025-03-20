using Contracts.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrderOptimizationController : ControllerBase
{
    /// <summary>
    /// Initiates the full optimization process and returns the results.
    /// </summary>
    [HttpPost("optimize")]
    public async Task<IActionResult> OptimizeOrders()
    {
        // Define the planning date as today in "yyyy-MM-dd" format.
        string planningDate = "2025-03-29";

        // Retrieve orders from your real DB using your DatabaseService.
        List<DeliveringOrders> orders = await DatabaseService.GetOrdersAsync(planningDate);

        // Collect the responses from order creation.
        List<string> createOrderResponses = new List<string>();
        foreach (var order in orders)
        {
            string response = await OptimoRouteService.CreateOrderAsync(order, planningDate);
            createOrderResponses.Add(response);
        }

        // Optional: wait for a short time to ensure orders are processed.
        await Task.Delay(2000);

        // Start planning using the orders and reference existing drivers in OptimoRoute.
        string planningResponse = await OptimoRouteService.StartPlanningAsync(planningDate, orders);

        await Task.Delay(5000);

        // Retrieve the optimized routes for the planning date.
        string routesResponse = await OptimoRouteService.GetRoutesByDateAsync(planningDate);

        // Build a result object to return to the frontend.
        var result = new
        {
            PlanningDate = planningDate,
            CreateOrderResponses = createOrderResponses,
            PlanningResponse = planningResponse,
            RoutesResponse = routesResponse
        };

        // Return a 200 OK response with the result as JSON.
        return Ok(result);
    }

}
