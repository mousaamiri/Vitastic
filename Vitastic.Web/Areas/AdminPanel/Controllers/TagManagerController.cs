using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Tag;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class TagManagerController(ITagManagerService tagService) : AdminController
{
    #region Index
    [HttpGet]
    public async Task<IActionResult> Index(string? searchQuery, int pageNumber = 1, int take = 10)
    {
        var response = await tagService.SearchAsync(searchQuery, pageNumber, take);

        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message;
            return View(new PaginatedData<TagDto>()); // مدل خالی برای جلوگیری از null
        }

        return View(response.Data);
    }
    #endregion

    #region Add
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] AddTagRequest dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "داده‌های ورودی نامعتبر است" });
        }

        var response = await tagService.CreateTagAsync(dto.Name);

        if (!response.IsSuccess)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(new { message = "برچسب با موفقیت اضافه شد", data = response.Data });
    }
    #endregion

    #region Edit
    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagRequest dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "داده‌های ورودی نامعتبر است" });
        }

        var response = await tagService.updateTagAsyncTask(id, dto.Name);

        if (!response.IsSuccess)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(new { message = "برچسب با موفقیت ویرایش شد", data = response });
    }
    #endregion
    #region Delete
    [HttpDelete]
    [Route("Delete/{id:guid}")]  // DELETE: /AdminPanel/TagManager/Delete/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await tagService.DeleteTagAsync(id);

        if (!response.IsSuccess)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(new { message = "برچسب با موفقیت حذف شد" });
    }
    #endregion
}
public class AddTagRequest
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateTagRequest
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
