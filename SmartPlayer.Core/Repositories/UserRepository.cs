using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SmartPlayer.Core.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(SmartPlayerEntities context)
            : base(context) { }

        public List<User> GetAllExcept(string userId)
        {
            return this.Context.Users
                .Include(x => x.UserSongVotes)
                .Include(u => u.UserSongVotes.Select(usv => usv.Song))
                .Where(x => x.Id != userId)
                .ToList();
        }

        public User GetUserByUsername(string username)
        {
            return this.Context.Users.First(x => x.UserName == username);
        }
    }
}
