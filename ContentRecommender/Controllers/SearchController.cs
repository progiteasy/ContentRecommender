using ContentRecommender.Data.Contexts;
using ContentRecommender.ViewModels.Reviews;
using ContentRecommender.ViewModels.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContentRecommender.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _dbContext;

        public SearchController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("~/search"), AllowAnonymous]
        public IActionResult GetSearchView()
        {
            return View("SearchView", new SearchViewModel());
        }

        [HttpPost("~/search"), AllowAnonymous]
        public async Task<IActionResult> SearchAsync([FromForm] SearchViewModel searchModel)
        {
            if (String.IsNullOrEmpty(searchModel.Query))
                return View("SearchView", searchModel);

            var foundReviews = await _dbContext.Reviews.
                Include(review => review.Comments).
                Include(review => review.Category).
                Include(review => review.Tags).
                Include(review => review.Ratings).
                Include(review => review.Author).
                Where(review => EF.Functions.FreeText(review.Title, searchModel.Query) ||
                    EF.Functions.FreeText(review.Text, searchModel.Query) ||
                    EF.Functions.FreeText(review.Category.Name, searchModel.Query) ||
                    review.Tags.Any(tag => EF.Functions.FreeText(tag.Name, searchModel.Query) ||
                    review.Comments.Any(comment => EF.Functions.FreeText(comment.Text, searchModel.Query)))).
                        ToListAsync();

            searchModel.ReviewModels = foundReviews.Select(review => new ReviewViewModel()
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
            }).ToList();

            return View("SearchView", searchModel);
        }
    }
}
