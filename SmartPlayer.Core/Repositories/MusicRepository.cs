using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
