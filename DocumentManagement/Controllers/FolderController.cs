using DocumentManagement.Data;
using DocumentManagement.Dto;
using DocumentManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FolderController : ControllerBase
    {
        private readonly DataContext _context;

        public FolderController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add-parent-folder")]
        public async Task<IActionResult> AddParentFolder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Folder name is required.");
            }

            var parentFolder = new ParentFolder { Name = name };
            _context.ParentFolders.Add(parentFolder);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully created folder", Folder = parentFolder });
        }

        [HttpGet]
        [Route("get-parent-folders")]
        public async Task<IActionResult> GetParentFolders()
        {
            var parentFolders = await _context.ParentFolders.ToListAsync();
            return Ok(parentFolders);
        }

        [HttpPut]
        [Route("edit-parent-folder/{id}")]
        public async Task<IActionResult> EditParentFolder(int id, [FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Folder name is required.");
            }

            var parentFolder = await _context.ParentFolders.FindAsync(id);
            if (parentFolder == null)
            {
                return NotFound("Parent folder not found.");
            }

            parentFolder.Name = name;

            _context.ParentFolders.Update(parentFolder);
            await _context.SaveChangesAsync();

            //return Ok(parentFolder);
            return Ok(new { Message = "Successfully Updated folder", Folder = parentFolder });
        }

        [HttpPost]
        [Route("add-subfolder")]
        public async Task<IActionResult> AddSubFolder(string name, int? parentFolderId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Subfolder name is required.");
            }

            ParentFolder? parentFolder = null;
            if (parentFolderId.HasValue)
            {
                parentFolder = await _context.ParentFolders.FindAsync(parentFolderId);
                if (parentFolder == null)
                {
                    return BadRequest("Parent folder not found.");
                }
            }

            var subFolder = new SubFolder
            {
                Name = name,
                ParentFolderId = parentFolderId
            };

            _context.SubFolders.Add(subFolder);
            await _context.SaveChangesAsync();

            // Gunakan DTO
            var subFolderDto = new SubFolderDto
            {
                Id = subFolder.Id,
                Name = subFolder.Name,
                ParentFolderId = subFolder.ParentFolderId
            };

            //return Ok(subFolderDto);
            return Ok(new { Message = "Successfully created subfolder", Subfolder = subFolderDto });

        }

        [HttpGet]
        [Route("get-subfolders")]
        public async Task<IActionResult> GetSubFolders()
        {
            var subFolders = await _context.SubFolders.Include(s => s.ParentFolder).ToListAsync();
            return Ok(subFolders);
        }

        [HttpPut]
        [Route("edit-subfolder/{id}")]
        public async Task<IActionResult> EditSubFolder(int id, [FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Subfolder name is required.");
            }

            var subFolder = await _context.SubFolders.FindAsync(id);
            if (subFolder == null)
            {
                return NotFound("Subfolder not found.");
            }

            subFolder.Name = name;

            _context.SubFolders.Update(subFolder);
            await _context.SaveChangesAsync();

            //return Ok(subFolder);
            return Ok(new { Message = "Successfully Updated subfolder", Subfolder = subFolder });
        }

        [HttpPost]
        [Route("add-folder")]
        public async Task<IActionResult> AddFolder(string name, int subFolderId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Folder name is required.");
            }

            var subFolder = await _context.SubFolders.FindAsync(subFolderId);
            if (subFolder == null)
            {
                return BadRequest("Subfolder not found.");
            }

            var folder = new Folder
            {
                Name = name,
                SubFolderId = subFolderId,
                SubFolder = subFolder
            };

            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();

            //return Ok(folder);
            return Ok(new { Message = "Successfully created subfolder", Folder = folder });
        }

        [HttpGet]
        [Route("get-folders")]
        public async Task<IActionResult> GetFolders()
        {
            var folders = await _context.Folders.Include(f => f.SubFolder).ToListAsync();
            return Ok(folders);
        }

        [HttpPut]
        [Route("edit-folder/{id}")]
        public async Task<IActionResult> EditFolder(int id, [FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Folder name is required.");
            }

            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return NotFound("Folder not found.");
            }

            folder.Name = name;

            _context.Folders.Update(folder);
            await _context.SaveChangesAsync();

            //return Ok(folder);
            return Ok(new { Message = "Successfully created subfolder", Folder = folder });
        }
    }
}
