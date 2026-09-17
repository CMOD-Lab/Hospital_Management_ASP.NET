using System.Data;
using System.Data.SqlClient;
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
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("Doctor_Information_By_ID1", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = doctorId;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving doctor {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        var doctors = new List<Doctor>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            string sql = string.IsNullOrEmpty(searchQuery)
                ? "SELECT Doctor.DoctorID as ID, Doctor.Name, D.DeptName as Department FROM Doctor JOIN Department D ON D.DeptNo = Doctor.DeptNo WHERE Doctor.Status = 1"
                : "SELECT a.DoctorID as ID, a.Name, D.DeptName as Department FROM department D join (SELECT * FROM Doctor WHERE Doctor.Status = 1 AND Doctor.Name like '%' + @DName + '%') a ON a.DeptNo = D.DeptNo";

            await using var cmd = new SqlCommand(sql, con);
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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving all doctors");
        }
        return doctors;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        var doctors = new List<Doctor>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrieveDeptDoctorInfo", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@deptName", SqlDbType.VarChar, 30).Value = deptName;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving doctors for department {DeptName}", deptName);
        }
        return doctors;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("PENDING_APPOINTMENTS2", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@DOCTOR_ID", SqlDbType.Int).Value = doctorId;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving pending appointments for doctor {DoctorId}", doctorId);
        }
        return appointments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("TODAYS_APPOINTMENTS", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@DOC_ID", SqlDbType.Int).Value = doctorId;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving today's appointments for doctor {DoctorId}", doctorId);
        }
        return appointments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var history = new List<Appointment>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrievePHistory", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dId", SqlDbType.Int).Value = doctorId;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving patient history for doctor {DoctorId}", doctorId);
        }
        return history;
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("APPROVE_APPOINTMENT", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error approving appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("delete_APPOINTMENT", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error deleting appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("UpdatePrescription", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", SqlDbType.Int).Value = doctorId;
            cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appointmentId;
            cmd.Parameters.Add("@Disease", SqlDbType.VarChar, 30).Value = disease;
            cmd.Parameters.Add("@progress", SqlDbType.VarChar, 50).Value = progress;
            cmd.Parameters.Add("@prescription", SqlDbType.VarChar, 60).Value = prescription;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error updating prescription for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Bill>> GenerateBillsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        var bills = new List<Bill>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("generate_bill", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@dId", SqlDbType.Int).Value = doctorId;

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
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error generating bills for doctor {DoctorId}", doctorId);
        }
        return bills;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("finishedPaid", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", SqlDbType.Int).Value = doctorId;
            cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error marking bill paid for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("finishedUnPaid", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@docId", SqlDbType.Int).Value = doctorId;
            cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error marking bill unpaid for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }
}
