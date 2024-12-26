using DocumentManagement.Data;
using DocumentManagement.Interface;
using DocumentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace DocumentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(DataContext context,IDocumentRepository documentRepository)
        {
            _context = context;
            _documentRepository = documentRepository;
        }
        private readonly List<string> _allowedExtensions = new()
        {
            ".pdf", ".doc", ".docx", ".txt", // Dokumen
            ".jpg", ".jpeg", ".png",         // Gambar
            ".xls", ".xlsx"                  // Spreadsheet
        };

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, int folderId)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("No files provided");
            }
            if (folderId == 0)
            {
                return BadRequest("FolderIds are required.");
            }
            var isFolderValid = await _documentRepository.AreFolderIdsValidAsync(folderId);
            if (!isFolderValid)
            {
                return BadRequest("Folder ID not found.");
            }
            var uploadedFiles = new List<Document>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!_allowedExtensions.Contains(extension))
                    {
                        return BadRequest($"File extension not allowed: {file.FileName}");
                    }
                    // Periksa apakah file dengan nama dan tipe yang sama sudah ada di database
                    var existingFile = await _documentRepository.GetDocumentByNameAndExtensionAsync(file.FileName, extension, folderId);
                    if (existingFile != null)
                    {
                        return BadRequest($"File with the same name and type already exists: {file.FileName}");
                    }
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);

                    var document = new Document
                    {
                        FileName = file.FileName,
                        FileContent = memoryStream.ToArray(),
                        FolderId = folderId
                    };

                    uploadedFiles.Add(document);
                }
            }

            /*_context.Documents.AddRange(uploadedFiles);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Files uploaded successfully", Files = uploadedFiles });*/
            var result = await _documentRepository.UpdateDocumentsAsync(uploadedFiles);

            if (result)
            {
                return Ok(new { Message = "Successfully Upload data" });
            }

            return BadRequest("Failed to upload data");
        }

        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> DownloadFiles([FromQuery] List<int> fileIds)
        {
            List<Document> documents;

            // Jika tidak ada fileIds yang diberikan, ambil semua dokumen dari database
            if (fileIds == null || !fileIds.Any())
            {
                documents = await _context.Documents.ToListAsync();
            }
            else
            {
                // Jika ada fileIds yang diberikan, ambil dokumen berdasarkan fileIds
                documents = await _context.Documents.Where(d => fileIds.Contains(d.Id)).ToListAsync();
            }

            // Jika hanya ada satu dokumen, kembalikan file tersebut
            if (documents.Count == 1)
            {
                var document = documents.First();
                return File(document.FileContent, "application/octet-stream", document.FileName);
            }

            // Jika lebih dari satu dokumen, buat file ZIP
            using var zipMemoryStream = new MemoryStream();
            using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var doc in documents)
                {
                    var zipEntry = archive.CreateEntry(doc.FileName, CompressionLevel.Fastest);
                    using var zipEntryStream = zipEntry.Open();
                    await zipEntryStream.WriteAsync(doc.FileContent);
                }
            }

            return File(zipMemoryStream.ToArray(), "application/zip", "Documents.zip");
        }

        [HttpDelete]
        [Route("delete-documents")]
        public async Task<IActionResult> DeleteDocuments([FromQuery] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("No document IDs provided.");
            }

            var result = await _documentRepository.DeleteDocumentsAsync(ids);

            if (!result)
            {
                return NotFound("One or more documents not found.");
            }

            return Ok(new { Message = "Documents successfully deleted." });
        }



    }
}
