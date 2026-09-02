using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the appointment repository.
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.AppointmentId == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Pending)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId
                && a.Status == AppointmentStatus.Approved
                && a.AppointmentDate.HasValue
                && a.AppointmentDate.Value.Date == today)
            .OrderBy(a => a.Timings)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId
                && a.Status == AppointmentStatus.Approved
                && a.AppointmentDate.HasValue
                && a.AppointmentDate.Value.Date == today)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Completed)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Completed)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId
                && a.Status == AppointmentStatus.Completed
                && a.FeedbackStatus == 2)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Appointment?> GetNotificationByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId
                && a.Status == AppointmentStatus.Approved
                && a.PatientNotification == 2)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAndPatientAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        // Return available time slots - appointments that are not yet booked for this doctor
        var bookedSlots = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Approved))
            .Select(a => a.Timings)
            .ToListAsync(cancellationToken);

        // Return empty list - actual slot logic depends on business rules
        return Enumerable.Empty<Appointment>();
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .AnyAsync(a => a.AppointmentId == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetHistoryByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Completed)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }
}
