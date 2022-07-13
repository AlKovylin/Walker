using System;
using System.Timers;

namespace Mobile_platform
{
    public class AO_DO_DI
    {
        ushort wInitialCode, wTotalBoard;
        ushort wBoardNo = 0;
        ushort wOutputPort = 0;
        ushort wInputPort = 0;
        ushort wCfgCode = UniDAQ.IXUD_AO_BI_10V;

        UniDAQ.IXUD_CARD_INFO[] sCardInfo = new UniDAQ.IXUD_CARD_INFO[UniDAQ.MAX_BOARD_NUMBER];
        UniDAQ.IXUD_DEVICE_INFO[] sDeviceInfo = new UniDAQ.IXUD_DEVICE_INFO[UniDAQ.MAX_BOARD_NUMBER];

        Timer ReadDITimer = new Timer();
        private object locker = new object();
        //private int alarmButton = 0;

        public delegate void AlarmButton();
        public event AlarmButton AlarmButtonPress;

        public delegate void AlarmButtonNO();
        public event AlarmButtonNO AlarmButtonNOPress;

        private bool flagAlarm = false;
        public ushort Create()
        {
            //Driver Initial
            wInitialCode = UniDAQ.Ixud_DriverInit(ref wTotalBoard);
            if (wInitialCode != UniDAQ.Ixud_NoErr)
            {
                return wInitialCode;
            }
            else 
            {
                for (ushort i = 0; i < 3; i++)
                {
                    ConfigAO(i);
                }
                //timer опрос аварийной кнопки
                ReadDITimer.Interval = 200;
                ReadDITimer.AutoReset = true;
                ReadDITimer.Enabled = true;
                ReadDITimer.Elapsed += OnTimedEvent;
                ReadDITimer.Start();

                return wInitialCode;
            }          
        }

        public ushort Close()//Driver Close
        {           
            wInitialCode = UniDAQ.Ixud_DriverClose();
            return wInitialCode;
        }

        private void ConfigAO(ushort wChannel)//Config Analog Output Range
        {           
            wInitialCode = UniDAQ.Ixud_ConfigAO(wBoardNo, wChannel, wCfgCode);
        }

        public void WriteAOVoltage(ushort wChannel, float fValue)//Write the Analog Output Value for Voltage 0-10V
        {
            wInitialCode = UniDAQ.Ixud_WriteAOVoltage(wBoardNo, wChannel, fValue);
        }
        public void WriteDO(ushort wChannel, ushort Value)//Write the Digital Output
        {          
            wInitialCode = UniDAQ.Ixud_WriteDOBit(wBoardNo, wOutputPort, wChannel, Value);
        }
        public void WriteDOBlue(ushort wChannel, ushort Value)//Write the Digital Output
        {
            wInitialCode = UniDAQ.Ixud_WriteDOBit(wBoardNo, 1, wChannel, Value);
        }
        private ushort ReadDI(ushort wChannel)//Read DI Bit x Value
        {
            ushort DIVal = 0;
            wInitialCode = UniDAQ.Ixud_ReadDIBit(wBoardNo, wInputPort, wChannel, ref DIVal);
            return DIVal;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)//Timer tick
        {
            //отправляет сигнал только один раз
            if (ReadDI(6) == 0 && !flagAlarm) { AlarmButtonPress(); flagAlarm = true; }
            if (ReadDI(6) == 1 && flagAlarm) { AlarmButtonNOPress(); flagAlarm = false; }
        }

        public int[] DataFotoElements()
        {
            int[] dataFotoElements = new int[6];

            for (ushort i = 0; i < 6; i++)
            {
                dataFotoElements[i] = ReadDI(i);
            }
            return dataFotoElements;
        }

        /*public int AlarmButton()
        {
            lock (locker)
            {
                return alarmButton;
            }
        }*/
    }
}