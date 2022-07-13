using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Xml.Serialization;
using Mobile_platform;

namespace Custom_setting
{
    public partial class Configuration : Form
    {
        private int speed1 = 0;
        private int speed2 = 0;
        private int speed3 = 0;

        private Timer ReadTimer = new Timer();
        private SerialPort COMport;
        private SerialPort COMportArduino;
        private XmlSerializer formatter = new XmlSerializer(typeof(SetDevice[]));

        private const int length_mass = 6;
        private SetDevice[] Port_ = new SetDevice[length_mass];
        private int i = 0;
        private int dataPort { get; set; }
        private bool flag1 = false;
        private bool flag2 = false;
        private bool flag3 = false;
        private bool flag4 = false;
        private bool flag5 = false;
        private bool flag6 = false;

        public Configuration()
        {
            InitializeComponent();
            comboBox_Port.Items.AddRange(SerialPort.GetPortNames());
            comboBox_ViewPort.Items.AddRange(SerialPort.GetPortNames());
            comboBox_Sensor.Items.AddRange(NameDevices.NameDevice);

            ReadTimer.Interval = 15;
            ReadTimer.Enabled = true;
            ReadTimer.Tick += new EventHandler(OnTimedEvent);
            ReadTimer.Stop();

            COMportArduino = new SerialPort("COM7", 115200, Parity.None, 8, StopBits.One);
            COMportArduino.Open();
            if (!COMportArduino.IsOpen)
            {
                MessageBox.Show("Не удалось открыть COM7");
            }
        }

        [Serializable]
        public class SetDevice
        {
            public string NamePort { get; set; }
            public int ValueSensor { get; set; }
            public string Device { get; set; }

            public SetDevice() { }

            public SetDevice(string namePort, int valueSensor, string device)
            {
                NamePort = namePort;
                ValueSensor = valueSensor;
                Device = device;
            }
        }
        private void button_Add_Click(object sender, EventArgs e)//добавить сопоставление устройств
        {
            if (comboBox_Port.Text != "" && comboBox_Sensor.Text != "")
            {
                textBox_Device.Text += comboBox_Port.Text + ">>" + comboBox_Sensor.Text + Environment.NewLine;
                if (i < length_mass)
                {
                    Port_[i] = new SetDevice((string)comboBox_Port.SelectedItem, 0, (string)comboBox_Sensor.SelectedItem);
                    comboBox_Port.Items.Remove(comboBox_Port.SelectedItem);
                    comboBox_Sensor.Items.Remove(comboBox_Sensor.SelectedItem);

                    if (i == (length_mass - 1))
                    {
                        button_Save.Enabled = true;
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
                if (comboBox_Port.Text == "") { MessageBox.Show("Не выбран порт"); }
                else if (i < length_mass) { MessageBox.Show("Не выбрано устройство"); }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)//очистить
        {
            i = 0;
            textBox_Device.Text = "";
            comboBox_Port.Items.Clear();
            comboBox_Port.Items.AddRange(SerialPort.GetPortNames());
            comboBox_Sensor.Items.Clear();
            comboBox_Sensor.Items.AddRange(NameDevices.NameDevice);
            button_Save.Enabled = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)//сохранить
        {
            using (FileStream fs = new FileStream("Settings.xml", FileMode.Create))
            {
                formatter.Serialize(fs, Port_);
            }
            button_Save.Enabled = false;
        }

        private void buttonReadPort_Click(object sender, EventArgs e)
        {
            button_ReadPort.Enabled = false;
            button_ClosePort.Enabled = true;
            CreatPort();
        }

        private void CreatPort()
        {
            string namePort = (string)comboBox_ViewPort.SelectedItem;
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
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка чтения порта данных\n." + ex.Message, "Сообщение", MessageBoxButtons.OK);               
            }
        }

        private void OnTimedEvent(Object source, EventArgs myEventArgs)
        {
            textBox_DataPort.Text = dataPort.ToString();
        }

        private void buttonClosePort_Click(object sender, EventArgs e)
        {
            button_ReadPort.Enabled = true;
            button_ClosePort.Enabled = false;

            ReadTimer.Stop();
            COMport.Dispose();
            COMport.Close();
            textBox_DataPort.Text = "Stop";
            dataPort = 0;
        }

        ///////////////////////////ДВИГАТЕЛЬ 1/////////////////////////////////////////
        private void trackBar_Speed1_Scroll(object sender, EventArgs e)//двигатель 1 скорость
        {
            label_Speed1.Text = trackBar_Speed1.Value.ToString();
            speed1 = trackBar_Speed1.Value;
        }

        private void buttonUp1_Click(object sender, EventArgs e)//1 вверх
        {
            if (flag1 == false)
            {
                COMportArduino.Write("$,B," + speed1.ToString() + ",B,0,B,0");
                buttonUp1.FlatStyle = FlatStyle.Standard;
                buttonUp1.BackColor = Color.Red;
                buttonUp1.Text = "Стоп";
                flag1 = true;
            }
            else
            {
                COMportArduino.Write("$,B,0,B,0,B,0");
                buttonUp1.FlatStyle = FlatStyle.System;
                buttonUp1.Text = "Вверх";
                flag1 = false;
            }
        }

        private void buttonDn1_Click(object sender, EventArgs e)//1 вниз
        {
            if (flag2 == false)
            {
                COMportArduino.Write("$,F," + speed1.ToString() + ",F,0,F,0");
                buttonDn1.FlatStyle = FlatStyle.Standard;
                buttonDn1.BackColor = Color.Red;
                buttonDn1.Text = "Стоп";
                flag2 = true;
            }
            else
            {
                COMportArduino.Write("$,F,0,F,0,F,0");
                buttonDn1.FlatStyle = FlatStyle.System;
                buttonDn1.Text = "Вниз";
                flag2 = false;
            }
        }
        ///////////////////////////ДВИГАТЕЛЬ 2/////////////////////////////////////////
        private void trackBar_Speed2_Scroll(object sender, EventArgs e)
        {
            label_Speed2.Text = trackBar_Speed2.Value.ToString();
            speed2 = trackBar_Speed2.Value;
        }
        private void buttonUp2_Click(object sender, EventArgs e)
        {
            if (flag3 == false)
            {
                COMportArduino.Write("$,B,0,B," + speed2.ToString() + ",B,0");
                buttonUp2.FlatStyle = FlatStyle.Standard;
                buttonUp2.BackColor = Color.Red;
                buttonUp2.Text = "Стоп";
                flag3 = true;
            }
            else
            {
                COMportArduino.Write("$,B,0,B,0,B,0");
                buttonUp2.FlatStyle = FlatStyle.System;
                buttonUp2.Text = "Вверх";
                flag3 = false;
            }
        }

        private void buttonDn2_Click(object sender, EventArgs e)
        {
            if (flag4 == false)
            {
                COMportArduino.Write("$,F,0,F," + speed2.ToString() + ",F,0");
                buttonDn2.FlatStyle = FlatStyle.Standard;
                buttonDn2.BackColor = Color.Red;
                buttonDn2.Text = "Стоп";
                flag4 = true;
            }
            else
            {
                COMportArduino.Write("$,F,0,F,0,F,0");
                buttonDn2.FlatStyle = FlatStyle.System;
                buttonDn2.Text = "Вниз";
                flag4 = false;
            }
        }

        ///////////////////////////ДВИГАТЕЛЬ 3/////////////////////////////////////////
        private void trackBar_Speed3_Scroll(object sender, EventArgs e)
        {
            label_Speed3.Text = trackBar_Speed3.Value.ToString();
            speed3 = trackBar_Speed3.Value;
        }

        private void buttonUp3_Click(object sender, EventArgs e)
        {
            if (flag5 == false)
            {
                COMportArduino.Write("$,B,0,B,0,B," + speed3.ToString());
                buttonUp3.FlatStyle = FlatStyle.Standard;
                buttonUp3.BackColor = Color.Red;
                buttonUp3.Text = "Стоп";
                flag5 = true;
            }
            else
            {
                COMportArduino.Write("$,B,0,B,0,B,0");
                buttonUp3.FlatStyle = FlatStyle.System;
                buttonUp3.Text = "Вверх";
                flag5 = false;
            }
        }

        private void buttonDn3_Click(object sender, EventArgs e)
        {
            if (flag6 == false)
            {
                COMportArduino.Write("$,F,0,F,0,F," + speed3.ToString());
                buttonDn3.FlatStyle = FlatStyle.Standard;
                buttonDn3.BackColor = Color.Red;
                buttonDn3.Text = "Стоп";
                flag6 = true;
            }
            else
            {
                COMportArduino.Write("$,F,0,F,0,F,0");
                buttonDn3.FlatStyle = FlatStyle.System;
                buttonDn3.Text = "Вниз";
                flag6 = false;
            }
        }

        private void buttonKalibr_Click(object sender, EventArgs e)//выполнить калибровку т.е. записать данные в файл
        {
            SetDevice[] newMass;
            FileStream fs;
            try
            {
                using (fs = new FileStream("Settings.xml", FileMode.Open))
                {
                    newMass = (SetDevice[])formatter.Deserialize(fs);
                    foreach (SetDevice a in newMass)
                    {
                        if (a.Device == NameDevices.NameDevice[0])//"Заднего двигателя"
                        {
                            a.ValueSensor = 5000;
                            while (a.ValueSensor == 5000)
                            {
                                a.ValueSensor = ReadPortKalibr(a.NamePort);
                            }
                        }
                        if (a.Device == NameDevices.NameDevice[1])//"Левого двигателя"
                        {
                            a.ValueSensor = 5000;
                            while (a.ValueSensor == 5000)
                            {
                                a.ValueSensor = ReadPortKalibr(a.NamePort);
                            }
                        }
                        if (a.Device == NameDevices.NameDevice[2])//"Правого двигателя"
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
                MessageBox.Show("Калибровка выполнена успешно.", "Сообщение.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            if (SData[0] == '$' && SData[1] == 'A')
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