using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Domain.Interfaces.Services;

public interface IStaffMemberService
{
    Task<IReadOnlyList<StaffMemberDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StaffMemberDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StaffMemberDto> CreateAsync(StaffMemberCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, StaffMemberUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffMemberDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
