using Elevator.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Elevator.Plugins
{
    public static class PluginsLoader
    {
        private const string pluginsDir = "Plugins";

        static string absPluginsDir =
            Path.Combine(
                Path.GetDirectoryName(typeof(PluginsLoader).Assembly.Location),
                pluginsDir
            );


        static internal List<IO> PluginsList;

        static PluginsLoader()
        {
            PluginsList = new List<IO>();
            ListPlugins();
        }

        private static void ListPlugins()
        {
            //PluginsList.AddRange(ListPlugins(typeof(PluginsLoader).Assembly));
            try
            {
                foreach (string fileName in Directory.EnumerateFiles(absPluginsDir, "*.*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        RegisterPlugins(Assembly.LoadFile(fileName));
                    }
                    catch { }
                }
            }
            catch { }
        }

        internal static List<IO> ListPlugins(Assembly asm)
        {
            List<IO> io = new List<IO>();
            foreach (Type t in asm.GetTypes())
                if ((t.GetInterfaces().Contains(typeof(IPlugin)) || t.BaseType == typeof(AbstractPlugin)) && !t.IsAbstract)
                    if (Activator.CreateInstance(t) is IPlugin plugin)
                        io.Add(plugin.Register());
            return io;
        }

        // these two methods could be invoked by an external assembly to register a plugin, this should be done before invoking
        // the mainwindow
        public static void RegisterPlugins(Assembly asm)
        {
            PluginsList.AddRange(ListPlugins(asm));
        }

        public static void RegisterPlugin(IPlugin plugin)
        {
            PluginsList.Add(plugin.Register());
        }
    }
}
