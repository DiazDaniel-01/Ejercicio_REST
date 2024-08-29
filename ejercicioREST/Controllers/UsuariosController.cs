using ejercicioREST.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace ejercicioREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Requiere autenticación para acceder a los métodos
    public class UsuariosController : ControllerBase
    {
        private readonly string con;

        public UsuariosController(IConfiguration configuration)
        {
            con = configuration.GetConnectionString("conexion");
        }

        // GET: api/usuarios
        [HttpGet("ListarUsuarios")]
       
        public IActionResult GetUsuarios()
        {
            try
            {
                var listaUsuarios = new List<Usuarios>();

                using (SqlConnection connection = new(con))
                {
                    connection.Open();

                    string query = @"SELECT IdUsuario, Nombre, Apellido, Correo, Contraseña, Activo 
                                     FROM USUARIO";

                    using (SqlCommand command = new(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var usuario = new Usuarios
                                {
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                                    Correo = reader.GetString(reader.GetOrdinal("Correo")),
                                    Contraseña = reader.GetString(reader.GetOrdinal("Contraseña")),
                                    Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
                                };

                                listaUsuarios.Add(usuario);
                            }
                        }
                    }
                }
                return Ok(listaUsuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // POST: api/usuarios
        [HttpPost]
        public IActionResult InsertUsuario([FromBody] Usuarios nuevoUsuario)
        {
            try
            {
                using (SqlConnection connection = new(con))
                {
                    connection.Open();

                    string query = @"INSERT INTO USUARIO (IdUsuario, Nombre, Apellido, Correo, Contraseña, Activo) 
                                     VALUES (@Nombre, @Apellido, @Correo, @Contraseña, @Activo)";

                    using (SqlCommand command = new(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nuevoUsuario.IdUsuario);
                        command.Parameters.AddWithValue("@Nombre", nuevoUsuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", nuevoUsuario.Apellido);
                        command.Parameters.AddWithValue("@Correo", nuevoUsuario.Correo);
                        command.Parameters.AddWithValue("@Contraseña", nuevoUsuario.Contraseña);
                        command.Parameters.AddWithValue("@Activo", nuevoUsuario.Activo);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Usuario insertado exitosamente.");
                        }
                        else
                        {
                            return BadRequest("Error al insertar el usuario.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] Usuarios usuario)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(con))
                {
                    connection.Open();

                    string query = @"UPDATE USUARIO 
                                     SET Nombre = @Nombre, 
                                         Apellido = @Apellido, 
                                         Correo = @Correo, 
                                         Contraseña = @Contraseña, 
                                         Activo = @Activo 
                                     WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", id);
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Correo", usuario.Correo);
                        command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                        command.Parameters.AddWithValue("@Activo", usuario.Activo);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Usuario actualizado exitosamente.");
                        }
                        else
                        {
                            return NotFound("Usuario no encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // DELETE: api/usuarios/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(con))
                {
                    connection.Open();

                    string query = "DELETE FROM USUARIO WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Usuario eliminado exitosamente.");
                        }
                        else
                        {
                            return NotFound("Usuario no encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
    }
}
