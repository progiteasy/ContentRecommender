using ContentRecommender.Data.Contexts;
using ContentRecommender.ViewModels.Home;
using ContentRecommender.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContentRecommender.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("~/"), AllowAnonymous]
        public async Task<IActionResult> GetHomeViewAsync()
        {


            var reviewCount = 5;
            var tagCount = 5;
            var recentReviews = await _dbContext.Reviews.
                Include(review => review.Author).
                Include(review => review.Category).
                Include(review => review.Ratings).
                Include(review => review.Likes).
                OrderByDescending(review => review.CreationDate).
                Take(reviewCount).
                ToListAsync();
            var topRatedReviews = await _dbContext.Reviews.
                Include(review => review.Author).
                Include(review => review.Category).
                Include(review => review.Ratings).
                Include(review => review.Likes).
                OrderByDescending(review => review.Ratings.Average(rating => rating.Value)).
                Take(reviewCount).
                ToListAsync();

            var homeModel = new HomeViewModel()
            {
                RecentReviews = recentReviews.Select(review => new ReviewViewModel()
                {
                    Id = review.Id,
                    CreationDate = review.CreationDate,
                    CategoryName = review.Category.Name,
                    Title = review.Title,
                    LikeCount = review.Likes.Count(),
                    AverageUserRating = review.Ratings.
                    Where(rating => rating.User != review.Author).
                    Select(rating => (int)rating.Value).
                    DefaultIfEmpty().
                    Average()
                }).ToList(),
                TopRatedReviews = recentReviews.Select(review => new ReviewViewModel()
                {
                    Id = review.Id,
                    CreationDate = review.CreationDate,
                    CategoryName = review.Category.Name,
                    Title = review.Title,
                    LikeCount = review.Likes.Count(),
                    AverageUserRating = review.Ratings.
                    Where(rating => rating.User != review.Author).
                    Select(rating => (int)rating.Value).
                    DefaultIfEmpty().
                    Average()
                }).ToList(),
                TagCloud = _dbContext.Tags.
                Select(tag => tag.Name).
                Take(tagCount).
                ToList()
            };

            return View("HomeView", homeModel);
        }
    }
}
