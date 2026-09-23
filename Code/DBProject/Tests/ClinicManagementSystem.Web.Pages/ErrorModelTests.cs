using ClinicManagementSystem.Web.Pages;
using Xunit;

namespace ClinicManagementSystem.Web.Pages;

public class ErrorModelTests
{
    [Fact]
    public void OnGet_DoesNotThrow()
    {
        var model = new ErrorModel();
        model.OnGet();
        Assert.NotNull(model);
    }
}
