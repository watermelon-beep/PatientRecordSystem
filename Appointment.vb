Imports System.Data.SqlClient
Imports Guna.UI2.WinForms

Public Class Appointment

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private Sub Appointment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadAppointments()
        LoadCounts()
    End Sub

    Private Sub SetupGrid()
        With appoint
            .AutoGenerateColumns = False
            .Columns.Clear()
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "ID", .DataPropertyName = "AppointmentID"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Date & Time", .DataPropertyName = "DateTime", .FillWeight = 130})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Patient Name", .DataPropertyName = "PatientName"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Service", .DataPropertyName = "Service"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Status", .DataPropertyName = "Status"})
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)

            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 250, 246)
            .AlternatingRowsDefaultCellStyle.ForeColor = Color.Black

            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .ReadOnly = True
            .RowTemplate.Height = 36
        End With
    End Sub

    Private Sub LoadAppointments()
        Try
            Dim dt As New DataTable()
            Using con As New SqlConnection(connStr)
                Using da As New SqlDataAdapter(
                    "SELECT 'APT-' + CAST(AppointmentID AS VARCHAR(10)) AS AppointmentID, " &
                    "FORMAT(AppointmentDate,'MMM dd, yyyy hh:mm tt') AS [DateTime], " &
                    "PatientName, Service, Status " &
                    "FROM Appointments ORDER BY Appointments.AppointmentDate", con)
                    da.Fill(dt)
                End Using
            End Using
            appoint.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Error loading appointments: " & ex.Message)
        End Try
    End Sub
    Private Sub LoadCounts()
        Try
            Using con As New SqlConnection(connStr)
                con.Open()
                Using cmd As New SqlCommand(
                    "SELECT COUNT(*), " &
                    "ISNULL(SUM(CASE WHEN Status='Confirmed' THEN 1 ELSE 0 END),0), " &
                    "ISNULL(SUM(CASE WHEN Status='Pending' THEN 1 ELSE 0 END),0), " &
                    "ISNULL(SUM(CASE WHEN Status='Cancelled' THEN 1 ELSE 0 END),0) " &
                    "FROM Appointments", con)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then
                            appoint.Text = r.GetInt32(0).ToString()
                            appoint.Text = r.GetInt32(1).ToString()
                            appoint.Text = r.GetInt32(2).ToString()
                            appoint.Text = r.GetInt32(3).ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading counts: " & ex.Message)
        End Try
    End Sub

End Class
