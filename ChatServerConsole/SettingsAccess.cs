using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ChatServerConsole
{
    public static class SettingsAccess
    {
        public static SettingsModel ReadData()
        {
            //Czytaj dane z Json z dysku.
            SettingsModel settings = new SettingsModel();
            try
            {
                using (StreamReader streamReader = new StreamReader("settings.json"))
                {
                    string json = streamReader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<SettingsModel>(json);
                }
            }
            //Jak ich nie ma stwórz plik z danymi defaltowymi z modelu
            catch (Exception)
            {
                string output = JsonConvert.SerializeObject(settings);
                using (StreamWriter streamWriter = new StreamWriter("settings.json"))
                {
                    streamWriter.Write(output);
                }
            }

            return settings;
        }
    }
}
