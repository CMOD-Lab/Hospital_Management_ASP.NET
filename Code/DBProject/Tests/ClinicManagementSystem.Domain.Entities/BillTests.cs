using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class BillTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var bill = new Bill();

        Assert.Equal(string.Empty, bill.Name);
        Assert.Null(bill.Description);
        Assert.Equal(0, bill.AppointmentId);
        Assert.Equal(0m, bill.Amount);
        Assert.False(bill.IsPaid);
        Assert.Null(bill.PaidDate);
        Assert.Null(bill.Appointment);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var appointment = new Appointment { Id = 3, Name = "Visit" };
        var paidDate = DateTime.UtcNow;
        var bill = new Bill
        {
            Name = "Bill 1",
            Description = "desc",
            AppointmentId = 3,
            Appointment = appointment,
            Amount = 99.95m,
            IsPaid = true,
            PaidDate = paidDate
        };

        Assert.Equal("Bill 1", bill.Name);
        Assert.Equal("desc", bill.Description);
        Assert.Equal(3, bill.AppointmentId);
        Assert.Same(appointment, bill.Appointment);
        Assert.Equal(99.95m, bill.Amount);
        Assert.True(bill.IsPaid);
        Assert.Equal(paidDate, bill.PaidDate);
    }
}
