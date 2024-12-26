namespace DocumentManagement.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
    }
}
