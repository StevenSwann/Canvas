using AuctionPOCOs;
using System.Collections.Generic;
using System.Linq;

namespace AuctionContext
{
    public class SqlServerCommentRepository : ICommentRepository
    {
        Context context;

        public SqlServerCommentRepository()
        {
            context = new Context();
        }

        public SqlServerCommentRepository(Context _context)
        {
            this.context = _context;
        }

        public List<Comment> GetAllCommentsForListing(int id)
        {
            return context.Comments.Where(c => c.Listing.Id == id).ToList();
        }

        public Comment AddComment(Comment comment)
        {
            context.Comments.Add(comment);
            context.SaveChanges();
            return comment;
        }

        public void DeleteComment(Comment comment)
        {
            Comment commentToDelete = context.Comments.First(c =>c.Id == comment.Id);
            context.Comments.Remove(commentToDelete);
            context.SaveChanges();
        }


        public void EditComment(Comment comment)
        {
            Comment commentToEdit = context.Comments.First(c => c.Id == comment.Id);
            commentToEdit.Content = comment.Content;
            context.SaveChanges();
        }
    }
}
