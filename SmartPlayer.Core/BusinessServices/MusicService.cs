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
using SmartPlayer.Core.Utility;

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

            var notNextSongIds = songRequest.PlayedSongIds ?? new List<int>();
            notNextSongIds.Add(currentSong.Id);

            var selectedSong = repo.GetAll()
                .Where(x => !notNextSongIds.Contains(x.Id))
                .OrderBy(x => Math.Abs(currentSong.Grade - x.Grade))
                .First();

            var songUrl = string.Format("http://{0}{1}{2}",
                IpV4Provider.GetLocalIPAddress(),
                ConfigurationManager.AppSettings["MediaServerLoadPort"],
                selectedSong.Guid);

            PlayerSongDto song = new PlayerSongDto()
            {
                Id = selectedSong.Id,
                Name = selectedSong.Name,
                Url =  songUrl
            };

            context.Dispose();

            return song;
        }

        public PlayerSongDto GetSong(int songId)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();
            MusicRepository repo = new MusicRepository(context);

            var requestedSong = repo.GetAll().First(x => x.Id == songId);

            var songUrl = string.Format("http://{0}{1}{2}",
                IpV4Provider.GetLocalIPAddress(),
                ConfigurationManager.AppSettings["MediaServerLoadPort"],
                requestedSong.Guid);

            PlayerSongDto song = new PlayerSongDto()
            {
                Id = requestedSong.Id,
                Name = requestedSong.Name,
                Url = songUrl
            };

            context.Dispose();

            return song;
        }

        public List<SongDto> SearchSong(string query)
        {
            SmartPlayerEntities context = new SmartPlayerEntities();
            MusicRepository repo = new MusicRepository(context);

            var requestedSongs = repo.SearchByTerm(query);

            context.Dispose();

            return requestedSongs.Select(x => new SongDto() { Id = x.Id, SongName = x.Name }).ToList();
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
