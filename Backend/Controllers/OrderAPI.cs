using Backend.Contracts;
using Business_Layer;
using Contracts.Contracts;
using Contracts.Contracts.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderAPI : ControllerBase
    {
        [HttpGet("GetTotalRevenuesAsync", Name = "GetTotalRevenuesAsync")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<RevenueDto>> GetTotalRevenuesAsync()
        {
            RevenueDto result = await clsOrder.GetTotalRevenuesAsync();
            if (result == null) return NotFound("Total Revenue In Month Not Found!");
            return Ok(result);
        }


        [HttpGet("GetRecentSalesAsync", Name = "GetRecentSalesAsync")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<RecentSalesDTO>>> GetRecentSalesAsync()
        {
            List<RecentSalesDTO> result = await clsOrder.GetRecentSalesAsync();
            if (result == null) return NotFound("No recent sales found!");
            return Ok(result);
        }


        [HttpGet("GetProductsForAllMonthsAsync", Name = "GetProductsForAllMonthsAsync")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<OrdersPerMonthDTO>>> GetProductsForAllMonthsAsync()
        {
            List<OrdersPerMonthDTO> result = await clsOrder.GetProductsForAllMonthsAsync();
            if (result == null) return NotFound("No orders found!");
            return Ok(result);
        }



        [HttpGet("GetCustomerOrders/{CustomerID}", Name = "GetCustomerOrders")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public async Task<ActionResult<List<CustomerOrdersDTO>>> GetCustomerOrders(int CustomerID)
        {
            if(CustomerID < 0)
            {
                return BadRequest("CustomerID should be greater than 0");  
            }
            List<CustomerOrdersDTO> result = await clsOrder.GetCustomerOrders(CustomerID);
            if (result == null) return NotFound("No orders found!");
            return Ok(result);
        }


        [HttpGet("{id}", Name = "GetOrderByID")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]

        public ActionResult<OrderRequestDTO> GetOrderByID(int id)
        {
            if (id < 1) return BadRequest("ID should be strictly greater than 0");
            clsOrder order = clsOrder.Find(id);
            if (order == null) return NotFound("Order Was Not Found !");
            return Ok(order.orderRequestDTO);
        }



        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        [HttpPost(Name = "Add")]
        public async Task<ActionResult<OrderRequestDTO>> AddOrder(OrderRequestDTO orderRequestDTO)
        {
            if (orderRequestDTO.Quantity < 0 || orderRequestDTO.TotalAmount < 0 || orderRequestDTO.DriverID < 1 || orderRequestDTO.CustomerID < 1 || orderRequestDTO.ProductID < 1)
                return BadRequest("Invalid order data");

            clsOrder order = new clsOrder(orderRequestDTO);
            if (!await order.SaveAsync())
                return StatusCode(500, "Couldn't save the order");

            orderRequestDTO.OrderID = order.OrderID;
            return CreatedAtRoute("GetOrderByID", new { id = orderRequestDTO.OrderID }, orderRequestDTO);
        }


        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("{id}", Name = "UpdateOrder")]
        public async Task<ActionResult<OrderRequestDTO>> UpdateOrder(int id, OrderRequestDTO orderRequestDTO)
        {
            if (id < 0)
            {
                return BadRequest("OrderID should be greater than 0");
            }

            //if (orderRequestDTO == null || orderRequestDTO.Quantity < 0 || orderRequestDTO.TotalAmount < 0)
            //{
            //    return BadRequest("Order data is not valid!");
            //}

            clsOrder order = clsOrder.Find(id);
            if (order == null)
            {
                return NotFound($"Order with ID = {id} was not found!");
            }

            // Update the order's properties from the DTO.
            //order.TotalAmount = orderRequestDTO.TotalAmount;
            order.OrderStatus = orderRequestDTO.OrderStatus;
            //order.Quantity = orderRequestDTO.Quantity;
            //order.OrderDate = orderRequestDTO.OrderDate;
            //order.ReceiveDate = orderRequestDTO.ReceiveDate;
            //order.Address = orderRequestDTO.Address;
            //order.Feedback = orderRequestDTO.Feedback;

            // Save the updated order asynchronously.
            if (!await order.SaveAsync())
            {
                return StatusCode(500, "Couldn't update the order");
            }

            orderRequestDTO.OrderID = order.OrderID;
            return Ok(order.orderRequestDTO);
        }

    }
}
