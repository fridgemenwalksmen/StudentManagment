using Microsoft.Data.SqlClient;
using StudentManagementApp.Database;

namespace StudentManagementApp;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("====================================");
            Console.WriteLine("       STUDENT MANAGEMENT SYSTEM");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1. Display all students");
            Console.WriteLine("2. Search for a student");
            Console.WriteLine("3. Register a student");
            Console.WriteLine("4. Enrol a student");
            Console.WriteLine("5. Capture or update a mark");
            Console.WriteLine("6. View student results");
            Console.WriteLine("7. View students without enrolments");
            Console.WriteLine("8. Record a payment");
            Console.WriteLine("9. Exit");
            Console.WriteLine();

            Console.Write("Select an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayAllStudents();
                    break;

                case "2":
                    SearchStudent();
                    break;

                case "3":
                    RegisterStudent();
                    break;

                case "4":
                    EnrolStudent();
                    break;

                case "5":
                    CaptureOrUpdateMark();
                    break;

                case "6":
                    ViewStudentResults();
                    break;

                case "7":
                    ViewStudentsWithoutEnrolments();
                    break;

                case "8":
                    RecordPayment();
                    break;

                case "9":
                    Console.WriteLine("Goodbye!");
                    return;

                default:
                    Console.WriteLine("Invalid selection. Please choose 1-9.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }


    static void DisplayAllStudents()
    {
        const string sql = """
            SELECT StudentID, StudentNumber, FullName, Email, Status
            FROM STUDENT
            ORDER BY StudentID;
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            Console.WriteLine();
            Console.WriteLine("========== ALL STUDENTS ==========");
            Console.WriteLine();

            bool found = false;

            while (reader.Read())
            {
                found = true;

                Console.WriteLine($"Student ID: {reader["StudentID"]}");
                Console.WriteLine($"Student Number: {reader["StudentNumber"]}");
                Console.WriteLine($"Full Name: {reader["FullName"]}");
                Console.WriteLine($"Email: {reader["Email"]}");
                Console.WriteLine($"Status: {reader["Status"]}");
                Console.WriteLine("----------------------------------");
            }

            if (!found)
            {
                Console.WriteLine("No students found.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Unable to retrieve students.");
            Console.WriteLine($"Database error: {ex.Message}");
        }
    }

  

    static void SearchStudent()
    {
        Console.WriteLine();
        Console.WriteLine("========== SEARCH FOR STUDENT ==========");
        Console.WriteLine();

        Console.Write("Enter student number: ");
        string? studentNumber = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            Console.WriteLine("Student number is required.");
            return;
        }

        const string sql = """
            SELECT StudentID, StudentNumber, FullName, Email, Status
            FROM STUDENT
            WHERE StudentNumber = @StudentNumber;
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@StudentNumber", System.Data.SqlDbType.VarChar, 20)
                .Value = studentNumber;

            using SqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine();
                Console.WriteLine("Student found!");
                Console.WriteLine($"Student ID: {reader["StudentID"]}");
                Console.WriteLine($"Student Number: {reader["StudentNumber"]}");
                Console.WriteLine($"Full Name: {reader["FullName"]}");
                Console.WriteLine($"Email: {reader["Email"]}");
                Console.WriteLine($"Status: {reader["Status"]}");
            }
            else
            {
                Console.WriteLine("No student was found with that student number.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Unable to search for the student.");
            Console.WriteLine($"Database error: {ex.Message}");
        }
    }



    static void RegisterStudent()
    {
        Console.WriteLine();
        Console.WriteLine("========== REGISTER STUDENT ==========");
        Console.WriteLine();

        Console.Write("Student number: ");
        string? studentNumber = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            Console.WriteLine("Student number is required.");
            return;
        }

        Console.Write("Full name: ");
        string? fullName = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            Console.WriteLine("Full name is required.");
            return;
        }

        Console.Write("Email: ");
        string? email = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email is required.");
            return;
        }

        Console.Write("Status (Active/Inactive): ");
        string? status = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(status))
        {
            Console.WriteLine("Status is required.");
            return;
        }

        if (!status.Equals("Active", StringComparison.OrdinalIgnoreCase) &&
            !status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Status must be Active or Inactive.");
            return;
        }

        status = status.Equals("Active", StringComparison.OrdinalIgnoreCase)
            ? "Active"
            : "Inactive";

        const string sql = """
            INSERT INTO STUDENT
                (StudentNumber, FullName, Email, Status)
            VALUES
                (@StudentNumber, @FullName, @Email, @Status);
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@StudentNumber", System.Data.SqlDbType.VarChar, 20)
                .Value = studentNumber;

            command.Parameters.Add("@FullName", System.Data.SqlDbType.VarChar, 100)
                .Value = fullName;

            command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 100)
                .Value = email;

            command.Parameters.Add("@Status", System.Data.SqlDbType.VarChar, 10)
                .Value = status;

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Student registered successfully.");
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                Console.WriteLine(
                    "A student with that student number or email already exists.");
            }
            else
            {
                Console.WriteLine("Unable to register the student.");
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
    }



    static void EnrolStudent()
    {
        Console.WriteLine();
        Console.WriteLine("========== ENROL STUDENT ==========");
        Console.WriteLine();

        int? studentId = ReadInt("Student ID: ");

        if (studentId == null)
        {
            return;
        }

        int? courseId = ReadInt("Course ID: ");

        if (courseId == null)
        {
            return;
        }

        DateTime enrolmentDate = DateTime.Now;

        const string sql = """
            INSERT INTO ENROLMENT
                (StudentID, CourseID, EnrolmentDate, FinalMark)
            VALUES
                (@StudentID, @CourseID, @EnrolmentDate, NULL);

            UPDATE STUDENT
            SET Status = 'Active'
            WHERE StudentID = @StudentID;
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                using SqlCommand command =
                    new SqlCommand(sql, connection, transaction);

                command.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                    .Value = studentId.Value;

                command.Parameters.Add("@CourseID", System.Data.SqlDbType.Int)
                    .Value = courseId.Value;

                command.Parameters.Add("@EnrolmentDate", System.Data.SqlDbType.DateTime2)
                    .Value = enrolmentDate;

                command.ExecuteNonQuery();

                transaction.Commit();

                Console.WriteLine("Student enrolled successfully.");
                Console.WriteLine("Student status has been set to Active.");
            }
            catch
            {
                if (transaction.Connection != null)
                {
                    transaction.Rollback();
                }

                throw;
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627)
            {
                Console.WriteLine(
                    "This student is already enrolled in this course.");
            }
            else if (ex.Number == 547)
            {
                Console.WriteLine(
                    "The specified student or course does not exist.");
            }
            else
            {
                Console.WriteLine("Unable to enrol the student.");
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
    }


    static void CaptureOrUpdateMark()
    {
        Console.WriteLine();
        Console.WriteLine("========== CAPTURE / UPDATE MARK ==========");
        Console.WriteLine();

        int? studentId = ReadInt("Student ID: ");

        if (studentId == null)
        {
            return;
        }

        int? courseId = ReadInt("Course ID: ");

        if (courseId == null)
        {
            return;
        }

        decimal? mark = ReadDecimal(
            "Final mark (0-100): ",
            0,
            100);

        if (mark == null)
        {
            return;
        }

        const string sql = """
            UPDATE ENROLMENT
            SET FinalMark = @FinalMark
            WHERE StudentID = @StudentID
              AND CourseID = @CourseID;
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@FinalMark", System.Data.SqlDbType.Decimal)
                .Value = mark.Value;

            command.Parameters["@FinalMark"].Precision = 5;
            command.Parameters["@FinalMark"].Scale = 2;

            command.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                .Value = studentId.Value;

            command.Parameters.Add("@CourseID", System.Data.SqlDbType.Int)
                .Value = courseId.Value;

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Student mark updated successfully.");
                Console.WriteLine("The change has been recorded by the audit trigger.");
            }
            else
            {
                Console.WriteLine(
                    "No enrolment was found for that student and course.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Unable to update the mark.");
            Console.WriteLine($"Database error: {ex.Message}");
        }
    }

 

    static void ViewStudentResults()
    {
        Console.WriteLine();
        Console.WriteLine("========== STUDENT RESULTS ==========");
        Console.WriteLine();

        int? studentId = ReadInt("Student ID: ");

        if (studentId == null)
        {
            return;
        }

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(
                    "dbo.usp_GetStudentResults",
                    connection);

            command.CommandType =
                System.Data.CommandType.StoredProcedure;

            command.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                .Value = studentId.Value;

            using SqlDataReader reader =
                command.ExecuteReader();

            bool found = false;

            while (reader.Read())
            {
                found = true;

                string finalMark;

                if (reader["FinalMark"] == DBNull.Value)
                {
                    finalMark = "Not captured";
                }
                else
                {
                    finalMark = reader["FinalMark"].ToString()!;
                }

                Console.WriteLine(
                    $"Student: {reader["StudentNumber"]} - {reader["FullName"]}");

                Console.WriteLine($"Course Code: {reader["CourseCode"]}");
                Console.WriteLine($"Course Name: {reader["CourseName"]}");
                Console.WriteLine($"Final Mark: {finalMark}");
                Console.WriteLine($"Result: {reader["Result"]}");
                Console.WriteLine("----------------------------------");
            }

            if (!found)
            {
                Console.WriteLine(
                    "No results were found for that student.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Unable to retrieve student results.");
            Console.WriteLine($"Database error: {ex.Message}");
        }
    }

  

    static void ViewStudentsWithoutEnrolments()
    {
        const string sql = """
            SELECT
                S.StudentID,
                S.StudentNumber,
                S.FullName,
                S.Email,
                S.Status
            FROM STUDENT S
            LEFT JOIN ENROLMENT E
                ON S.StudentID = E.StudentID
            WHERE E.StudentID IS NULL
            ORDER BY S.StudentID;
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            Console.WriteLine();
            Console.WriteLine("========== STUDENTS WITHOUT ENROLMENTS ==========");
            Console.WriteLine();

            bool found = false;

            while (reader.Read())
            {
                found = true;

                Console.WriteLine($"Student ID: {reader["StudentID"]}");
                Console.WriteLine($"Student Number: {reader["StudentNumber"]}");
                Console.WriteLine($"Full Name: {reader["FullName"]}");
                Console.WriteLine($"Email: {reader["Email"]}");
                Console.WriteLine($"Status: {reader["Status"]}");
                Console.WriteLine("----------------------------------");
            }

            if (!found)
            {
                Console.WriteLine("All students have at least one enrolment.");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine(
                "Unable to retrieve students without enrolments.");

            Console.WriteLine($"Database error: {ex.Message}");
        }
    }


    static void RecordPayment()
    {
        Console.WriteLine();
        Console.WriteLine("========== RECORD PAYMENT ==========");
        Console.WriteLine();

        int? studentId = ReadInt("Student ID: ");

        if (studentId == null)
        {
            return;
        }

        decimal? amount = ReadDecimal(
            "Payment amount: ",
            0.01m,
            decimal.MaxValue);

        if (amount == null)
        {
            return;
        }

        Console.Write("Payment reference number: ");
        string? referenceNumber = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            Console.WriteLine("Reference number is required.");
            return;
        }

        DateTime paymentDate = DateTime.Now;

        const string sql = """
            INSERT INTO PAYMENT
                (StudentID, Amount, PaymentDate, ReferenceNumber)
            VALUES
                (@StudentID, @Amount, @PaymentDate, @ReferenceNumber);
            """;

        try
        {
            using SqlConnection connection =
                DatabaseConnection.GetConnection();

            connection.Open();

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                .Value = studentId.Value;

            command.Parameters.Add("@Amount", System.Data.SqlDbType.Decimal)
                .Value = amount.Value;

            command.Parameters["@Amount"].Precision = 10;
            command.Parameters["@Amount"].Scale = 2;

            command.Parameters.Add("@PaymentDate", System.Data.SqlDbType.DateTime2)
                .Value = paymentDate;

            command.Parameters.Add(
                    "@ReferenceNumber",
                    System.Data.SqlDbType.VarChar,
                    50)
                .Value = referenceNumber;

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Payment recorded successfully.");
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                Console.WriteLine(
                    "That payment reference number already exists.");
            }
            else if (ex.Number == 547)
            {
                Console.WriteLine(
                    "The specified student does not exist.");
            }
            else
            {
                Console.WriteLine("Unable to record the payment.");
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }
    }


    static int? ReadInt(string message)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int value))
        {
            Console.WriteLine("Please enter a valid whole number.");
            return null;
        }

        if (value <= 0)
        {
            Console.WriteLine("The number must be greater than zero.");
            return null;
        }

        return value;
    }



    static decimal? ReadDecimal(
        string message,
        decimal minimum,
        decimal maximum)
    {
        Console.Write(message);

        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal value))
        {
            Console.WriteLine("Please enter a valid number.");
            return null;
        }

        if (value < minimum || value > maximum)
        {
            Console.WriteLine(
                $"Value must be between {minimum} and {maximum}.");

            return null;
        }

        return value;
    }
}

