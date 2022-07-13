using System;
using System.Xml.Serialization;
using System.IO;

namespace Mobile_platform
{
    [Serializable]
    public class SettDevice
    {
        //имя порта, значение датчика на момент калибровки, сопоставленное устройство (NameDevice)
        public string NamePort { get; set; }
        public int ValueSensor { get; set; }
        public string Device { get; set; }

        public SettDevice()
        { }

        public SettDevice(string namePort, int valueSensor, string device)
        {
            NamePort = namePort;
            ValueSensor = valueSensor;
            Device = device;
        }
    }
}