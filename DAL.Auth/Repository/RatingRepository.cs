using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;
using DAL.Auth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Auth.Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public RatingRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task SaveChanges()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<Rating?> GetRating(string id)
        {
            return await _repositoryContext.Rating.FirstOrDefaultAsync(x => x.Id == new Guid(id));
        }

        public async Task<Rating?> GetRatingByUserAndReviewIds(string userId, string reviewId)
        {
            return await _repositoryContext.Rating.FirstOrDefaultAsync(x => x.UserId == userId && x.ReviewId == new Guid(reviewId));
        }

        public async Task<IList<Rating>> GetRatingsByReviewId(string id)
        {
            return await _repositoryContext.Rating.Where(x => x.ReviewId == new Guid(id)).ToListAsync();
        }

        public async Task CreateRating(Rating rating)
        {
            await _repositoryContext.Rating.AddAsync(rating);
        }

        public async Task UpdateRating(Rating rating)
        {
            _repositoryContext.Rating.Update(rating);

            await Task.CompletedTask;
        }

        public async Task DeleteRating(Rating rating)
        {
            _repositoryContext.Rating.Remove(rating);

            await Task.CompletedTask;
        }
    }
}
