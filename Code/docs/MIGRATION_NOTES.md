# Migration Notes

## What Was Migrated

### From ASP.NET Web Forms (.NET 4.5.2) to .NET 8 Razor Pages

| Old File | New File | Notes |
|----------|----------|-------|
| SignUp.aspx | Pages/Index.cshtml | Login + Registration combined |
| Admin/AdminHome.aspx | Pages/Admin/AdminHome.cshtml | Dashboard |
| Admin/DoctorRegistrationForm.aspx | Pages/Admin/DoctorRegistrationForm.cshtml | |
| Admin/AddStaff.aspx | Pages/Admin/AddStaff.cshtml | |
| Admin/ManageClinic.aspx | Pages/Admin/ManageClinic.cshtml | |
| Patient/PatientHome.aspx | Pages/Patient/PatientHome.cshtml | |
| Patient/ViewDoctors.aspx | Pages/Patient/ViewDoctors.cshtml | |
| Patient/DoctorProfile.aspx | Pages/Patient/DoctorProfile.cshtml | |
| Patient/TakeAppointment.aspx | Pages/Patient/TakeAppointment.cshtml | |
| Patient/AppointmentTaker.aspx | Pages/Patient/AppointmentTaker.cshtml | |
| Patient/AppointmentRequestSent.aspx | Pages/Patient/AppointmentRequestSent.cshtml | |
| Patient/BillsHistory.aspx | Pages/Patient/BillsHistory.cshtml | |
| Patient/TreatmentHistory.aspx | Pages/Patient/TreatmentHistory.cshtml | |
| Patient/CurrentAppointment.aspx | Pages/Patient/CurrentAppointment.cshtml | |
| Patient/PatientNotifications.aspx | Pages/Patient/PatientNotifications.cshtml | |
| Patient/PatientFeedback.aspx | Pages/Patient/PatientFeedback.cshtml | |
| Doctor/DoctorHome.aspx | Pages/Doctor/DoctorHome.cshtml | |
| Doctor/PendingAppointment.aspx | Pages/Doctor/PendingAppointment.cshtml | |
| Doctor/PatientHistory.aspx | Pages/Doctor/PatientHistory.cshtml | |
| Doctor/HistoryUpdate.aspx | Pages/Doctor/HistoryUpdate.cshtml | |
| Doctor/Bill.aspx | Pages/Doctor/Bill.cshtml | |
| Doctor/PreviousHistory.aspx | Pages/Doctor/PreviousHistory.cshtml | |
| DAL/myDAL.cs | Infrastructure/Repositories/*.cs | Split by domain |
| Web.config | appsettings.json | |
| Global.asax | Program.cs | |

## Key Differences from Web Forms

1. **No ViewState** - State managed via session and TempData
2. **No Code-Behind** - Page Models with proper separation
3. **Dependency Injection** - All services injected via constructor
4. **Async/Await** - All I/O operations are async
5. **Tag Helpers** - Replace server controls
6. **Middleware** - Replace HTTP modules/handlers

## Breaking Changes

- Session access changed from `Session["key"]` to `HttpContext.Session.GetInt32("key")`
- Response.Redirect changed to `RedirectToPage()`
- No `Response.Write()` - use model properties and Razor syntax

## Configuration Changes

- Connection string moved from Web.config to appsettings.json
- Logging configured via Serilog in Program.cs
- No more ApplicationInsights.config (use built-in telemetry)

## Known Issues

- Integration tests require a live database connection
- The `System.Data.SqlClient` package is used for ADO.NET (consider migrating to `Microsoft.Data.SqlClient` for newer features)
