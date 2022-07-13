//Подключение двигателей
//Левый двигатель//PioDa -> DI 0,1; DO 0
//Правый двигатель//PioDa -> DI 2,3; DO 1
//Задний двигатель//PioDa -> DI 4,5; DO 2

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Xml.Serialization;
using UniDAQ_Ns;

namespace Custom_setting
{
    public partial class Form1 : Form
    {
        ushort wInitialCode, wTotalBoard;
        ushort wBoardNo = 0;
        ushort wOutputPort = 0;
        ushort wCfgCode = UniDAQ.IXUD_AO_BI_10V;


        UniDAQ.IXUD_CARD_INFO[] sCardInfo = new UniDAQ.IXUD_CARD_INFO[UniDAQ.MAX_BOARD_NUMBER];
        UniDAQ.IXUD_DEVICE_INFO[] sDeviceInfo = new UniDAQ.IXUD_DEVICE_INFO[UniDAQ.MAX_BOARD_NUMBER];

        Timer ReadTimer = new Timer();
        SerialPort COMport;
        XmlSerializer formatter = new XmlSerializer(typeof(SetDevice[]));

        private string[] NameDevice = { "Левого двигателя", "Правого двигателя", "Заднего двигателя", "Датчик высоты", "Датчик направления" };
        private const int length_mass = 5;
        public SetDevice[] Port_ = new SetDevice[length_mass];
        private int i = 0;
        private int dataPort;
        private object locker = new object();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        bool flag5 = false;
        bool flag6 = false;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            comboBox3.Items.AddRange(SerialPort.GetPortNames());

            ReadTimer.Interval = 15;
            ReadTimer.Enabled = true;
            ReadTimer.Tick += new EventHandler(OnTimedEvent);
            ReadTimer.Stop();

            try
            {
                wInitialCode = UniDAQ.Ixud_DriverInit(ref wTotalBoard);

                for (ushort i = 0; i < 3; i++)
                {
                    ConfigAO(i);
                }
            }
            catch
            {
                MessageBox.Show("PioDa не обнаружена");
            }
        }

        [Serializable]
        public class SetDevice
        {
            public string NamePort { get; set; }
            public int ValueSensor { get; set; }
            public string Device { get; set; }

            public SetDevice()
            {}
            public SetDevice(string namePort, int valueSensor, string device)
            {
                NamePort = namePort;
                ValueSensor = valueSensor;
                Device = device;
            }
        }
        private void ConfigAO(ushort wChannel)//Config Analog Output Range
        {
            wInitialCode = UniDAQ.Ixud_ConfigAO(wBoardNo, wChannel, wCfgCode);
        }
        public void WriteAOVoltage(ushort wChannel, float Value)//Write the Analog Output Value for Voltage 0-10V
        {
            wInitialCode = UniDAQ.Ixud_WriteAOVoltage(wBoardNo, wChannel, Value);
        }
        public void WriteDO(ushort wChannel, ushort Value)//Write the Digital Output
        {
            wInitialCode = UniDAQ.Ixud_WriteDOBit(wBoardNo, wOutputPort, wChannel, Value);
        }
        private void button1_Click(object sender, EventArgs e)//добавить сопоставление
        {
            if(comboBox1.Text != "" && comboBox2.Text != "")
            {
                textBox1.Text += comboBox1.Text + ">>" + comboBox2.Text + Environment.NewLine;
                if (i < length_mass)
                {
                    Port_[i] = new SetDevice((string)comboBox1.SelectedItem, 0, (string)comboBox2.SelectedItem);
                    comboBox1.Items.Remove(comboBox1.SelectedItem);
                    comboBox2.Items.Remove(comboBox2.SelectedItem);

                    if (i == (length_mass - 1))
                    {
                        button2.Enabled = true;
                    }
                    i++;
                }
                else
                {
                    MessageBox.Show("Назначение портов устройствам завершено");
                }
            }
            else 
            {
                if(comboBox1.Text == ""){ MessageBox.Show("Не выбран порт"); }
                else if (i < length_mass){ MessageBox.Show("Не выбрано устройство"); }
            }          
        }

        private void button3_Click(object sender, EventArgs e)//очистить
        {
            i = 0;
            textBox1.Text = "";
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(NameDevice);
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)//сохранить
        {
            using (FileStream fs = new FileStream("Settings.xml", FileMode.Create))
            {
                formatter.Serialize(fs, Port_);
            }
            button2.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CreatPort();
        }

        private void CreatPort()
        {
            string namePort = (string)comboBox3.SelectedItem;
            COMport = new SerialPort(namePort, 19200, Parity.None, 8, StopBits.One);
            COMport.Open();
            if (COMport.IsOpen)
            {
                COMport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                ReadTimer.Start();
            }
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
                    int cDec = int.Parse(H, System.Globalization.NumberStyles.HexNumber);

                    dataPort = cDec;
                }
            }
            catch { }
        }

        private int DataPort()
        {
            lock (locker)
            {
                return dataPort;
            }
        }

        private void OnTimedEvent(Object source, EventArgs myEventArgs)
        {
            textBox2.Text = DataPort().ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReadTimer.Stop();
            COMport.Dispose();
            COMport.Close();
            textBox2.Text = "Stop";
            dataPort = 0;
        }

        ///////////////////////////ДВИГАТЕЛЬ 1/////////////////////////////////////////
        private void trackBar1_Scroll(object sender, EventArgs e)//двигатель 1 скорость
        {
            label9.Text = trackBar1.Value.ToString();
            float volt = trackBar1.Value / 10;
            WriteAOVoltage(0, volt);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (flag1 == false)
            {
                WriteDO(0, 1);
                button6.FlatStyle = FlatStyle.Standard;
                button6.BackColor = Color.Red;
                button6.Text = "<< Стоп";
                flag1 = true;
            }
            else
            {
                WriteDO(0, 0);
                button6.FlatStyle = FlatStyle.System;
                button6.Text = "<< Пуск";
                flag1 = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (flag2 == false)
            {
                WriteDO(1, 1);
                button7.FlatStyle = FlatStyle.Standard;
                button7.BackColor = Color.Red;
                button7.Text = "Стоп >>";
                flag2 = true;
            }
            else
            {
                WriteDO(1, 0);
                button7.FlatStyle = FlatStyle.System;
                button7.Text = "Пуск >>";
                flag2 = false;
            }
        }
        ///////////////////////////ДВИГАТЕЛЬ 2/////////////////////////////////////////
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label10.Text = trackBar2.Value.ToString();
            float volt = trackBar2.Value / 10;
            WriteAOVoltage(1, volt);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (flag3 == false)
            {
                WriteDO(2, 1);
                button9.FlatStyle = FlatStyle.Standard;
                button9.BackColor = Color.Red;
                button9.Text = "<< Стоп";
                flag3 = true;
            }
            else
            {
                WriteDO(2, 0);
                button9.FlatStyle = FlatStyle.System;
                button9.Text = "<< Пуск";
                flag3 = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (flag4 == false)
            {
                WriteDO(3, 1);
                button8.FlatStyle = FlatStyle.Standard;
                button8.BackColor = Color.Red;
                button8.Text = "Стоп >>";
                flag4 = true;
            }
            else
            {
                WriteDO(3, 0);
                button8.FlatStyle = FlatStyle.System;
                button8.Text = "Пуск >>";
                flag4 = false;
            }
        }
     
        ///////////////////////////ДВИГАТЕЛЬ 3/////////////////////////////////////////
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label14.Text = trackBar3.Value.ToString();
            float volt = trackBar3.Value / 10;
            WriteAOVoltage(2, volt);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (flag5 == false)
            {
                WriteDO(4, 1);
                button11.FlatStyle = FlatStyle.Standard;
                button11.BackColor = Color.Red;
                button11.Text = "<< Стоп";
                flag5 = true;
            }
            else
            {
                WriteDO(4, 0);
                button11.FlatStyle = FlatStyle.System;
                button11.Text = "<< Пуск";
                flag5 = false;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (flag6 == false)
            {
                WriteDO(5, 1);
                button10.FlatStyle = FlatStyle.Standard;
                button10.BackColor = Color.Red;
                button10.Text = "Стоп >>";
                flag6 = true;
            }
            else
            {
                WriteDO(5, 0);
                button10.FlatStyle = FlatStyle.System;
                button10.Text = "Пуск >>";
                flag6 = false;
            }
        }

        private void button12_Click(object sender, EventArgs e)//выполнить калибровку
        {
            SetDevice[] newMass;

            try
            {
                using (FileStream fs = new FileStream("Settings.xml", FileMode.Open))
                {
                    newMass = (SetDevice[])formatter.Deserialize(fs);
                    foreach (SetDevice a in newMass)
                    {
                        if (a.Device == "Левого двигателя")
                        {
                            a.ValueSensor = 5000;
                            while (a.ValueSensor == 5000) 
                            {
                                a.ValueSensor = ReadPortKalibr(a.NamePort);
                            }
                        }
                        if (a.Device == "Правого двигателя")
                        {
                            a.ValueSensor = 5000;
                            while (a.ValueSensor == 5000)
                            {
                                a.ValueSensor = ReadPortKalibr(a.NamePort);
                            }
                        }
                        if (a.Device == "Заднего двигателя")
                        {
                            a.ValueSensor = 5000;
                            while (a.ValueSensor == 5000)
                            {
                                a.ValueSensor = ReadPortKalibr(a.NamePort);
                            }
                        }
                    }
                }
                using (FileStream fs_ = new FileStream("Kalibr.xml", FileMode.Create))
                {
                    formatter.Serialize(fs_, newMass);
                }
            }
            catch
            {
                MessageBox.Show("Отсутствует файл сопоставления");
            }
        }

        private int ReadPortKalibr(string namePort)
        {
            bool flagReadEnd = false;
            string SensorData = "";
            char[] SData;
            char[] cMBbuf = new char[3];

            COMport = new SerialPort(namePort, 19200, Parity.None, 8, StopBits.One);
            COMport.Open();

            while (!COMport.IsOpen) { }

            while (!flagReadEnd)
            {
                SensorData = COMport.ReadLine();
                if (SensorData.Length > 0) flagReadEnd = true;
            }
            COMport.Dispose();
            COMport.Close();

            SData = SensorData.ToCharArray();

            if (SData[0] == '$' && SData[1] == 'A')  // $A
            {
                for (int y = 0; y < 3; y++)
                {
                    cMBbuf[y] = SData[y + 2];
                }
                string H = new string(cMBbuf);
                int cDec = int.Parse(H, System.Globalization.NumberStyles.HexNumber);

                return cDec;
            }
            return 5000;
        }
    }
}