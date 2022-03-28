using System.Collections.Generic;

namespace ContentRecommender.ViewModels.Reviews
{
    public class ReviewEditViewModel
    {
        public ICollection<ReviewViewModel> ReviewModels { get; set; }
        public ReviewViewModel ReviewModel { get; set; }
        public IList<string> AllCategoryNames { get; set; }
        public IList<string> AllTags { get; set; }
    }
}
