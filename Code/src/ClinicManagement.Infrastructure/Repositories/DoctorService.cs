using System.Data;
using Npgsql;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Doctor service using ADO.NET stored procedures (migrated from myDAL doctor section).
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly string _connectionString;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IConfiguration configuration, ILogger<DoctorService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Doctor?> GetDoctorByIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("Doctor_Information_By_ID1", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new Doctor
                {
                    DoctorId = doctorId,
                    Name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    Phone = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DeptName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Specialization = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };
            }
            return null;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving doctor {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        var doctors = new List<Doctor>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            string sql = string.IsNullOrEmpty(searchQuery)
                ? "SELECT Doctor.DoctorID as ID, Doctor.Name, D.DeptName as Department FROM Doctor JOIN Department D ON D.DeptNo = Doctor.DeptNo WHERE Doctor.Status = 1"
                : "SELECT a.DoctorID as ID, a.Name, D.DeptName as Department FROM department D join (SELECT * FROM Doctor WHERE Doctor.Status = 1 AND Doctor.Name LIKE '%' || @DName || '%') a ON a.DeptNo = D.DeptNo";

            await using var cmd = new NpgsqlCommand(sql, con);
            if (!string.IsNullOrEmpty(searchQuery))
                cmd.Parameters.AddWithValue("@DName", searchQuery);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                doctors.Add(new Doctor
                {
                    DoctorId = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    DeptName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving all doctors");
        }
        return doctors;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        var doctors = new List<Doctor>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("RetrieveDeptDoctorInfo", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@deptName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = deptName;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                doctors.Add(new Doctor
                {
                    DoctorId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    DeptName = deptName,
                    ChargesPerVisit = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                    ReputeIndex = reader.IsDBNull(3) ? 0 : Convert.ToSingle(reader.GetValue(3))
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving doctors for department {DeptName}", deptName);
        }
        return doctors;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("PENDING_APPOINTMENTS2", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@DOCTOR_ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                appointments.Add(new Appointment
                {
                    AppointmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    PatientName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Timings = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DoctorId = doctorId
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving pending appointments for doctor {DoctorId}", doctorId);
        }
        return appointments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("TODAYS_APPOINTMENTS", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@DOC_ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                appointments.Add(new Appointment
                {
                    AppointmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    PatientName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Timings = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DoctorId = doctorId
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving today's appointments for doctor {DoctorId}", doctorId);
        }
        return appointments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var history = new List<Appointment>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("RetrievePHistory", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dId", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                history.Add(new Appointment
                {
                    AppointmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    PatientName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Timings = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Disease = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Progress = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Prescription = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DoctorId = doctorId
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving patient history for doctor {DoctorId}", doctorId);
        }
        return history;
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("APPROVE_APPOINTMENT", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@APPOINT_ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error approving appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("delete_APPOINTMENT", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@APPOINT_ID", NpgsqlTypes.NpgsqlDbType.Integer).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error deleting appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("UpdatePrescription", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;
            cmd.Parameters.Add("@appointid", NpgsqlTypes.NpgsqlDbType.Integer).Value = appointmentId;
            cmd.Parameters.Add("@Disease", NpgsqlTypes.NpgsqlDbType.Varchar).Value = disease;
            cmd.Parameters.Add("@progress", NpgsqlTypes.NpgsqlDbType.Varchar).Value = progress;
            cmd.Parameters.Add("@prescription", NpgsqlTypes.NpgsqlDbType.Varchar).Value = prescription;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error updating prescription for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Bill>> GenerateBillsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var bills = new List<Bill>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("generate_bill", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dId", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            int rowIndex = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                bills.Add(new Bill
                {
                    BillId = rowIndex++,
                    AppointmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    PatientId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    DoctorName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Timings = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Amount = reader.IsDBNull(4) ? 0 : Convert.ToDecimal(reader.GetValue(4))
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error generating bills for doctor {DoctorId}", doctorId);
        }
        return bills;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("finishedPaid", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;
            cmd.Parameters.Add("@appointid", NpgsqlTypes.NpgsqlDbType.Integer).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error marking bill paid for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("finishedUnPaid", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;
            cmd.Parameters.Add("@appointid", NpgsqlTypes.NpgsqlDbType.Integer).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error marking bill unpaid for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }
}
