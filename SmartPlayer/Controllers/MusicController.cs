using SmartPlayer.Core.BusinessServices;
using SmartPlayer.Core.DTOs;
using SmartPlayer.Validators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SmartPlayer.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Music")]
    public class MusicController : ApiController
    {
        [Route("Upload")]
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task UploadSingleFile()
        {
            var serverUploadFolder = ConfigurationManager.AppSettings["MediaServerSaveBaseUrl"];
            var streamProvider = new MultipartFormDataStreamProvider(serverUploadFolder);
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            MusicService service = new MusicService();
            for (int i = 0; i < streamProvider.Contents.Count; i++)
            {
                string fileOriginalName = streamProvider.Contents[i].Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });
                string fileGuid = Path.GetFileName(streamProvider.FileData.First().LocalFileName);

                service.Store(fileOriginalName, fileGuid);
            }

        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<List<SongDto>> GetAll()
        {
            MusicService service = new MusicService();
            var allSongs = service.GetAllSongs();
            return allSongs;
        }

        [Route("GetSong")]
        [HttpGet]
        public async Task<PlayerSongDto> GetSong(int songId)
        {
            MusicService service = new MusicService();
            var song = service.GetSong(songId, User.Identity.Name);
            return song;
        }

        [Route("SearchSong")]
        [HttpGet]
        public async Task<List<SongDto>> SearchSong(string s)
        {
            MusicService service = new MusicService();
            var songs = service.SearchSong(s);
            return songs;
        }

        [Route("Next")]
        [HttpPost]
        public async Task<PlayerSongDto> GetNextSong(NextSongDto nextSongRequest)
        {
            MusicService service = new MusicService();
            var song = service.GetNextSong(nextSongRequest, User.Identity.Name);
            return song;
        }

        [Route("Rate")]
        [HttpPost]
        [Authorize]
        public async Task RateSong(SongRatingDto rating)
        {
            MusicService service = new MusicService();
            var username = this.User.Identity.Name;
            service.RateSong(rating, username);
        }
    }
}
