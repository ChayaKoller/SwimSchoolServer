# Error codes

| Code | HTTP | Message |
|---|---|---|
| STUDENT_NOT_FOUND | 404 | התלמידה לא נמצאה |
| GROUP_NOT_FOUND | 404 | הקבוצה לא נמצאה |
| LESSON_NOT_FOUND | 404 | השיעור לא נמצא |
| TEACHER_NOT_FOUND | 404 | המורה לא נמצאה |
| ENROLLMENT_NOT_FOUND | 404 | הרישום לא נמצא |
| REPORT_NOT_FOUND | 404 | הדוח לא נמצא |
| FORBIDDEN | 403 | אין לך הרשאה לפעולה הזו |
| DUPLICATE_IDENTITY | 409 | כבר קיימת תלמידה עם תעודת הזהות הזו |
| DATA_CHANGED | 409 | הנתונים שונו בינתיים. נא לרענן |
| GROUP_INACTIVE | 409 | הקבוצה לא פעילה |
| ALREADY_ENROLLED | 409 | התלמידה כבר רשומה לקבוצה הזו |
| GROUP_FULL | 409 | הקבוצה מלאה |
| SPOT_TAKEN | 409 | המקום נתפס הרגע. נא לרענן ולנסות שוב |
| GROUP_HAS_STUDENTS | 409 | יש בקבוצה תלמידות רשומות. יש לבטל את הרישום שלהן קודם |
| DOMAIN_RULE | 409 | הפעולה אינה אפשרית במצב הנוכחי |
| SAME_TEACHER | 400 | התלמידה כבר שייכת למורה הזו |
| MAX_BELOW_ENROLLED | 400 | יש בקבוצה יותר תלמידות מהמספר החדש |
| LESSON_HAS_ATTENDANCE | 400 | אי אפשר לשנות או למחוק שיעור שכבר סומנה לו נוכחות |
| STUDENT_NOT_IN_GROUP | 400 | התלמידה לא רשומה לקבוצה הזו |
| PAYMENT_ABOVE_DUE | 400 | הסכום ששולם גבוה מהסכום לתשלום |
| FUTURE_LESSON | 400 | אי אפשר לסמן נוכחות לשיעור שעוד לא התקיים |
| VALIDATION_FAILED | 400 | יש שדות שלא מולאו נכון |
| SERVER_ERROR | 500 | אירעה שגיאה. נסי שוב מאוחר יותר |
