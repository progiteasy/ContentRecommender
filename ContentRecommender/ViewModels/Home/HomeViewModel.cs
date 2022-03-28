using ContentRecommender.ViewModels.Reviews;
using System.Collections.Generic;

namespace ContentRecommender.ViewModels.Home
{
    public class HomeViewModel
    {
        public IList<ReviewViewModel> RecentReviews { get; set; }
        public IList<ReviewViewModel> TopRatedReviews { get; set; }
        public IList<string> TagCloud { get; set; }
    }
}
