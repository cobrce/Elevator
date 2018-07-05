namespace DummyPlcPlugin
{
    internal class DiscreteTimer
    {
        private uint _delay;
        private uint _firstTimerValue;
        private DummyPLC _plc;

        public DiscreteTimer(DummyPLC plc, uint msDelay)
        {
            _delay = msDelay;
            _plc = plc;
        }
        public void Init()
        {
            _firstTimerValue = _plc.Timer;
        }
        public bool IsTimeUp()
        {
            return (_plc.Timer - _firstTimerValue >= _delay);
        }
    }
}