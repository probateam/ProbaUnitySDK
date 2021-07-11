namespace Proba
{

    [System.Serializable]
    public class EventDataScheme
    {
        public int ID;
        public string CLASS;
        public string BODY;
        public string DATE;


        internal EventDataScheme() { }

        internal EventDataScheme(int ID, string CLASS, string BODY, string DATE)
        {
            this.ID = ID;
            this.CLASS = CLASS;
            this.BODY = BODY;
            this.DATE = DATE;
        }
    }
}