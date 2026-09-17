using System.Data;
using Npgsql;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Admin service using ADO.NET stored procedures (migrated from myDAL admin section).
/// </summary>
public class AdminService : IAdminService
{
    private readonly string _connectionString;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IConfiguration configuration, ILogger<AdminService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<(int TotalDoctors, int TotalPatients, decimal TotalIncome)> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            int totalDoctors = 0, totalPatients = 0;
            decimal totalIncome = 0;

            await using (var cmd = new NpgsqlCommand("SELECT * FROM Total_Patient", con))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    totalDoctors = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
            }

            await using (var cmd = new NpgsqlCommand("SELECT * FROM Total_Doctors", con))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    totalPatients = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
            }

            await using (var cmd = new NpgsqlCommand("SELECT * FROM Income", con))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    totalIncome = reader.IsDBNull(0) ? 0 : Convert.ToDecimal(reader.GetValue(0));
            }

            return (totalDoctors, totalPatients, totalIncome);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving dashboard stats");
            return (0, 0, 0);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = new List<Department>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("SELECT * FROM Department_View", con);
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
            _logger.LogError(ex, "PostgreSQL error retrieving departments");
        }
        return departments;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("SELECT * FROM Appointment_view", con);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                appointments.Add(new Appointment
                {
                    AppointmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    DoctorName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    PatientName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Timings = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving all appointments");
        }
        return appointments;
    }

    /// <inheritdoc/>
    public async Task<bool> AddDoctorAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("AddDoctor", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Name;
            cmd.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Email;
            cmd.Parameters.Add("@Password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Password;
            cmd.Parameters.Add("@BirthDate", NpgsqlTypes.NpgsqlDbType.Date).Value = doctor.BirthDate;
            cmd.Parameters.Add("@dept", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.DeptNo;
            cmd.Parameters.Add("@gender", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Gender;
            cmd.Parameters.Add("@Address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Address;
            cmd.Parameters.Add("@Exp", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Experience;
            cmd.Parameters.Add("@Salary", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Salary;
            cmd.Parameters.Add("@charges", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.ChargesPerVisit;
            cmd.Parameters.Add("@phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Phone;
            cmd.Parameters.Add("@spec", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Specialization;
            cmd.Parameters.Add("@qual", NpgsqlTypes.NpgsqlDbType.Varchar).Value = doctor.Qualification;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error adding doctor {DoctorName}", doctor.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("DeleteDoctor", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = doctorId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error deleting doctor {DoctorId}", doctorId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddStaffAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("AddStaff", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Name;
            cmd.Parameters.Add("@BirthDate", NpgsqlTypes.NpgsqlDbType.Date).Value = staff.BirthDate;
            cmd.Parameters.Add("@Phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Phone;
            cmd.Parameters.Add("@gender", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Gender;
            cmd.Parameters.Add("@salary", NpgsqlTypes.NpgsqlDbType.Integer).Value = staff.Salary;
            cmd.Parameters.Add("@Designation", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Designation;
            cmd.Parameters.Add("@Qualification", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Qualification;
            cmd.Parameters.Add("@Address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = staff.Address;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error adding staff {StaffName}", staff.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("DELETESTAFF", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = staffId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error deleting staff {StaffId}", staffId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Staff>> GetAllStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        var staffList = new List<Staff>();
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            string sql = string.IsNullOrEmpty(searchQuery)
                ? "SELECT * FROM STAFF_VIEW"
                : "SELECT StaffID as ID, Name, Designation FROM OtherStaff WHERE Name LIKE '%' || @pName || '%'";

            await using var cmd = new NpgsqlCommand(sql, con);
            if (!string.IsNullOrEmpty(searchQuery))
                cmd.Parameters.AddWithValue("@pName", searchQuery.Trim());

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                staffList.Add(new Staff
                {
                    StaffId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Designation = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error retrieving all staff");
        }
        return staffList;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckDoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("CheckDoctorEmail", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = email;
            cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            return (int)cmd.Parameters["@status"].Value! == 1;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error checking doctor email {Email}", email);
            return false;
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
            _logger.LogError(ex, "PostgreSQL error retrieving all doctors in admin service");
        }
        return doctors;
    }
}
