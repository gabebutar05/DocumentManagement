namespace DocumentManagement.Dto
{
    public class SubFolderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
    }
}
