//Обработка концевых выключателей и отключение двигателей при их срабатывании
//происходит в Arduino

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mobile_platform
{
    public class Moving
    {
        private COM_Sensor COM1;// { get; set; }//датчик заднего двигателя
        private COM_Sensor COM2;// { get; set; }//датчик левого  двигателя
        private COM_Sensor COM3;// { get; set; }//датчик правого двигателя
        private COM_Arduino COM7;// { get; set; }//ардуино

        private SettDevice[] deviceKalibr { get; set; }//имя порта, значение датчика на момент калибровки, сопоставленное устройство (NameDevice)

        CancellationTokenSource token = new CancellationTokenSource();
        //private Logger Log = new Logger();

        public delegate void StopMove();
        public event StopMove endMove;

        private int Ugol_T1, Ugol_T2, Ugol_T3;//углы рычагов двигателей (в абсолютные величины соответствующие ед. изм. датчикам)
        private int Ugol_T1_Tec, Ugol_T2_Tec, Ugol_T3_Tec;//текущий угол 
        private int Ugol_put_1, Ugol_put_2, Ugol_put_3;//путь      

        private double Speed_1, Speed_2, Speed_3;//скорости двигателей
        private int napravlenie_1, napravlenie_2, napravlenie_3;
        private string vector_1, vector_2, vector_3;

        public bool flagStop = false;

        private string Message_move;//сообщение к Arduino
        private const string Forward = "F";//движение вниз
        private const string Back = "B";//движение вверх

        private bool flagALARM = false;

        public Moving(COM_Sensor port1, COM_Sensor port2, COM_Sensor port3, COM_Arduino port7, SettDevice[] device)
        {
            COM1 = port1;
            COM2 = port2;
            COM3 = port3;
            COM7 = port7;

            COM7.ArduinoSendDataEvent += COM7_ArduinoSendDataEvent;//подписка на событие получения данных от Arduino

            deviceKalibr = device;
        }

        private void COM7_ArduinoSendDataEvent(string data)
        {
            if (data == "ALARMON")//нажата кнопка "авария"
            {
                TokenCancel();//прерываем текущую задачу
                endMove.Invoke();//вызываем событие прекращения работы
                flagALARM = true;
            }
            if (data == "ALARMOFF")//кнопка "авария" отпущена
            {
                flagALARM = false;
            }
        }

        /// <summary>
        /// Переводит градусное значение углов, которое нужно достичь, в абсолютные величины соответствующие ед. изм. датчиков. 
        /// Запускает платформу и передаёт управление функции отслеживания текущего положения.
        /// </summary>
        /// <param name="ugol_T1"></param>
        /// <param name="ugol_T2"></param>
        /// <param name="ugol_T3"></param>
        /// <param name="speed"></param>
        public void Preparation(double ugol_T1, double ugol_T2, double ugol_T3, double speed)
        {
            //double speed = _speed;
            //переводим градусное значение углов, которое нужно достичь, в абсолютные величины соответствующие ед. изм. датчиков
            //c учётом коэфф. передачи на шестернях между редуктором и датчиком
            //90 градусов из математики при пересчёте = 0 - это крайнее верхнее положение,
            //0 градусов = 1396 единиц в данных датчика
            //-90 гр. = 2792
            Ugol_T1 = (int)((1024 - ugol_T1 / 90 * 1024) * PhysicalChar.Koeff_pered);
            Ugol_T2 = (int)((1024 - ugol_T2 / 90 * 1024) * PhysicalChar.Koeff_pered);
            Ugol_T3 = (int)((1024 - ugol_T3 / 90 * 1024) * PhysicalChar.Koeff_pered);

            //получаем текущее положение в абсолютных значениях сенсоров
            Tec_polog_rychagov();

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

            //ограничение скорости при малых углах
            /*if (Ugol_put_1 < Ugol_put_2 && Ugol_put_1 < Ugol_put_3)//угол 1 наименьший
            {
                if ((speed / Ugol_put_1) > 0.01) { speed = 2.2; }
            }
            else if (Ugol_put_2 < Ugol_put_3)//угол 2 наименьший
            {
                if ((speed / Ugol_put_2) > 0.01) { speed = 2.2; }
            }
            else//угол 3 наименьший
            {
                if ((speed / Ugol_put_3) > 0.01) { speed = 2.2; }
            }*/

            //вычисляем скорости для каждого двигателя
            //находим наибольший угол пути и для его прохождения применяем наибольшую скорость остальные расчитываем в пропорции
            /*if (Ugol_put_1 > Ugol_put_2 && Ugol_put_1 > Ugol_put_3)
            {
                Speed_1 = speed;
                Speed_2 = Speed_1 * Ugol_put_2 / Ugol_put_1;
                Speed_3 = Speed_1 * Ugol_put_3 / Ugol_put_1;
            }
            else if (Ugol_put_2 > Ugol_put_3)
            {
                Speed_2 = speed;
                Speed_1 = Speed_2 * Ugol_put_1 / Ugol_put_2;
                Speed_3 = Speed_2 * Ugol_put_3 / Ugol_put_2;
            }
            else
            {
                Speed_3 = speed;
                Speed_1 = Speed_3 * Ugol_put_1 / Ugol_put_3;
                Speed_2 = Speed_3 * Ugol_put_2 / Ugol_put_3;
            }*/


            //делаем скорость всех двигателей одинаковой
            //если требуется вычисление скоростей относительно пути - этот код закамментить, а выше открыть
            Speed_1 = speed;
            Speed_2 = speed;
            Speed_3 = speed;

            //формируем сообщение для пуска моторов            
            if (napravlenie_1 < 0) { vector_1 = Back; } else { vector_1 = Forward; }//> 0 по часовой, < 0 - против
            if (napravlenie_2 < 0) { vector_2 = Back; } else { vector_2 = Forward; }
            if (napravlenie_3 < 0) { vector_3 = Back; } else { vector_3 = Forward; }

            GenerationgMessage();

            COM7.WritePort(Message_move);//отправка сообщения на пуск двигателей      

            token = new CancellationTokenSource();//создаём новый экземпляр токена для новой задачи
            Run_moveAsync();//запускаем цикл отслеживания положения рычагов двигателей                                
        }

        /// <summary>
        /// Запускает выполнение асинхронной задачи 
        /// </summary>
        private async void Run_moveAsync()
        {
            if (!flagALARM)
            {
                await Task.Factory.StartNew(() => { Run_move(token.Token); });
            }
        }

        /// <summary>
        /// Функция останова двигателей при достижении нужного положения, принимает токен в качестве параметра для внешнего прерывания выполнения операции
        /// </summary>
        /// <param name="token"></param>
        private void Run_move(CancellationToken token)
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
                if (f1)
                {
                    if (napravlenie_1 > 0)//вниз
                    {
                        if (Ugol_T1_Tec >= Ugol_T1)
                        {
                            StopMotor1();
                            f1 = false;
                        }
                        //if ((Ugol_T1_Tec + PhysicalChar.RunOutDistance) > Ugol_T1)//костыль регулирования скорости чтобы не ломать концевики
                            //Speed_1 = PhysicalChar.RunOutSpeed;
                    }
                    else if (napravlenie_1 < 0)//вверх 
                    {
                        if (Ugol_T1_Tec <= Ugol_T1)
                        {
                            StopMotor1();
                            f1 = false;
                        }
                        //if ((Ugol_T1_Tec - PhysicalChar.RunOutDistance) < Ugol_T1)//костыль регулирования скорости чтобы не ломать концевики
                          //Speed_1 = PhysicalChar.RunOutSpeed;
                    }
                }
                //ДВИГАТЕЛЬ 2
                if (f2)
                {
                    if (napravlenie_2 > 0)//вниз
                    {
                        if (Ugol_T2_Tec >= Ugol_T2)
                        {
                            StopMotor2();
                            f2 = false;
                        }
                        //if ((Ugol_T2_Tec + PhysicalChar.RunOutDistance) > Ugol_T2)//костыль регулирования скорости чтобы не ломать концевики
                          // Speed_2 = PhysicalChar.RunOutSpeed;
                    }
                    else if (napravlenie_2 < 0)//вверх 
                    {
                        if (Ugol_T2_Tec <= Ugol_T2)
                        {
                            StopMotor2();
                            f2 = false;
                        }
                        //if ((Ugol_T2_Tec - PhysicalChar.RunOutDistance) < Ugol_T2)//костыль регулирования скорости чтобы не ломать концевики
                           // Speed_2 = PhysicalChar.RunOutSpeed;
                    }
                }
                //ДВИГАТЕЛЬ 3
                if (f3)
                {
                    if (napravlenie_3 > 0)//вниз
                    {
                        if (Ugol_T3_Tec >= Ugol_T3)
                        {
                            StopMotor3();
                            f3 = false;
                        }
                        //if ((Ugol_T3_Tec + PhysicalChar.RunOutDistance) > Ugol_T3)//костыль регулирования скорости чтобы не ломать концевики
                           // Speed_3 = PhysicalChar.RunOutSpeed;
                    }
                    else if (napravlenie_3 < 0)//вверх 
                    {
                        if (Ugol_T3_Tec <= Ugol_T3)
                        {
                            StopMotor3();
                            f3 = false;
                        }
                        //if ((Ugol_T3_Tec - PhysicalChar.RunOutDistance) < Ugol_T3)//костыль регулирования скорости чтобы не ломать концевики
                           //Speed_3 = PhysicalChar.RunOutSpeed;
                    }
                }

                //if (!f1 && !f2 && !f3)
                if (f1 == false && f2 == false && f3 == false)
                {
                    //StopMotors(); 
                    f = false;
                    endMove.Invoke();
                    //Log.WriteLogLine($"П1 = {Ugol_T1 - Ugol_T1_Tec}, П2 = {Ugol_T2 - Ugol_T2_Tec}, П3 = {Ugol_T3 - Ugol_T3_Tec}");
                }

                if (token.IsCancellationRequested)
                {
                    f = false;
                    StopMotors();
                }
            }
        }
        private void Tec_polog_rychagov()//расчёт текущего положения рычагов
        {
            //получаем текущее положение в абсолютных значениях сенсоров
            int Data_1 = COM1.DataPort;
            int Data_2 = COM2.DataPort;
            int Data_3 = COM3.DataPort;

            //высчитываем абсолютное текущее положение рычагов с учётом калибровки
            Ugol_T1_Tec = deviceKalibr[0].ValueSensor - Data_1 + 698 + 250;//698 соответствует 45 гр. в математике и крайнему верхнему положению рычага моторредуктора
            Ugol_T2_Tec = deviceKalibr[1].ValueSensor - Data_2 + 698 + 250;//250 - дополнительный коэефф. для выравнивания рычага в горизонте, доп. см. сlass PhysicalChar Max_height и Min_height
            Ugol_T3_Tec = deviceKalibr[2].ValueSensor - Data_3 + 698 + 250;
        }

        /// <summary>
        /// Отменяет выполнение текущей итерации перемещения.
        /// </summary>
        public void TokenCancel()
        {
            token.Cancel();
        }

        public void StopMotors()
        {
            COM7.WritePort("$,B,0,B,0,B,0");
        }

        private void StopMotor1()
        {
            Speed_1 = 0;
            GenerationgMessage();
            COM7.WritePort(Message_move);
        }

        private void StopMotor2()
        {
            Speed_2 = 0;
            GenerationgMessage();
            COM7.WritePort(Message_move);
        }

        private void StopMotor3()
        {
            Speed_3 = 0;
            GenerationgMessage();
            COM7.WritePort(Message_move);
        }

        private void GenerationgMessage()//формирование сообщения в arduino
        {
            Message_move = "$," + vector_1 + ',' + Math.Round(Speed_1).ToString() + ',' + vector_2 + ',' + Math.Round(Speed_2).ToString() + ',' + vector_3 + ',' + Math.Round(Speed_3).ToString();
        }
    }
}
