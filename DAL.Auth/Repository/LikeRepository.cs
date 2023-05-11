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
    public class LikeRepository : ILikeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public LikeRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task SaveChanges()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<IList<Like>> GetAllLikesThatWereGivenToUserByUserId(string userId)
        {
            return await _repositoryContext.Like
                .Join(_repositoryContext.Review.Where(x => x.UserId == userId), l => l.ReviewId, r => r.Id, (l, r) => new Like
                    {
                        Id = l.Id,
                        UserId = userId
                    })
                .ToListAsync();
        }

        public async Task<Like?> GetLike(string id)
        {
            return await _repositoryContext.Like.FirstOrDefaultAsync(x => x.Id == new Guid(id));
        }

        public async Task<Like?> GetLikeByUserAndReviewIds(string userId, string reviewId)
        {
            return await _repositoryContext.Like.FirstOrDefaultAsync(x => x.UserId == userId && x.ReviewId == new Guid(reviewId));
        }

        public async Task<IList<Like>> GetLikesByReviewId(string id)
        {
            return await _repositoryContext.Like.Where(x => x.ReviewId == new Guid(id)).ToListAsync();
        }

        public async Task CreateLike(Like like)
        {
            await _repositoryContext.Like.AddAsync(like);
        }

        public async Task DeleteLike(Like like)
        {
             _repositoryContext.Like.Remove(like);

             await Task.CompletedTask;
        }
    }
}
