using System;
using System.Collections.Generic;

namespace ContentRecommender.ViewModels.Reviews
{
    public class ReviewViewModel
    {
        public bool IsSelected { get; set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CategoryName { get; set; }
        public string AuthorName { get; set; }
        public double AverageUserRating { get; set; }
        public byte AuthorRating { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreationDate { get; set; }
        public IList<string> Tags { get; set; }
    }
}
