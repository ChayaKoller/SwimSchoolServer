using SwimSchool.Application.Attendance.Dtos;

namespace SwimSchool.Application.Attendance;

public interface IAttendanceService
{
    Task<LessonAttendanceDto> GetLessonAsync(int lessonId, CancellationToken ct = default);
    Task UpdateLessonAsync(UpdateAttendanceRequest request, CancellationToken ct = default);
    Task<GroupAttendanceGridDto> GetGridAsync(int groupId, CancellationToken ct = default);
    Task UpdateGridAsync(int groupId, UpdateGroupAttendanceRequest request, CancellationToken ct = default);
}
