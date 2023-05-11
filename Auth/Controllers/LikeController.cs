using Auth.Entities.DataTransferObjects;
using DAL.Auth.Models;
using DAL.Auth.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/likes")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILikeRepository _likeRepository;

        public LikeController(ILikeRepository likeRepository, UserManager<User> userManager)
        {
            _likeRepository = likeRepository;
            _userManager = userManager;
        }

        [HttpPost("createlike")]
        [Authorize]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequestDto createLikeRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(createLikeRequestDto.Email);

            if (user == null)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "User not found." },
                    Data = ""
                });
            }

            if (!(await _likeRepository.GetLikeByUserAndReviewIds(user.Id, createLikeRequestDto.ReviewId) == null))
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Like already exists." },
                    Data = ""
                });
            }

            var like = new Like()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ReviewId = new Guid(createLikeRequestDto.ReviewId)
            };

            try
            {
                await _likeRepository.CreateLike(like);
                await _likeRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to create like." },
                    Data = ""
                });
            }

            return Ok(new
            {
                isSuccess = true,
                Errors = "",
                Data = like
            });
        }

        [HttpDelete("deletelike/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLike(string id)
        {
            try
            {
                var like = await _likeRepository.GetLike(id);

                if (like != null)
                {
                    await _likeRepository.DeleteLike(like);
                    await _likeRepository.SaveChanges();
                }
                else
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Like not found." },
                        Data = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to delete Like." },
                    Data = ""
                });
            }

            return Ok();
        }

        [HttpPost("isliked")]
        [Authorize]
        public async Task<IActionResult> IsLiked([FromBody] GetLikeRequestDto getLikeRequestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(getLikeRequestDto.Email);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "User not found." },
                        Data = ""
                    });
                }

                var like = await _likeRepository.GetLikeByUserAndReviewIds(user.Id, getLikeRequestDto.ReviewId);

                if (like == null)
                {
                    return Ok(new
                    {
                        IsSuccess = true,
                        Errors = "",
                        Data = ""
                    });
                }

                return Ok(new
                {
                    isSuccess = true,
                    Errors = "",
                    Data = like
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to get Like." },
                    Data = ""
                });
            }
        }


        [HttpGet("getallreviewlikes/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllReviewLikes(string id)
        {
            try
            {
                var allReviewLikes = await _likeRepository.GetLikesByReviewId(id);
                var likeCount = allReviewLikes.Count();

                return Ok(new
                {
                    IsSuccess = true,
                    Errors = "",
                    Data = likeCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to get all likes." },
                    Data = ""
                });
            }
        }

        [HttpGet("getalluserslikes/{email}")]
        [Authorize]
        public async Task<IActionResult> GetAllUsersLikes(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "User not found." },
                        Data = ""
                    });
                }

                var likes = await _likeRepository.GetAllLikesThatWereGivenToUserByUserId(user.Id);
                int allUserLikes = likes.Count();

                return Ok(new
                {
                    IsSuccess = true,
                    Errors = "",
                    Data = allUserLikes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Failed to get all likes." },
                    Data = ""
                });
            }
        }
    }
}
