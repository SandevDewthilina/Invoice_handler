using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

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
        public async Task<List<string[]>> SaveFile(IFormFile file)
        {
            var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "invoices");
            // var filepath = Path.Combine(folderPath, file.FileName);
            // await using Stream fileStream = new FileStream(filepath, FileMode.Create);
            // await file.CopyToAsync(fileStream);
            
            var splitPages = SplitPdf(file);
            var filePaths = new List<string[]>();

            for (int i = 0; i < splitPages.Count; i++)
            {
                var filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}-part-{i + 1}.pdf";
                var pageBytes = splitPages[i];
                var filePath = Path.Combine(folderPath, filename);
                filePaths.Add(new string[] {Path.Combine("invoices", filename), filename});
                await File.WriteAllBytesAsync(filePath, pageBytes);
            }

            return filePaths;
        }
        
        private List<byte[]> SplitPdf(IFormFile pdfFile)
        {
            List<byte[]> splitPages = new List<byte[]>();

            using (PdfDocument inputDocument = PdfReader.Open(pdfFile.OpenReadStream(), PdfDocumentOpenMode.Import))
            {
                for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
                {
                    PdfDocument outputDocument = new PdfDocument();
                    outputDocument.AddPage(inputDocument.Pages[pageIndex]);

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        outputDocument.Save(outputStream);
                        splitPages.Add(outputStream.ToArray());
                    }
                }
            }

            return splitPages;
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