using System.Collections.Generic;

namespace Proba.Scripts.SharedClasses
{
    internal class BatchEventViewModel
    {
        internal List<AchievementEventViewModel> Achievements = new List<AchievementEventViewModel>();
        internal List<AdvertisementEventViewModel> Advertisements = new List<AdvertisementEventViewModel>();
        internal List<BusinessEventViewModel> Businesses = new List<BusinessEventViewModel>();
        internal List<ContentViewEventViewModel> ContentViews = new List<ContentViewEventViewModel>();
        internal List<DesignEventViewModel> DesignEvent = new List<DesignEventViewModel>();
        internal List<ProgressionEventViewModel> Progressions = new List<ProgressionEventViewModel>();
        internal List<SocialEventViewModel> Socials = new List<SocialEventViewModel>();
        internal List<TapEventViewModel> Taps = new List<TapEventViewModel>();
    }
}
