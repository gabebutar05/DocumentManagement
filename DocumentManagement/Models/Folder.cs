namespace DocumentManagement.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubFolderId { get; set; }
        public SubFolder SubFolder { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();

    }
}
