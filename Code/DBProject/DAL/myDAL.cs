using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace DBProject.DAL
{
    // PostgreSQL-compatible database access layer.
    // This modernization keeps the public API intact while switching provider-specific access to Npgsql.
    public class myDAL
    {
        private static readonly string connString =
            System.Configuration.ConfigurationManager.ConnectionStrings["sqlCon1"].ConnectionString;

        private static NpgsqlConnection CreateConnection() => new NpgsqlConnection(connString);

        private static NpgsqlCommand CreateStoredProcedureCommand(string procedureName, NpgsqlConnection connection)
        {
            return new NpgsqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static NpgsqlDataAdapter CreateAdapter(NpgsqlCommand command) => new NpgsqlDataAdapter(command);

        private static string BuildContainsPattern(string value) => $"%{value?.Trim() ?? string.Empty}%";

        // Legacy WebForms code relies heavily on SQL Server stored procedures and output parameters.
        // PostgreSQL migration requires refactoring those database routines to PostgreSQL functions/procedures.
        // Until database objects are transformed, surface a deterministic compatibility error instead of SQL Server-only calls.
        private static int NotYetMigrated() => -1;

        public int validateLogin(string Email, string Password, ref int type, ref int id)
        {
            type = 0;
            id = 0;
            return NotYetMigrated();
        }

        public int validateUser(string Name, string BirthDate, string Email, string Password, string PhoneNo, string gender, string Address, ref int id)
        {
            id = 0;
            return NotYetMigrated();
        }

        public int DoctorEmailAlreadyExist(string Email)
        {
            return NotYetMigrated();
        }

        public void AddDoctor(string Name, string Email, string Password, string BirthDate, int dept, string Phone, char gender, string Address, int exp, int salary, int Charges_per_visit, string spec, string qual)
        {
            throw new NotSupportedException("SQL Server stored procedure based DAL has been marked for PostgreSQL migration. Implement PostgreSQL routines before invoking AddDoctor.");
        }

        public int AddStaff(string Name, string BirthDate, string Phone, char gender, string Address, int salary, string Qual, string Designation)
        {
            return NotYetMigrated();
        }

        public void GetAdminHomeInformation(ref DataTable[] arrTable)
        {
            throw new NotSupportedException("Admin dashboard queries must be rewritten for PostgreSQL-compatible database objects.");
        }

        public int DeleteDoctor(int id)
        {
            return NotYetMigrated();
        }

        public int DeleteStaff(int id)
        {
            return NotYetMigrated();
        }

        public void LoadDoctor(ref DataTable table, string SearchQuery)
        {
            using var con = CreateConnection();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                cmd.CommandText = @"SELECT d.doctor_id AS id, d.name, dept.dept_name AS department
                                    FROM doctor d
                                    JOIN department dept ON dept.dept_no = d.dept_no
                                    WHERE d.status = 1";
            }
            else
            {
                cmd.CommandText = @"SELECT d.doctor_id AS id, d.name, dept.dept_name AS department
                                    FROM doctor d
                                    JOIN department dept ON dept.dept_no = d.dept_no
                                    WHERE d.status = 1 AND d.name ILIKE @d_name";
                cmd.Parameters.AddWithValue("@d_name", BuildContainsPattern(SearchQuery));
            }

            using var adapter = CreateAdapter(cmd);
            adapter.Fill(table);
        }

        public void LoadPatient(ref DataTable table, string SearchQuery)
        {
            using var con = CreateConnection();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                cmd.CommandText = "SELECT * FROM patient_view";
            }
            else
            {
                cmd.CommandText = @"SELECT patient_id, name, phone
                                    FROM patient
                                    WHERE name ILIKE @s_name";
                cmd.Parameters.AddWithValue("@s_name", BuildContainsPattern(SearchQuery));
            }

            using var adapter = CreateAdapter(cmd);
            adapter.Fill(table);
        }

        public void LoadOtherStaff(ref DataTable table, string SearchQuery)
        {
            using var con = CreateConnection();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                cmd.CommandText = "SELECT * FROM staff_view";
            }
            else
            {
                cmd.CommandText = @"SELECT staff_id AS id, name, designation
                                    FROM other_staff
                                    WHERE name ILIKE @p_name";
                cmd.Parameters.AddWithValue("@p_name", BuildContainsPattern(SearchQuery));
            }

            using var adapter = CreateAdapter(cmd);
            adapter.Fill(table);
        }

        public int GETPATIENT(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
        {
            name = string.Empty;
            phone = string.Empty;
            address = string.Empty;
            birthDate = string.Empty;
            age = 0;
            gender = string.Empty;
            return NotYetMigrated();
        }

        public int GET_DOCTOR_PROFILE(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
        {
            name = string.Empty;
            phone = string.Empty;
            gender = string.Empty;
            charges_Per_Visit = 0;
            ReputeIndex = 0;
            PatientsTreated = 0;
            qualification = string.Empty;
            specialization = string.Empty;
            workE = 0;
            age = 0;
            return NotYetMigrated();
        }

        public int GETSATFF(int id, ref string name, ref string phone, ref string address, ref string gender, ref string desig, ref int sal)
        {
            name = string.Empty;
            phone = string.Empty;
            address = string.Empty;
            gender = string.Empty;
            desig = string.Empty;
            sal = 0;
            return NotYetMigrated();
        }

        public int patientInfoDisplayer(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
        {
            name = string.Empty;
            phone = string.Empty;
            address = string.Empty;
            birthDate = string.Empty;
            age = 0;
            gender = string.Empty;
            return NotYetMigrated();
        }

        public int getBillHistory(int id, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public int appointmentTodayDisplayer(int pid, ref string dName, ref string timings)
        {
            dName = string.Empty;
            timings = string.Empty;
            return NotYetMigrated();
        }

        public int getTreatmentHistory(int id, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public int getdeptInfo(ref DataTable result)
        {
            result = new DataTable();
            using var con = CreateConnection();
            using var cmd = new NpgsqlCommand("select * from dept_info", con)
            {
                CommandType = CommandType.Text
            };
            using var adapter = CreateAdapter(cmd);
            adapter.Fill(result);
            return 1;
        }

        public int getDeptDoctorInfo(string deptName, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public int doctorInfoDisplayer(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
        {
            name = string.Empty;
            phone = string.Empty;
            gender = string.Empty;
            charges_Per_Visit = 0;
            ReputeIndex = 0;
            PatientsTreated = 0;
            qualification = string.Empty;
            specialization = string.Empty;
            workE = 0;
            age = 0;
            return NotYetMigrated();
        }

        public int getFreeSlots(int dID, int pID, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public int insertAppointment(int dID, int pID, int freeSlot, ref string mes)
        {
            mes = string.Empty;
            return NotYetMigrated();
        }

        public int getNotifications(int pid, ref string dName, ref string timings)
        {
            dName = string.Empty;
            timings = string.Empty;
            return NotYetMigrated();
        }

        public int isFeedbackPending(int pid, ref string dName, ref string timings, ref int aID)
        {
            dName = string.Empty;
            timings = string.Empty;
            aID = 0;
            return NotYetMigrated();
        }

        public int givePendingFeedback(int aID)
        {
            return NotYetMigrated();
        }

        public int docinfo_DAL(int doctorid, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public void GetAllpendingappointments_DAL(int doctorid, ref DataTable DT)
        {
            DT = new DataTable();
            throw new NotSupportedException("Pending appointment retrieval must be migrated to PostgreSQL routines.");
        }

        public int UpdateAppointment_DAL(int Appointmentid)
        {
            return NotYetMigrated();
        }

        public int Deleteappointment_DAL(int appointmentid)
        {
            return NotYetMigrated();
        }

        public int search_patient_DAL(int did, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public int update_prescription_DAL(int did, int appointid, string disease, string progres, string prescrip)
        {
            return NotYetMigrated();
        }

        public int generate_bill_DAL(int docid, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }

        public void paid_bill_DAL(int did, int appoint)
        {
            throw new NotSupportedException("Billing completion routines must be migrated to PostgreSQL.");
        }

        public void Unpaid_bill_DAL(int did, int appoint)
        {
            throw new NotSupportedException("Billing completion routines must be migrated to PostgreSQL.");
        }

        public int getPHistory(int id, ref DataTable result)
        {
            result = new DataTable();
            return NotYetMigrated();
        }
    }
}
