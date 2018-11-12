using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiJWT.Controllers
{
    [Authorize]
    [RoutePrefix("api/cliente")]
    public class ClienteController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Models.Cliente cliente)
        {
            if (cliente == null)
                return BadRequest();

            using (SqlConnection connection =
                new System.Data.SqlClient.SqlConnection(
             System.Configuration.ConfigurationManager.ConnectionStrings
             ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" INSERT INTO CLIENTE " +
                    "(CLI_IDENTIFICACION, CLI_NOMBRE, CLI_EMAIL, " +
                    "CLI_FECHA_NACIMIENTO,CLI_ESTADO, USERNAME) VALUES " +
                    "(@CLI_IDENTIFICACION, @CLI_NOMBRE, @CLI_EMAIL, " +
                    "@CLI_FECHA_NACIMIENTO,@CLI_ESTADO, @USERNAME )");

                command.Parameters.AddWithValue("@CLI_IDENTIFICACION", cliente.cli_identificacion);
                command.Parameters.AddWithValue("@CLI_NOMBRE", cliente.cli_nombre);
                command.Parameters.AddWithValue("@CLI_EMAIL", cliente.cli_email);
                command.Parameters.AddWithValue("@CLI_FECHA_NACIMIENTO", cliente.cli_fec_nac);
                command.Parameters.AddWithValue("@CLI_ESTADO", cliente.cli_estado);
                command.Parameters.AddWithValue("@USERNAME", cliente.username);

                int filasAfectadas = command.ExecuteNonQuery();
                if(filasAfectadas > 0)
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }          
            }
        }

        [HttpPost]
        [Route("obtener")]
        public IHttpActionResult ObtenerCliente(Models.ClienteRequest request)
        {
            Models.Cliente cliente = new Models.Cliente();

            using (SqlConnection connection =
                     new System.Data.SqlClient.SqlConnection(
                         System.Configuration.ConfigurationManager.ConnectionStrings
                         ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" SELECT CLI_COD_CLIENTE, " +
                    "CLI_IDENTIFICACION, CLI_NOMBRE, CLI_EMAIL, CLI_FEC_NAC, " +
                        "CLI_ESTADO, USERNAME "+
               "FROM CLIENTE WHERE USERNAME = @USERNAME ", connection);

                command.Parameters.AddWithValue("@USERNAME", request.Username);
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    cliente.cli_cod_cliente = dr.GetInt32(0);
                    cliente.cli_identificacion = dr.GetString(1).Trim();
                    cliente.cli_nombre = dr.GetString(2).Trim();
                    cliente.cli_email = dr.GetString(3).Trim();
                    cliente.cli_fec_nac = dr.GetDateTime(4);
                    cliente.username = dr.GetString(5).Trim();
                }
                connection.Close();
            }
            return Ok(cliente);
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            List<Models.Cliente> listaClientes = new List<Models.Cliente>();

            using (SqlConnection connection =
                       new System.Data.SqlClient.SqlConnection(
                           System.Configuration.ConfigurationManager.ConnectionStrings
                           ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" SELECT CLI_COD_CLIENTE, "+
                    "CLI_IDENTIFICACION, CLI_NOMBRE, CLI_EMAIL, CLI_FECHA_NACIMIENTO, "+
                        "CLI_ESTADO, USERNAME FROM CLIENTE  ", connection);

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    Models.Cliente cliente = new Models.Cliente();
                    cliente.cli_cod_cliente = dr.GetInt32(0);
                    cliente.cli_identificacion = dr.GetString(1).Trim();
                    cliente.cli_nombre = dr.GetString(2).Trim();
                    cliente.cli_email = dr.GetString(3).Trim();
                    cliente.cli_fec_nac = dr.GetDateTime(4);
                    cliente.username = dr.GetString(5).Trim();
                 
                    listaClientes.Add(cliente);
                }

                connection.Close();
            }
            
            return Ok(listaClientes);
        }
    }
}
