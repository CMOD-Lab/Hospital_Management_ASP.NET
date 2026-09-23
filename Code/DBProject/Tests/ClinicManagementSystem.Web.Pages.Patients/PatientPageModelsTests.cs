using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using ClinicManagementSystem.Web.Pages.Patients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClinicManagementSystem.Web.Pages.Patients;

public class PatientPageModelsTests
{
    [Fact]
    public async Task IndexModel_OnGetAsync_LoadsItems()
    {
        var service = new Mock<IPatientService>();
        service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<PatientDto> { new(1, "Pat", "p@b.com", "1", DateTime.UtcNow, "F", "Addr") });
        var model = new IndexModel(service.Object);
        await model.OnGetAsync(CancellationToken.None);
        Assert.Single(model.Items);
    }

    [Fact]
    public async Task DetailsModel_OnGetAsync_SetsItem()
    {
        var service = new Mock<IPatientService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new PatientDto(1, "Pat", "p@b.com", "1", DateTime.UtcNow, "F", "Addr"));
        var model = new DetailsModel(service.Object);
        await model.OnGetAsync(1, CancellationToken.None);
        Assert.NotNull(model.Item);
    }

    [Fact]
    public async Task DeleteModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IPatientService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((PatientDto?)null);
        var model = new DeleteModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteModel_OnPostAsync_Redirects()
    {
        var service = new Mock<IPatientService>();
        var model = new DeleteModel(service.Object);
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_InvalidModel_ReturnsPage()
    {
        var service = new Mock<IPatientService>();
        var model = new CreateModel(service.Object);
        model.ModelState.AddModelError("x", "bad");
        Assert.IsType<PageResult>(await model.OnPostAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IPatientService>();
        var model = new CreateModel(service.Object)
        {
            Input = new CreateModel.PatientInputModel { Name = "Pat", Email = "p@b.com", PhoneNumber = "1", BirthDate = DateTime.UtcNow.Date, Gender = "F", Address = "Addr" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(CancellationToken.None));
        service.Verify(x => x.CreateAsync(It.IsAny<PatientCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditModel_OnGetAsync_WhenMissing_ReturnsNotFound()
    {
        var service = new Mock<IPatientService>();
        service.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((PatientDto?)null);
        var model = new EditModel(service.Object);
        Assert.IsType<NotFoundResult>(await model.OnGetAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task EditModel_OnPostAsync_ValidModel_Redirects()
    {
        var service = new Mock<IPatientService>();
        var model = new EditModel(service.Object)
        {
            Input = new CreateModel.PatientInputModel { Name = "Pat", Email = "p@b.com", PhoneNumber = "1", BirthDate = DateTime.UtcNow.Date, Gender = "F", Address = "Addr" }
        };
        Assert.IsType<RedirectToPageResult>(await model.OnPostAsync(1, CancellationToken.None));
        service.Verify(x => x.UpdateAsync(1, It.IsAny<PatientUpdateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
