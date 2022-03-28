using ContentRecommender.ViewModels.Reviews;
using System.Collections.Generic;

namespace ContentRecommender.ViewModels.Search
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public IList<ReviewViewModel> ReviewModels { get; set; } = new List<ReviewViewModel>();
    }
}
