using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Auth.Entities.DataTransferObjects;
using Auth.Entities.Models;
using Auth.Enums;
using Auth.Features;
using DAL.Auth.Models;
using DAL.Auth.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRatingRepository _ratingRepository;

        public ReviewController(IReviewRepository reviewRepository, UserManager<User> userManager,
            ILikeRepository likeRepository, IRatingRepository ratingRepository)
        {
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _likeRepository = likeRepository;
            _ratingRepository = ratingRepository;
        }

        [HttpGet("getreview/{id}")]
        [Authorize]
        public async Task<IActionResult> GetReviewById(string id)
        {
            var enhancedReview = new EnhancedReview();

            try
            {
                var review = await _reviewRepository.GetByReviewId(id);

                if (review == null)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "There is no such review." },
                        Data = ""
                    });
                }

                var reviewRatings = await _ratingRepository.GetRatingsByReviewId(review.Id.ToString());
                var resultRating = reviewRatings.Average(x => x.Value);

                var allReviewLikes = await _likeRepository.GetLikesByReviewId(review.Id.ToString());
                var likeCount = allReviewLikes.Count();

                var currentUser = await _userManager.FindByIdAsync(review.UserId);

                enhancedReview.Id = review.Id;
                enhancedReview.Nickname = currentUser.Nickname;
                enhancedReview.Title = review.Title;
                enhancedReview.Description = review.Description;
                enhancedReview.ReviewText = review.ReviewText;
                enhancedReview.Theme = review.Theme;
                enhancedReview.Image = review.Image;
                enhancedReview.Created = review.Created;
                enhancedReview.Link = review.Link;
                enhancedReview.UserId = review.UserId;
                enhancedReview.Rating = resultRating;
                enhancedReview.LikesCount = likeCount;
            }
            catch
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Something went wrong." },
                    Data = ""
                });
            }

            return Ok(new
            {
                IsSuccess = true,
                Errors = "",
                Data = enhancedReview
            });
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllReviews()
        {
            IList<EnhancedReview> resultReviews = new List<EnhancedReview>();
            IList<Review> wholeReviews = new List<Review>();

            try
            {
                wholeReviews = await _reviewRepository.GetAllReviews();

                foreach (var review in wholeReviews)
                {
                    var reviewRatings = await _ratingRepository.GetRatingsByReviewId(review.Id.ToString());
                    var resultRating = reviewRatings.Average(x => x.Value);

                    var allReviewLikes = await _likeRepository.GetLikesByReviewId(review.Id.ToString());
                    var likeCount = allReviewLikes.Count();

                    var currentUser = await _userManager.FindByIdAsync(review.UserId);

                    var resultReview = new EnhancedReview()
                    {
                        Id = review.Id,
                        Nickname = currentUser.Nickname,
                        Title = review.Title,
                        Description = review.Description,
                        ReviewText = review.ReviewText,
                        Theme = review.Theme,
                        Image = review.Image,
                        Created = review.Created,
                        Link = review.Link,
                        UserId = review.UserId,
                        Rating = resultRating,
                        LikesCount = likeCount
                    };

                    resultReviews.Add(resultReview);
                }
            }
            catch
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Something went wrong." },
                    Data = ""
                });
            }

            return Ok(new
            {
                IsSuccess = true,
                Errors = "",
                Data = resultReviews
            });
        }

        [HttpGet("getbyemail/{email}")]
        [Authorize]
        public async Task<IActionResult> GetReviewsByEmail(string email)
        {
            IList<EnhancedReview> resultReviews = new List<EnhancedReview>();
            IList<Review> wholeReviews = new List<Review>();

            var user = await _userManager.FindByEmailAsync(email);

            try
            {
                wholeReviews = await _reviewRepository.GetReviewsByUserId(user.Id);

                foreach (var review in wholeReviews)
                {
                    var reviewRatings = await _ratingRepository.GetRatingsByReviewId(review.Id.ToString());
                    var resultRating = reviewRatings.Average(x => x.Value);

                    var allReviewLikes = await _likeRepository.GetLikesByReviewId(review.Id.ToString());
                    var likeCount = allReviewLikes.Count();

                    var resultReview = new EnhancedReview()
                    {
                        Id = review.Id,
                        Nickname = user.Nickname,
                        Title = review.Title,
                        Description = review.Description,
                        ReviewText = review.ReviewText,
                        Theme = review.Theme,
                        Image = review.Image,
                        Created = review.Created,
                        Link = review.Link,
                        UserId = review.UserId,
                        Rating = resultRating,
                        LikesCount = likeCount
                    };

                    resultReviews.Add(resultReview);
                }
            }
            catch
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Something went wrong." },
                    Data = ""
                });
            }

            return Ok(new
            {
                IsSuccess = true,
                Errors = "",
                Data = resultReviews
            });
        }

        [HttpPost("createreview")]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto createReviewRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(createReviewRequestDto.Email);

            if (user == null)
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            var resultReview = new Review()
            {
                Id = Guid.NewGuid(),
                Title = createReviewRequestDto.Title,
                Description = createReviewRequestDto.Description,
                ReviewText = createReviewRequestDto.ReviewText,
                Theme = createReviewRequestDto.Theme,
                Image = CropImageFeature.CropImage(createReviewRequestDto.Image, (int)ImageSizeEnum.Default),
                Created = DateTime.Parse(DateTime.Now.ToString()).ToUniversalTime(),
                Link = LinkGeneratorFeature.GenerateLink(),
                UserId = user.Id,
            };

            try
            {
                await _reviewRepository.CreateReview(resultReview);
                await _reviewRepository.SaveChanges();
            }
            catch
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to create review." },
                    Data = ""
                });
            }
            
            return Ok();
        }

        [HttpPost("updatereview")]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewRequestDto updateReviewRequestDto)
        {               
            try
            {
                var currentReview = await _reviewRepository.GetByReviewId(updateReviewRequestDto.Id);

                currentReview.Title = updateReviewRequestDto.Title;
                currentReview.Description = updateReviewRequestDto.Description;
                currentReview.ReviewText = updateReviewRequestDto.ReviewText;
                currentReview.Theme = updateReviewRequestDto.Theme;
                currentReview.Image = CropImageFeature.CropImage(updateReviewRequestDto.Image, (int)ImageSizeEnum.Default);

                await _reviewRepository.UpdateReview(currentReview);
                await _reviewRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to update review." },
                    Data = ""
                });
            }

            return Ok();
        }

        [HttpDelete("deletereview/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(string id)
        {
            try
            {
                var review = await _reviewRepository.GetByReviewId(id);

                if (review != null)
                {
                    var likes = await _likeRepository.GetLikesByReviewId(id);

                    foreach (var like in likes)
                    {
                        await _likeRepository.DeleteLike(like);
                    }

                    var ratings = await _ratingRepository.GetRatingsByReviewId(id);

                    foreach (var rating in ratings)
                    {
                        await _ratingRepository.DeleteRating(rating);
                    }

                    await _reviewRepository.DeleteReview(review);

                    await _reviewRepository.SaveChanges();
                }
                else
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Review not found." },
                        Data = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to delete review." },
                    Data = ""
                });
            }

            return Ok();
        }
    }
}
