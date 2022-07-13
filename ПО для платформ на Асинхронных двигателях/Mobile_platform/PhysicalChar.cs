namespace Mobile_platform
{
    public static class PhysicalChar
    {
        //геометрия платформы
        public const int L_Rychaga = 50;//длина рычага, мм
        public const int L_Schatuna = 141;//длина шатуна, мм

        //координаты точек, mm
        public const double T1_x = 0;
        public const double T1_z = 352.00;
        public const double T2_x = 304.84;
        public const double T2_z = -176.00;
        public const double T3_x = -304.84;
        public const double T3_z = -176.00;

        //пределы положений, мм
        public const double Max_kren = 9.31;
        public const double Min_kren = -9.31;

        public const double Max_tangag = 10.72;
        public const double Min_tangag = -10.72;

        //в соответствии с расчётами конструкторов должно работать в диапазоне от 91мм до 191мм,
        //установленные значения крайних положений подобраны опытным путём в соответствии с фактическим расположеним концевиков
        //данные значения обеспечивают касание кивка лапок концевиков, но не срабатывание, т.е. есть зазор на проскок
        //дополнительная корректировка положения рычага см. class Moving => private void Tec_polog_rychagov() => //250 - дополнительный коэефф. для выравнивания рычага в горизонте
        public const int Max_height = 185;//191;
        public const int Min_height = 110;//91;

        //дополнительные значения
        public const int Average_height = 141;
        public const double Ugol_serediny = 11.54;

        //моторредукторы
        public const double Max_Speed = 91.27;//мм*с = 255
        public const double Min_Speed = 9.13;//мм*с = 140
        public const double Average_height_speed = 60;//скарость перемещения в среднее положение
        public const double RunOutSpeed = 65;//скорость выбега (применяется на завершающем отрезке)
        public const double RunOutDistance = 500;//расстояние выбега в единицах датчика
        public const double Koeff_speed = 2.794;//коэфф. для перевода мм/с в 8-ми биное представление 0-255(91,27мм*с мах скорость точки на валу)
        public const double Koeff_pered = 1.68421;//1.363636//соотношение шестерней на редукторе и на датчике

        //минимальное и максимальное положение в единицах датчика
        public const int Min_ = 698;
        public const int Max_ = 2094;
    }
}
