using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;

namespace DAL.Auth.Repository.Interfaces
{
    public interface IRatingRepository : IUnitOfWork
    {
        Task<Rating?> GetRating(string id);

        Task<Rating?> GetRatingByUserAndReviewIds(string userId, string reviewId);

        Task<IList<Rating>> GetRatingsByReviewId(string id);

        Task CreateRating(Rating rating);

        Task UpdateRating(Rating rating);

        Task DeleteRating(Rating rating);
    }
}
