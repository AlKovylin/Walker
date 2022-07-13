using System;
using System.IO;
using System.IO.Ports;
//using System.Runtime.InteropServices;  

namespace Mobile_platform
{
    public class COM_Sensor
    {
        private SerialPort Port;
        private string Name { get; set; }
        public int DataPort { get; private set; }

        //public delegate void PortSendData(int data);
        //public event PortSendData PortSendDataEvent;

        public COM_Sensor(string name)
        {
            Name = name;
        }

        public void CreatPort()
        {
            try
            {
                Port = new SerialPort(Name, 19200, Parity.None, 8, StopBits.One);
                Port.Open();

                if (Port.IsOpen)
                {
                    StartReadPort();
                }
                else
                {
                    throw new ApplicationException("Не удалось открыть порт {0}" + Name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void StartReadPort()
        {
            try
            {
                Port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            catch(Exception e)
            {
                throw new Exception(Port.PortName + e.GetBaseException().Message);
            }
            //Port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedEventHandler);
        }

        private void ErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new Exception(Port.PortName + e.ToString());
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Port = (SerialPort)sender;
            string SensorData = Port.ReadLine();
            char[] SData = SensorData.ToCharArray();
            char[] cMBbuf = new char[3];

            try
            {
                if (SData[0] == '$' && SData[1] == 'A')  // $A
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cMBbuf[y] = SData[y + 2];
                    }
                    string H = new string(cMBbuf);

                    DataPort = int.Parse(H, System.Globalization.NumberStyles.HexNumber);
                    //PortSendDataEvent?.Invoke(DataPort);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Stop()
        {
            try
            {
                //Port.Dispose();
                Port.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class COM_Arduino
    {
        private SerialPort Port;
        private string Name { get; set; }
        private string DataPort { get; set; }

        public delegate void ArduinoSendData(string data);
        public event ArduinoSendData ArduinoSendDataEvent;

        public COM_Arduino(string name)
        {
            Name = name;
        }

        public bool CreatPort()
        {
            try
            {
                Port = new SerialPort(Name, 115200, Parity.None, 8, StopBits.One);
                Port.Open();

                if (Port.IsOpen)
                {
                    StartReadPort();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }

        private void StartReadPort()
        {
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void ErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new IOException(Port.PortName + e.ToString());
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Port = (SerialPort)sender;
            DataPort = Port.ReadLine();

            ArduinoSendDataEvent?.Invoke(DataPort);
        }

        public void WritePort(string data)
        {
            Port.WriteLine(data);
        }

        public bool Stop()
        {
            try
            {
                //Port.Dispose();
                Port.Close();
                return true;
            }
            catch (IOException ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
    }
}