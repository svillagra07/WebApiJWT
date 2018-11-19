using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiJWT.Models
{
    public class Transferencia
    {
        public int tra_cod_transferencia { get; set; }
        public int cli_cod_cliente { get; set; }
        public int tra_cuenta_origen { get; set; }
        public int tra_cuenta_destino { get; set; }
        public string tra_descripcion { get; set; }
        public string tra_estado { get; set; }
        public DateTime tra_fecha { get; set; }
        public decimal tra_monto { get; set; }
    }
}