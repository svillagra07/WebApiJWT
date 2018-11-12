using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiJWT.Models
{
    public class ClienteRequest
    {
        public int Cli_cod_cliente { get; set; }
        public string Username { get; set; }
    }
}