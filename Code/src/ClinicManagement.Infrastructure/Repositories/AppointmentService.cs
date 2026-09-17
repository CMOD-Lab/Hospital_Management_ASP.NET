using System.Data;
using System.Data.SqlClient;
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
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("select * from deptInfo", con)
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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving department info");
        }
        return departments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        var slots = new List<Appointment>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrieveFreeSlots", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dID", SqlDbType.Int).Value = doctorId;
            cmd.Parameters.Add("@pID", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving free slots for doctor {DoctorId}", doctorId);
        }
        return slots;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        string message = string.Empty;
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            con.InfoMessage += (sender, e) => { message += "\n" + e.Message; };

            await using var cmd = new SqlCommand("insertInAppointmentTable", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dID", SqlDbType.Int).Value = doctorId;
            cmd.Parameters.Add("@pID", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@freeSlot", SqlDbType.Int).Value = freeSlot;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return (true, message);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error booking appointment for patient {PatientId} with doctor {DoctorId}", patientId, doctorId);
            return (false, ex.Message);
        }
    }
}
