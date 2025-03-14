using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;
using System.ComponentModel;
using Contracts.Contracts;

namespace DataAccess_Layer
{
    public class clsOrderData
    {

        public static int AddNewOrder(decimal TotalAmount, byte OrderStatus, int Quantity, DateTime OrderDate, DateTime ReceiveDate, string Address, string Feedback, int CustomerID, int ProductID, int DriverID)
        {
            int ID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Order ( 
                            TotalAmount, OrderStatus, Quantity, OrderDate, ReceiveDate, Address, Feedback, CustomerID, ProductID, DriverID)
                            VALUES (@TotalAmount, @OrderStatus, @Quantity, @OrderDate, @ReceiveDate, @Address, @Feedback, @CustomerID, @ProductID, @DriverID);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
            command.Parameters.AddWithValue("@OrderStatus", OrderStatus);
            command.Parameters.AddWithValue("@Quantity", Quantity);
            command.Parameters.AddWithValue("@OrderDate", OrderDate);
            command.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Feedback", Feedback);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            command.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    ID = insertedID;
                }
            }

            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);

            }

            finally
            {
                connection.Close();
            }


            return ID;
        }
        public static bool UpdateOrder(int OrderID, decimal TotalAmount, byte OrderStatus, int Quantity, DateTime OrderDate, DateTime ReceiveDate, string Address, string Feedback, int CustomerID, int ProductID, int DriverID)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update Order set 
                            
TotalAmount = @TotalAmount,
OrderStatus = @OrderStatus,
Quantity = @Quantity,
OrderDate = @OrderDate,
ReceiveDate = @ReceiveDate,
Address = @Address,
Feedback = @Feedback,
CustomerID = @CustomerID,
ProductID = @ProductID,
DriverID = @DriverID
                            where OrderID = @OrderID";


            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderID", OrderID);
            command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
            command.Parameters.AddWithValue("@OrderStatus", OrderStatus);
            command.Parameters.AddWithValue("@Quantity", Quantity);
            command.Parameters.AddWithValue("@OrderDate", OrderDate);
            command.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Feedback", Feedback);
            command.Parameters.AddWithValue("@CustomerID", CustomerID);
            command.Parameters.AddWithValue("@ProductID", ProductID);
            command.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }
        public static bool GetOrderInfoByOrderID(int OrderID, ref decimal TotalAmount, ref byte OrderStatus, ref int Quantity, ref DateTime OrderDate, ref DateTime ReceiveDate, ref string Address, ref string Feedback, ref int CustomerID, ref int ProductID, ref int DriverID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Order WHERE OrderID = @OrderID";


            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    // The record was found
                    isFound = true;

                    TotalAmount = (decimal)reader["TotalAmount"];
                    OrderStatus = (byte)reader["OrderStatus"];
                    Quantity = (int)reader["Quantity"];
                    OrderDate = (DateTime)reader["OrderDate"];
                    ReceiveDate = (DateTime)reader["ReceiveDate"];
                    Address = (string)reader["Address"];
                    Feedback = (string)reader["Feedback"];
                    CustomerID = (int)reader["CustomerID"];
                    ProductID = (int)reader["ProductID"];
                    DriverID = (int)reader["DriverID"];

                }
                else
                {
                    // The record was not found
                    isFound = false;
                }

                reader.Close();


            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }
        public static bool DeleteOrder(int OrderID)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Delete Order 
                            where OrderID = @OrderID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                connection.Open();

                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {

                connection.Close();

            }

            return (rowsAffected > 0);

        }
        public static bool IsOrderExist(int OrderID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Order WHERE OrderID = @OrderID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }
        public static DataTable GetAllOrder()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "select * from Order";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)

                {
                    dt.Load(reader);
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }

        public static async Task<RevenueDto> GetTotalRevenuesAsync()
        {
            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (var command = new SqlCommand("GetTotalRevenues", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Declare output parameters
                    command.Parameters.Add(new SqlParameter("@CurrentTotalRevenue", SqlDbType.SmallMoney) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@RevenuePercentage", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@CurrentSales", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@SalesPercentage", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@CurrentTodaySales", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@TodaySalesPercentage", SqlDbType.Int) { Direction = ParameterDirection.Output });

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return new RevenueDto
                    {
                        CurrentTotalRevenue = (command.Parameters["@CurrentTotalRevenue"].Value != DBNull.Value) ? Convert.ToDecimal(command.Parameters["@CurrentTotalRevenue"].Value) : 0,
                        RevenuePercentage = (command.Parameters["@RevenuePercentage"].Value != DBNull.Value) ? Convert.ToInt32(command.Parameters["@RevenuePercentage"].Value) : 0,
                        CurrentSales = (command.Parameters["@CurrentSales"].Value != DBNull.Value) ? Convert.ToInt32(command.Parameters["@CurrentSales"].Value) : 0,
                        SalesPercentage = (command.Parameters["@SalesPercentage"].Value != DBNull.Value) ? Convert.ToInt32(command.Parameters["@SalesPercentage"].Value) : 0,
                        CurrentTodaySales = (command.Parameters["@CurrentTodaySales"].Value != DBNull.Value) ? Convert.ToInt32(command.Parameters["@CurrentTodaySales"].Value) : 0,
                        TodaySalesPercentage = (command.Parameters["@TodaySalesPercentage"].Value != DBNull.Value) ? Convert.ToInt32(command.Parameters["@TodaySalesPercentage"].Value) : 0
                    };
                }
            }
        }

        public static async Task<List<RecentSalesDTO>> GetRecentSalesAsync()
        {
            var salesList = new List<RecentSalesDTO>();

            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (var command = new SqlCommand("RecentSales", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            salesList.Add(new RecentSalesDTO
                            {
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                            });
                        }
                    }
                }
            }

            return salesList;
        }

        public static async Task<List<OrdersPerMonthDTO>> GetProductsForAllMonthsAsync()
        {
            var salesList = new List<OrdersPerMonthDTO>();

            using (var connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (var command = new SqlCommand("GetProductsForAllMonths", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            salesList.Add(new OrdersPerMonthDTO
                            {
                                Month = Convert.ToInt32(reader["Month"]),
                                Orders = Convert.ToInt32(reader["Orders"])
                            });
                        }
                    }
                }
            }

            return salesList;
        }


    }
}
