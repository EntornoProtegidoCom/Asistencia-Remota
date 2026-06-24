using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SendMailChecador
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem // Ajusta si necesitas cuenta específica
            };

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "SendMailChecadorService",
                DisplayName = "Send Mail Checador Service",
                Description = "Servicio que envía correos de registros pendientes del checador.",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
