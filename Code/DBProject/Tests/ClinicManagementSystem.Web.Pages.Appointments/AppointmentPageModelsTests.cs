using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Web.Pages.Appointments;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClinicManagementSystem.Web.Pages.Appointments;

public class AppointmentPageModelsTests
{
    [Fact]
    public async Task IndexModel_OnGetAsync_LoadsItems()
    {
        var service = new Mock<IAppointmentService>();
        service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AppointmentDto> { new(1, "Visit", 1, "Pat", 2, "Doc", DateTime.UtcNow, "Pending", null, null, null) });
        var model = new IndexModel(service.Object);
        await model.OnGetAsync(CancellationToken.None);
        Assert.Single(model.Items);
    }

    [Fact]
    public async Task DetailsModel_OnGetAsync_SetsItem()
    {
        var service = new Mock<IAppointmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new AppointmentDto(1, "Visit", 1, "Pat", 2, "Doc", DateTime.UtcNow, "Pending", null, null, null));
        var model = new DetailsModel(service.Object);
        await model.OnGetAsync(1, CancellationToken.None);
        Assert.NotNull(model.Item);
    }

    [Fact]
    public async Task DeleteModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IAppointmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((AppointmentDto?)null);
        var model = new DeleteModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteModel_OnPostAsync_Redirects()
    {
        var service = new Mock<IAppointmentService>();
        var model = new DeleteModel(service.Object);
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_InvalidModel_ReturnsPage()
    {
        var service = new Mock<IAppointmentService>();
        var model = new CreateModel(service.Object);
        model.ModelState.AddModelError("x", "bad");
        Assert.IsType<PageResult>(await model.OnPostAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IAppointmentService>();
        var model = new CreateModel(service.Object)
        {
            Input = new CreateModel.AppointmentInputModel { Name = "Visit", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Pending" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(CancellationToken.None));
        service.Verify(x => x.CreateAsync(It.IsAny<AppointmentCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IAppointmentService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((AppointmentDto?)null);
        var model = new EditModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task EditModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IAppointmentService>();
        var model = new EditModel(service.Object)
        {
            Input = new CreateModel.AppointmentInputModel { Name = "Visit", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Pending" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.UpdateAsync(1, It.IsAny<AppointmentUpdateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
