using System.Data;
using Npgsql;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Appointment service using ADO.NET stored procedures (migrated from myDAL appointment section).
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly string _connectionString;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IConfiguration configuration, ILogger<AppointmentService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = new List<Department>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("select * from deptInfo", con)
            {
                CommandType = CommandType.Text
            };

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                departments.Add(new Department
                {
                    DeptNo = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    DeptName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving department info");
        }
        return departments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        var slots = new List<Appointment>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("RetrieveFreeSlots", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dID", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;
            cmd.Parameters.Add("@pID", NpgsqlTypes.NpgsqlDbType.Integer).Value = patientId;
            cmd.Parameters.Add("@count", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            int rowIndex = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                slots.Add(new Appointment
                {
                    AppointmentId = rowIndex++,
                    DoctorId = doctorId,
                    PatientId = patientId,
                    Timings = reader.IsDBNull(0) ? string.Empty : reader.GetString(0)
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving free slots for doctor {DoctorId}", doctorId);
        }
        return slots;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        string message = string.Empty;
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("insertInAppointmentTable", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dID", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;
            cmd.Parameters.Add("@pID", NpgsqlTypes.NpgsqlDbType.Integer).Value = patientId;
            cmd.Parameters.Add("@freeSlot", NpgsqlTypes.NpgsqlDbType.Integer).Value = freeSlot;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return (true, message);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error booking appointment for patient {PatientId} with doctor {DoctorId}", patientId, doctorId);
            return (false, ex.Message);
        }
    }
}
