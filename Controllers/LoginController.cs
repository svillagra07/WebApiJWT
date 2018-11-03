using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Security;
using WebApiJWT.Models;

namespace WebApiJWT.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            if (Membership.ValidateUser(login.Username, login.Password))
            {
                var token = TokenGenerator.GenerateTokenJwt(login.Username);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(AppUsuario usuario)
        {
            if (usuario == null)
                return BadRequest();
            MembershipCreateStatus status = new MembershipCreateStatus();
            MembershipUser user = Membership.CreateUser(usuario.Username, usuario.Password, usuario.Email,"aaa","bbb",true,out status);

            if (status == MembershipCreateStatus.Success)
            {
                return Ok(usuario);
            }
            else
            {
                return InternalServerError();
            }
        }

        private bool ReconfigurarUsuario(AppUsuario usuario)
        {
            bool resultado = false;

            using (SqlConnection connection =
             new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SamaConnectionString"].ConnectionString))
            {
                SqlCommand command =
                    new SqlCommand(" UPDATE AppUsuario SET Password = @Password, Estado = @Estado, IntentosFallidos = 0, FechaUltimaActividad = @FechaUltimaActividad " +
                                   " WHERE Codigo = @Codigo AND Username = @Username ", connection);

                command.Parameters.AddWithValue("@Username", usuario.Username);
                command.Parameters.AddWithValue("@Password", usuario.Password);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Estado", usuario.Estado);
                command.Parameters.AddWithValue("@IntentosFallidos", usuario.IntentosFallidos);
                command.Parameters.AddWithValue("@FechaUltimaActividad", DateTime.Now);

                connection.Open();

                int filasAfectadas = command.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    resultado = true;
                }

                connection.Close();
            }

            return resultado;
        }

        private bool RegistrarUsuario(AppUsuario usuario)
        {
            bool resultado = false;

            using (SqlConnection connection =
               new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SamaConnectionString"].ConnectionString))
            {
                SqlCommand command =
                    new SqlCommand(" INSERT INTO AppUsuario (Username,Nombre,Password,Email,Estado,IntentosFallidos,FechaInclusion,FechaUltimaActividad) " +
                                   " VALUES ( @Username,@Nombre,@Password,@Email,@Estado,@IntentosFallidos,@FechaInclusion,@FechaUltimaActividad )", connection);

                command.Parameters.AddWithValue("@Username", usuario.Username);
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Password", usuario.Password);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Estado", usuario.Estado);
                command.Parameters.AddWithValue("@IntentosFallidos", usuario.IntentosFallidos);
                command.Parameters.AddWithValue("@FechaInclusion", usuario.FechaInclusion);
                command.Parameters.AddWithValue("@FechaUltimaActividad", usuario.FechaUltimaActividad);

                connection.Open();

                int filasAfectadas = command.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    resultado = true;
                }

                connection.Close();
            }

            return resultado;

        }

        private bool ValidarUsuario(LoginRequest login)
        {
            bool resultado = false;

            using (SqlConnection connection =
                 new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SamaConnectionString"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(" select Password from AppUsuario where Username = @Username ", connection);

                command.Parameters.AddWithValue("@Username", login.Username);

                connection.Open();

                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    if (login.Password.Equals(dr.GetString(0)))
                    {
                        resultado = true;
                    }
                }
                connection.Close();
            }

            return resultado;
        }
    }
}
