using Elevator.Automation;
using Elevator.Automation.IOReadWrite;
using Elevator.Automation.Types;
using Microsoft.Win32;
using System;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Collections.Generic;

namespace Elevator
{
    class SaverLoader
    {
        const string saveTitle = "Save Elevator plugin's config";
        const string loadTitle = "Load Elevator plugin's config";
        const string filter = "json|*.json|all files|*.*";

        static PointBinder binder = new PointBinder();

        internal static bool LoadCopyTo(IOContext iocontext)
        {
            return CopyContextIO(Load(iocontext.EngineUP.PlcIoPoint.GetType()), iocontext);
        }

        private static bool CopyContextIO(IOContext from, IOContext to)
        {
            if (from != null)
            {
                if (from.Doors.Count != to.Doors.Count)
                    return false;

                for (int i = 0; i < from.Doors.Count; i++)
                    CopyDoor(from.Doors[i], to.Doors[i]);

                to.EngineUP.PlcIoPoint = from.EngineUP.PlcIoPoint;
                to.EngineDown.PlcIoPoint = from.EngineDown.PlcIoPoint;
                return true;
            }
            return false;
        }

        private static void CopyDoor(Door from, Door to)
        {
            to.OpenDoor = from.OpenDoor;
            to.CloseDoor = from.CloseDoor;
            to.LevelButton = from.LevelButton;
            to.DoorOpenSensor = from.DoorOpenSensor;
            to.DoorClosedSensor = from.DoorClosedSensor;
            to.PositionSensor = from.PositionSensor;
        }

        internal static IOContext Load(Type pointType)
        {
            binder.pointType = pointType;
            OpenFileDialog openFileDialog = new OpenFileDialog() { Title = loadTitle, Filter = filter };

            if (openFileDialog.ShowDialog() == true)
            {
                return JsonConvert.DeserializeObject<IOContext>(
                    File.ReadAllText(openFileDialog.FileName),
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            SerializationBinder = binder
                        }
                    );
            }
            return null;
        }

        internal static bool Save(IOContext iocontext)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = saveTitle, Filter = filter };
            if (saveFileDialog.ShowDialog() == true)
            {

                File.WriteAllText(
                    saveFileDialog.FileName,
                    JsonConvert.SerializeObject(
                        iocontext,
                        Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            SerializationBinder = binder,
                            TypeNameHandling = TypeNameHandling.Auto
                        }
                    )
                );
                return true;
            }
            return false;
        }
    }
    class PointBinder : ISerializationBinder
    {
        internal Type pointType;
        Dictionary<string, Type> knownTypes = new Dictionary<string, Type>();


        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.ToString();
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == pointType.Name)
                return pointType;
            else
                throw new Exception($"This configuration file is for {assemblyName}");
            //if (knownTypes.ContainsKey(typeName))
            //    return knownTypes[typeName];

            //foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (asm.GetName().Name == assemblyName)
            //    {
            //        Type[] types = asm.GetTypes();
            //        foreach (Type type in types)
            //            if (type.Name == typeName)
            //                return knownTypes[typeName] = type;
            //    }
            //}
            //return null;
        }
    }
}
