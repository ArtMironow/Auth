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
    public class ReviewRepository : IReviewRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public ReviewRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task SaveChanges()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<IList<Review>> GetAllReviews()
        {
            return await _repositoryContext.Review.ToListAsync();
        }

        public async Task<IList<Review>> GetReviewsByUserId(string userId)
        {
            return await _repositoryContext.Review.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Review?> GetByReviewId(string id)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(id, out guidOutput);

            if (!isValid)
            {
                return null;
            }

            return await _repositoryContext.Review.Where(x => x.Id == new Guid(id)).FirstOrDefaultAsync();
        }

        public async Task CreateReview(Review review)
        { 
            await _repositoryContext.Review.AddAsync(review);
        }

        public async Task UpdateReview(Review review)
        {
            _repositoryContext.Review.Update(review);

            await Task.CompletedTask;
        }

        public async Task DeleteReview(Review review)
        {
            _repositoryContext.Review.Remove(review);

            await Task.CompletedTask;
        }
    }
}
