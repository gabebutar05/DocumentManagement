namespace DocumentManagement.Models
{
    public class SubFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
        public ParentFolder? ParentFolder { get; set; }
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    }
}
