namespace SwimSchool.Application.Common.Exceptions;

public static class ErrorCodes
{
    public const string StudentNotFound = "STUDENT_NOT_FOUND";
    public const string StudentNotFoundMessage = "התלמידה לא נמצאה";

    public const string GroupNotFound = "GROUP_NOT_FOUND";
    public const string GroupNotFoundMessage = "הקבוצה לא נמצאה";

    public const string LessonNotFound = "LESSON_NOT_FOUND";
    public const string LessonNotFoundMessage = "השיעור לא נמצא";

    public const string TeacherNotFound = "TEACHER_NOT_FOUND";
    public const string TeacherNotFoundMessage = "המורה לא נמצאה";

    public const string DuplicateIdentity = "DUPLICATE_IDENTITY";
    public const string DuplicateIdentityMessage = "כבר קיימת תלמידה עם תעודת הזהות הזו";

    public const string Forbidden = "FORBIDDEN";
    public const string ForbiddenMessage = "אין לך הרשאה לפעולה הזו";

    public const string SameTeacher = "SAME_TEACHER";
    public const string SameTeacherMessage = "התלמידה כבר שייכת למורה הזו";

    public const string MaxBelowEnrolled = "MAX_BELOW_ENROLLED";
    public const string MaxBelowEnrolledMessage = "יש בקבוצה יותר תלמידות מהמספר החדש";

    public const string DataChanged = "DATA_CHANGED";
    public const string DataChangedMessage = "הנתונים שונו בינתיים. נא לרענן";

    public const string LessonHasAttendance = "LESSON_HAS_ATTENDANCE";
    public const string LessonHasAttendanceMessage = "אי אפשר לשנות או למחוק שיעור שכבר סומנה לו נוכחות";
}
