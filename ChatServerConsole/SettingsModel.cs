using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerConsole
{
    public class SettingsModel
    {
        public string SerwerName { get; set; } = "Serwer testowy";
        public string IpAdress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 3000;
        public int BufferSize { get; set; } = 512;
    }
}
