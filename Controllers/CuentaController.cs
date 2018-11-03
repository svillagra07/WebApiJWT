using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiJWT.Models;
namespace WebApiJWT.Controllers
{
    [Authorize]
    [RoutePrefix("api/cuenta")]
    public class CuentaController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ObtenerCuentas(CuentaRequest request)
        {
            List<Cuenta> cuentas = new List<Cuenta>();

            using (SqlConnection connection =
                     new System.Data.SqlClient.SqlConnection(
                         System.Configuration.ConfigurationManager.ConnectionStrings
                         ["BANKINGConnection"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" SELECT CUE_COD_CUENTA, CLI_COD_CLIENTE, " +
                    " CUE_DESCRIPCION, CUE_SALDO, CUE_ESTADO, CUE_MONEDA " +
               "FROM CUENTA WHERE CLI_COD_CLIENTE = @CLI_COD_CLIENTE ", connection);

                command.Parameters.AddWithValue("@CLI_COD_CLIENTE", request.cli_cod_cliente);
                connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Cuenta cuenta = new Cuenta();

                    cuenta.cue_cod_cuenta = dr.GetInt32(0);
                    cuenta.cli_cod_cliente = dr.GetInt32(1);
                    cuenta.cue_descripcion = dr.GetString(2).Trim();
                    cuenta.cue_saldo = dr.GetDecimal(3);
                    cuenta.cue_estado = dr.GetString(4);
                    cuenta.cue_moneda = dr.GetString(5);

                    cuentas.Add(cuenta);
                }
                connection.Close();
            }

            return Ok(cuentas);
        }
    }
}
