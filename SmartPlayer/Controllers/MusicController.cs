﻿using SmartPlayer.Core.BusinessServices;
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

namespace SmartPlayer.Controllers
{
    [RoutePrefix("api/Music")]
    public class MusicController : ApiController
    {
        [Route("Upload")]
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task UploadSingleFile()
        {
            var serverUploadFolder = ConfigurationManager.AppSettings["MediaServerBaseUrl"];
            var streamProvider = new MultipartFormDataStreamProvider(serverUploadFolder);
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            MusicService service = new MusicService();
            string fileOriginalName = streamProvider.Contents[0].Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });
            string fileGuid = Path.GetFileName(streamProvider.FileData.First().LocalFileName);

            service.Store(fileOriginalName, fileGuid);
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<List<SongDto>> GetAll()
        {
            MusicService service = new MusicService();
            var allSongs = service.GetAllSongs();
            return allSongs;
        }
    }
}