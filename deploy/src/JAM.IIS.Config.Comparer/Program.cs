using System;
using System.Configuration;
using System.IO;

namespace JAM.IIS.Config.Comparer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) help();
            if (args.Length < 3) error("Faltan argumentos");

            var src = args[0];
            var dst = args[1];
            var mode = args[2];

            if (!File.Exists(src)) error("Archivo de origen no encontrado");
            if (!File.Exists(dst)) error("Archivo de destino no encontrado");

            if (string.IsNullOrEmpty(mode))
            {
                help();
                error("Modo invalido");
            }

            switch (mode)
            {
                case "comparar":
                    comparar(src, dst);
                    break;
                case "copiar":
                    copiar(src, dst);
                    break;
            }

        }

        static AppSettingsSection srcSettings = null;
        static AppSettingsSection dstSettings = null;

        static Configuration srcConfig = null;
        static Configuration dstConfig = null;
        static void comparar(string src, string dst)
        {
            CargarAppSetting(src, dst, ref srcSettings, ref dstSettings);

            foreach(KeyValueConfigurationElement setting in srcSettings.Settings)
            {
                if (dstSettings.Settings[setting.Key] == null)
                {
                    //No existe
                    Console.WriteLine("key '{0}' no existe", setting.Key);
                }
                else
                {
                    if (!setting.Value.Equals(dstSettings.Settings[setting.Key].Value))
                    {
                        Console.WriteLine("key '{0}' es diferente", setting.Key);
                    }
                    else
                    {
                        Console.WriteLine("key '{0}' igual", setting.Key);
                    }
                }
            }
        }

        static void copiar(string src, string dst)
        {
            var save = false;
            CargarAppSetting(src, dst, ref srcSettings, ref dstSettings);
            foreach (KeyValueConfigurationElement setting in srcSettings.Settings)
            {
                if (dstSettings.Settings[setting.Key] == null)
                {
                    //Crear
                    dstSettings.Settings.Add(setting.Key, setting.Value);
                    Console.WriteLine("key '{0}' creada", setting.Key);
                    save = true;
                }
                else
                {
                    if (!setting.Value.Equals(dstSettings.Settings[setting.Key].Value))
                    {
                        dstSettings.Settings[setting.Key].Value = setting.Value;
                        Console.WriteLine("key '{0}' modificada", setting.Key);
                        save = true;
                    }
                }
            }
            if (save)
                dstConfig.Save();
            else
                Console.WriteLine("No hay keys diferentes");
        }

        static void CargarAppSetting(string src, string dst, ref AppSettingsSection srcAppSetting, ref AppSettingsSection dstAppSetting)
        {
            var srcConfigFileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = src
            };
            var dstConfigFileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = dst
            };
            srcConfig = ConfigurationManager.OpenMappedExeConfiguration(srcConfigFileMap, ConfigurationUserLevel.None);
            dstConfig = ConfigurationManager.OpenMappedExeConfiguration(dstConfigFileMap, ConfigurationUserLevel.None);
            
            if (srcConfig == null)
                error("Error al cargar archivo de origen");
            if (srcConfig == null)
                error("Error al cargar archivo de destino");

            srcAppSetting = srcConfig.AppSettings;
            dstAppSetting = dstConfig.AppSettings;
        }


        static void help()
        {
            Console.WriteLine("");
            Console.WriteLine("Comparador y homologador de app.config o web.config.");
            Console.WriteLine("Ejemplo de uso: " + System.AppDomain.CurrentDomain.FriendlyName + " origen destino modo");
            Console.WriteLine("modo: comparar o copiar");
        }

        static void error(string mensaje, bool salir=true)
        {
            Console.WriteLine("");
            Console.WriteLine("Error: " + mensaje);
            if (salir)
                Environment.Exit(-1);
        }
    }
}
