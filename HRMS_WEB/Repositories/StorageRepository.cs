using System;
using System.IO;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly HRMSDbContext _db;

        public StorageRepository(IWebHostEnvironment hostingEnvironment, HRMSDbContext db)
        {
            this._hostingEnvironment = hostingEnvironment;
            _db = db;
        }
        public async Task SaveFile(IFormFile file)
        {
            var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "invoices");
            var filepath = Path.Combine(folderPath, file.FileName);
            await using Stream fileStream = new FileStream(filepath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        public async Task DeleteUpload(int Id)
        {
            
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            var path = Path.Combine(_hostingEnvironment.WebRootPath, upload.FilePath);
            if ( System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            // delete from databaseo
            _db.Upload.Remove(upload);
            await _db.SaveChangesAsync();
        }
    }
}