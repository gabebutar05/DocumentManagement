using DocumentManagement.Data;
using DocumentManagement.Interface;
using DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DataContext _context;

        public DocumentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AreFolderIdsValidAsync(int folderId)
        {
            // Cek apakah folderId ada di database
            var isValid = await _context.Folders.AnyAsync(f => f.Id == folderId);

            return isValid;
        }

        public async Task<bool> DeleteDocumentsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return false;
            }

            // Cari dokumen berdasarkan daftar ID
            var documents = await _context.Documents
                                          .Where(d => ids.Contains(d.Id))
                                          .Include(d => d.Folder) 
                                          .ToListAsync();

            if (documents.Count != ids.Count)
            {
                return false;
            }
            _context.Documents.RemoveRange(documents);

            // Simpan perubahan ke database
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<Document?> GetDocumentByNameAndExtensionAsync(string fileName, string extension, int folderId)
        {
            var documents = await _context.Documents
                                  .Where(d => d.FolderId == folderId)
                                  .ToListAsync();

            // Temukan dokumen yang memiliki nama dan ekstensi yang sama
            return documents.FirstOrDefault(d =>
                d.FileName == fileName &&
                Path.GetExtension(d.FileName).ToLowerInvariant() == extension.ToLowerInvariant());
        }

        public async Task<bool> UpdateDocumentsAsync(List<Document> documents)
        {
            try
            {
                
                _context.Documents.AddRange(documents);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
