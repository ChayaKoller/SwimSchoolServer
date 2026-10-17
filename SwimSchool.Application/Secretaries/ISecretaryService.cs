using SwimSchool.Application.Secretaries.Dtos;

namespace SwimSchool.Application.Secretaries;

public interface ISecretaryService
{
    Task<IReadOnlyList<SecretaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<SecretaryDto> CreateAsync(CreateSecretaryRequest request, CancellationToken ct = default);
    Task DeactivateAsync(int id, CancellationToken ct = default);
    Task SetPasswordAsync(int id, SetPasswordRequest request, CancellationToken ct = default);
}
