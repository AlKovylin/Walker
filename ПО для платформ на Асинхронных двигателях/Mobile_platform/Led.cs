using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mobile_platform
{
    public class Led
    {
        private AO_DO_DI PioDa { get; set; }

        public Led(AO_DO_DI PioDa)
        {
            // TODO: Complete member initialization
            this.PioDa = PioDa;
        }
        //ON
        public void RedON()
        {
            PioDa.WriteDO(6, 1);
        }
        public void GreenON()
        {
            PioDa.WriteDO(7, 1);
        }
        public void BlueON()
        {
            PioDa.WriteDOBlue(0, 1);
        }
        //OFF
        public void RedOFF()
        {
            PioDa.WriteDO(6, 0);
        }
        public void GreenOFF()
        {
            PioDa.WriteDO(7, 0);
        }
        public void BlueOFF()
        {
            PioDa.WriteDOBlue(0, 0);
        }
        public void OFF_LED()
        {
            RedOFF();
            GreenOFF();
            BlueOFF();
        }
    }
}
