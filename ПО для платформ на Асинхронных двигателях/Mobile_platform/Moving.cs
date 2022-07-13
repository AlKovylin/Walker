using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;



namespace Mobile_platform
{
    public class Moving
    {
        private COM_Sensor COM1 { get; set; }//датчик переднего левого двигателя
        private COM_Sensor COM2 { get; set; }//датчик переднего правого двигателя
        private COM_Sensor COM3 { get; set; }//датчик заднего двигателя
        
        private AO_DO_DI PioDa { get; set; }
        private SettDevice[] deviceKalibr{ get; set; }//имя порта, значение датчика на момент калибровки, сопоставленное устройство (NameDevice)

        private Settings Sett { get; set; }      

        CancellationTokenSource token = new CancellationTokenSource();

        //private StreamWriter sw = new StreamWriter("LogMoving2.txt", true);

        public delegate void StopMove();
        public event StopMove endMove;

        private int Ugol_T1, Ugol_T2, Ugol_T3;//углы рычагов двигателей (в абсолютные величины соответствующие ед. изм. датчикам)
        private int Ugol_T1_Tec, Ugol_T2_Tec, Ugol_T3_Tec;//текущий угол 
        private int Ugol_put_1, Ugol_put_2, Ugol_put_3;//путь

        //минимальное и максимальное положение в единицах датчика
        private int Min_ = 0;
        private int Max_ = 3449;

        private double Kpered = 1.68421;//соотношение шестерней на редукторе и на датчике
        //private double Kpered = 0.59375;

        private double Speed_1, Speed_2, Speed_3, Speed_current;//_1, Speed_current_2, Speed_current_3;

        int napravlenie_1, napravlenie_2, napravlenie_3;       

        public bool flagStop = false;       
                
        public Moving(COM_Sensor port1, COM_Sensor port2, COM_Sensor port3, AO_DO_DI pioDa, SettDevice[] device, Settings sett)
        {
            COM1 = port1;
            COM2 = port2;
            COM3 = port3;
            
            PioDa = pioDa;
            deviceKalibr = device;
            Sett = sett;
        }

        public void Preparation(double ugol_T1, double ugol_T2, double ugol_T3, double speed)
        {            
            //переводим градусное значение углов которые нужно достич в абсолютные величины соответствующие ед. изм. датчикам
            //c учётом коэфф. передачи на шестернях между редуктором и датчиком и обратным вращением датчика
            if (ugol_T1 >= 0) { Ugol_T1 = (int)((1024 - (ugol_T1 / 90 * 1024)) * Kpered); }
            else { Ugol_T1 = (int)((-ugol_T1 / 90 * 1024 + 1024) * Kpered); }

            if (ugol_T2 >= 0) { Ugol_T2 = (int)((1024 - (ugol_T2 / 90 * 1024)) * Kpered); }
            else { Ugol_T2 = (int)((-ugol_T2 / 90 * 1024 + 1024) * Kpered); }

            if (ugol_T3 >= 0) { Ugol_T3 = (int)((1024 - (ugol_T3 / 90 * 1024)) * Kpered); }
            else { Ugol_T3 = (int)((-ugol_T3 / 90 * 1024 + 1024) * Kpered); }

            //получаем текущее положение в абсолютных значениях сенсоров
            Tec_polog_rychagov();//текущее положение рычагов 

            //определяем путь для каждого рычага для вычисления индивидуальных скоростей
            Ugol_put_1 = Ugol_T1 - Ugol_T1_Tec;
            Ugol_put_2 = Ugol_T2 - Ugol_T2_Tec;
            Ugol_put_3 = Ugol_T3 - Ugol_T3_Tec;

            //направление вращения определяется знаком угла, сохраним эти значения
            napravlenie_1 = Ugol_put_1;
            napravlenie_2 = Ugol_put_2;
            napravlenie_3 = Ugol_put_3;

            //приводим значение углов пути к положительному для вычисления скоростей
            if (Ugol_put_1 < 0) { Ugol_put_1 = -Ugol_put_1; }
            if (Ugol_put_2 < 0) { Ugol_put_2 = -Ugol_put_2; }
            if (Ugol_put_3 < 0) { Ugol_put_3 = -Ugol_put_3; }

            //вычисляем скорости для каждого двигателя
            //находим наибольший угол пути и для его прохождения применяем наибольшую скорость остальные расчитываем в пропорции
            if (Ugol_put_1 > Ugol_put_2 && Ugol_put_1 > Ugol_put_3)
            {               
                Speed_1 = speed;                             
                Speed_2 = (Speed_1 * Ugol_put_2) / Ugol_put_1;
                Speed_3 = (Speed_1 * Ugol_put_3) / Ugol_put_1;
            }
            else if (Ugol_put_2 > Ugol_put_3)
            {
                Speed_2 = speed;
                Speed_1 = (Speed_2 * Ugol_put_1) / Ugol_put_2;
                Speed_3 = (Speed_2 * Ugol_put_3) / Ugol_put_2;
            }
            else
            {             
                Speed_3 = speed;
                Speed_1 = (Speed_3 * Ugol_put_1) / Ugol_put_3;
                Speed_2 = (Speed_3 * Ugol_put_2) / Ugol_put_3;
            }

            Set_Speed_1((float)Speed_1);
            Set_Speed_2((float)Speed_2);
            Set_Speed_3((float)Speed_3);

            Pusk_motor1(napravlenie_1);
            Pusk_motor2(napravlenie_2);
            Pusk_motor3(napravlenie_3);

            //WriteLog("ВХОД В ЦИКЛ\r\n");            

            token = new CancellationTokenSource();//создаём новый экземпляр токена для новой задачи
            Run_moveAsync();//запускаем цикл отслеживания положения рычагов двигателей                                
        }        

        private async void Run_moveAsync()
        {
            await Task.Factory.StartNew(() => { Run_move(token.Token); });//запускаем выполнение асинхронной задачи  
        }
        private void Run_move(CancellationToken token)//функция останова двигателей при достижении нужного положения, принимает токен в качестве параметра для внешнего прерывания выполнения операции
        {
            bool f1 = true;
            bool f2 = true;
            bool f3 = true;

            bool f = true;

            while (f)
            {
                Tec_polog_rychagov();//текущее положение
                
                //в зависимости от направления вращения проверяем не достигнут ли заданный угол и если да, то стоп двигатель
                //ДВИГАТЕЛЬ 1               
                if (napravlenie_1 > 0)//вниз
                {
                    if (Ugol_T1_Tec >= Ugol_T1-100)
                    { Stop_motor1(); f1 = false; }

                    if (Ugol_T1_Tec <= Min_ + 200 && Speed_1 > 15)
                    { Set_Speed_1(15); }                             
                }
                else//вверх 
                {
                    if (Ugol_T1_Tec <= Ugol_T1 + 100)
                    { Stop_motor1(); f1 = false; }

                    if (Ugol_T1_Tec >= Max_ - 200 && Speed_1 > 15)
                    { Set_Speed_1(15); }
                }

                /*if (napravlenie_1 > 0)//вниз
                {
                    if (Ugol_T1_Tec >= Ugol_T1 - 10)//останов мотора при достижении нужного положения с упреждающим зазором
                    { Stop_motor1(); f1 = false; }

                    if((100 - (Ugol_T1_Tec / Ugol_T1 * 100)) <= 40)// && Speed_1 > 15)//20% до конца пути
                    {
                        Speed_1 = Speed_1 * (1 - Ugol_T1_Tec / Ugol_T1);
                        Set_Speed_1((float)Speed_1);
                    }
                }
                else//вверх 
                {
                    if (Ugol_T1_Tec <= Ugol_T1 + 10)
                    { Stop_motor1(); f1 = false; }

                    if ((100 - (Ugol_T1 / Ugol_T1_Tec * 100)) <= 40)// && Speed_1 > 15)//20% до конца пути
                    {
                        Speed_1 = Speed_1 * (1 - Ugol_T1 / Ugol_T1_Tec);
                        Set_Speed_1((float)Speed_1);
                    }
                }*/


                //ДВИГАТЕЛЬ 2               
                if (napravlenie_2 > 0)//вниз
                {
                    if (Ugol_T2_Tec >= Ugol_T2 - 100)
                    { Stop_motor2(); f2 = false; }

                    if (Ugol_T2_Tec <= Min_ + 200 && Speed_2 > 15)
                    { Set_Speed_2(15); }  
                }
                else//вверх 
                {
                    if (Ugol_T2_Tec <= Ugol_T2 + 100)
                    { Stop_motor2(); f2 = false; }

                    if (Ugol_T2_Tec >= Max_ - 200 && Speed_2 > 15)
                    { Set_Speed_2(15); }
                }

                //ДВИГАТЕЛЬ 3
                if (napravlenie_3 > 0)//вниз
                {
                    if (Ugol_T3_Tec >= Ugol_T3 - 100)
                    { Stop_motor3(); f3 = false; }

                    if (Ugol_T3_Tec <= Min_ + 200 && Speed_3 > 15)
                    { Set_Speed_3(15); }  
                }
                else//вверх 
                {
                    if (Ugol_T3_Tec <= Ugol_T3 + 100)
                    { Stop_motor3(); f3 = false; }

                    if (Ugol_T3_Tec >= Max_ - 200 && Speed_3 > 15)
                    { Set_Speed_3(15); }
                }

                if (!f1 && !f2 && !f3) { StopMotors(); f = false; endMove(); }

                if (token.IsCancellationRequested)
                {
                    StopMotors();
                    f = false;
                }

                //WriteLog("Ugol_T1_Tec: " + Ugol_T1_Tec.ToString());
                //WriteLog("Ugol_T1: " + Ugol_T1.ToString());
                //WriteLog("Speed_current: " + Speed_current.ToString());
                
                //WriteLog("");

                //Thread.Sleep(5);
            }
        }
        private void Tec_polog_rychagov()//расчёт текущего положения рычагов
        {
            //получаем текущее положение в абсолютных значениях сенсоров
            int Data_1 = COM1.DataPort;
            int Data_2 = COM2.DataPort;
            int Data_3 = COM3.DataPort;
            //высчитываем абсолютное текущее положение рычагов с учётом калибровки
            //мотор 1, COM1
            if (Data_1 > deviceKalibr[0].ValueSensor) { Ugol_T1_Tec = 4096 - Data_1 + deviceKalibr[0].ValueSensor; }
            else { Ugol_T1_Tec = deviceKalibr[0].ValueSensor - Data_1; }

            //мотор 2, COM2
            if (Data_2 > deviceKalibr[1].ValueSensor) { Ugol_T2_Tec = 4096 - Data_2 + deviceKalibr[1].ValueSensor; }
            else { Ugol_T2_Tec = deviceKalibr[1].ValueSensor - Data_2; }

            //мотор 3, COM3
            if (Data_3 > deviceKalibr[2].ValueSensor) { Ugol_T3_Tec = 4096 - Data_3 + deviceKalibr[2].ValueSensor; }
            else { Ugol_T3_Tec = deviceKalibr[2].ValueSensor - Data_3; }
        }
        /*public void Tec_polog_rychagov(out int u1, out int u2, out int u3)
        {
            Tec_polog_rychagov();
        }*/
        public void TokenCancel()//вызов отмены выполнения текущей операции
        {
            token.Cancel();
        }

        public void Refresh_settings(Settings sett)
        {
            Sett = sett;
        }

        public Settings Get_settings()
        {
            return Sett;
        }

        //управление моторами
        private void Pusk_motor1(int vector)//>0 по часовой, <0 - против
        {
            if(vector > 0)
            {
                PioDa.WriteDO(0, 1);                
            }
            else if(vector < 0)
            {
                PioDa.WriteDO(1, 1);                
            }
        }
        private void Pusk_motor2(int vector)//>0 по часовой, <0 - против
        {
            if (vector > 0)
            {
                PioDa.WriteDO(2, 1);
            }
            else if (vector < 0)
            {
                PioDa.WriteDO(3, 1);
            }
        }
        private void Pusk_motor3(int vector)//>0 по часовой, <0 - против
        {
            if (vector > 0)
            {
                PioDa.WriteDO(4, 1);
            }
            else if (vector < 0)
            {
                PioDa.WriteDO(5, 1);
            }
        }
        public void Stop_motor1()
        {
            PioDa.WriteDO(0, 0);
            PioDa.WriteDO(1, 0);
        }
        public void Stop_motor2()
        {
            PioDa.WriteDO(2, 0);
            PioDa.WriteDO(3, 0);
        }
        public void Stop_motor3()
        {
            PioDa.WriteDO(4, 0);
            PioDa.WriteDO(5, 0);
        }

        public void StopMotors()
        {
            Stop_motor1();
            Stop_motor2();
            Stop_motor3();
            Set_Speed_1(0);
            Set_Speed_2(0);
            Set_Speed_3(0);
        }
        public void Set_Speed_1(float fValue)//0-10 вольт
        {
            PioDa.WriteAOVoltage(0, fValue);
        }
        public void Set_Speed_2(float fValue)//0-10 вольт
        {
            PioDa.WriteAOVoltage(1, fValue);
        }
        public void Set_Speed_3(float fValue)//0-10 вольт
        {
            PioDa.WriteAOVoltage(2, fValue);
        }
        /*public void WriteLog(string arg)
        {
            //sw.WriteLine((curData = DateTime.Now).ToString() + "|| " + arg);
            try
            {
                sw.WriteLine(arg);
            }
            catch { }
        }
        public void StopWriteLog()
        {
            sw.Close();
        }*/
    }
}
