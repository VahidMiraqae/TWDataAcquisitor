namespace CorrectionGannFinder
{
    internal class GannFanMatch
    {
        public GannFanMatch(GannFan fan)
        {
            Fan = fan;
        }

        public float Score { get; set; }
        public GannFan Fan { get; }
    }
}