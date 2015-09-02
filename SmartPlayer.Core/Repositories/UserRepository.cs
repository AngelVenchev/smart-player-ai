using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(SmartPlayerEntities context)
            : base(context) { }

        public List<User> GetAllExcept(string userId)
        {
            return this.Context.Users
                .Include("UserSongVote")
                .Where(x => x.Id != userId)
                .ToList();

        }

    }
}
