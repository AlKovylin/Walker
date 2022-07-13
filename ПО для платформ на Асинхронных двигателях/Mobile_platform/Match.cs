//Перед первым запуском должна быть проведена калибровка.
//После считывания файла калибровки, программа верно пересчитывает показания датчиков
//и верно определяет своё текущее положение.
//Двигатели работают в диапазоне 180 градусов, никогда не выполняя полный оборот.
//макс. скорость двигателя 1380 об/мин.; коэфф. редукции 63;
//скорость на выходном валу редуктора 0,365 об/с; 0,365*50(длина рычага)=18,25мм/с
//314,16 длина окружности, 17,214с. - полный оборот при частоте 50Гц

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_platform
{
    public class Math_hodunki
    {
        //геометрия платформы
        private const int L_Rychaga = 50;//длина рычага, мм
        private const int L_Schatuna = 141;//длина шатуна, мм

        //координаты точек, mm
        private const double T1_x = 0;
        private const double T1_z = 352.00;
        private const double T2_x = 304.84;
        private const double T2_z = -176.00;
        private const double T3_x = -304.84;
        private const double T3_z = -176.00;

        //пределы положений, мм
        private const double Max_kren = 9.31;
        private const double Min_kren = -9.31;

        private const double Max_tangag = 10.72;
        private const double Min_tangag = -10.72;

        private const int Max_vysota = 191;
        private const int Min_vysota = 91;

        //дополнительные значения
        private const int Seredina_po_vysote = 141;
        private const double Ugol_serediny = 11.54;

        //переменные для расчёта
        private double Kren = 0;
        private double Tangag = 0;
        private double Vysota = 91;
        private double Y1, Y2, Y3, Dy, SinF, yK, SinQ, Q, P;

        //моторредукторы
        private const double Max_Speed = 91.27;//мм*с = 50Гц
        private const double Min_Speed = 9.127;//мм*с = 5Гц
        private const double Koeff_speed = 0.5478;//коэфф. для перевода мм/с в Гц(91,27мм*с мах скорость точки на валу); 1,8254 - для обратного перевода
        //private int Speed_dv = 0;//скорость об/мин.
        //private int Koef_redukcii = 0;//коэффициент редукции

        //функции задания крена, тангажа и высоты
        public void Set_Kren(double value)
        {
            Kren = value;
        }

        public void Set_Tangag(double value)
        {
            Tangag = value;
        }

        public void Set_Vysota(double value)
        {
            Vysota = value;
        }

        public void Calc_ugol_rychaga(out double Ugol_T1, out double Ugol_T2, out double Ugol_T3)
        {
            double P = Vysota / (Math.Cos(Math.Abs(Tangag) * Math.PI / 180) * Math.Cos(Math.Abs(Kren) * Math.PI / 180));

            double Y_T1 = P - T1_x * Math.Cos((90 + Kren) * Math.PI / 180) - T1_z * Math.Cos((90 + Tangag) * Math.PI / 180);
            double Y_T2 = P - T2_x * Math.Cos((90 + Kren) * Math.PI / 180) - T2_z * Math.Cos((90 + Tangag) * Math.PI / 180);
            double Y_T3 = P - T3_x * Math.Cos((90 + Kren) * Math.PI / 180) - T3_z * Math.Cos((90 + Tangag) * Math.PI / 180);

            double Sin_T1 = Math.Round((Math.Pow(L_Rychaga, 2) + Math.Pow(Y_T1, 2) - Math.Pow(L_Schatuna, 2)) / (2 * Y_T1 * L_Rychaga), 1);
            double Sin_T2 = Math.Round((Math.Pow(L_Rychaga, 2) + Math.Pow(Y_T2, 2) - Math.Pow(L_Schatuna, 2)) / (2 * Y_T2 * L_Rychaga), 1);
            double Sin_T3 = Math.Round((Math.Pow(L_Rychaga, 2) + Math.Pow(Y_T3, 2) - Math.Pow(L_Schatuna, 2)) / (2 * Y_T3 * L_Rychaga), 1);

            Ugol_T1 = Math.Round(Math.Asin(Math.Round(Sin_T1, 1)) * 180 / Math.PI, 2);
            Ugol_T2 = Math.Round(Math.Asin(Math.Round(Sin_T2, 1)) * 180 / Math.PI, 2);
            Ugol_T3 = Math.Round(Math.Asin(Math.Round(Sin_T3, 1)) * 180 / Math.PI, 2);
        }
        public void Calc_parameters(double ugol_1, double ugol_2, double ugol_3, out double Kren, out double Tangag, out double Vysota)
        {
            Y1 = Math.Sqrt(Math.Pow(L_Schatuna, 2) - Math.Pow(L_Rychaga, 2) + (Math.Pow(L_Rychaga, 2) * Math.Pow((Math.Sin((ugol_1 * Math.PI) / 180)), 2))) + L_Rychaga * Math.Sin((ugol_1 * Math.PI) / 180);
            Y2 = Math.Sqrt(Math.Pow(L_Schatuna, 2) - Math.Pow(L_Rychaga, 2) + (Math.Pow(L_Rychaga, 2) * Math.Pow((Math.Sin((ugol_2 * Math.PI) / 180)), 2))) + L_Rychaga * Math.Sin((ugol_2 * Math.PI) / 180);
            Y3 = Math.Sqrt(Math.Pow(L_Schatuna, 2) - Math.Pow(L_Rychaga, 2) + (Math.Pow(L_Rychaga, 2) * Math.Pow((Math.Sin((ugol_3 * Math.PI) / 180)), 2))) + L_Rychaga * Math.Sin((ugol_3 * Math.PI) / 180);

            Dy = Y2 - Y3;
            SinF = Dy / (T2_x + Math.Abs(T3_x));
            yK = Y2 - (Dy / 2);
            SinQ = Math.Abs(yK - Y1) / (T1_z + Math.Abs(T2_z));
            Q = (Math.Asin(SinQ) * 180) / Math.PI;

            Kren = (Math.Asin(SinF) * 180) / Math.PI;

            if (ugol_1 < 0) { Tangag = -Q; }
            else { Tangag = Q; }

            P = Y1 + (T1_z * Math.Cos(((90 + Tangag) * Math.PI) / 180));

            Vysota = Math.Cos((Kren * Math.PI) / 180) * Math.Cos((Tangag * Math.PI) / 180) * P;
        }
        public double Calc_speed(double speed)
        {
            if (speed > 0 && speed < Min_Speed)
            {
                speed = Min_Speed;
            }
            if (speed > Max_Speed)
            {
                speed = Max_Speed;
            }
            return speed * Koeff_speed * 0.2;//пересчитываем в герцы
        }
    }
}
