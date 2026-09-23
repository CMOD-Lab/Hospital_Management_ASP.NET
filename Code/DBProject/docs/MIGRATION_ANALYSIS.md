# ASP.NET Web Forms to .NET 8 Migration Analysis

This document inventories the current Web Forms application and records migration blockers identified using the provided upgrade analysis rules.

## Inventory

- Web Forms pages (.aspx): 22
- Code-behind files (.aspx.cs): 22
- Master pages (.master/.Master): 3
- User controls (.ascx): 0
- Global.asax files: 0 found
- Web.config files: 1
- Project files (.csproj): 1
- Package manifests (packages.config): 1

## Web Forms pages

### Root
- `SignUp.aspx`

### Admin
- `Admin/AddStaff.aspx`
- `Admin/AdminHome.aspx`
- `Admin/DoctorRegistrationForm.aspx`
- `Admin/ManageClinic.aspx`

### Doctor
- `Doctor/Bill.aspx`
- `Doctor/DoctorHome.aspx`
- `Doctor/HistoryUpdate.aspx`
- `Doctor/PatientHistory.aspx`
- `Doctor/PendingAppointment.aspx`
- `Doctor/PreviousHistory.aspx`

### Patient
- `Patient/AppointmentRequestSent.aspx`
- `Patient/AppointmentTaker.aspx`
- `Patient/BillsHistory.aspx`
- `Patient/CurrentAppointment.aspx`
- `Patient/DoctorProfile.aspx`
- `Patient/PatientFeedback.aspx`
- `Patient/PatientHome.aspx`
- `Patient/PatientNotifications.aspx`
- `Patient/TakeAppointment.aspx`
- `Patient/TreatmentHistory.aspx`
- `Patient/ViewDoctors.aspx`

## Master pages
- `Admin/Admin.Master`
- `Doctor/DoctorMaster.Master`
- `Patient/PatientMaster.Master`

## Key migration blockers

### Critical blockers
1. The project is a legacy ASP.NET Web Application targeting .NET Framework 4.5.2 with `System.Web` and Web Forms project type GUIDs.
2. All page models inherit from `System.Web.UI.Page` or `System.Web.UI.MasterPage`, which do not exist in .NET 8.
3. The application relies on `Web.config` and HTTP modules, including `Microsoft.ApplicationInsights.Web.ApplicationInsightsHttpModule`.
4. The application uses server controls and postback events (`<asp:GridView>`, `<asp:Button>`, `OnClick`, `OnRowCommand`, `runat="server"`) that must be redesigned in Razor Pages or MVC.
5. Session-based page flow is deeply coupled to navigation state (`Session["idoriginal"]`, `Session["deptOriginal"]`, `Session["dID"]`, `Session["freeSlot"]`, `Session["aID"]`).
6. The DAL is built on ADO.NET with `SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `DataSet`, and `DataTable`, requiring redesign to EF Core 8 or modern data access.

### High blockers
1. Legacy NuGet packages are referenced through `packages.config`, including Web Forms-specific Application Insights packages and CodeDom compiler packages.
2. Configuration and startup behavior are tied to `Web.config` rather than `appsettings.json` and `Program.cs`.
3. Page lifecycle usage (`Page_Load`, `IsPostBack`) and Response-based navigation (`Response.Redirect`) are widespread.

### Component complexity assessment
- Complex: `SignUp.aspx`, `Patient/TakeAppointment.aspx`, `Patient/ViewDoctors.aspx`, `Patient/AppointmentTaker.aspx`, `Patient/DoctorProfile.aspx`, most GridView-based pages, all master pages, and `DAL/myDAL.cs`.
- Medium: information display pages such as `Patient/CurrentAppointment.aspx`, `Patient/PatientNotifications.aspx`, `Patient/PatientHome.aspx`.
- Complex overall migration due to architecture, UI model, state management, and data access coupling.

## Recommended migration path

1. Create a new SDK-style .NET 8 solution using clean architecture.
2. Replace Web Forms pages with Razor Pages or MVC controllers and views.
3. Replace session-driven navigation with route parameters, TempData, or scoped services.
4. Move connection strings and settings from `Web.config` to `appsettings.json`.
5. Replace ADO.NET DAL methods with EF Core 8 repositories or Dapper-based services.
6. Replace Application Insights Web module configuration with ASP.NET Core telemetry registration in `Program.cs`.
7. Remove `packages.config`, legacy compiler packages, and all `System.Web` references.

## Files requiring migration changes
- `Clinic Management System.csproj`
- `Web.config`
- `packages.config`
- `DAL/myDAL.cs`
- All `.aspx` files
- All `.aspx.cs` files
- All `.master` files
- All `.Master.cs` files

## Notes

This repository was not converted to .NET 8 in place because the provided rules identify Web Forms and `System.Web` usage as incompatible with .NET 8. The correct remediation is an application redesign to ASP.NET Core rather than incremental compatibility edits inside the current Web Forms project.