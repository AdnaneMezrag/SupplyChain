using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Business_Layer;
using Contracts.Contracts;

public class OptimoRouteService
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiKey = "1cb0813656347cf2cce1ee96e3df569evApfrKuE"; // Replace with your API key

    /// <summary>
    /// Creates or syncs an order in the OptimoRoute system.
    /// Uses the "SYNC" operation so that if the order exists, it is updated; if not, it is created.
    /// Now includes load and time window parameters.
    /// </summary>
    public static async Task<string> CreateOrderAsync(DeliveringOrders order, string date)
    {
        // Use valid addresses in Algeria.
        string validAddress = order.Address;

        var payload = new
        {
            operation = "SYNC", // Create or update the order
            orderNo = order.OrderId.ToString(),
            type = "D",         // Delivery order
            date = date,
            location = new
            {
                address = validAddress,
                locationNo = $"LOC{order.OrderId:D3}",
                locationName = $"Order Location {order.OrderId}",
                acceptPartialMatch = true
            },
            duration = order.ServiceTime,
            load1 = 1,          // Adding a load parameter to force assignment
            twFrom = "08:00",   // Time window start
            twTo = "09:00"      // Time window end
        };

        string url = $"https://api.optimoroute.com/v1/create_order?key={ApiKey}";
        string jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    /// <summary>
    /// Starts planning by sending orders to OptimoRoute and referencing the existing drivers.
    /// Here we use useDrivers to reference drivers already configured in your account.
    /// </summary>
    public static async Task<string> StartPlanningAsync(string date, List<DeliveringOrders> orders)
    {
        var payload = new
        {
            date = date,
            useOrderObjects = orders.ConvertAll(o => new { orderNo = o.OrderId.ToString() }),
            useDrivers = new List<object>
            {
                new { driverSerial = "001" },
                new { driverSerial = "002" }
            }
        };

        string url = $"https://api.optimoroute.com/v1/start_planning?key={ApiKey}";
        string jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    /// <summary>
    /// Retrieves optimized routes for the specified planning date.
    /// Now includes the route polyline in the output.
    /// </summary>
    public static async Task<string> GetRoutesByDateAsync(string date)
    {
        try
        {
            string url = $"https://api.optimoroute.com/v1/get_routes?key={ApiKey}&date={date}&includeRoutePolyline=true";
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    /// <summary>
    /// Full integration process:
    /// 1. Create/Sync orders in OptimoRoute.
    /// 2. Start planning using orders and existing drivers in OptimoRoute.
    /// 3. Retrieve optimized routes.
    /// </summary>
    //public async Task RunOptimizationProcess()
    //{
    //    string planningDate = "2025-03-26"; // The planning date

    //    // Retrieve orders from the simulated database.
    //    List<DeliveringOrders> orders = await DatabaseService.GetOrdersAsync(planningDate);

    //    // 1. Create/Sync each order in OptimoRoute.
    //    Console.WriteLine("Creating/Syncing Orders:");
    //    foreach (var order in orders)
    //    {
    //        string createOrderResponse = await CreateOrderAsync(order, planningDate);
    //        Console.WriteLine($"Order {order.OrderId} Create Response: {createOrderResponse}");
    //    }

    //    // Optional: Wait a bit for the orders to be processed.
    //    await Task.Delay(2000);

    //    // 2. Start planning using the orders and existing drivers in OptimoRoute.
    //    string planningResponse = await StartPlanningAsync(planningDate, orders);
    //    Console.WriteLine("Start Planning Response: " + planningResponse);

    //    await Task.Delay(5000);

    //    // 3. Retrieve optimized routes for the same date.
    //    string routesResponse = await GetRoutesByDateAsync(planningDate);
    //    Console.WriteLine("Optimized Routes: " + routesResponse);
    //}

}

public static class DatabaseService
{
    public static async Task<List<DeliveringOrders>> GetOrdersAsync(string date)
    {
        await Task.Delay(100);
        return await clsOrder.GetDeliveringOrders();
    }

}
