using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SmartPlayer.Core.DTOs;

namespace SmartPlayer.Core.Repositories
{
    public class MusicRepository : BaseRepository<Song>
    {
        public MusicRepository(SmartPlayerEntities context)
            : base(context) { }

        public List<Song> SearchByTerm(string term)
        {
            return Context.Songs.Where(s => s.Name.ToLower().Contains(term.ToLower())).Take(10).ToList();
        }

        public List<Song> GetNextSongBasedOnUserAndGrade(int currentSongId, List<int> excludedIds = null)
        {
            Song currentSong = this.Context.Songs
                .Where(x => x.Id == currentSongId)
                .Include(x => x.UserSongVotes)
                .Include(x => x.CorrelationsAsPrimary)
                .Include(x => x.CorrelationsAsPrimary.Select(y => y.PrimarySong ))
                .Include(x => x.CorrelationsAsSecondary)
                .Include(x => x.CorrelationsAsSecondary.Select(y => y.SecondarySong))
                .First();

            var coeffsAsPrimary = currentSong.CorrelationsAsPrimary
                .Select(x => new SongSongCoefficient 
                {
                    Song = x.SecondarySong,
                    Coefficient = x.CorrelationScore 
                });

            var coeffsAsSecondary = currentSong.CorrelationsAsSecondary
                .Select(x => new SongSongCoefficient
                {
                    Song = x.PrimarySong,
                    Coefficient = x.CorrelationScore
                });

            var bestSongs = coeffsAsPrimary
                .Union(coeffsAsSecondary)
                .OrderByDescending(x => x.Coefficient)
                .Select(x => x.Song);

            return bestSongs.Take(10).ToList();
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
