Imports System.Data.SqlClient

Public Class Adminreports

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private Sub Adminreports_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadReports()
    End Sub

    Private Sub SetupGrid()
        With Guna2DataGridView1

            .AutoGenerateColumns = False
            .Columns.Clear()
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Report Type", .DataPropertyName = "ReportType"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Description", .DataPropertyName = "Description"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Period", .DataPropertyName = "Period"})
            .Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = "Status", .DataPropertyName = "Status"})
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill


            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 128, 0)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 128, 0)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)


            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230)
            .DefaultCellStyle.SelectionForeColor = Color.Black


            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 250, 246)
            .AlternatingRowsDefaultCellStyle.ForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.Font = New Font("Segoe UI", 10)
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230)
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black


            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .ReadOnly = True
            .RowTemplate.Height = 36
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None


            .ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(0, 128, 0)
            .ThemeStyle.HeaderStyle.ForeColor = Color.White
            .ThemeStyle.RowsStyle.BackColor = Color.White
            .ThemeStyle.RowsStyle.ForeColor = Color.Black
            .ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(244, 250, 246)
            .ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Black
        End With
    End Sub

    Private Sub LoadReports()
        Dim dt As New DataTable()
        Using con As New SqlConnection(connStr)
            Using da As New SqlDataAdapter("SELECT ReportType, Description, " &
                "FORMAT(PeriodFrom,'MMM dd, yyyy') + ' - ' + FORMAT(PeriodTo,'MMM dd, yyyy') AS Period, Status FROM Reports", con)
                da.Fill(dt)
            End Using
        End Using
        Guna2DataGridView1.DataSource = dt
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click

    End Sub
End Class