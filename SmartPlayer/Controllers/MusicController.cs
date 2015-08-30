using SmartPlayer.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SmartPlayer.Controllers
{
    public class MusicController : ApiController
    {
        const string ServerUploadFolder = @"D:\Dropbox\SmartPlayerFileServer";

        [Route("api/Music/Upload")]
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task UploadSingleFile()
        {
            var streamProvider = new MultipartFormDataStreamProvider(ServerUploadFolder);
            var test = await Request.Content.ReadAsMultipartAsync(streamProvider);

            //return new FileResult
            //{
            //    FileNames = streamProvider.FileData.Select(entry => entry.LocalFileName),
            //    Names = streamProvider.FileData.Select(entry => entry.Headers.ContentDisposition.FileName),
            //    ContentTypes = streamProvider.FileData.Select(entry => entry.Headers.ContentType.MediaType),
            //    Description = streamProvider.FormData["description"],
            //    CreatedTimestamp = DateTime.UtcNow,
            //    UpdatedTimestamp = DateTime.UtcNow,
            //    DownloadLink = "TODO, will implement when file is persisited"
            //};
        }
    }
}
