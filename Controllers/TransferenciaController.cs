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
    [RoutePrefix("api/transferencia")]
    public class TransferenciaController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Models.Transferencia transferencia)
        {
            if (transferencia == null)
                return BadRequest();

            using (SqlConnection connection =
                new System.Data.SqlClient.SqlConnection(
             System.Configuration.ConfigurationManager.ConnectionStrings
             ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" INSERT INTO TRANSFERENCIA " +
                    "(  cli_cod_cliente, "+
                    "    tra_cuenta_origen, "+ 
                    "    tra_cuenta_destino, "+
                    "    tra_descripcion, "+
                    "    tra_estado, "+
                    "    tra_fecha, "+ 
                    "    tra_monto) VALUES " +
                    "( @cli_cod_cliente, " +
                    "    @tra_cuenta_origen, " +
                    "    @tra_cuenta_destino, " +
                    "    @tra_descripcion, " +
                    "    @tra_estado, " +
                    "    @tra_fecha, " +
                    "    @tra_monto)",connection);

                command.Parameters.AddWithValue("@cli_cod_cliente", transferencia.cli_cod_cliente);
                command.Parameters.AddWithValue("@tra_cuenta_origen", transferencia.tra_cuenta_origen);
                command.Parameters.AddWithValue("@tra_cuenta_destino", transferencia.tra_cuenta_destino );
                command.Parameters.AddWithValue("@tra_descripcion",  transferencia.tra_descripcion);
                command.Parameters.AddWithValue("@tra_estado", transferencia.tra_estado );
                command.Parameters.AddWithValue("@tra_fecha", transferencia.tra_fecha );
                command.Parameters.AddWithValue("@tra_monto", transferencia.tra_monto );

                connection.Open();

                int filasAfectadas = command.ExecuteNonQuery();
                if (filasAfectadas > 0)
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
        public IHttpActionResult ObtenerTransferencias(Models.TransferenciaRequest request)
        {
            List<Models.Transferencia> transferencias = new List<Models.Transferencia>();

            using (SqlConnection connection =
                     new System.Data.SqlClient.SqlConnection(
                         System.Configuration.ConfigurationManager.ConnectionStrings
                         ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" SELECT tra_cod_transferencia, cli_cod_cliente, tra_cuenta_origen, tra_cuenta_destino, tra_descripcion, tra_estado, tra_fecha, tra_monto "+
                                                    " FROM  Transferencia "+
                                                    " Where cli_cod_cliente = @cli_cod_cliente ", connection);

                command.Parameters.AddWithValue("@cli_cod_cliente", request.Cli_cod_cliente);
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Models.Transferencia transferencia = new Models.Transferencia();

                    transferencia.tra_cod_transferencia = dr.GetInt32(0);
                    transferencia.cli_cod_cliente = dr.GetInt32(1);
                    transferencia.tra_cuenta_origen = dr.GetInt32(2);
                    transferencia.tra_cuenta_destino = dr.GetInt32(3);
                    transferencia.tra_descripcion = dr.GetString(4);
                    transferencia.tra_estado = dr.GetString(5);
                    transferencia.tra_fecha = dr.GetDateTime(6);
                    transferencia.tra_monto = dr.GetDecimal(7);

                    transferencias.Add(transferencia);
                }
                connection.Close();
            }

            return Ok(transferencias);
        }
    }
}
