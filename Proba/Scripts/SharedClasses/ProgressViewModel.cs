namespace Proba.Scripts.SharedClasses
{
    internal class ProgressViewModel : BaseEventDataViewModel
    {
        public string progress;
        public string configurations;

        internal ProgressViewModel(string progress, string configurations)
        {
            this.progress = progress;
            this.configurations = configurations;
        }

        internal ProgressViewModel()
        {

        }
    }
}