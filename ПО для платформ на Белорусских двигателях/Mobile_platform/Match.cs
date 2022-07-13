//Перед первым запуском должна быть проведена калибровка.
//После считывания файла калибровки, программа верно пересчитывает показания датчиков
//и верно определяет своё текущее положение.
//Двигатели работают в диапазоне 180 градусов, никогда не выполняя полный оборот.
//макс. скорость двигателя 1380 об/мин.; коэфф. редукции 63;
//скорость на выходном валу редуктора 0,365 об/с; 0,365*50(длина рычага)=18,25мм/с
//314,16 длина окружности, 17,214с. - полный оборот при частоте 50Гц

using System;

namespace Mobile_platform
{
    public class Match
    {
        //переменные для расчёта
        private double Kren { get; set; } = 0;
        private double Tangag { get; set; } = 0;
        private double Height { get; set; } = 91;
        private double Y1, Y2, Y3, Dy, SinF, yK, SinQ, Q, P;

        //функции задания крена, тангажа и высоты
        public void Set_Kren(double kren)
        {
            Kren = kren;
        }

        public void Set_Tangag(double tangag)
        {
            Tangag = tangag;
        }

        public void Set_Vysota(double height)
        {
            Height = height;
        }

        /// <summary>
        /// Расчитывает углы рычагов, соответствующие заданному положению, в градусах.
        /// </summary>
        /// <param name="Ugol_T1"></param>
        /// <param name="Ugol_T2"></param>
        /// <param name="Ugol_T3"></param>
        public void Calc_ugol_rychaga(out double Ugol_T1, out double Ugol_T2, out double Ugol_T3)
        {
            double P = Height / (Math.Cos(Math.Abs(Tangag) * Math.PI / 180) * Math.Cos(Math.Abs(Kren) * Math.PI / 180));

            double Y_T1 = P - PhysicalChar.T1_x * Math.Cos((90 + Kren) * Math.PI / 180) - PhysicalChar.T1_z * Math.Cos((90 + Tangag) * Math.PI / 180);
            double Y_T2 = P - PhysicalChar.T2_x * Math.Cos((90 + Kren) * Math.PI / 180) - PhysicalChar.T2_z * Math.Cos((90 + Tangag) * Math.PI / 180);
            double Y_T3 = P - PhysicalChar.T3_x * Math.Cos((90 + Kren) * Math.PI / 180) - PhysicalChar.T3_z * Math.Cos((90 + Tangag) * Math.PI / 180);

            double Sin_T1 = Math.Round((Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(Y_T1, 2) - Math.Pow(PhysicalChar.L_Schatuna, 2)) / (2 * Y_T1 * PhysicalChar.L_Rychaga), 1);
            double Sin_T2 = Math.Round((Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(Y_T2, 2) - Math.Pow(PhysicalChar.L_Schatuna, 2)) / (2 * Y_T2 * PhysicalChar.L_Rychaga), 1);
            double Sin_T3 = Math.Round((Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(Y_T3, 2) - Math.Pow(PhysicalChar.L_Schatuna, 2)) / (2 * Y_T3 * PhysicalChar.L_Rychaga), 1);

            double Ugol_T1_ = Math.Round(Math.Asin(Math.Round(Sin_T1, 1)) * 180 / Math.PI, 2);
            double Ugol_T2_ = Math.Round(Math.Asin(Math.Round(Sin_T2, 1)) * 180 / Math.PI, 2);
            double Ugol_T3_ = Math.Round(Math.Asin(Math.Round(Sin_T3, 1)) * 180 / Math.PI, 2);

            //дополнительный пересчёт с учётом новой архитектуры приводов
            Ugol_T1 = -0.000028 * Math.Pow(Ugol_T1_, 3) + 0.000828 * Math.Pow(Ugol_T1_, 2) + 0.730826 * Ugol_T1_ - 5.775405;
            Ugol_T2 = -0.000028 * Math.Pow(Ugol_T2_, 3) + 0.000828 * Math.Pow(Ugol_T2_, 2) + 0.730826 * Ugol_T2_ - 5.775405;
            Ugol_T3 = -0.000028 * Math.Pow(Ugol_T3_, 3) + 0.000828 * Math.Pow(Ugol_T3_, 2) + 0.730826 * Ugol_T3_ - 5.775405;
        }

        public void Calc_parameters(double ugol_1, double ugol_2, double ugol_3, out double Kren, out double Tangag, out double Vysota)//ТРЕБУЕТ КОРРЕКТИРОВКИ ПОД НОВЫЕ ДВИГАТЕЛИ
        {
            Y1 = Math.Sqrt(Math.Pow(PhysicalChar.L_Schatuna, 2) - Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(PhysicalChar.L_Rychaga, 2) * Math.Pow(Math.Sin(ugol_1 * Math.PI / 180), 2)) + PhysicalChar.L_Rychaga * Math.Sin(ugol_1 * Math.PI / 180);
            Y2 = Math.Sqrt(Math.Pow(PhysicalChar.L_Schatuna, 2) - Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(PhysicalChar.L_Rychaga, 2) * Math.Pow(Math.Sin(ugol_2 * Math.PI / 180), 2)) + PhysicalChar.L_Rychaga * Math.Sin(ugol_2 * Math.PI / 180);
            Y3 = Math.Sqrt(Math.Pow(PhysicalChar.L_Schatuna, 2) - Math.Pow(PhysicalChar.L_Rychaga, 2) + Math.Pow(PhysicalChar.L_Rychaga, 2) * Math.Pow(Math.Sin(ugol_3 * Math.PI / 180), 2)) + PhysicalChar.L_Rychaga * Math.Sin(ugol_3 * Math.PI / 180);

            Dy = Y2 - Y3;
            SinF = Dy / (PhysicalChar.T2_x + Math.Abs(PhysicalChar.T3_x));
            yK = Y2 - Dy / 2;
            SinQ = Math.Abs(yK - Y1) / (PhysicalChar.T1_z + Math.Abs(PhysicalChar.T2_z));
            Q = Math.Asin(SinQ) * 180 / Math.PI;

            Kren = Math.Asin(SinF) * 180 / Math.PI;

            Tangag = (ugol_1 < 0) ? -Q : Q;

            P = Y1 + PhysicalChar.T1_z * Math.Cos((90 + Tangag) * Math.PI / 180);

            Vysota = Math.Cos(Kren * Math.PI / 180) * Math.Cos(Tangag * Math.PI / 180) * P;
        }

        /// <summary>
        /// Пересчитывает скорость в ШИМ 0-255.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public double Calc_speed(double speed)
        {
            speed = (speed < 0) ? -speed : speed;
            speed = (speed < PhysicalChar.Min_Speed) ? PhysicalChar.Min_Speed : speed;
            speed = (speed > PhysicalChar.Max_Speed) ? PhysicalChar.Max_Speed : speed;

            double Speed = Math.Round(speed * PhysicalChar.Koeff_speed);//пересчитываем в знвченеи от 0 до 255

            return Speed;
        }
    }
}
