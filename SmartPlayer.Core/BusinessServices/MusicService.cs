using SmartPlayer.Core.Repositories;
using SmartPlayer.Core.SongAnalyzer;
using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Assemblies;
using System.Configuration;
using System.IO;
using SmartPlayer.Core.DTOs;

namespace SmartPlayer.Core.BusinessServices
{
    public class MusicService
    {
        public void Store(string originalFileName, string guid)
        {
            string mediaServerUrlBase = ConfigurationManager.AppSettings["MediaServerBaseUrl"];
            string fullName = Path.Combine(mediaServerUrlBase, guid);

            double songGrade = Analyzer.GetGradeFor(fullName);

            Song song = new Song()
            {
                Name = originalFileName,
                Guid = guid,
                Grade = songGrade
            };

            SmartPlayerEntities context = new SmartPlayerEntities();
            MusicRepository repo = new MusicRepository(context);

            repo.Create(song);

            context.Dispose(); // TODO find a better way to handle dbcontext
        }

        public List<SongDto> GetAllSongs()
        {
            SmartPlayerEntities context = new SmartPlayerEntities();

            MusicRepository repo = new MusicRepository(context);

            var allSongs = repo.GetAll()
                .Select(x => new SongDto
                {
                    Id = x.Id,
                    SongName = x.Name
                })
                .ToList();

            context.Dispose();

            return allSongs;
        }
    }
}
