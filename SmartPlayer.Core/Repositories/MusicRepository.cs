using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SmartPlayer.Core.Repositories
{
    public class MusicRepository : BaseRepository<Song>
    {
        public MusicRepository(SmartPlayerEntities context)
            : base(context) { }

        public List<Song> SearchByTerm(string term)
        {
            return Context.Songs.Where(s => s.Name.Contains(term)).ToList();
        }

        public Song GetNextSongBasedOnUserAndGrade(double currentSongGrade, List<int> excludedSongs)
        {
            var query = this.Context.Songs
                .Where(x => !excludedSongs.Contains(x.Id))
                .Include(x => x.UserSongVotes)
                .OrderBy(x => Math.Abs(currentSongGrade - x.Grade));

            return query.First();
        }

        public Song GetSongById(int songId)
        {
            var query = this.Context.Songs
                .Where(x => x.Id == songId)
                .Include(x => x.UserSongVotes);

            return query.First();
        }
    }
}
