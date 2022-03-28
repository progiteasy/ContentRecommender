using System.Collections.Generic;

namespace ContentRecommender.ViewModels.Reviews
{
    public class ReviewCreationViewModel
    {
        public IList<string> AllCategoryNames { get; set; }
        public IList<string> AllTags { get; set; }
        public ReviewViewModel ReviewModel { get; set; }
    }
}
