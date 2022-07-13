using System;
using System.IO;

namespace Mobile_platform
{
    public class Logger
    {
        private StreamWriter sw;

        public void WriteLogLine(string arg)
        {
            if (Directory.Exists("L:\\"))//если подключен диск с метко L
            {
                sw = new StreamWriter("D:\\LogMoving.txt", true);
                sw.WriteLine(DateTime.Now + "|| " + arg);
                sw.Close();
            }
            else//если диска нет, то пишем в каталог программы
            {
                sw = new StreamWriter("LogMoving.txt", true);
                sw.WriteLine(DateTime.Now + "|| " + arg);
                sw.Close();
            }
        }

        public void CloseWriteLog()
        {
            sw.Close();
        }
    }
}
