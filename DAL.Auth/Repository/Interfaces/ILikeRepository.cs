using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;

namespace DAL.Auth.Repository.Interfaces
{
    public interface ILikeRepository : IUnitOfWork
    {
        Task<IList<Like>> GetAllLikesThatWereGivenToUserByUserId(string userId);

        Task<Like?> GetLike(string id);

        Task<Like?> GetLikeByUserAndReviewIds(string userId, string reviewId);

        Task<IList<Like>> GetLikesByReviewId(string id);

        Task CreateLike(Like like);

        Task DeleteLike(Like like);
    }
}
