using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSendMail.App_Code
{
    internal class RegistroPendiente
    {
        public string Correo { get; set; }
        public string HashRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string TipoRegistro { get; set; }
        public string NombreCompleto { get; set; }
        public int IDRegistro { get; set; }
        public int IDTrabajador { get; set; }
        public byte[] Foto { get; set; }
    }
}
