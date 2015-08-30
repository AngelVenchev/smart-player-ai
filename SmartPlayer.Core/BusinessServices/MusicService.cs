using SmartPlayer.Core.Repositories;
using SmartPlayer.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.BusinessServices
{
    public class MusicService
    {
        public void Store(string originalFileName, string guid)
        {
            Song song = new Song()
            {
                Name = originalFileName,
                Guid = guid,
                Grade = 0.1 // TODO: Implement song grading
            };
            SmartPlayerEntities context = new SmartPlayerEntities();
            MusicRepository repo = new MusicRepository(context);
            try
            {
                repo.Create(song);
            }
            catch (DbEntityValidationException ex)
            {
                var a = ex;
            }
            context.Dispose(); // TODO find a better way to handle dbcontext
        }
    }
}
