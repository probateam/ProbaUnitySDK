using System;

namespace Proba
{

    [Serializable]
    public class LogDataScheme
    {
        public string DATE;
        public string BODY;

        internal LogDataScheme() { }

        internal LogDataScheme(string DATE, string BODY)
        {
            this.DATE = DATE;
            this.BODY = BODY;
        }
    }
}