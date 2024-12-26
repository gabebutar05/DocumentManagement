namespace DocumentManagement.Models
{
    public class ParentFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SubFolder> SubFolders { get; set; } = new List<SubFolder>();
    }
}
