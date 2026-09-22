using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default);
}
