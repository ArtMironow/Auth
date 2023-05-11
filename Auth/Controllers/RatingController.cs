using Auth.Entities.DataTransferObjects;
using DAL.Auth.Models;
using DAL.Auth.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository, UserManager<User> userManager)
        {
            _ratingRepository = ratingRepository;
            _userManager = userManager;
        }

        [HttpPost("createrating")]
        [Authorize]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingRequestDto createRatingRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(createRatingRequestDto.Email);

            if (user == null)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "User not found." },
                    Data = ""
                });
            }

            if (!(await _ratingRepository.GetRatingByUserAndReviewIds(user.Id, createRatingRequestDto.ReviewId) == null))
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Rating already exists." },
                    Data = ""
                });
            }
           
            var rating = new Rating()
            {
                Id = Guid.NewGuid(),
                Value = createRatingRequestDto.Value,
                UserId = user.Id,
                ReviewId = new Guid(createRatingRequestDto.ReviewId)
            };

            try
            {
                await _ratingRepository.CreateRating(rating);
                await _ratingRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to create Rating." },
                    Data = ""
                });
            }

            return Ok(new
            {
                isSuccess = true,
                Errors = "",
                Data = rating
            });
        }

        [HttpPost("updaterating")]
        [Authorize]
        public async Task<IActionResult> UpdateRating([FromBody] UpdateRatingRequestDto updateRatingRequestDto)
        {          
            try
            {
                var currentRating = await _ratingRepository.GetRating(updateRatingRequestDto.Id);
                currentRating.Value = updateRatingRequestDto.Value;

                await _ratingRepository.UpdateRating(currentRating);
                await _ratingRepository.SaveChanges();

                return Ok(new
                {
                    isSuccess = true,
                    Errors = "",
                    Data = currentRating
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to update rating." },
                    Data = ""
                });
            }
        }

        [HttpDelete("deleterating/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRating(string id)
        {
            try
            {
                var rating = await _ratingRepository.GetRating(id);

                if (rating != null)
                {
                    await _ratingRepository.DeleteRating(rating);
                    await _ratingRepository.SaveChanges();
                }
                else
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Rating record not found." },
                        Data = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to delete Rating record." },
                    Data = ""
                });
            }

            return Ok();
        }

        [HttpGet("getreviewsrating/{id}")]
        [Authorize]
        public async Task<IActionResult> GetReviewsRating(string id)
        {
            double? rating = 0;

            try
            {
                var wholeRatings = await _ratingRepository.GetRatingsByReviewId(id);

                rating = wholeRatings.Average(x => x.Value);
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
                Data = rating
            });
        }

        [HttpPost("getratingbyemailandreviewid")]
        [Authorize]
        public async Task<IActionResult> GetRatingByEmailAndReviewId([FromBody] GetRatingByEmailAndReviewIdRequestDto getRatingByEmailAndReviewIdRequestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(getRatingByEmailAndReviewIdRequestDto.Email);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "User not found." },
                        Data = ""
                    });
                }

                var rating = await _ratingRepository.GetRatingByUserAndReviewIds(user.Id, getRatingByEmailAndReviewIdRequestDto.ReviewId);

                if (rating == null)
                {
                    return Ok(new
                    {
                        isSuccess = true,
                        Errors = "",
                        Data = ""
                    });
                }

                return Ok(new
                {
                    isSuccess = true,
                    Errors = "",
                    Data = rating
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to create Rating." },
                    Data = ""
                });
            }
        }

    }
}
