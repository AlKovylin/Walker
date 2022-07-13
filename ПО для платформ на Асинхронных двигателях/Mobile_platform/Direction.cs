using System.Threading;
using System.Threading.Tasks;

namespace Mobile_platform
{
    public class Direction
    {
        private COM_Sensor COM4 { get; set; }//датчик высоты
        private COM_Sensor COM5 { get; set; }//датчик направления

        CancellationTokenSource tokenD = new CancellationTokenSource();
        CancellationTokenSource tokenH = new CancellationTokenSource();

        private object locker = new object();
        private int Kalibr_com4 { get; set; }
        private int Kalibr_com5 { get; set; }

        private bool flagFirstPusk = false;//для предотвращения вызова останова незапущенной асинхронной операции

        private int DirectonAngel;
        private int Height_position;

        private const int Dopusk = 2000;//для обнаружения перехода через 0(при резком изменении показаний на величину более чем Dopusk)
        private const int SensorResolution = 4096;//разрешение датчика
        private const double ReductionCoeff = 30.9;//коэфф. редукции
        private const int Circle = 360;//360 градусов       

        public Direction(COM_Sensor port4, COM_Sensor port5)
        {
            COM4 = port4;
            COM5 = port5;
        }
        public void Start()
        {
            if (flagFirstPusk)//если происходит повторный пуск
            {
                Stop();
            }

            Run_readSensorsAsyncD();
            Run_readSensorsAsyncH();
            flagFirstPusk = true;
        }

        public void Stop()
        {
            tokenD.Cancel();
            tokenH.Cancel();
            tokenD = new CancellationTokenSource();//создаём новый экземпляр токена для новой задачи
            tokenH = new CancellationTokenSource();
            flagFirstPusk = false;
        }

        private async void Run_readSensorsAsyncD()
        {
            await Task.Factory.StartNew(() => { Run_readDirectonSensor(tokenD.Token); });//запускаем выполнение асинхронной задачи  
        }

        private async void Run_readSensorsAsyncH()
        {
            await Task.Factory.StartNew(() => { Run_readHeightSensor(tokenH.Token); });
        }

        private void Run_readDirectonSensor(CancellationToken token)
        {
            bool f = true;
            int CurrentPosition;//текущая позиция
            int Offset;//смещение
            int Temp = 0;
            int SensorData;
            int Vector = 0;

            double FullTurnover = SensorResolution * ReductionCoeff;//полный поворот пояса в единицах датчика
            double ReverseCoeff = Circle / FullTurnover;//коэфф. для перевода расчётного значения в градусы

            Kalibr_com5 = COM5.DataPort;

            while (f)
            {
                SensorData = COM5.DataPort;

                if (SensorData < Kalibr_com5) { CurrentPosition = SensorResolution - Kalibr_com5 + SensorData; }
                else { CurrentPosition = SensorData - Kalibr_com5; }

                Offset = CurrentPosition - Temp;

                Temp = CurrentPosition;

                if (Offset > Dopusk) { Vector -= SensorResolution - Offset; }
                else if (Offset < -Dopusk) { Vector += SensorResolution + Offset; }
                else { Vector += Offset; }

                double tempVector;

                if (Vector > FullTurnover)
                {
                    tempVector = Vector - FullTurnover + Offset;
                    Vector = (int)tempVector;
                    DirectonAngel = (int)(tempVector * ReverseCoeff);
                }
                else if (Vector <= -1)
                {
                    tempVector = Vector + FullTurnover;

                    if (Vector < -FullTurnover)
                    {
                        tempVector += FullTurnover;
                        Vector = (int)tempVector;
                    }
                    DirectonAngel = (int)(tempVector * ReverseCoeff);
                }
                else
                {
                    DirectonAngel = (int)(Vector * ReverseCoeff);
                }

                Thread.Sleep(1);

                if (token.IsCancellationRequested)
                {
                    f = false;
                }
            }
        }
        public int Get_direction()
        {
            lock (locker)
            {
                return DirectonAngel;
            }
        }

        private void Run_readHeightSensor(CancellationToken token)
        {
            bool f = true;
            int CurrentPosition;//текущая позиция
            int Offset;//смещение
            int Temp = 0;
            int SensorData;
            int Oboroty = 0;

            Kalibr_com4 = COM4.DataPort;

            while (f)
            {
                SensorData = COM4.DataPort;

                if (SensorData < Kalibr_com4) { CurrentPosition = SensorResolution - Kalibr_com4 + SensorData; }
                else { CurrentPosition = SensorData - Kalibr_com4; }

                Offset = CurrentPosition - Temp;

                if (Offset >= Dopusk) { Oboroty--; }
                if (Offset <= -Dopusk) { Oboroty++; }

                Temp = CurrentPosition;

                Height_position = (int)((SensorResolution * Oboroty + CurrentPosition) * 0.0390625);

                Thread.Sleep(1);

                if (token.IsCancellationRequested)
                {
                    f = false;
                }
            }
        }
        public int Get_Height_position()
        {
            lock (locker)
            {
                return Height_position;
            }
        }
    }
}
