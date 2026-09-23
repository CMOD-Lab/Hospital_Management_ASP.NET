using ClinicManagementSystem.Web.Pages;
using Xunit;

namespace ClinicManagementSystem.Web.Pages;

public class IndexModelTests
{
    [Fact]
    public void OnGet_DoesNotThrow()
    {
        var model = new IndexModel();
        model.OnGet();
        Assert.NotNull(model);
    }
}
