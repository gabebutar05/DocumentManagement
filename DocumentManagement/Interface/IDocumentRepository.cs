using DocumentManagement.Models;

namespace DocumentManagement.Interface
{
    public interface IDocumentRepository
    {
        Task<bool> UpdateDocumentsAsync(List<Document> documents);
        Task<bool> AreFolderIdsValidAsync(int folderId);
        Task<Document?> GetDocumentByNameAndExtensionAsync(string fileName, string extension, int folderId);
        Task<bool> DeleteDocumentsAsync(List<int> ids);
    }
}
