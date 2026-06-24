using SendMailChecador.App_Code;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace SendMailChecador
{
    partial class SendMailService : ServiceBase
    {
        private Timer _timer;
        private int _intervalMs; // ahora se obtiene desde App.config
        private EventLog _eventLog;
        private const string EventSource = "SendMailChecadorService";
        private const string EventLogName = "Application";

        public SendMailService()
        {
            ServiceName = EventSource;
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
            //ProcessCycle();
            InitializeComponent();
        }

        private void InicializarEventLog()
        {
            try
            {
                if (!EventLog.SourceExists(EventSource))
                {
                    EventLog.CreateEventSource(new EventSourceCreationData(EventSource, EventLogName));
                }
                _eventLog = new EventLog
                {
                    Source = EventSource,
                    Log = EventLogName
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("No se pudo inicializar EventLog: " + ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            EscribirLogInfo("Servicio iniciando...");

            // Leer intervalo desde configuración (minutos)
            var intervaloStr = ConfigurationManager.AppSettings["IntervalMinutes"];
            if (!int.TryParse(intervaloStr, out var intervaloMin) || intervaloMin <= 0)
            {
                intervaloMin = 5; // valor por defecto
                EscribirLogWarning("IntervalMinutes inválido o no definido. Se usa 5 minutos por defecto.");
            }
            _intervalMs = intervaloMin * 60_000;
            EscribirLogInfo($"Intervalo configurado: {intervaloMin} minuto(s) ({_intervalMs} ms).");

            try
            {
                _timer = new Timer(_intervalMs);
                _timer.Elapsed += TimerElapsed;
                _timer.AutoReset = true;
                _timer.Start();

                ProcessCycle(); // primera ejecución inmediata
                EscribirLogInfo("Servicio iniciado correctamente.");
            }
            catch (Exception ex)
            {
                EscribirLogError("Error en OnStart: " + ex.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
            EscribirLogInfo("Servicio deteniéndose...");
            try
            {
                _timer?.Stop();
                _timer?.Dispose();
                EscribirLogInfo("Servicio detenido.");
            }
            catch (Exception ex)
            {
                EscribirLogError("Error en OnStop: " + ex.Message);
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ProcessCycle();
        }

        private void ProcessCycle()
        {
            try
            {
                var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
                var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                var smtpFallbackHost = ConfigurationManager.AppSettings["SmtpFallbackHost"] ?? "smtp.office365.com";
                var smtpPortValue = ConfigurationManager.AppSettings["SmtpPort"];
                var enviaFoto = ConfigurationManager.AppSettings["EnviaFoto"];
                if (!int.TryParse(smtpPortValue, out var smtpPort))
                {
                    EscribirLogWarning("Puerto SMTP inválido.");
                    return;
                }

                MainSendMail.consultaEnviosPendientes(smtpHost, smtpPort, smtpUser, smtpPassword, enviaFoto, smtpFallbackHost);
            }
            catch (Exception ex)
            {
                EscribirLogError("Error en ciclo de servicio: " + ex.Message);
            }
        }

        private void EscribirLogInfo(string mensaje)
        {
            if (_eventLog != null)
                _eventLog.WriteEntry(mensaje, EventLogEntryType.Information);
            else
                Trace.WriteLine(mensaje);
        }

        private void EscribirLogWarning(string mensaje)
        {
            if (_eventLog != null)
                _eventLog.WriteEntry(mensaje, EventLogEntryType.Warning);
            else
                Trace.WriteLine("WARN: " + mensaje);
        }

        private void EscribirLogError(string mensaje)
        {
            if (_eventLog != null)
                _eventLog.WriteEntry(mensaje, EventLogEntryType.Error);
            else
                Trace.WriteLine("ERROR: " + mensaje);
        }
    }
}
