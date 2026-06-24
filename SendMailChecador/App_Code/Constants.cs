using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSendMail.App_Code
{
    public class Constants
    {
        public Constants()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region MensajesAlUsuario

        public const string sInsercionExito = "El registro se inserto con éxito.";
        public const string sEnvioExito = "La nota ha sido enviada exitosamente.";
        public const string sInsercionError = "El registro no pudo ser insertado.";
        public const string sModificacionExito = "El registro se modifico con éxito.";
        public const string sModificacionExito2 = "La petición fue enviada con éxito.";
        public const string sModificacionError = "El registro no pudo ser modificado.";
        public const string sEmpleadoyautoExiste = "El registro no pudo ser insertado porque ya existe el mismo empleado con el mismo auto.";
        public const string sAutobajaovendido = "El registro no pudo ser insertado porque el auto esta en estatus de baja, vendido, siniestro o robo.";
        public const string sEmpleadoconautoyestatus = "El registro no pudo ser insertado porque el empleado tiene otro auto con estatus disponible o asignado o reasignado.";
        public const string sFaltaelemento = "El registro no pudo ser insertado porque faltan elementos en la estructura.";
        public const string sExistepolizaeinciso = "La operación no pudo ser completada porque ya existe esa misma póliza e inciso para ese auto.";
        public const string sEliminacionExito = "El registro se elimino con éxito.";
        public const string sEliminacionError = "El registro no pudo ser eliminado.";
        public const string sPermisoExito = "Los permisos se actualizaron con éxito.";
        public const string sPermisoError = "Ocurrio un error al intentar actualizar los permiso.";
        public const string SErrorAuto = "Ocurrio un error al intentar agregar, el auto no existe, es necesario darlo de alta en el catalogo de autos.";
        public const string sProductosActExito = "Los productos se actualizaron con éxito.";
        public const string sProductosActError = "Ocurrio un error al intentar actualizar los productos.";
        public const string sLlaveExiste = "La clave ya existe";

        public const string sPolizaExiste = "Error!, Ya existe el número de poliza en la base de datos.";
        public const string sEmpleadoExiste = "Error!, Ya existe un usuario con este mismo ID.";

        public const string sDesvincularExito = "Se ha desvinculado con éxito al empleado.";
        public const string sDesvincularError = "Error!, No se ha desvinculado al empleado.";


        #endregion
        #region Store_Procedures

        public const string sp_registro_checador = "sp_registro_checador";
        public const string sp_consulta_checador = "sp_consulta_checador";
        public const string sp_seguridad_usuarios = "sp_seguridad_usuarios";

        public const string sp_catalogo_trabajadores = "sp_catalogo_trabajadores";
        public const string sp_ConsultaEnviosPendientes = "sp_ConsultaEnviosPendientes"; 


        #endregion
    }
}
