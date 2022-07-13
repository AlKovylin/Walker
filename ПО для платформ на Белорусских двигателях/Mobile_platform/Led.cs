namespace Mobile_platform
{
    public class Led
    {
        private COM_Arduino Port;

        public Led(COM_Arduino port_)
        {
            Port = port_;
        }
        //ON
        public void RedON()
        {
            Port.WritePort("#,R");
        }
        public void GreenON()
        {
            Port.WritePort("#,G");
        }
        public void BlueON()
        {
            Port.WritePort("#,B");
        }
        //OFF
        public void OFF_LED()
        {
            Port.WritePort("#,D");
        }
    }
}
