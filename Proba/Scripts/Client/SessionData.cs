using Proba.Scripts.SharedClasses;

namespace Proba.Scripts.Client
{
    internal class SessionData
    {
        public int Id { get; set; }
        public StartSessionViewModel StartSession { get; set; }
        public EndSessionViewModel EndSession { get; set; }
        public BatchEventViewModel Events { get; set; }
        public bool Ended { get; set; }
    }
}
