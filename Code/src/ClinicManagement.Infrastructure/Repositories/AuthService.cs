using System.Data;
using Npgsql;
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
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("Login", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = email;
            cmd.Parameters.Add("@password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = password;
            cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@ID", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@type", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int status = (int)cmd.Parameters["@status"].Value!;
            int type = (int)cmd.Parameters["@type"].Value!;
            int id = (int)cmd.Parameters["@ID"].Value!;

            return (status, (UserType)type, id);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error during login for email {Email}", email);
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
            await using var con = new NpgsqlConnection(_connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("PatientSignup", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = name;
            cmd.Parameters.Add("@address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = address;
            cmd.Parameters.Add("@gender", NpgsqlTypes.NpgsqlDbType.Varchar).Value = gender;
            cmd.Parameters.Add("@date", NpgsqlTypes.NpgsqlDbType.Date).Value = DateOnly.Parse(birthDate);
            cmd.Parameters.Add("@email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = email;
            cmd.Parameters.Add("@password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = password;
            cmd.Parameters.Add("@phone", NpgsqlTypes.NpgsqlDbType.Char).Value = phone;
            cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@ID", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            int status = (int)cmd.Parameters["@status"].Value!;
            int id = status != 0 ? (int)cmd.Parameters["@ID"].Value! : 0;

            return (status, id);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "PostgreSQL error during patient registration for email {Email}", email);
            return (-1, 0);
        }
    }
}
