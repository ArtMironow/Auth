using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Auth.Repository.Interfaces
{
    public interface IReviewRepository : IUnitOfWork
    {
        Task<IList<Review>> GetAllReviews();

        Task<IList<Review>> GetReviewsByUserId(string userId);

        Task<Review> GetByReviewId(string id);

        Task CreateReview(Review review);

        Task UpdateReview(Review review);

        Task DeleteReview(Review review);
    }
}
