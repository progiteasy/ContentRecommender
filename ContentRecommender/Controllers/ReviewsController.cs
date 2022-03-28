using ContentRecommender.Data.Contexts;
using ContentRecommender.Data.Models;
using ContentRecommender.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContentRecommender.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public ReviewsController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("/users/{userName}/reviews"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetReviewsViewAsync([FromRoute] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            
            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var userReviews = await _dbContext.Reviews.
                Include(review => review.Author).
                Where(review => review.Author == user).
                Include(review => review.Category).
                Include(review => review.Ratings).
                Include(review => review.Likes).
                ToListAsync();
            var userReviewModels = userReviews.Select(review => new ReviewViewModel()
            {
                Id = review.Id,
                CreationDate = review.CreationDate,
                CategoryName = review.Category.Name,
                Title = review.Title,
                LikeCount = review.Likes.Count(),
                AverageUserRating = review.Ratings.
                    Where(rating => rating.User != user).
                    Select(rating => (int)rating.Value).
                    DefaultIfEmpty().
                    Average()
            }).ToList();

            return View("ReviewsView", userReviewModels);
        }

        [HttpGet("/users/{userName}/reviews/create"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetReviewCreationViewAsync([FromRoute] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var reviewCreationModel = new ReviewCreationViewModel()
            {
                AllTags = await _dbContext.Tags.Select(tag => tag.Name).ToListAsync(),
                AllCategoryNames = await _dbContext.Categories.Select(category => category.Name).ToListAsync()
            };

            return View("ReviewCreationView", reviewCreationModel);
        }

        [HttpPost("/users/{userName}/reviews/create"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreateReviewAsync([FromRoute] string userName, ReviewCreationViewModel reviewCreationModel)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var review = new Review()
            {
                Title = reviewCreationModel.ReviewModel.Title,
                Text = reviewCreationModel.ReviewModel.Text,
                CreationDate = DateTime.Now
            };
            var reviewAuthor = user;
            var reviewCategory = await _dbContext.Categories.
                SingleAsync(category => category.Name == reviewCreationModel.ReviewModel.CategoryName);
            var reviewTags = reviewCreationModel.ReviewModel.Tags.
                Select(tagName => _dbContext.Tags.SingleOrDefault(tag => tag.Name == tagName) ?? new Tag() { Name = tagName });

            review.Author = reviewAuthor;
            review.Category = reviewCategory;
            review.Tags = reviewTags.ToList();
            review.Ratings.Add(new Rating()
            {
                Review = review,
                User = user,
                Value = reviewCreationModel.ReviewModel.AuthorRating
            });

            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            return Redirect($"~/users/{user.UserName}/reviews");
        }

        [HttpGet("/users/{userName}/reviews/{reviewId}/edit"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetReviewEditViewAsync([FromRoute] string userName, [FromRoute] long reviewId)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var review = await _dbContext.Reviews.
                Include(review => review.Category).
                Include(review => review.Tags).
                Include(review => review.Ratings).
                SingleOrDefaultAsync(review => review.Id == reviewId);

            if (review == null)
                return NotFound();
            if (review.Author != user)
                return NotFound();

            var reviewModel = new ReviewViewModel()
            {
                Title = review.Title,
                Text = review.Text,
                CategoryName = review.Category.Name,
                AuthorRating = review.Ratings.Single(rating => rating.User == user).Value
            };

            var reviewEditModel = new ReviewEditViewModel()
            {
                AllTags = review.Tags.Select(tag => tag.Name).ToList(),
                AllCategoryNames = await _dbContext.Categories.Select(category => category.Name).ToListAsync(),
                ReviewModel = reviewModel
            };

            return View("ReviewEditView", reviewEditModel);
        }

        [HttpPost("/users/{userName}/reviews/{reviewId}/edit"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> EditReviewAsync([FromRoute] string userName, [FromRoute] long reviewId, ReviewEditViewModel reviewEditModel)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var review = await _dbContext.Reviews.
                Include(review => review.Category).
                Include(review => review.Tags).
                Include(review => review.Ratings).
                ThenInclude(rating => rating.User).
                SingleOrDefaultAsync(review => review.Id == reviewId);

            if (review == null)
                return NotFound();
            if (review.Author != user)
                return NotFound();

            review.Title = reviewEditModel.ReviewModel.Title;
            review.Text = reviewEditModel.ReviewModel.Text;
            review.Category = await _dbContext.Categories.
                SingleAsync(category => category.Name == reviewEditModel.ReviewModel.CategoryName);

            var reviewTags = reviewEditModel.ReviewModel.Tags.
                Select(tagName => _dbContext.Tags.SingleOrDefault(tag => tag.Name == tagName) ?? new Tag() { Name = tagName });
            review.Tags = reviewTags.ToList();

            var reviewRating = review.Ratings.Single(rating => rating.User == user);

            reviewRating.Value = reviewEditModel.ReviewModel.AuthorRating;

            await _dbContext.SaveChangesAsync();

            return Redirect($"~/users/{user.UserName}/reviews");
        }

        [HttpPost("/users/{userName}/reviews/{reviewId}/delete"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteReviewAsync([FromRoute] string userName, [FromRoute] long reviewId)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var review = await _dbContext.Reviews.
                Include(review => review.Category).
                Include(review => review.Tags).
                Include(review => review.Ratings).
                ThenInclude(rating => rating.User).
                SingleOrDefaultAsync(review => review.Id == reviewId);

            if (review == null)
                return NotFound();
            if (review.Author != user)
                return NotFound();

            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();

            return Redirect("~/users");
        }

        [HttpGet("/users/{userName}/reviews/{reviewId}"), Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetReviewViewAsync([FromRoute] string userName, [FromRoute] long reviewId)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound();
            if (user.UserName.ToUpper() != User.Identity.Name.ToUpper() && !User.IsInRole("Admin"))
                return Forbid();

            var review = await _dbContext.Reviews.
                Include(review => review.Category).
                Include(review => review.Tags).
                Include(review => review.Ratings).
                ThenInclude(rating => rating.User).
                SingleOrDefaultAsync(review => review.Id == reviewId);

            if (review == null)
                return NotFound();
            if (review.Author != user)
                return NotFound();

            var userReviewModel = new ReviewViewModel()
            {
                Text = review.Text,
                Title = review.Title
            };

            return View("ReviewView", userReviewModel);
        }
    }
}
