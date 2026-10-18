# SwimSchool API

שרת ASP.NET Core 8 Web API לניהול בית ספר לשחייה: רישום תלמידות לקבוצות, נוכחות בשיעורים, תשלומים והפקת דוחות טיפולים (PDF בעברית) לקופות החולים ולחברות הביטוח.

המערכת נבנתה עבור לקוחה אמיתית ומוכנה לפריסה בענן (Docker + Render), מעבר לדרישת ההרצה המקומית של הפרויקט.

> המסמך כתוב בעברית, והמונחים הטכניים נשארו באנגלית.
תודה ענקית למורה נהנתי ולמדתי המון מהקורס!!!!
תודה על המאור והסבלנות תמיד
מעריכה
 אשמ אם המורה תבדוק לי בסוף כי המערכת אמיתית והעבודה על צד הלקוח נעשית לפרויקט בריאקט 
לכן כרגע צד הלקוח לא מיצג בכלל את היקף הפרויקט הוא מינמלי ולא מראה מספיק על המערכת שאמורה לקום.
---

## תיאור המערכת

במערכת שני סוגי משתמשים: **מורה** ו**מזכירה**. כל מזכירה שייכת למורה אחת. מערכת אחת משרתת כמה מורות, אבל **כל מורה רואה רק את הנתונים שלה**, ומזכירה רואה רק את הנתונים של המורה שלה. הבידוד נאכף בשרת: כל שאילתה מסוננת לפי `OwningTeacherId` שנלקח מה-JWT, ונתון של מורה אחרת מחזיר 404 (כדי לא לחשוף שהוא קיים).

| יכולת | מורה | מזכירה |
|---|:---:|:---:|
| תלמידות: רישום, עדכון, צפייה מסוננת | ✓ | ✓ |
| קבוצות: יצירה, עדכון, צפייה מסוננת | ✓ | ✓ |
| רישום תלמידה לקבוצה, ביטול, עדכון תשלום | ✓ | ✓ |
| דוחות טיפולים: יצירה, עדכון, הורדת PDF | ✓ | ✓ |
| נוכחות: לפי שיעור ובטבלת נוכחות מלאה לקבוצה | ✓ | |
| שיעורים: לוח השבוע, הוספה, הזזה, מחיקה | ✓ | |
| מחיקה בטוחה של תלמידה או קבוצה | ✓ | |
| העברת תלמידה למורה אחרת | ✓ | |
| ניהול מזכירות, הגדרות ברירת מחדל לדוחות | ✓ | |

---

## המשאב המוגבל והטיפול בתחרות

**המשאב:** מקום בקבוצה. לכל קבוצה יש `MaxStudents` ומונה `EnrolledCount`.

**איפה התחרות:** מורה והמזכירה שלה עובדות במקביל, ושתיהן יכולות לרשום תלמידה (כל אחת תלמידה אחרת) למקום האחרון באותו רגע. שתיהן קוראות שיש מקום פנוי, ושתיהן היו מקבלות אישור. את זה אסור שיקרה.

**איך המערכת מגיבה:** optimistic concurrency.
- `Group.Version` ממופה לעמודת המערכת `xmin` של PostgreSQL (`IsRowVersion()` ב-`GroupConfiguration`), ולכן נכנס לתנאי ה-`WHERE` של כל `UPDATE` על הקבוצה.
- `EnrollmentService.EnrollAsync` בודק מקום פנוי, מגדיל את המונה (`Group.ReserveSpot()`) ושומר הכול ב-`SaveChangesAsync` אחד, כלומר באותה טרנזקציה.
- אם מישהו שינה את הקבוצה בינתיים, EF Core זורק `DbUpdateConcurrencyException`. ה-`UnitOfWork` בשכבת Infrastructure תופס **רק** את החריגה הזו והופך אותה ל-`ConflictException`, כך ששכבת Application לא מכירה את EF Core.
- השירות מחזיר `409 Conflict` עם הקוד `SPOT_TAKEN` וההודעה "המקום נתפס הרגע. נא לרענן ולנסות שוב". קבוצה שכבר מלאה מחזירה `409` עם `GROUP_FULL`.
- שכבת הגנה נוספת בבסיס הנתונים: `CHECK` constraint שמונע `EnrolledCount > MaxStudents`, ואינדקס ייחודי חלקי (`WHERE "IsActive"`) שמונע רישום פעיל כפול של אותה תלמידה לאותה קבוצה, ועדיין מאפשר רישום מחדש אחרי ביטול.

**ההוכחה:** `tests/SwimSchool.Infrastructure.Tests/Concurrency/LastSpotConcurrencyTests.cs` רץ מול PostgreSQL אמיתי (Testcontainers):
1. שני `DbContext` נפרדים קוראים את אותה קבוצה ומשנים אותה. הראשון שומר, השני מקבל `DbUpdateConcurrencyException`.
2. אותו מקרה דרך `UnitOfWork` מחזיר `DATA_CHANGED`.
3. מורה ומזכירה רושמות יחד למקום האחרון: אחת מצליחה, השנייה מקבלת `SPOT_TAKEN`.
4. מרוץ אמיתי: 8 משתמשים לוחצים "רישום" באותו רגע על 3 מקומות. הקבוצה אף פעם לא עוברת את המכסה, והמונה תמיד תואם את מספר הרישומים בפועל.

---

## עמידה בדרישות

### 1. Web API & REST
- שמות משאבים ברבים ופעולות דרך HTTP methods: `api/students`, `api/groups`, `api/enrollments`, `api/reports`, `api/secretaries` ועוד, עם משאבים מקוננים כמו `api/groups/{groupId}/lessons` ו-`api/students/{studentId}/reports`.
- סטטוסים: `200`, `201` עם `Location` (`CreatedAtAction` ביצירת תלמידה, קבוצה, רישום ודוח), `204`, `400`, `401`, `403`, `404`, `409`, וגם `429` להגבלת ניסיונות התחברות. כל endpoint מתעד את הסטטוסים שלו ב-`ProducesResponseType`.
- Swagger פעיל בסביבת Development, כולל כפתור Authorize ל-JWT.

### 2. Model Binding & Validation
- `[FromRoute]`, `[FromQuery]` (סינון ו-pagination, וגם `version` במחיקת קבוצה) ו-`[FromBody]` לפי ההקשר.
- Data Annotations על ה-DTOs, כולל attributes מותאמים: `StrongPassword`, `PastDate`, `NoFutureDates`, `NoDuplicates`.
- שגיאות ModelState חוזרות בפורמט האחיד של המערכת (`ValidationResponseFactory`).
- ולידציה עסקית שתלויה במצב הנתונים (קבוצה מלאה, תשלום מעל הסכום, שיעור שכבר סומנה בו נוכחות) נמצאת בשכבת ה-Service בלבד.

### 3. ארכיטקטורה בשכבות
הפרויקט בנוי ב-Clean Architecture: ארבעה פרויקטים ב-solution אחד, והתלויות מצביעות פנימה בלבד, לכיוון ה-Domain.

| שכבה | תוכן | תלויה ב |
|---|---|---|
| `SwimSchool.Domain` | Entities, Enums, כללים עסקיים של ישות בודדת (למשל `Group.ReserveSpot()`) | **כלום: אפס תלויות ואפס חבילות NuGet** |
| `SwimSchool.Application` | Services, DTOs, Mappers, Interfaces (repositories, `IUnitOfWork`, `ICurrentUser`, `IPasswordHasher`, `IReportPdfGenerator`), חריגות עסקיות | Domain |
| `SwimSchool.Infrastructure` | `DbContext`, Configurations, Migrations, Repositories, JWT, hashing, PDF | Application |
| `SwimSchool.API` | Controllers דקים, Middleware, CLI, `Program.cs` | Application, ו-Infrastructure לרישום השירותים בלבד |

- API מכיר את Infrastructure רק דרך `AddInfrastructure(...)` ב-`Program.cs`.
- אין EF Core ב-Domain או ב-Application, ואף Controller לא מזריק `DbContext`.
- **הכללים האלה נאכפים בבדיקות** (`tests/SwimSchool.Architecture.Tests`, NetArchTest): Domain לא תלוי בשום שכבה או framework, Application לא תלוי ב-Infrastructure או ב-API, Infrastructure לא תלוי ב-API, ו-Controllers משתמשים רק בשירותי Application. שבירה של הארכיטקטורה מפילה את הבדיקות.
- גרסאות החבילות מנוהלות במקום אחד: `Directory.Packages.props` (Central Package Management).

### 4. Dependency Injection
- כל התלויות מוזרקות ב-constructor (primary constructors), בלי `new` על service.
- רישום מרוכז: `AddApplication()` ו-`AddInfrastructure(...)`.
- Lifetimes מודעים: `DbContext`, repositories, `UnitOfWork` ו-services הם Scoped. Singleton נשמר רק לרכיבים חסרי מצב (`IDateTimeProvider`, `IPasswordHasher`, `IJwtTokenGenerator`, מחולל ה-PDF), שאינם מחזיקים שום רכיב Scoped. singleton שמחזיק `DbContext` היה משתף חיבור אחד בין בקשות, ולכן זה באג.

### 5. EF Core: DbContext ו-Migrations
- Code-First, `DbSet` לכל ישות (7 ישויות).
- ארבע migrations בהיסטוריה: `InitialCreate`, `AddUserSecurityFields`, `AddReportDetails`, `UniqueActiveEnrollment`.
- ה-connection string נקרא מהקונפיגורציה (`ConnectionStrings:Default`), ובפיתוח הוא נשמר ב-User Secrets.
- `DevelopmentDataSeeder`: נתוני דמו שנטענים אוטומטית בפיתוח, כך שהמערכת עולה ורצה מיד, **בלי שום התערבות ידנית בבסיס הנתונים** (ה-migrations רצות בעלייה).

### 6. EF Core: שאילתות ו-CRUD
- CRUD מלא, סינון ומיון ב-LINQ (`StudentFilter`, `GroupFilter`, `ReportFilter`).
- Pagination אמיתי מול בסיס הנתונים: `Skip`/`Take` בתוך השאילתה (`QueryableExtensions.ToPagedAsync`), עם `TotalCount` וגודל עמוד מקסימלי 100.
- `AsNoTracking` בשאילתות הקריאה בכל ה-repositories.
- מחיקה בטוחה: soft delete (`IsActive`) כשיש היסטוריה, ומחיקה אמיתית רק כשאין.

### 7. EF Core: קשרי גומלין
- one-to-many: מורה ← קבוצות, קבוצה ← שיעורים, מורה ← תלמידות.
- many-to-many: תלמידות ↔ קבוצות דרך ישות הקשר `Enrollment`, שמחזיקה גם נתונים משלה (סכום לתשלום, סכום ששולם, תאריך רישום).
- כל ההגדרות ב-Fluent API (`IEntityTypeConfiguration` לכל ישות), ו-`DeleteBehavior.Restrict` על כל מפתח זר.
- `Include` ו-`ThenInclude` לטעינה מראש, בלי N+1.

### 8. DTO ומיפוי
- ישויות Domain לא נכנסות ולא יוצאות דרך ה-API. יש DTOs נפרדים ל-request ול-response, וה-request מכיל רק שדות שהמשתמש באמת מקליד.
- **החלטה מקצועית: Riok.Mapperly במקום AutoMapper.** לגרסה של AutoMapper שהייתה בשימוש פורסמה פגיעות אבטחה (CVE), ולכן הוחלפה ב-Mapperly, שמייצר את קוד המיפוי בזמן הקומפילציה (source generator): בלי reflection, מהיר יותר, ושדה שלא מופה הוא שגיאת build (`RMG012` מוגדר כ-error). המיפויים נמצאים ב-mapper סטטי לכל feature (`StudentMapper`, `GroupMapper` וכו'), ונבדקים ב-`MapperTests`.

### 9. Asynchronous Programming
- השרשרת אסינכרונית מה-Controller ועד ה-`DbContext`.
- `CancellationToken` מתקבל בכל action ומועבר לאורך כל הדרך. בקשה שהלקוח ביטל מסתיימת בשקט עם 499 ולא נרשמת כשגיאה.

### 10. Middleware
- `GlobalExceptionHandler` (`IExceptionHandler`): ממפה כל סוג חריגה לסטטוס ומחזיר JSON אחיד: `status`, `code`, `message`, `traceId`.
- `CorrelationIdMiddleware`: מקבל `X-Correlation-Id` מהלקוח (אחרי ולידציה) או מייצר חדש, מחזיר אותו ב-header ומכניס אותו ל-scope של הלוג, כך שכל שורות הלוג של הבקשה נושאות אותו.
- `RequestLoggingMiddleware`: שורת לוג לכל בקשה עם method, path, סטטוס וזמן.
- סדר ה-pipeline: Correlation Id ראשון (כדי שגם שגיאות יקבלו מזהה), אחריו לוג הבקשה, טיפול בשגיאות, CORS, rate limiting, ורק אז `UseAuthentication` לפני `UseAuthorization`.

### 11. Configuration
- `appsettings.json` ו-`appsettings.Development.json`, גישה דרך `IOptions` (`JwtOptions`, `ClientCorsOptions`) עם `ValidateDataAnnotations` ו-`ValidateOnStart`: מפתח JWT חסר או קצר מ-32 תווים עוצר את השרת כבר בעלייה.
- **אין סוד בקוד ואין סוד ב-repo.** סיסמת בסיס הנתונים, מפתח ה-JWT וסיסמת משתמשי הדמו נמצאים ב-User Secrets בפיתוח ובמשתני סביבה בענן.

### 12. JWT Authentication & Authorization
- התחברות (`POST api/auth/login`), יצירת token עם claims (משתמש, role, המורה הבעלים, security stamp) וולידציה מלאה בשרת: issuer, audience, חתימה, אלגוריתם ותוקף.
- שני roles עם הרשאות שונות בפועל: `Teacher` ו-`Secretary` (`[Authorize(Roles = ...)]`). **FallbackPolicy** מחייב התחברות בכל endpoint שלא סומן אחרת, כך ששכחה של `[Authorize]` לא פותחת endpoint.
- **הרשמה מבוקרת:** אין הרשמה פתוחה, כי זו מערכת פנימית של עסק. מורה נוספת דרך פקודת CLI חד-פעמית (`add-teacher`), ומזכירה נוספת על ידי המורה שלה (`POST api/secretaries`) ומשויכת אליה אוטומטית.
- אבטחה נוספת: security stamp שמבטל tokens קיימים בשינוי סיסמה או השבתת משתמש, סיסמאות ב-hash של ASP.NET Identity, ו-rate limit של 5 ניסיונות בדקה להתחברות.

### 13. Unit Tests: xUnit ו-Moq
- `SwimSchool.Application.Tests`: בדיקות לשכבת ה-Service עם repositories מדומים ב-Moq, כולל הלוגיקה של המשאב המוגבל, גם המקרה התקין וגם המקרים שנדחים (`GROUP_FULL`, `SPOT_TAKEN`, `ALREADY_ENROLLED`).
- `SwimSchool.Infrastructure.Tests`: בדיקות התחרות מול PostgreSQL אמיתי (Testcontainers), ובדיקות רישום מחדש אחרי ביטול.
- `SwimSchool.Architecture.Tests`: חמש בדיקות שאוכפות את כללי השכבות.
- סך הכול 76 בדיקות.

### NLog
- `NLog.Web.AspNetCore`, והקוד ממשיך להשתמש ב-`ILogger` הרגיל.
- `nlog.Development.config`: קונסול וקבצים (`logs/swimschool-*.log`, ו-`logs/errors-*.log` לשגיאות בלבד). `nlog.config` לייצור: JSON ל-stdout, שמתאים ללוגים בענן.
- כל שורה כוללת `CorrelationId`.
- רמות: Debug לפיתוח (ובקשות `/health`), Information לזרימה רגילה, Warning לבקשה שהסתיימה ב-4xx, כולל כל התנגשות 409 על מקום בקבוצה, ו-Error לחריגה לא צפויה (עם ה-exception המלא).
- לא נכתבים ללוג סיסמאות, tokens או גוף בקשה: לוג הבקשה כולל רק method, path, סטטוס וזמן.

---

## מעבר לדרישות

- **קוד ונוסח שגיאה לכל מקרה:** 30 קודי שגיאה, כל אחד עם הודעה ברורה בעברית (למשל `GROUP_FULL`, `SPOT_TAKEN`, `PAYMENT_ABOVE_DUE`), מרוכזים ב-`ErrorCodes.cs` ומתועדים ב-[`docs/error-codes.md`](docs/error-codes.md). הלקוח יודע להציג הודעה מדויקת בלי לנחש.
- **דוח טיפולים PDF בעברית** (QuestPDF, RTL, גופן Heebo ולוגואים מוטמעים): עמוד אחד, בנוי לפי הטופס האמיתי שהלקוחה מגישה לקופה. התאריכים ממולאים מהנוכחות בפועל וניתנים לעריכה. המבנה מוכן לטופס נפרד לכל קופה (`IReportPdfGenerator`).
- **תמחור אמיתי:** מחיר הקבוצה הוא ברירת מחדל בלבד, והסכום שנקבע ברישום הוא הסכום האמיתי, כי מורות נותנות הנחות לפי שיקול דעתן.
- **שיעורים גמישים:** יצירה אוטומטית של שיעורים שבועיים או רשימת תאריכים ידנית, הזזה ומחיקה (נחסמות כשכבר סומנה נוכחות), ושיעורי השלמה.
- **Docker ו-Render:** `Dockerfile` רב-שלבי (restore מוטמן, image של Debian כדי שהספרייה הנייטיבית של QuestPDF תעבוד, ריצה כמשתמש לא-root), `ForwardedHeaders` ו-HSTS מאחורי proxy, migrations בעלייה (`Database__MigrateOnStartup`, בלי נתוני דמו בייצור), health check ב-`/health` שבודק גם את בסיס הנתונים, ו-`EnableRetryOnFailure` לחיבור.
- **CORS** מוגדר לפי סביבה, עם רשימת origins, methods ו-headers מפורשת.
- כל התאריכים נשמרים ב-UTC, ותאריך "היום" מחושב לפי שעון ישראל במקום שזה משנה.

---

## צד הלקוח

לקוח React שמדגים את השרת מקצה לקצה נמצא ב-repository נפרד.

---

## הרצה מקומית

**דרישות:** .NET 8 SDK, PostgreSQL (נבדק על גרסה 18), ו-Docker להרצת בדיקות ה-Infrastructure.

הדרך המהירה להרים PostgreSQL:
```bash
docker run --name swimschool-pg -e POSTGRES_PASSWORD=<password> -p 5432:5432 -d postgres:18
```

הגדרת הסודות (פעם אחת, משורש ה-solution):
```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=swimschool_dev;Username=postgres;Password=<password>" --project SwimSchool.API
dotnet user-secrets set "Jwt:Key" "<random key, at least 32 characters>" --project SwimSchool.API
dotnet user-secrets set "DevSeed:Password" "<password for the demo users>" --project SwimSchool.API
```

הרצה:
```bash
dotnet run --project SwimSchool.API
```
בעלייה בסביבת Development השרת מריץ את ה-migrations, יוצר את בסיס הנתונים וטוען נתוני דמו. Swagger זמין ב-`http://localhost:5136/swagger`.

הוספת מורה (לא נדרש לדמו, נועד לייצור):
```bash
dotnet run --project SwimSchool.API -- add-teacher
```

הרצת הבדיקות (Docker צריך לרוץ):
```bash
dotnet test
```

---

## משתמשי דמו

נוצרים אוטומטית בפיתוח. הסיסמה של כולם היא הערך ש-`DevSeed:Password` קיבל ב-User Secrets, כדי שאף סיסמה לא תישמר ב-repo.

| שם | Role | אימייל | שייכת ל |
|---|---|---|---|
| Dana Levi | Teacher | dana.levi@example.com | |
| Noa Cohen | Teacher | noa.cohen@example.com | |
| Yael Mizrahi | Secretary | yael.mizrahi@example.com | Dana Levi |
| Tamar Peretz | Secretary | tamar.peretz@example.com | Noa Cohen |

כדי לראות את בידוד הנתונים: מתחברים כ-Dana ואחר כך כ-Noa, וכל אחת רואה רק את התלמידות והקבוצות שלה.

כדי לראות את ה-409: מתחברים כ-Dana בחלון אחד וכ-Yael בחלון אחר, ורושמים במקביל שתי תלמידות שונות למקום האחרון באותה קבוצה.

---

## מבנה הפרויקט

```
SwimSchool.sln
├── SwimSchool.Domain            Entities, Enums, domain rules
├── SwimSchool.Application       Services, DTOs, Mappers, Interfaces, Exceptions
├── SwimSchool.Infrastructure    DbContext, Configurations, Migrations, Repositories, JWT, PDF
├── SwimSchool.API               Controllers, Middleware, CLI, Program.cs, nlog configs
├── tests
│   ├── SwimSchool.Application.Tests      xUnit + Moq
│   ├── SwimSchool.Infrastructure.Tests   Testcontainers (PostgreSQL)
│   └── SwimSchool.Architecture.Tests     NetArchTest
├── docs/error-codes.md
└── Dockerfile
```

## טכנולוגיות

.NET 8 · ASP.NET Core Web API · EF Core 8 + Npgsql · PostgreSQL · JWT Bearer · NLog · Riok.Mapperly · QuestPDF · Swashbuckle · xUnit · Moq · Testcontainers · NetArchTest · Docker
