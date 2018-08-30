#region

using System.Collections.Generic;
using System.Web;

#endregion

namespace xDBCommentsManager
{
    public interface ICommentRepository
    {
        void Insert(Comment comment);

        IList<Comment> Retrieve(string postId, bool? approve, int? pageNumber, int? pageSize, string order, string parentId);

        void Update(Comment comment);

        void Delete(Comment comment);

        HtmlString NestedListing(List<Comment> comment,int index, int replyLevel, System.Text.StringBuilder sb, bool hasChild = false);
    }
}