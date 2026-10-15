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

    public const string EnrollmentNotFound = "ENROLLMENT_NOT_FOUND";
    public const string EnrollmentNotFoundMessage = "הרישום לא נמצא";

    public const string ReportNotFound = "REPORT_NOT_FOUND";
    public const string ReportNotFoundMessage = "הדוח לא נמצא";

    public const string StudentNotInGroup = "STUDENT_NOT_IN_GROUP";
    public const string StudentNotInGroupMessage = "התלמידה לא רשומה לקבוצה הזו";

    public const string PaymentAboveDue = "PAYMENT_ABOVE_DUE";
    public const string PaymentAboveDueMessage = "הסכום ששולם גבוה מהסכום לתשלום";

    public const string GroupInactive = "GROUP_INACTIVE";
    public const string GroupInactiveMessage = "הקבוצה לא פעילה";

    public const string AlreadyEnrolled = "ALREADY_ENROLLED";
    public const string AlreadyEnrolledMessage = "התלמידה כבר רשומה לקבוצה הזו";

    public const string GroupFull = "GROUP_FULL";
    public const string GroupFullMessage = "הקבוצה מלאה";

    public const string SpotTaken = "SPOT_TAKEN";
    public const string SpotTakenMessage = "המקום נתפס הרגע. נא לרענן ולנסות שוב";

    public const string FutureLesson = "FUTURE_LESSON";
    public const string FutureLessonMessage = "אי אפשר לסמן נוכחות לשיעור שעוד לא התקיים";

    public const string GroupHasStudents = "GROUP_HAS_STUDENTS";
    public const string GroupHasStudentsMessage = "יש בקבוצה תלמידות רשומות. יש לבטל את הרישום שלהן קודם";
}
