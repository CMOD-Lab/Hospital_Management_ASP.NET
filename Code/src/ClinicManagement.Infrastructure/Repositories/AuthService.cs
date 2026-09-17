using System.Data;
using System.Data.SqlClient;
using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// Authentication service using ADO.NET stored procedures (migrated from myDAL).
/// Replaces System.Web session with return values for the web layer to handle.
/// </summary>
public class AuthService : IAuthService
{
    private readonly string _connectionString;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<(int Status, UserType UserType, int UserId)> ValidateLoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("Login", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@email", SqlDbType.VarChar, 30).Value = email;
            cmd.Parameters.Add("@password", SqlDbType.VarChar, 20).Value = password;
            cmd.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@type", SqlDbType.Int).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int status = (int)cmd.Parameters["@status"].Value;
            int type = (int)cmd.Parameters["@type"].Value;
            int id = (int)cmd.Parameters["@ID"].Value;

            return (status, (UserType)type, id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error during login for email {Email}", email);
            return (-1, UserType.Patient, 0);
        }
    }

    /// <inheritdoc/>
    public async Task<(int Status, int PatientId)> RegisterPatientAsync(
        string name, string birthDate, string email, string password,
        string phone, string gender, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand("PatientSignup", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@name", SqlDbType.VarChar, 20).Value = name;
            cmd.Parameters.Add("@address", SqlDbType.VarChar, 40).Value = address;
            cmd.Parameters.Add("@gender", SqlDbType.VarChar, 1).Value = gender;
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = birthDate;
            cmd.Parameters.Add("@email", SqlDbType.VarChar, 30).Value = email;
            cmd.Parameters.Add("@password", SqlDbType.VarChar, 20).Value = password;
            cmd.Parameters.Add("@phone", SqlDbType.Char, 15).Value = phone;
            cmd.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int status = (int)cmd.Parameters["@status"].Value;
            int id = status != 0 ? (int)cmd.Parameters["@ID"].Value : 0;

            return (status, id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error during patient registration for email {Email}", email);
            return (-1, 0);
        }
    }
}
