using ejercicioREST.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ejercicioREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Orders_HistoryController : ControllerBase
    {

        private readonly string con;

        public Orders_HistoryController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }


        // GET: api/orders_completes
        [HttpGet("orders_complete")]
        public IEnumerable<Orders_History> GetOrderComplete()
        {
            var list_Orders = new List<Orders_History>();

            using (SqlConnection connection = new(con))
            {
                connection.Open();

                string query = @"SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE, 
                                    (QUANTITY * PRICE) AS NetAmount 
                                FROM ORDERS_HISTORY 
                                WHERE STATUS = 'EXECUTED'";

                using (SqlCommand command = new(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Orders_History
                            {
                                TxNumber = reader.GetInt32(reader.GetOrdinal("TX_NUMBER")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("ORDER_DATE")),
                                Action = reader.GetString(reader.GetOrdinal("ACTION")),
                                Status = reader.GetString(reader.GetOrdinal("STATUS")),
                                Symbol = reader.GetString(reader.GetOrdinal("SYMBOL")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY")),
                                Price = reader.GetDecimal(reader.GetOrdinal("PRICE")),
                                NetAmount = reader.GetDecimal(reader.GetOrdinal("NetAmount"))
                            };

                            list_Orders.Add(order);
                        }
                    }
                }


                return list_Orders;
            }
        }

        // GET api/<Orders_HistoryController>/5 
        [HttpGet("orders-by-year/{year}")]
        public IEnumerable<Orders_History> GetOrdersByYear(int year)
        {
            var list_Year = new List<Orders_History>();

            using (SqlConnection connection = new(con))
            {
                connection.Open();

                string query = @"SELECT * FROM ORDERS_HISTORY WHERE YEAR(ORDER_DATE) = @Year";

                using (SqlCommand command = new(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Orders_History
                            {
                                TxNumber = reader.GetInt32(reader.GetOrdinal("TX_NUMBER")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("ORDER_DATE")),
                                Action = reader.GetString(reader.GetOrdinal("ACTION")),
                                Status = reader.GetString(reader.GetOrdinal("STATUS")),
                                Symbol = reader.GetString(reader.GetOrdinal("SYMBOL")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY")),
                                Price = reader.GetDecimal(reader.GetOrdinal("PRICE")),
                            };

                            list_Year.Add(order);
                        }
                    }
                }
            }
            return list_Year;
        }

        // POST api/insert-order>
        [HttpPost("insert-order")]
        public IActionResult InsertOrder([FromBody] Orders_History newOrder)
        {
            using (SqlConnection connection = new(con))
            {
                connection.Open();
                string query = @"
                INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE)
                VALUES (@OrderDate, @Action, 'PENDING', @Symbol, @Quantity, @Price);
                ";
                using (SqlCommand command = new(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                    command.Parameters.AddWithValue("@Action", newOrder.Action);
                    command.Parameters.AddWithValue("@Symbol", newOrder.Symbol);
                    command.Parameters.AddWithValue("@Quantity", newOrder.Quantity);
                    command.Parameters.AddWithValue("@Price", newOrder.Price);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Verificar si la inserción fue exitosa
                    if (rowsAffected > 0)
                    {
                        return Ok("Orden insertada exitosamente.");
                    }
                    else
                    {
                        return BadRequest("Error al insertar la orden.");
                    }
                }
            }
        }

        // PUT api/<Orders_HistoryController>/5
        [HttpPut("{txNumber}")]
        public IActionResult PutOrder(int txNumber, [FromBody] Orders_History order)
        {
                // Cadena de conexión obtenida del constructor a través de IConfiguration
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                string query = @"UPDATE ORDERS_HISTORY
                            SET ORDER_DATE = @ORDER_DATE, 
                                ACTION = @ACTION, 
                                STATUS = @STATUS, 
                                SYMBOL = @SYMBOL, 
                                QUANTITY = @QUANTITY, 
                                PRICE = @PRICE
                                WHERE TX_NUMBER = @TX_NUMBER";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Asignar valores a los parámetros
                    command.Parameters.AddWithValue("@TX_NUMBER", txNumber);
                    command.Parameters.AddWithValue("@ORDER_DATE", order.OrderDate);
                    command.Parameters.AddWithValue("@ACTION", order.Action ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@STATUS", order.Status ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SYMBOL", order.Symbol ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@QUANTITY", order.Quantity);
                    command.Parameters.AddWithValue("@PRICE", order.Price);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Se actualizo exitosamente." });
                    }
                    else
                    {
                        return NotFound(new { message = "No funciono." });
                    }
                }
            }


        }

        // DELETE api/<Orders_HistoryController>/5
        [HttpDelete("{txNumber}")]
        public IActionResult DeleteOrder(int txNumber)
        {
                // Cadena de conexión obtenida del constructor a través de IConfiguration
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                string query = "DELETE FROM ORDERS_HISTORY WHERE TX_NUMBER = @TxNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TxNumber", txNumber);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Orden eliminada exitosamente." });
                    }
                    else
                    {
                        return NotFound(new { message = "No funciono." });
                    }
                }
            }
        }
    }
}
