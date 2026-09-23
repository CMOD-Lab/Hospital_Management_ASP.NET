using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Web.Pages.Doctors;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public class DoctorPageModelsTests
{
    [Fact]
    public async Task IndexModel_OnGetAsync_LoadsItems()
    {
        var service = new Mock<IDoctorService>();
        service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<DoctorDto> { new(1, "Doc", "a@b.com", "1", "M", "MBBS", "Cardio", "Addr", 1m, 2, 3, "Dept") });
        var model = new IndexModel(service.Object);

        await model.OnGetAsync(CancellationToken.None);

        Assert.Single(model.Items);
    }

    [Fact]
    public async Task DetailsModel_OnGetAsync_SetsItem()
    {
        var service = new Mock<IDoctorService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new DoctorDto(1, "Doc", "a@b.com", "1", "M", "MBBS", "Cardio", "Addr", 1m, 2, 3, "Dept"));
        var model = new DetailsModel(service.Object);

        await model.OnGetAsync(1, CancellationToken.None);

        Assert.NotNull(model.Item);
    }

    [Fact]
    public async Task DeleteModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IDoctorService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((DoctorDto?)null);
        var model = new DeleteModel(service.Object);

        var result = await model.OnGetAsync(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteModel_OnPostAsync_Redirects()
    {
        var service = new Mock<IDoctorService>();
        var model = new DeleteModel(service.Object);

        var result = await model.OnPostAsync(1, CancellationToken.None);

        Assert.IsType<RedirectToPageResult>(result);
        service.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_InvalidModel_ReturnsPage()
    {
        var service = new Mock<IDoctorService>();
        var model = new CreateModel(service.Object);
        model.ModelState.AddModelError("x", "bad");

        var result = await model.OnPostAsync(CancellationToken.None);

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IDoctorService>();
        var model = new CreateModel(service.Object)
        {
            Input = new CreateModel.DoctorInputModel { Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Spec", Address = "Addr", ChargesPerVisit = 1m, ExperienceYears = 1, DepartmentId = 1 }
        };

        var result = await model.OnPostAsync(CancellationToken.None);

        Assert.IsType<RedirectToPageResult>(result);
        service.Verify(x => x.CreateAsync(It.IsAny<DoctorCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IDoctorService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((DoctorDto?)null);
        var model = new EditModel(service.Object);

        var result = await model.OnGetAsync(1, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EditModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IDoctorService>();
        var model = new EditModel(service.Object)
        {
            Input = new CreateModel.DoctorInputModel { Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Spec", Address = "Addr", ChargesPerVisit = 1m, ExperienceYears = 1, DepartmentId = 1 }
        };

        var result = await model.OnPostAsync(1, CancellationToken.None);

        Assert.IsType<RedirectToPageResult>(result);
        service.Verify(x => x.UpdateAsync(1, It.IsAny<DoctorUpdateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
