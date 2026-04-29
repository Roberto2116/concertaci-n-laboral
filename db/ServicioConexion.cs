using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Proyecto_GRRLN_expediente.db
{
    public enum TipoEstadoConexion
    {
        Desconectado, // Rojo
        Limitado,     // Amarillo (Internet ok, DB no)
        Conectado     // Verde
    }

    public static class ServicioConexion
    {
        private const string HostSupabase = "aws-1-us-west-2.pooler.supabase.com";
        private const int PuertoSupabase = 6543;

        public static async Task<(TipoEstadoConexion Estado, string Detalle)> ValidarConexionAsync()
        {
            // 1. Verificar Red Local
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return (TipoEstadoConexion.Desconectado, "Sin conexión de red: Verifica tu cable Ethernet o Wi-Fi.");
            }

            // 2. Verificar Salida a Internet (Ping rápido a un DNS confiable)
            bool tieneInternet = await ProbarPingAsync("8.8.8.8");
            if (!tieneInternet)
            {
                return (TipoEstadoConexion.Desconectado, "Sin acceso a Internet: Estás conectado a una red pero no hay salida al exterior.");
            }

            // 3. Verificar Puerto de Supabase
            bool puertoAbierto = await ProbarPuertoTcpAsync(HostSupabase, PuertoSupabase);
            if (!puertoAbierto)
            {
                return (TipoEstadoConexion.Limitado, $"Puerto {PuertoSupabase} Bloqueado: Tienes internet, pero la red de tu oficina bloquea la conexión a la base de datos.");
            }

            return (TipoEstadoConexion.Conectado, "Conexión exitosa: El sistema está sincronizado con la nube de Supabase.");
        }

        private static async Task<bool> ProbarPingAsync(string host)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(host, 1500);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch { return false; }
        }

        private static async Task<bool> ProbarPuertoTcpAsync(string host, int puerto)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var task = client.ConnectAsync(host, puerto);
                    if (await Task.WhenAny(task, Task.Delay(2000)) == task)
                    {
                        await task;
                        return true;
                    }
                    return false;
                }
            }
            catch { return false; }
        }
    }
}
