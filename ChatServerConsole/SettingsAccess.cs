using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerConsole
{
    public static class SettingsAccess
    {
        public static SettingsModel ReadData()
        {
            //Czytaj dane z Json z dysku.
            //Jak ich nie ma stwórz plik z danymi defaltowymi z modelu
            return new SettingsModel();
        }
    }
}
