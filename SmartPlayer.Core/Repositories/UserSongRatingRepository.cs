using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.Repositories
{
    public class UserSongRatingRepository : BaseRepository<UserSongVote>
    {
        public UserSongRatingRepository(SmartPlayerEntities context)
            : base(context) { }

        public List<UserSongVote> GetUserSongVotes(string userId)
        {
            return Context.UserSongVotes
                .Where(x => x.UserId == userId)
                .ToList();
        }
    }
}
