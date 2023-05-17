using FileManagementWebApp.Data;
using FileManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;

namespace FileManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var files = await dbContext.FileInfos.ToListAsync();
            return View(files);
        }

        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Failed");
            }

            var fileInfo = new FileInformation
            {
                FileName = file.FileName,
                UploadDate = DateTime.Now
            };


            Stopwatch stopwatch = Stopwatch.StartNew();
            var archiveFilePath = await ArchiveFile(file);
            stopwatch.Stop();

            if (System.IO.File.Exists(archiveFilePath))
            {
                fileInfo.ArchiveFilePath = archiveFilePath;
                fileInfo.Status = "Success";
                fileInfo.ArchivingTime = stopwatch.Elapsed;
            }
            else
            {
                fileInfo.Status = "Failed";
            }

            dbContext.FileInfos.Add(fileInfo);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        public IActionResult Download(int id)
        {
            var fileInfo = dbContext.FileInfos.Find(id);
            if (fileInfo == null || fileInfo.Status != "Success")
            {
                return RedirectToAction("Failed");
            }

            var archiveFilePath = fileInfo.ArchiveFilePath;


            if (!System.IO.File.Exists(archiveFilePath))
            {

                return RedirectToAction("Index");
            }


            var fileBytes = System.IO.File.ReadAllBytes(archiveFilePath);

            var contentType = "application/zip";
            var fileName = Path.GetFileName(archiveFilePath);

            return File(fileBytes, contentType, fileName);
        }

        private async Task<string> ArchiveFile(IFormFile file)
        {
            var archiveFileName = $"{Guid.NewGuid()}.zip";

            var archiveFilePath = Path.Combine("ArchiveFolder", archiveFileName);

            if (!Directory.Exists("ArchiveFolder"))
            {
                Directory.CreateDirectory("ArchiveFolder");
            }

            using (var fileStream = new FileStream(archiveFilePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry(file.FileName);

                    using (var entryStream = entry.Open())
                    {
                        await file.CopyToAsync(entryStream);
                    }
                }
            }
            return archiveFilePath;
        }
    }
}
