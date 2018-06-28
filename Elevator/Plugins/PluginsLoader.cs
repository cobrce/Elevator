using Elevator.Automation;
using Elevator.Automation.IOReadWrite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
        static private List<Assembly> _loadedassemblies;

        static PluginsLoader()
        {
            PluginsList = new List<IO>();
            _loadedassemblies = new List<Assembly>();
            SetAssemblySover();
            ListPlugins();
        }

        private static void SetAssemblySover()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // this happens when depedency are in "Plugins" directory, at this point all assemblies found in that folder
            // are already loaded, so this function will try to solve the missing assembly from the _loadedassemblies
            foreach (Assembly assembly in _loadedassemblies)
                if (assembly.FullName == args.Name)
                    return assembly;
            return null;
        }

        private static void ListPlugins()
        {
            try
            {
                foreach (string fileName in Directory.EnumerateFiles(absPluginsDir, "*.*", SearchOption.TopDirectoryOnly))
                    try
                    {
                        _loadedassemblies.Add(Assembly.LoadFile(fileName));
                    }
                    catch { }
                foreach (Assembly asm in _loadedassemblies)
                    try
                    {
                        RegisterPlugins(asm);
                    }
                    catch { }
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
