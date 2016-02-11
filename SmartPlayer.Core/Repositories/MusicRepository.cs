﻿using SmartPlayer.Data;
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
            return Context.Songs.Where(s => s.Name.ToLower().Contains(term.ToLower())).Take(10).ToList();
        }

        public List<Song> GetNextSongBasedOnUserAndGrade(double currentSongGrade, List<int> excludedIds = null)
        {
            IQueryable<Song> query = this.Context.Songs
                .Include(x => x.UserSongVotes)
                .OrderBy(x => Math.Abs(currentSongGrade - x.Grade));

            if(excludedIds != null)
            {
                query = query.Where(x => !excludedIds.Contains(x.Id));
            }

            return query.Take(10).ToList();
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
