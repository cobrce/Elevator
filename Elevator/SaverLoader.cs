using Elevator.Automation;
using Microsoft.Win32;
using System;
using System.IO;
using System.Web.Script.Serialization;

namespace Elevator
{
    class SaverLoader
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        const string saveTitle = "Save Elevator plugin's config";
        const string loadTitle = "Load Elevator plugin's config";
        const string filter = "json|*.json|all files|*.*";

        internal static bool LoadCopyTo(IOContext iocontext)
        {
            return CopyContextIO(Load(), iocontext);
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

        internal static IOContext Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Title = loadTitle, Filter = filter };

            if (openFileDialog.ShowDialog() == true)
                return serializer.Deserialize<IOContext>(File.ReadAllText(openFileDialog.FileName));

            return null;
        }

        internal static bool Save(IOContext iocontext)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = saveTitle, Filter = filter };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, serializer.Serialize(iocontext));
                return true;
            }
            return false;
        }
    }
}
