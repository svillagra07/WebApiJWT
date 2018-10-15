using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiJWT.Models
{
    public class Usuario
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}