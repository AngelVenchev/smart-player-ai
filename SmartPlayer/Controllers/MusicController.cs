using SmartPlayer.Core.BusinessServices;
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
    public class MusicController : ApiController
    {
        [Route("api/Music/Upload")]
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
    }
}
