using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DllFunc;
//using FuncForManager;
using Setts;

namespace FormForTest
{
    public partial class Form1 : Form
    {
        FuncForManager FFM = new FuncForManager();
        Settings Setttt = new Settings();
        bool flagKalibr = false;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)//Старт
        {
            textBox1.Text = FFM.PL_Start().ToString();
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)//Крен
        {
            double kren = Convert.ToDouble(textBox2.Text);
            double speed = Convert.ToDouble(textBox15.Text);
            FFM.PL_Move_kren(kren, speed);
        }

        private void button3_Click(object sender, EventArgs e)//Тангаж
        {
            double tangag = Convert.ToDouble(textBox3.Text);
            double speed = Convert.ToDouble(textBox15.Text);
            FFM.PL_Move_tangag(tangag, speed);
        }

        private void button4_Click(object sender, EventArgs e)//Высота
        {
            double vysota = Convert.ToDouble(textBox4.Text);
            double speed = Convert.ToDouble(textBox15.Text);
            FFM.PL_Move_height(vysota, speed);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FFM.StopWriter();
            FFM.StopMotors();   
        }

        private void button5_Click(object sender, EventArgs e)//Стоп
        {
            FFM.StopWriter();
            FFM.StopMotors();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox5.Text = FFM.ReadSensor_1().ToString();
            textBox6.Text = FFM.ReadSensor_2().ToString();
            textBox7.Text = FFM.ReadSensor_3().ToString();
            if(flagKalibr)
            {
                textBox24.Text = FFM.PL_Read_direction().ToString();
                textBox23.Text = FFM.PL_Read_height_position().ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FFM.Token__();
        }

        private void button7_Click(object sender, EventArgs e)//скорость 2
        {
            double vysota = (float)(Convert.ToDouble(textBox8.Text));
            double speed = Convert.ToDouble(textBox15.Text);
            FFM.PL_Move_height(vysota, speed);
        }

        private void button8_Click(object sender, EventArgs e)//SetAngelKren
        {
            double kren_min = Convert.ToDouble(textBox9.Text);
            double kren_max = Convert.ToDouble(textBox10.Text);
            FFM.PL_Set_angle_kren(kren_min, kren_max);
        }

        private void button10_Click(object sender, EventArgs e)//SetMaxS_D
        {
            double SetMaxS_D = Convert.ToDouble(textBox14.Text);
            FFM.PL_Set_maxSpeed_deviation(SetMaxS_D);
        }

        private void button12_Click(object sender, EventArgs e)//ReadSettMove
        {
            Setttt = FFM.Read_Move_Sett();

            textBox13.Text = Setttt.MaxSpeed_deviation.ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox17.Text = Convert.ToString(Math.Round(FFM.PL_Read_angle_kren(), 2));
            textBox18.Text = Convert.ToString(Math.Round(FFM.PL_Read_angle_tangag(), 2));
            textBox19.Text = Convert.ToString(Math.Round(FFM.PL_Read_height(), 2));
        }

        private void button14_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            //Thread.Sleep(2);
            textBox20.Text = FFM.PL_Stop().ToString();
        }

        private void button15_Click(object sender, EventArgs e)//старт чтения датчиков поворота и высоты
        {
            FFM.StartDirect();
            timer2.Enabled = true;
        }

        private void button16_Click(object sender, EventArgs e)//стоп чтения датчиков поворота и высоты
        {
            FFM.StopDirect();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox21.Text = FFM.PL_Read_direction().ToString();
            textBox22.Text = FFM.PL_Read_height_position().ToString();
        }

        private void button17_Click(object sender, EventArgs e)//AutoCentr
        {
            FFM.PL_Auto_centr();
        }

        private void button18_Click(object sender, EventArgs e)//AutoGorizont
        {
            FFM.PL_Auto_horizon();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            FFM.PL_Kalibration();
            flagKalibr = true;
        }

        private void button20_Click(object sender, EventArgs e)//ON RED
        {
            FFM.LedRedOn();
        }

        private void button21_Click(object sender, EventArgs e)//OFF RED
        {
            FFM.LedRedOff();
        }

        private void button23_Click(object sender, EventArgs e)//ON GREEN
        {
            FFM.LedGreenOn();
        }

        private void button22_Click(object sender, EventArgs e)//OFF GREEN
        {
            FFM.LedGreenOff();
        }

        private void button25_Click(object sender, EventArgs e)//ON BLUE
        {
            FFM.LedBlueOn();
        }

        private void button24_Click(object sender, EventArgs e)//OFF BLUE
        {
            FFM.LedBlueOff();
        }

        private void button26_Click(object sender, EventArgs e)//alarm
        {
            FFM.PL_Alarm(1);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            FFM.PL_Alarm(0);
        }
    }
}
