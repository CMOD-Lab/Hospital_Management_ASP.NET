using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Web.Pages.Departments;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClinicManagementSystem.Web.Pages.Departments;

public class DepartmentPageModelsTests
{
    [Fact]
    public async Task IndexModel_OnGetAsync_LoadsItems()
    {
        var service = new Mock<IDepartmentService>();
        service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<DepartmentDto> { new(1, "Dept", "Desc") });
        var model = new IndexModel(service.Object);
        await model.OnGetAsync(CancellationToken.None);
        Assert.Single(model.Items);
    }

    [Fact]
    public async Task DetailsModel_OnGetAsync_SetsItem()
    {
        var service = new Mock<IDepartmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new DepartmentDto(1, "Dept", "Desc"));
        var model = new DetailsModel(service.Object);
        await model.OnGetAsync(1, CancellationToken.None);
        Assert.NotNull(model.Item);
    }

    [Fact]
    public async Task DeleteModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IDepartmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((DepartmentDto?)null);
        var model = new DeleteModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteModel_OnPostAsync_Redirects()
    {
        var service = new Mock<IDepartmentService>();
        var model = new DeleteModel(service.Object);
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_InvalidModel_ReturnsPage()
    {
        var service = new Mock<IDepartmentService>();
        var model = new CreateModel(service.Object);
        model.ModelState.AddModelError("x", "bad");
        Assert.IsType<PageResult>(await model.OnPostAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IDepartmentService>();
        var model = new CreateModel(service.Object)
        {
            Input = new CreateModel.DepartmentInputModel { Name = "Dept", Description = "Desc" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(CancellationToken.None));
        service.Verify(x => x.CreateAsync(It.IsAny<DepartmentCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IDepartmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((DepartmentDto?)null);
        var model = new EditModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task EditModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IDepartmentService>();
        var model = new EditModel(service.Object)
        {
            Input = new CreateModel.DepartmentInputModel { Name = "Dept", Description = "Desc" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.UpdateAsync(1, It.IsAny<DepartmentUpdateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
