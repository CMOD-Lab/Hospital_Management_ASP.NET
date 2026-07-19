using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>Appointment repository implementation</summary>
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
        try
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment with ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Pending)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today && a.Status == AppointmentStatus.Approved)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.AppointmentDate.Date == today
                    && a.Status == AppointmentStatus.Approved, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Return available time slots (appointments not yet booked for today)
            var today = DateTime.UtcNow.Date;
            var bookedSlots = await _context.Appointments.AsNoTracking()
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today
                    && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.FreeSlot)
                .ToListAsync(cancellationToken);

            // Generate available slots (1-10 for demo)
            var allSlots = Enumerable.Range(1, 10)
                .Where(s => !bookedSlots.Contains(s))
                .Select(s => new Appointment
                {
                    FreeSlot = s,
                    Timings = $"{8 + s}:00 - {9 + s}:00",
                    DoctorId = doctorId
                });

            return allSlots;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding appointment");
            throw;
        }
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment with ID: {Id}", appointment.AppointmentId);
            throw;
        }
    }

    public async Task<bool> ApproveAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, cancellationToken);
            if (appointment == null) return false;
            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, cancellationToken);
            if (appointment == null) return false;
            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<Appointment?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.Status == AppointmentStatus.Completed
                    && !a.FeedbackGiven, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, cancellationToken);
            if (appointment == null) return false;
            appointment.FeedbackGiven = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.CountAsync(cancellationToken);
    }
}
