using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface ICommentRepository
    {
        List<Comment> GetAllCommentsForListing(int id);
        Comment AddComment(Comment comment);
        void DeleteComment(Comment comment);
        void EditComment(Comment comment);
    }
}
