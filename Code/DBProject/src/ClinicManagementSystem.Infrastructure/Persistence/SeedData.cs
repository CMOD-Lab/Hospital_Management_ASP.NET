using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagementSystem.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ClinicDbContext>();
        await context.Database.EnsureCreatedAsync();

        if (await context.Departments.AnyAsync())
        {
            return;
        }

        var departments = new[]
        {
            new Department { Name = "Cardiology", Description = "Heart care", CreatedBy = "seed" },
            new Department { Name = "Dermatology", Description = "Skin care", CreatedBy = "seed" },
            new Department { Name = "Neurology", Description = "Brain and nerves", CreatedBy = "seed" }
        };

        await context.Departments.AddRangeAsync(departments);

        var doctor = new Doctor
        {
            Name = "Dr. Sarah Ahmed",
            Email = "doctor@clinic.local",
            PhoneNumber = "1234567890",
            Gender = "F",
            Address = "Main Street",
            BirthDate = new DateTime(1985, 5, 1),
            Qualification = "MBBS",
            Specialization = "Cardiology",
            YearsOfExperience = 10,
            Salary = 9000,
            ChargesPerVisit = 150,
            Department = departments[0],
            CreatedBy = "seed"
        };

        var patient = new Patient
        {
            Name = "John Doe",
            Email = "patient@clinic.local",
            PhoneNumber = "5550001234",
            Gender = "M",
            Address = "Elm Street",
            BirthDate = new DateTime(1995, 8, 20),
            CreatedBy = "seed"
        };

        var staff = new StaffMember
        {
            Name = "Reception User",
            PhoneNumber = "5551112222",
            Gender = "F",
            Address = "Clinic Reception",
            BirthDate = new DateTime(1990, 2, 10),
            Qualification = "BBA",
            Designation = "Receptionist",
            Salary = 3000,
            CreatedBy = "seed"
        };

        await context.Doctors.AddAsync(doctor);
        await context.Patients.AddAsync(patient);
        await context.StaffMembers.AddAsync(staff);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            DoctorId = doctor.Id,
            PatientId = patient.Id,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Status = "Pending",
            Notes = "Initial consultation",
            CreatedBy = "seed"
        };

        await context.Appointments.AddAsync(appointment);
        await context.SaveChangesAsync();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync("admin@clinic.local") is null)
        {
            var user = new ApplicationUser { UserName = "admin@clinic.local", Email = "admin@clinic.local", EmailConfirmed = true };
            await userManager.CreateAsync(user, "Admin123!");
        }
    }
}
