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
            string mediaServerUrlBase = ConfigurationManager.AppSettings["MediaServerSaveBaseUrl"];
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

        public PlayerSongDto GetNextSong(NextSongDto songRequest)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();

            MusicRepository repo = new MusicRepository(context);

            // Add recommendation system logic
            var currentSong = repo.GetAll().First(x => x.Id == songRequest.CurrentSongId);

            var selectedSong = repo.GetAll()
                .Where(x => !songRequest.PlayedSongIds.Contains(x.Id))
                .OrderBy(x => Math.Abs(currentSong.Grade - x.Grade))
                .First();

            PlayerSongDto song = new PlayerSongDto()
            {
                Id = selectedSong.Id,
                Name = selectedSong.Name,
                Url = ConfigurationManager.AppSettings["MediaServerLoadBaseUrl"] + selectedSong.Guid
            };

            context.Dispose();

            return song;
        }

        public void RateSong(SongRatingDto rating, string userName)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();

            UserRepository userRepo = new UserRepository(context);

            User currentUser = userRepo.GetAll().First(x => x.Email == userName);

            context.UserSongVotes.Add(new UserSongVote()
                {
                    Rating = rating.Rating,
                    SongId = rating.SongId,
                    UserId = currentUser.Id
                });
            context.SaveChanges();
        }
    }
}
