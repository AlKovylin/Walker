//формат строки файла
//крен,тангаж,высота,скорость
//(double,double,double,double)
//в одной строке должны быть заданы значения либо крена и тангажа, либо высоты
//соответственно не заданный параметр должен быть равен нолю
//примеры верных строк:
//5,5,0,60,5   0,0,50,60,4   0,7,0,60,3   5,0,0,60,5
//таймер отправляет команды на выполнение каждые 100мс
//команда 0,0,0,0,5 или 0,0,0,70,8 - ничего не делаем
//последняя цифра указывает через какое количество циклов таймера равных 200мс нужно начать выполнение следующей команды
//
//попытка устанавливать новый интервал таймера для каждой команды оказалась не удачной,
//после каждой второй перезаписи интервал оказывался значительно больше ожидаемого
//вычитание предыдущего значения не помогло, деление пополам - да, но актуально только для равных промежутков, но тогда зачем огород...
//два таймера перезапускающих друг друга проблему не решили

using DllFunc;
using Mobile_platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;

namespace FormForTest
{
    class Demo
    {
        private FuncForManager funcForManager { get; set; }
        private System.Timers.Timer timer = new System.Timers.Timer();
        private List<StructCommand> DemoCommands = new List<StructCommand>();
        /// <summary>
        /// Счётчик команд для движения по списку DemoCommands.
        /// </summary>
        private int chCommand;
        /// <summary>
        /// Счётчик количества циклов таймера до начала выполнения следующей команды. 
        /// Позволяет задать время выполнения команды с шагом равным интервалу таймера.
        /// На данный момент с шагом 200мс.
        /// </summary>
        private int chNumberOfTimes;
        private string path = "Demo.txt";

        public Demo(FuncForManager funcForManager)
        {
            this.funcForManager = funcForManager;
            timer.Interval = 200;
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        public void Start()
        {
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] command = line.Split(new char[] { ',' });
                            StructCommand SC = new StructCommand();
                            SC.Kren = double.Parse(command[0]);
                            SC.Tangag = double.Parse(command[1]);
                            SC.Height = double.Parse(command[2]);
                            SC.Speed = double.Parse(command[3]);
                            SC.NumberOfTimes = int.Parse(command[4]);
                            DemoCommands.Add(SC);
                        }
                    }
                    chCommand = 0;//сбрасываем счётчик команд
                    chNumberOfTimes = DemoCommands[chCommand].NumberOfTimes;//устанавливаем кол-во циклов для первой команды
                    funcForManager.LedOff();
                    Thread.Sleep(5);
                    funcForManager.LedRedOn();
                    Thread.Sleep(5);
                    funcForManager.LedBlueOn();
                    //Thread.Sleep(5);
                    timer.Start();
                    RunCommand();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (chNumberOfTimes == 0)//если время выполнения предыдущей команды завершено
            {
                chCommand++;//счётчик на следующую команду
                
                if (chCommand > DemoCommands.Count)//если дошли до конца,
                    chCommand = 0;//то возвращаемся в начало

                chNumberOfTimes = DemoCommands[chCommand].NumberOfTimes;//устанавливаем кол-во циклов для новой команды

                RunCommand();
            }
            else
            {
                chNumberOfTimes--;
            }
        }

        /// <summary>
        /// Запускает выполнение отдельной команды
        /// </summary>
        private void RunCommand()
        {
            if (DemoCommands[chCommand].Height > 0)//если указана высота, то выполняем перемещение по высоте
            {
                funcForManager.PL_Move_height(DemoCommands[chCommand].Height, DemoCommands[chCommand].Speed);
            }
            else
            {
                funcForManager.PL_Move(DemoCommands[chCommand].Kren, DemoCommands[chCommand].Tangag, DemoCommands[chCommand].Speed);
            }
        }

        public void Stop()
        {
            Settings Sett = new Settings();
            double speed = Sett.MinSpeed;

            timer.Stop();//завершаем цикл
            
            funcForManager.PL_Move_kren(0, speed);//выравниваем по крену
            Thread.Sleep(2000);
            funcForManager.PL_Move_tangag(0, speed);//выравниваем по тангажу
            Thread.Sleep(2000);

            var vysota = PhysicalChar.Average_height;
            funcForManager.PL_Move_height(vysota, speed);//переводим в центральное положение

            Thread.Sleep(5);
            funcForManager.LedOff();
            Thread.Sleep(5);
            funcForManager.LedGreenOn();
        }
    }
}