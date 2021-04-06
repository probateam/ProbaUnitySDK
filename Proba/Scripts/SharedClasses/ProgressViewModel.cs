namespace Proba.Scripts.SharedClasses
{
    internal class ProgressViewModel : BaseEventDataViewModel
    {
        public string Progress;
        public string Configurations;

        internal ProgressViewModel(string progress, string configurations)
        {
            Progress = progress;
            Configurations = configurations;
        }

        internal ProgressViewModel()
        {

        }
    }
}