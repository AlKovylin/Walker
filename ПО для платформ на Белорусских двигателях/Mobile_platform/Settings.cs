using System;
using System.Xml.Serialization;
using System.IO;

namespace Mobile_platform
{
    [Serializable]
    public class Settings
    {
        public double Kren_max { get; set; }
        public double Kren_min { get; set; }
        public double Tangag_max { get; set; }
        public double Tangag_min { get; set; }
        /// <summary>
        /// Максимальная скорость наклона.
        /// </summary>
        public double MaxSpeed_deviation { get; set; }//максимальная скорость наклона мм*с
        /// <summary>
        /// Максимальная скорость перемещений.
        /// </summary>
        public double MaxSpeed_move { get; set; }//максимальная скорость перемещения по вертикали мм*с
        /// <summary>
        /// Скорость перехода в центарльное положение.
        /// </summary>
        public double Speed_centering { get; set; }//скорость перехода в центральное положение
        /// <summary>
        /// Минимальная скорость перемещений.
        /// </summary>
        public double MinSpeed { get; set; }//мин. скорость - актуально для платформы на дв. пост. тока. т.к. может не хватать тяги
    }

    public class FunkSettings
    {
        private XmlSerializer formatter { get; set; }

        public FunkSettings()
        {
            formatter = new XmlSerializer(typeof(Settings));
        }

        private Settings Read_settings()
        {
            Settings Temp;

            try
            {
                using (FileStream fs = new FileStream("Settings1.xml", FileMode.Open))
                {

                    return Temp = (Settings)formatter.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void Save_settings(Settings Sets)
        {
            try
            {
                using (FileStream fs_ = new FileStream("Settings1.xml", FileMode.Create))
                {
                    formatter.Serialize(fs_, Sets);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Set_kren(double kren_min, double kren_max)
        {
            if (kren_max <= PhysicalChar.Max_kren && kren_min >= PhysicalChar.Min_kren)
            {
                Settings TempSet = Read_settings();
                TempSet.Kren_max = kren_max;
                TempSet.Kren_min = kren_min;
                Save_settings(TempSet);
            }
            else { throw new ApplicationException("Не допустимое значение крена."); }
        }

        public void Set_tangag(double tangag_min, double tangag_max)
        {
            if (tangag_max <= PhysicalChar.Max_tangag && tangag_min >= PhysicalChar.Min_tangag)
            {
                Settings TempSet = Read_settings();
                TempSet.Tangag_max = tangag_max;
                TempSet.Tangag_min = tangag_min;
                Save_settings(TempSet);
            }
            else { throw new ApplicationException("Не допустимое значение крена."); }

        }
        public void Set_maxSpeed_deviation(double maxSpeed_deviation)
        {
            Settings TempSet = Read_settings();

            if (maxSpeed_deviation > TempSet.MinSpeed && maxSpeed_deviation < PhysicalChar.Max_Speed)
            {
                TempSet.MaxSpeed_deviation = maxSpeed_deviation;
                Save_settings(TempSet);
            }
            else 
            { 
                throw new ApplicationException("Скорость наклонов не должна быть меньше минимальной и больше максимальной скоростей."); 
            }
        }

        public void Set_maxSpeed_move(double maxSpeed_move)
        {
            Settings TempSet = Read_settings();

            if (maxSpeed_move <= PhysicalChar.Max_Speed && maxSpeed_move >= PhysicalChar.Min_Speed && maxSpeed_move > TempSet.MinSpeed)
            {

                TempSet.MaxSpeed_move = maxSpeed_move;
                Save_settings(TempSet);
            }
            else
            {
                throw new ApplicationException("Ошибка установки максимальной скорости.");
            }

        }
        public void Set_speed_centering(double speed_centering)
        {
            Settings TempSet = Read_settings();

            if (speed_centering > TempSet.MinSpeed && speed_centering < TempSet.MaxSpeed_move)
            {
                TempSet.Speed_centering = speed_centering;
                Save_settings(TempSet);
            }
            else
            {
                throw new ApplicationException("Скорость центрирования не должна быть меньше минимальной и больше максимальной скоростей.");
            }
        }

        public void Set_min_speed(double min_speed)
        {
            Settings TempSet = Read_settings();

            if (min_speed > 0 && min_speed < TempSet.MaxSpeed_move)
            {
                TempSet.MinSpeed = min_speed;
                Save_settings(TempSet);
            }
            else
            {
                throw new ApplicationException("Минимальная скорость не может быть меньше нуля и больше максимальной скорости");
            }
        }

        public Settings Get_settings()
        {
            Settings TempSet = Read_settings();
            return TempSet;
        }
    }
}