using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DllFunc;
using Mobile_platform;

namespace FormForTest
{
    public partial class Form1 : Form
    {
        private FuncForManager FFM = new FuncForManager();
        private Demo demo;
        //private Settings Sett = new Settings();

        public Form1()
        {
            InitializeComponent();
            demo = new Demo(FFM);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //FFM.StopWriter();
            //FFM.StopMotors();   
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox_COM1.Text = FFM.ReadSensor_1().ToString();
            textBox_COM2.Text = FFM.ReadSensor_2().ToString();
            textBox_COM3.Text = FFM.ReadSensor_3().ToString();
            textBox_COM4.Text = FFM.ReadSensor_4().ToString();
            textBox_COM5.Text = FFM.ReadSensor_5().ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            textBox_Direct.Text = FFM.PL_Read_direction().ToString();
            textBox_HeightDir.Text = FFM.PL_Read_height_position().ToString();
        }


        //СТАРТ-СТОП
        private void button_Start_Click(object sender, EventArgs e)//Старт
        {
            try
            {
                FFM.PL_Start();
                textBox_StatusOnOff.Text = "OK";
                timer1.Enabled = true;
                button_StartReadDirHei.Enabled = true;
                groupBox_Motion_control.Enabled = true;
                groupBox_LED.Enabled = true;
                groupBox_Testing.Enabled = true;
                groupBox5.Enabled = true;
                button_E_STOP.Enabled = true;
                button_E_STOP.BackColor = System.Drawing.Color.Red;
                button_Stop.Enabled = true;
                button_Start.Enabled = false;
                ReadСurrentSettings();
                trackBar_Speed_Scroll(sender, e);
            }
            catch (Exception ex)
            {
                textBox_StatusOnOff.Text = "ERROR";
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)//стоп
        {
            try
            {
                timer1.Enabled = false;
                FFM.PL_Stop();
                //Thread.Sleep(2);
                textBox_StatusOnOff.Text = "OK";
                groupBox_Motion_control.Enabled = false;
                groupBox_LED.Enabled = false;
                groupBox_Testing.Enabled = false;
                groupBox5.Enabled = false;
                button_E_STOP.Enabled = false;
                button_E_STOP.BackColor = System.Drawing.SystemColors.Control;
                button_Stop.Enabled = false;
                button_Start.Enabled = true;
            }
            catch (Exception ex)
            {
                textBox_StatusOnOff.Text = "ERROR";
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_E_STOP_Click(object sender, EventArgs e)
        {
            //FFM.StopWriter();
            FFM.StopMotors();
        }

        //УПРАВЛЕНИЕ ДВИЖЕНИЕМ
        private void trackBar_Speed_Scroll(object sender, EventArgs e)//установка скорости
        {
            Settings sett = FFM.GetSett();

            if (trackBar_Speed.Value < sett.MinSpeed)//если пользователь пытается выставить скорость меньше минимальной,
                trackBar_Speed.Value = (int)sett.MinSpeed;//то ничего у него не выйдет

            label_Speed.Text = trackBar_Speed.Value.ToString();
        }

        //фиксированные положения
        private void button_Height1_Click(object sender, EventArgs e)//нижнее положение
        {
            try
            {
                double vysota = PhysicalChar.Min_height;
                double speed = double.Parse(label_Speed.Text);
                FFM.PL_Move_height(vysota, speed);
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Height2_Click(object sender, EventArgs e)//среднее положение
        {
            try
            {
                double vysota = PhysicalChar.Average_height;
                double speed = double.Parse(label_Speed.Text);
                FFM.PL_Move_height(vysota, speed);
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Height3_Click(object sender, EventArgs e)//верхнее положение
        {
            try
            {
                double vysota = PhysicalChar.Max_height;
                double speed = double.Parse(label_Speed.Text);
                FFM.PL_Move_height(vysota, speed);
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        //свободное перемещение
        private void button_Height_Click(object sender, EventArgs e)//высота
        {
            if (double.TryParse(textBox_Height.Text, out double height))//число?
            {
                double speed = trackBar_Speed.Value;
                try
                {
                    FFM.PL_Move_height(height, speed);
                }
                catch (ApplicationException ex)
                {
                    textBox_Exception_AddText(ex.Message);
                }
            }
            else { ShowErrorNumberMessage(); }
        }

        private void button_Kren_Click(object sender, EventArgs e)//крен
        {
            if (double.TryParse(textBox_Kren.Text, out double kren))
            {
                double speed = trackBar_Speed.Value;
                try
                {
                    FFM.PL_Move_kren(kren, speed);
                }
                catch (ApplicationException ex)
                {
                    textBox_Exception_AddText(ex.Message);
                }
            }
            else { ShowErrorNumberMessage(); }
        }

        private void button_Tangag_Click(object sender, EventArgs e)//тангаж
        {
            if (double.TryParse(textBox_Kren.Text, out double tangag))
            {
                double speed = trackBar_Speed.Value;
                try
                {
                    FFM.PL_Move_tangag(tangag, speed);
                }
                catch (ApplicationException ex)
                {
                    textBox_Exception_AddText(ex.Message);
                }
            }
            else { ShowErrorNumberMessage(); }
        }

        //ТЕСТИРОВАНИЕ
        //Авария
        private void buttonTest_alarm_Click(object sender, EventArgs e) => FFM.PL_Alarm(true);
        private void buttonTest_resetAlarm_Click(object sender, EventArgs e) => FFM.PL_Alarm(false);

        private void button_AutoCentr_Click(object sender, EventArgs e) => FFM.PL_Auto_centr();
        private void button_AutoGorizont_Click(object sender, EventArgs e) => FFM.PL_Auto_horizon();

        private void button_Token_Click(object sender, EventArgs e) => FFM.Token__();

        private void button_StartReadDirHei_Click(object sender, EventArgs e)//старт чтения датчиков поворота и высоты
        {
            button_StartReadDirHei.Enabled = false;
            button_StopRed.Enabled = true;

            FFM.PL_Kalibration();
            timer2.Enabled = true;
        }

        private void button_StopRed_Click(object sender, EventArgs e)//стоп чтения датчиков поворота и высоты
        {
            button_StartReadDirHei.Enabled = true;
            button_StopRed.Enabled = false;

            timer2.Enabled = false;
            FFM.StopDirect();
        }

        private void button_ReverseMatch_Click(object sender, EventArgs e)//обратная математика, расчёт текущих углов и высоты на основании текущих показаний датчиков
        {
            textBox17.Text = Convert.ToString(Math.Round(FFM.PL_Read_angle_kren(), 2));
            textBox18.Text = Convert.ToString(Math.Round(FFM.PL_Read_angle_tangag(), 2));
            textBox19.Text = Convert.ToString(Math.Round(FFM.PL_Read_height(), 2));
        }


        //НАСТРОЙКИ
        private void button_Set_operating_mode_Click(object sender, EventArgs e)
        {
            FFM.PL_Set_mode((int)Modes.operating_mode);
            button_Set_setting_mode.Enabled = true;
            button_Set_operating_mode.Enabled = false;
            panel8.Enabled = false;
            groupBox_Motion_control.Enabled = true;
            groupBox_COM_ports.Enabled = true;
            groupBox_LED.Enabled = true;
            groupBox_Testing.Enabled = true;
        }

        private void button_Set_setting_mode_Click(object sender, EventArgs e)
        {
            FFM.PL_Set_mode((int)Modes.setting_mode);
            button_Set_setting_mode.Enabled = false;
            button_Set_operating_mode.Enabled = true;
            panel8.Enabled = true;
            groupBox_Motion_control.Enabled = false;
            groupBox_COM_ports.Enabled = false;
            groupBox_LED.Enabled = false;
            groupBox_Testing.Enabled = false;
        }
        private void button_SetKren_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_setKrenMin.Text, out double kren_min) && TestStrToDouble(textBox_setKrenMax.Text, out double kren_max))
                    FFM.PL_Set_angle_kren(kren_min, kren_max);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_SetTangag_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_setTangagMin.Text, out double tangag_min) && TestStrToDouble(textBox_setTangagMax.Text, out double tangag_max))
                    FFM.PL_Set_angle_tangag(tangag_min, tangag_max);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Set_maxSpeed_deviation_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_Set_maxSpeed_deviation.Text, out double maxSpeed_deviation))
                    FFM.PL_Set_maxSpeed_deviation(maxSpeed_deviation);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Set_maxSpeed_move_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_Set_maxSpeed_move.Text, out double maxSpeed_move))
                    FFM.PL_Set_maxSpeed_move(maxSpeed_move);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Set_speed_centering_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_Set_speed_centering.Text, out double Speed_centering))
                    FFM.PL_Set_Speed_centering(Speed_centering);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        private void button_Set_min_Speed_Click(object sender, EventArgs e)
        {
            try
            {
                if (TestStrToDouble(textBox_Set_min_Speed.Text, out double Min_Speed))
                    FFM.PL_Set_min_Speed_move(Min_Speed);

                ReadСurrentSettings();
            }
            catch (ApplicationException ex)
            {
                textBox_Exception_AddText(ex.Message);
            }
        }

        //ТЕКУЩИЕ ЗНАЧЕНИЯ
        /// <summary>
        /// Получает текущие настройки Мин. и Макс. значения тангажа и крена, уставки по скоростям.
        /// Отображает полученные значения в соответствующих полях на форме.
        /// </summary>
        private void ReadСurrentSettings()
        {
            Settings sett = FFM.GetSett();

            tbKrenMIN.Text = sett.Kren_min.ToString();
            tbKrenMAX.Text = sett.Kren_max.ToString();
            tbTangagMIN.Text = sett.Tangag_min.ToString();
            tbTangagMAX.Text = sett.Tangag_max.ToString();
            tbMaxSpeed_deviation.Text = sett.MaxSpeed_deviation.ToString();
            tbMaxSpeed_move.Text = sett.MaxSpeed_move.ToString();
            tbSpeed_centering.Text = sett.Speed_centering.ToString();
            tbMinSpeed.Text = sett.MinSpeed.ToString();
        }

        //ДЕМОНСТРАЦИЯ ИЗ ФАЙЛА
        private void button_DemoPusk_Click(object sender, EventArgs e)
        {
            demo.Start();
            button_DemoStop.Enabled = true;
            button_DemoPusk.Enabled = false;
            panel4.Enabled = false;
            panel5.Enabled = false;
            panel6.Enabled = false;
        }
        private void button_DemoStop_Click(object sender, EventArgs e)
        {
            demo.Stop();
            button_DemoPusk.Enabled = true;
            button_DemoStop.Enabled = false;
            panel4.Enabled = true;
            panel5.Enabled = true;
            panel6.Enabled = true;
        }

        //LED
        private void btn_redON_Click(object sender, EventArgs e) => FFM.LedRedOn();
        private void btn_redOFF_Click(object sender, EventArgs e) => FFM.LedOff();

        private void btn_greenON_Click(object sender, EventArgs e) => FFM.LedGreenOn();
        private void btn_greenOFF_Click(object sender, EventArgs e) => FFM.LedOff();

        private void btn_blueON_Click(object sender, EventArgs e) => FFM.LedBlueOn();
        private void btn_blueOFF_Click(object sender, EventArgs e) => FFM.LedOff();

        //Вспомогательные
        /// <summary>
        /// Выводит сообщения об ошибках в окно формы.
        /// </summary>
        /// <param name="massege"></param>
        private void textBox_Exception_AddText(string massege)//добавить заполнение сверху
        {
            textBox_Exception.AppendText(massege + Environment.NewLine);
        }
        /// <summary>
        /// Проверяет является ли введённый текст числом типа double
        /// </summary>
        /// <param name="argStr"></param>
        /// <param name="argDouble"></param>
        /// <returns>bool and double</returns>
        private bool TestStrToDouble(string argStr, out double argDouble)
        {
            if (double.TryParse(argStr, out double argDouble1))
            {
                argDouble = argDouble1;
                return true;
            }
            else
            {
                ShowErrorNumberMessage();
                argDouble = 0;
                return false;
            }
        }
        /// <summary>
        /// Выводит окно об ошибке ввода числового значения.
        /// </summary>
        private void ShowErrorNumberMessage()
        {
            MessageBox.Show("Вводите число.", "Ошибка", MessageBoxButtons.OK);
        }
    }
}
