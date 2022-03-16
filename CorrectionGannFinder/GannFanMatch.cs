namespace CorrectionGannFinder
{
    internal class GannFanMatch
    {
        public GannFanMatch(GannFan[] fan)
        {
            Fans = fan;
        }

        public float Score { get; set; }
        public GannFan[] Fans { get; }
    }
}