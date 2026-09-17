using System.Data;
using System.Data.SqlClient;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Patient service using ADO.NET stored procedures (migrated from myDAL patient section).
/// </summary>
public class PatientService : IPatientService
{
    private readonly string _connectionString;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IConfiguration configuration, ILogger<PatientService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Patient?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrievePatientData", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@phone", SqlDbType.Char, 15).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@birthDate", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@address", SqlDbType.VarChar, 40).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@age", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@gender", SqlDbType.Char, 1).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return new Patient
            {
                PatientId = patientId,
                Name = cmd.Parameters["@name"].Value?.ToString() ?? string.Empty,
                Phone = cmd.Parameters["@phone"].Value?.ToString() ?? string.Empty,
                BirthDate = cmd.Parameters["@birthDate"].Value?.ToString() ?? string.Empty,
                Address = cmd.Parameters["@address"].Value?.ToString() ?? string.Empty,
                Gender = cmd.Parameters["@gender"].Value?.ToString() ?? string.Empty
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving patient {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Patient>> GetAllPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        var patients = new List<Patient>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            string sql = string.IsNullOrEmpty(searchQuery)
                ? "SELECT * FROM PATIENT_VIEW"
                : "SELECT Patient.PatientID, Patient.Name, Patient.Phone FROM Patient WHERE patient.name LIKE '%' + @SName + '%'";

            await using var cmd = new SqlCommand(sql, con);
            if (!string.IsNullOrEmpty(searchQuery))
                cmd.Parameters.AddWithValue("@SName", searchQuery.Trim());

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                patients.Add(new Patient
                {
                    PatientId = reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Phone = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving all patients");
        }
        return patients;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Bill>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var bills = new List<Bill>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrieveBillHistory", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@pId", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            int rowIndex = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                bills.Add(new Bill
                {
                    BillId = rowIndex++,
                    PatientId = patientId,
                    DoctorName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    Timings = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Amount = reader.IsDBNull(2) ? 0 : Convert.ToDecimal(reader.GetValue(2)),
                    IsPaid = !reader.IsDBNull(3) && Convert.ToBoolean(reader.GetValue(3))
                });
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving bill history for patient {PatientId}", patientId);
        }
        return bills;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TreatmentHistory>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var history = new List<TreatmentHistory>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrieveTreatmentHistory", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@pId", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            int rowIndex = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                history.Add(new TreatmentHistory
                {
                    HistoryId = rowIndex++,
                    PatientId = patientId,
                    DoctorName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    Timings = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Disease = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Progress = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Prescription = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving treatment history for patient {PatientId}", patientId);
        }
        return history;
    }

    /// <inheritdoc/>
    public async Task<Appointment?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrieveCurrentAppointment", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@pid", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int count = (int)cmd.Parameters["@count"].Value;
            if (count == 0) return null;

            return new Appointment
            {
                PatientId = patientId,
                DoctorName = cmd.Parameters["@dName"].Value?.ToString() ?? string.Empty,
                Timings = cmd.Parameters["@timings"].Value?.ToString() ?? string.Empty
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving current appointment for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var notifications = new List<Appointment>();
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrievePatientNotifications", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@pId", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int count = (int)cmd.Parameters["@count"].Value;
            if (count > 0)
            {
                notifications.Add(new Appointment
                {
                    PatientId = patientId,
                    DoctorName = cmd.Parameters["@dName"].Value?.ToString() ?? string.Empty,
                    Timings = cmd.Parameters["@timings"].Value?.ToString() ?? string.Empty
                });
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving notifications for patient {PatientId}", patientId);
        }
        return notifications;
    }

    /// <inheritdoc/>
    public async Task<(bool HasPending, Appointment? PendingAppointment)> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("RetrievePendingFeedback", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@pId", SqlDbType.Int).Value = patientId;
            cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@aID", SqlDbType.Int).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int count = (int)cmd.Parameters["@count"].Value;
            if (count == 0) return (false, null);

            var appointment = new Appointment
            {
                AppointmentId = (int)cmd.Parameters["@aID"].Value,
                PatientId = patientId,
                DoctorName = cmd.Parameters["@dName"].Value?.ToString() ?? string.Empty,
                Timings = cmd.Parameters["@timings"].Value?.ToString() ?? string.Empty
            };
            return (true, appointment);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error retrieving pending feedback for patient {PatientId}", patientId);
            return (false, null);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("storeFeedback", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@aId", SqlDbType.Int).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error submitting feedback for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }
}
