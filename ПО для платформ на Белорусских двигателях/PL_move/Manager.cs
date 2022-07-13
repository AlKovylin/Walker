//Версия для работы с двигателями постоянного тока
//управление через Arduino

/////Расположение двигателей/////
//ВИД СВЕРХУ//
//   фронт
//(COM2)    (COM3)  
//   2-------3
//    \     /
//     \   /
//      \ /
//       1 (COM1)
//задний двигатель

//Датчик высоты//COM4
//Датчик направления//COM5
//Arduino//COM7
///////////////////////////////
//Перед первым запуском должна быть проведена калибровка. Калибровка производится в крайнем верхнем положении.
//Датчики нужно выставит так чтобы не было переходов через 0 (обработка таких событий не реализована), например, 3000 в крайнем верхнем положении.
//Датчик работает в противоход рычагу.
//После считывания файла калибровки, программа верно пересчитывает показания датчиков
//и верно определяет своё текущее положение.
//После калибровки нужно сместить рычаги от калибровочного положения в рабочую область, иначе
//они могут начать вращения не в ту сторону ввиду дребезга датчиков.
///////////////////////////////
/////Arduino/////
//Формат сообщения: String = "$,B,255,F,255,B,255", где $ - префикс(упр. двигателями), В и F направление вращения, 0-255 скорость
//String = "#,A", где # - префикс(led), A,B,C,D - red, green, blue, off

using System;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using Mobile_platform;

namespace DllFunc
{
    public class FuncForManager
    {
        private static COM_Sensor COM1;
        private static COM_Sensor COM2;
        private static COM_Sensor COM3;
        private static COM_Sensor COM4;
        private static COM_Sensor COM5;
        private static COM_Arduino COM7;

        private static Match Match_MP = new Match();
        private static Settings Sett = new Settings();
        private static Moving Move_;
        private static Direction Direct_;
        private static Led Leds;

        private FunkSettings FunkSett = new FunkSettings();
        private SetDevice[] deviceMass = new SetDevice[6];//имя порта, значение датчика на момент калибровки, сопоставленное устройство (NameDevice)

        //private System.Timers.Timer timerStop = new System.Timers.Timer();//таймер для принудительного завершения работы после парковки в случае не удачи

        //private string[] NameDevice = NameDevices.NameDevice;
        private Logger Log = new Logger();
        private bool flagEndMove { get; set; } = false;//флаг завершения цикла движения
        private Modes Mode = Modes.operating_mode;//0 - рабочий режим, 1 - режим настройки
        private double Ret_kren, Ret_tangag, Ret_height;
        private bool flagALARM = false;

        public FuncForManager() { }

        //КОСТЫЛЬ. Создание экземпляра соответствующего класса из Mobile_platform не удаётся прочитать настройки.
        //Нужно разобраться и поправить.
        [Serializable]
        public class SetDevice : SettDevice
        {

        }

        /// <summary>
        /// Производит начальную инициализацию устройств. Платформа не выходит из парковочного положения.
        /// </summary>
        /// <returns></returns>
        public void PL_Start()
        {
            try
            {
                ReadKalibr();
            }
            catch (Exception e)
            {
                throw new Exception("ReadKalibr. " + e.Message);
            }

            try
            {
                COM1 = new COM_Sensor(deviceMass[0].NamePort);//датчик переднего левого двигателя
                COM2 = new COM_Sensor(deviceMass[1].NamePort);//датчик переднего правого двигателя
                COM3 = new COM_Sensor(deviceMass[2].NamePort);//датчик заднего двигателя
                COM4 = new COM_Sensor(deviceMass[3].NamePort);//датчик поворота корпуса
                COM5 = new COM_Sensor(deviceMass[4].NamePort);//датчик положения по высоте
                COM7 = new COM_Arduino(deviceMass[5].NamePort);//порт Arduino

                COM1.CreatPort();
                COM2.CreatPort();
                COM3.CreatPort();
                COM4.CreatPort();
                COM5.CreatPort();
                COM7.CreatPort();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            try
            {
                Refresh_settings();//читаем настройки из файла
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            try
            {
                Move_ = new Moving(COM1, COM2, COM3, COM7, deviceMass);
                Direct_ = new Direction(COM4, COM5);
                Leds = new Led(COM7);
                //flagEndMove = false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            Move_.endMove += EndMove;//подписка на событие завершения цикла движения
            COM7.ArduinoSendDataEvent += COM7_ArduinoSendDataEvent;//подписка на событие получения данных от Arduino
            LedOff();
            Thread.Sleep(10);
            LedGreenOn();
        }

        private void COM7_ArduinoSendDataEvent(string data)
        {
            if (data == "ALARMON") { flagALARM = true; } //PL_Alarm(true); }
            if (data == "ALARMOFF") { flagALARM = false; } /*Thread.Sleep(100);*/ //LedGreenOn(); }
        }

        private void EndMove()
        {
            flagEndMove = true;
        }

        /// <summary>
        /// Переводит платформу в парковочное состояние и производит корректное завершение работы устройств
        /// </summary>
        /// <returns></returns>
        public void PL_Stop()//условно готово
        {
            try
            {
                flagEndMove = false;

                Match_MP.Set_Kren(0);
                Match_MP.Set_Tangag(0);
                PL_Move_height(PhysicalChar.Min_height, Sett.MinSpeed);

                while (!flagEndMove) { Thread.Sleep(100); }//МОЖНО ДОБАВИТЬ ВЫХОД ИЗ ЦИКЛА ПО ТАЙМЕРУ НА СЛУЧАЙ КОЛЛИЗИЙ

                LedBlueOn();

                Thread.Sleep(50);

                if (flagEndMove)
                {
                    Direct_.Stop();

                    COM1.Stop();
                    COM2.Stop();
                    COM3.Stop();
                    COM4.Stop();
                    COM5.Stop();
                    COM7.Stop();
                }
                else
                {
                    throw new ApplicationException("Ошибка завершения работы");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void StopMotors()
        {
            Move_.StopMotors();
        }

        /// <summary>
        /// Выполняет запись показаний датчиков высоты и направления. Может быть вызвана только после успешного выполнения функции "Start".
        /// </summary>
        /// <returns></returns>
        public void PL_Kalibration()//условно готово
        {
            try
            {
                Direct_.Start();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Считывает данные калибровки датчиков из файла созданного утилитой
        /// </summary>
        /// <returns></returns>
        private void ReadKalibr()//условно готово
        {
            SetDevice[] readMass;
            XmlSerializer formatter = new XmlSerializer(typeof(SetDevice[]));

            try
            {
                using (FileStream fs = new FileStream("Kalibr.xml", FileMode.Open))
                {
                    readMass = (SetDevice[])formatter.Deserialize(fs);

                    foreach (SetDevice a in readMass)
                    {
                        if (a.Device == NameDevices.NameDevice[0]) deviceMass[0] = a;//"Заднего двигателя"(1)                     
                        if (a.Device == NameDevices.NameDevice[1]) deviceMass[1] = a;//"Левого двигателя"(2)                       
                        if (a.Device == NameDevices.NameDevice[2]) deviceMass[2] = a;//"Правого двигателя"(3)   
                        if (a.Device == NameDevices.NameDevice[3]) deviceMass[3] = a;//"Датчик высоты"
                        if (a.Device == NameDevices.NameDevice[4]) deviceMass[4] = a;//"Датчик направления"
                        if (a.Device == NameDevices.NameDevice[5]) deviceMass[5] = a;//"Ардуино"                                                 
                    }
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                throw new Exception(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Блокирует платформу в текущем положении до поступления сигнала о снятии тревоги
        /// true - тревога (аварийный останов), false - отмена аварии. Возвращает true в случае удачи
        /// и Exception в случае не удачи.
        /// </summary>
        /// <param name=""></param>
        /// <returns>true || Exception</returns>
        public bool PL_Alarm(bool alarmData)//условно готово
        {
            try
            {
                if (alarmData)
                {
                    Move_.TokenCancel();//останавливаем предыдущую операцию

                    COM7.WritePort("%,A,L");
                }
                else
                {
                    COM7.WritePort("%,N,O");
                }
                flagALARM = alarmData;
                return true;
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// задаёт режим работы. 0 - рабочий режим, 1 - режим настройки и тестирования
        /// возвращает true в случае успешного включения режима или код ошибки
        /// различие режимов состоит в том, что в рабочем режиме нельзя менять настройки
        /// По умолчанию Mode = 0.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool PL_Set_mode(int data)//условно готово
        {
            if (data == (int)Modes.operating_mode || data == (int)Modes.setting_mode)
            {
                Mode = (Modes)data;
                return true;
            }
            else
            {
                throw new ApplicationException("Неверное значение режима работы.");
            }
        }

        //функции настроек (значения записываются в файл и используются при работе платформы во всех режимах)

        /// <summary>
        /// Установка ограничений крена. Диапазон значений -9,31 до 9,31.
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <returns></returns>
        public void PL_Set_angle_kren(double xmin, double xmax)//условно готово
        {
            if (Mode == Modes.setting_mode)//только в режиме настройки и тестирования
            {
                try
                {
                    FunkSett.Set_kren(xmin, xmax);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }

        /// <summary>
        /// Установка ограничений тангажа. Диапазон значений -10,72 до 10,72.
        /// </summary>
        /// <param name="ymin"></param>
        /// <param name="ymax"></param>
        /// <returns></returns>
        public void PL_Set_angle_tangag(double ymin, double ymax)//условно готово
        {
            if (Mode == Modes.setting_mode)
            {
                try
                {
                    FunkSett.Set_tangag(ymin, ymax);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }

        /// <summary>
        /// Установить макс. скорость отклонения (наклона) мм*с. Минимальная скорость 9.127 мм*с, макс. 91.27 мм*с
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Set_maxSpeed_deviation(double s)//условно готово
        {
            if (Mode == Modes.setting_mode)
            {
                try
                {
                    FunkSett.Set_maxSpeed_deviation(s);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }
        /// <summary>
        /// Установить макс. скорость перемещения (изменение положения по вертикали) мм*с. Минимальная скорость 9.127 мм*с, макс. 91.27 мм*с
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Set_maxSpeed_move(double s)//условно готово
        {
            if (Mode == Modes.setting_mode)
            {
                try
                {
                    FunkSett.Set_maxSpeed_move(s);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }
        /// <summary>
        /// Установить скорость центрирования. Минимальная скорость 9.127 мм*с, макс. 91.27 мм*с
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Set_Speed_centering(double s)//условно готово
        {
            if (Mode == Modes.setting_mode)
            {
                try
                {
                    FunkSett.Set_speed_centering(s);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }

        /// <summary>
        /// Устанавливает минимальную скорость перемещений платформы.
        /// </summary>
        /// <param name="s"></param>
        public void PL_Set_min_Speed_move(double s)//условно готово
        {
            if (Mode == Modes.setting_mode)
            {
                try
                {
                    FunkSett.Set_min_speed(s);
                    Refresh_settings();
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else { throw new ApplicationException("Данная операция возможна только в режиме настройки."); }
        }

        /// <summary>
        /// Читает текущие настройки из файла углов и скоростей.
        /// </summary>
        private void Refresh_settings()//условно готово
        {
            try
            {
                Sett = FunkSett.Get_settings();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Возвращает текущие настройки.
        /// </summary>
        /// <returns>Мин. и Макс. значения тангажа и крена, уставки по скоростям.</returns>
        public Settings GetSett()
        {
            return Sett;
        }

        //функции информирования о состоянии
        /// <summary>
        /// Функция не реализована.
        /// </summary>
        /// <returns></returns>
        public string PL_Read_status()
        {
            return "1";
        }
        /// <summary>
        /// Функция возвращает текущее направление игрока. Значения от 0 до 360
        /// </summary>
        /// <returns></returns>
        public int PL_Read_direction()//УСЛОВНО ГОТОВО
        {
            return Direct_.Get_direction();
        }
        /// <summary>
        /// Функция не реализована.
        /// </summary>
        /// <returns></returns>
        public int PL_Read_speed()
        {
            return 1;
        }
        /// <summary>
        /// Функция возвращает текущее положение пояса игрока по высоте. При калибровке присваивается значение 0.
        /// Положительные показания - движение вверх, отрицательные значения - движение вниз. Ед.изм. мм.
        /// </summary>
        /// <returns></returns>
        public int PL_Read_height_position()//УСЛОВНО ГОТОВО
        {
            return Direct_.Get_Height_position();
        }
        /// <summary>
        /// Возвращает текущее значение высоты подвижности
        /// </summary>
        /// <returns></returns>
        public double PL_Read_height()//ТРЕБУЕТ КОРРЕКТИРОВКИ ПОД НОВЫЕ ДВИГАТЕЛИ
        {
            //HKT();
            //return Ret_height;
            return 1;//заглушка
        }
        /// <summary>
        /// Возвращает текущее значение крена (наклон вправо-влево)
        /// </summary>
        /// <returns></returns>
        public double PL_Read_angle_kren()//ТРЕБУЕТ КОРРЕКТИРОВКИ ПОД НОВЫЕ ДВИГАТЕЛИ
        {
            //HKT();
            //return Ret_kren;
            return 1;//заглушка
        }
        /// <summary>
        /// Возвращает текущее значение тангажа (наклон вперёд-назад)
        /// </summary>
        /// <returns></returns>
        public double PL_Read_angle_tangag()//ТРЕБУЕТ КОРРЕКТИРОВКИ ПОД НОВЫЕ ДВИГАТЕЛИ
        {
            //HKT();
            //return Ret_tangag;
            return 1;//заглушка
        }

        /// <summary>
        /// Обеспечивает обратный пересчёт крена, тангажа и высоты от текущих углов рычагов платформы.
        /// </summary>
        private void HKT()//ТРЕБУЕТ КОРРЕКТИРОВКИ ПОД НОВЫЕ ДВИГАТЕЛИ. Функции использующие эту функцию оснащены заглушками до решения задачи.
        {
            int data1 = COM1.DataPort;
            int data2 = COM2.DataPort;
            int data3 = COM3.DataPort;

            double U1, U2, U3;

            //высчитываем абсолютное текущее положение рычагов с учётом калибровки
            //мотор 1, COM1
            if (data1 > deviceMass[0].ValueSensor) { U1 = 4096 - data1 + deviceMass[0].ValueSensor; }
            else { U1 = deviceMass[0].ValueSensor - data1; }

            //мотор 2, COM2
            if (data2 > deviceMass[1].ValueSensor) { U2 = 4096 - data2 + deviceMass[1].ValueSensor; }
            else { U2 = deviceMass[1].ValueSensor - data2; }

            //мотор 3, COM3
            if (data3 > deviceMass[2].ValueSensor) { U3 = 4096 - data3 + deviceMass[2].ValueSensor; }
            else { U3 = deviceMass[2].ValueSensor - data3; }

            //пересчитываем в градусы
            if ((U1 / PhysicalChar.Koeff_pered) < 1024) { U1 = ((1024 - (U1 / PhysicalChar.Koeff_pered)) / 1024) * 90; }
            else { U1 = -(((U1 / PhysicalChar.Koeff_pered) - 1024) / 1024) * 90; }

            if ((U2 / PhysicalChar.Koeff_pered) < 1024) { U2 = ((1024 - (U2 / PhysicalChar.Koeff_pered)) / 1024) * 90; }
            else { U2 = -(((U2 / PhysicalChar.Koeff_pered) - 1024) / 1024) * 90; }

            if ((U3 / PhysicalChar.Koeff_pered) < 1024) { U3 = ((1024 - (U3 / PhysicalChar.Koeff_pered)) / 1024) * 90; }
            else { U3 = -(((U3 / PhysicalChar.Koeff_pered) - 1024) / 1024) * 90; }

            Match_MP.Calc_parameters(U1, U2, U3, out Ret_kren, out Ret_tangag, out Ret_height);
        }

        //функции команды
        /// <summary>
        /// Задаёт изменение крена на указанную величину и с заданной скоростью.
        /// от -9,31 до 9,31; скорость от 9,127мм*с до 91,27мм*с
        /// </summary>
        /// <param name="x"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Move_kren(double x, double s)//условно готово
        {
            if (!flagALARM)
            {
                try
                {
                    if (x <= Sett.Kren_max && x >= Sett.Kren_min)
                    {
                        double speed = s > 0 ? s <= Sett.MaxSpeed_deviation ? s : s : Sett.MaxSpeed_deviation;//скорость должна быть больше нуля и меньше максимально допустимой(предустановленной)

                        Match_MP.Set_Kren(x);
                        Match_MP.Set_Tangag(0);
                        Match_MP.Set_Vysota(PhysicalChar.Average_height);
                        MoveStart(s);
                    }
                    else
                    {
                        Log.WriteLogLine($"Move_kren X = {x}, S = {s}; Недопустимый угол крена.");
                        throw new Exception("Недопустимый угол крена.");
                    }
                }
                catch (ApplicationException e)
                {
                    Log.WriteLogLine($"Move_kren X = {x}, S = {s}; Exeption {e.Message}");
                    throw new Exception(e.ToString());
                }
            }
            else
            {
                throw new Exception("Не допустимо в режиме аварии");
            }
        }
        /// <summary>
        /// Задаёт изменение крена на указанную величину и с заданной скоростью.
        /// от -10,72 до 10,72; скорость от 9,127мм*с до 91,27мм*с
        /// </summary>
        /// <param name="y"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Move_tangag(double y, double s)//условно готово
        {
            if (!flagALARM)
            {
                try
                {
                    if (y <= Sett.Tangag_max && y >= Sett.Tangag_min)
                    {
                        double speed = s > 0 ? s <= Sett.MaxSpeed_deviation ? s : s : Sett.MaxSpeed_deviation;

                        Match_MP.Set_Tangag(y);
                        Match_MP.Set_Tangag(0);
                        Match_MP.Set_Vysota(PhysicalChar.Average_height);
                        MoveStart(s);
                    }
                    else
                    {
                        Log.WriteLogLine($"Move_tangag Y = {y}, S = {s}; Недопустимый угол тангажа.");
                        throw new Exception("Недопустимый угол тангажа.");
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLogLine($"Move_tangag Y = {y}, S = {s}; Exeption {e.Message}");
                    throw new Exception(e.ToString());
                }
            }
            else
            {
                throw new Exception("Не допустимо в режиме аварии");
            }
        }
        /// <summary>
        /// Задаёт изменение высоты на указанную величину и с заданной скоростью.
        /// от 91 до 191 (лучше работать в диапазоне от 95 до 185); скорость от 9,127мм*с до 91,27мм*с
        /// </summary>
        /// <param name="z"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Move_height(double z, double s)//условно готово
        {
            if (!flagALARM)
            {
                double height;
                try
                {
                    if (z <= PhysicalChar.Max_height - 15 && z >= PhysicalChar.Min_height + 15)//15-ограничение хода по высоте чтобы не клинило
                    {
                        Match_MP.Set_Vysota(z);
                        Match_MP.Set_Tangag(0);
                        Match_MP.Set_Tangag(0);
                        MoveStart(s);
                    }
                    else
                    {
                        Log.WriteLogLine($"Move_height Z = {z}, S = {s}; Недопустимая высота.");
                        throw new Exception("Недопустимая высота");
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLogLine($"Move_height Z = {z}, S = {s}; Exeption {e.Message}");
                    throw new Exception(e.ToString());
                }
            }
            else
            {
                throw new Exception("Не допустимо в режиме аварии");
            }
        }
        /// <summary>
        /// Задаёт изменение по двум координатам на указанную величину и с заданной скоростью.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public void PL_Move(double x, double y, double s)//условно готово
        {
            if (!flagALARM)
            {
                try
                {
                    if (x <= Sett.Kren_max && x >= Sett.Kren_min)
                        Match_MP.Set_Kren(x);
                    else
                    {
                        Log.WriteLogLine($"Move_kren X = {x}, S = {s}; Недопустимый угол крена.");
                        throw new Exception("Недопустимый угол крена.");
                    }


                    if (y <= Sett.Tangag_max && y >= Sett.Tangag_min)
                        Match_MP.Set_Tangag(y);
                    else
                    {
                        Log.WriteLogLine($"Move_tangag Y = {y}, S = {s}; Недопустимый угол тангажа.");
                        throw new Exception("Недопустимый угол тангажа.");
                    }

                    MoveStart(s);
                }
                catch (Exception e)
                {
                    Log.WriteLogLine($"Move X = {x}, Y = {y}, S = {s}; Exeption {e.Message}");
                    throw new Exception(e.ToString());
                }
            }
            else
            {
                throw new Exception("Не допустимо в режиме аварии");
            }
        }
        /// <summary>
        /// Выполняет подготовку к запуску новой итерации.
        /// </summary>
        /// <param name="speed"></param>
        private void MoveStart(double speed)//условно готово
        {
            double Speed;

            if (speed < Sett.MinSpeed)
                speed = Sett.MinSpeed;

            Move_.TokenCancel();
            Thread.Sleep(5);

            try
            {
                Match_MP.Calc_ugol_rychaga(out double Ugol_T1, out double Ugol_T2, out double Ugol_T3);

                if (!double.IsNaN(Ugol_T1) && !double.IsNaN(Ugol_T2) && !double.IsNaN(Ugol_T3))
                {
                    Speed = Match_MP.Calc_speed(speed);
                    Move_.Preparation(Ugol_T1, Ugol_T2, Ugol_T3, Speed);
                }
                else
                {
                    Log.WriteLogLine($"MoveStart. Не удаётся рассчитать углы. Задано не достижимое " +
                        $"положение платформы. Результат вычислений: Ugol_T1 = {Ugol_T1}, Ugol_T2 = {Ugol_T2}, Ugol_T3 = {Ugol_T3}");
                    throw new ApplicationException("Не удаётся рассчитать углы. Задано не достижимое положение платформы.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogLine($"MoveStart. {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        //автоматические функции
        /// <summary>
        /// Автоматического центрирования по вертикали
        /// </summary>
        /// <returns></returns>
        public void PL_Auto_centr()//условно готово
        {
            if (!flagALARM)
            {
                try
                {
                    Match_MP.Set_Vysota(PhysicalChar.Average_height);
                    Match_MP.Set_Kren(0);
                    Match_MP.Set_Tangag(0);
                    MoveStart(PhysicalChar.Average_height_speed);
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException(e.ToString());
                }
            }
            else
            {
                throw new ApplicationException("Не допустимо в режиме аварии");
            }
        }
        /// <summary>
        /// Автоматического приведения в горизонтальное состояние
        /// </summary>
        /// <returns></returns>
        public void PL_Auto_horizon()//ТРЕБУЕТСЯ ТЕСТИРОВАНИЕ И ПОДТВЕРЖДЕНИЕ ПРАВИЛЬНОСТИ ПОНИМАНИЯ ЗАДАЧИ
        {
            /*if (!flagALARM)
            {
                PL_Read_angle_tangag();
                if (Match_MP.Set_Vysota(Ret_height) && Match_MP.Set_Kren(0) && Match_MP.Set_Tangag(0))
                {
                    MoveStart(Sett.Speed_centering);//НУЖНО ВОЗВРАЩАЕМОЕ ЗНАЧЕНИЕ ПРИ NaN
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }*/
        }

        #region Для отладки
        public void SetSpeeds(float s)
        {
            //Move_.Set_Speed_1(s);
            //Move_.Set_Speed_2(s);
            //Move_.Set_Speed_3(s);
        }

        public int ReadSensor_1()
        {
            return COM1.DataPort;
        }
        public int ReadSensor_2()
        {
            return COM2.DataPort;
        }
        public int ReadSensor_3()
        {
            return COM3.DataPort;
        }
        public int ReadSensor_4()
        {
            return COM4.DataPort;
        }
        public int ReadSensor_5()
        {
            return COM5.DataPort;
        }
        public void Token__()
        {
            Move_.TokenCancel();
        }
        public void StopWriter()
        {
            //Move_.StopWriteLog();
            Log.CloseWriteLog();
        }

        public void StopDirect()
        {
            Direct_.Stop();
        }

        public void LedRedOn()
        {
            Leds.RedON();
        }

        public void LedGreenOn()
        {
            Leds.GreenON();
        }

        public void LedBlueOn()
        {
            Leds.BlueON();
        }
        public void LedOff()
        {
            Leds.OFF_LED();
        }
        #endregion
    }
}
