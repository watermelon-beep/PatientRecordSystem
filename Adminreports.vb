Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports Guna.UI2.WinForms


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

        Try


            If Guna2DataGridView1.Rows.Count = 0 OrElse
       Guna2DataGridView1.Rows.Cast(Of DataGridViewRow)().
       All(Function(r) r.IsNewRow) Then

                MessageBox.Show(
            "No report data available!",
            "Generate Report",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        )
                Return
            End If


            Using save As New SaveFileDialog()

                save.Title = "Generate Clinic Report"
                save.Filter = "DOCS File (*.docs)|*.docs"
                save.FileName = "Barangay_Health_Clinic_Report_" &
                        DateTime.Now.ToString("yyyyMMdd") & ".docs"

                If save.ShowDialog() = DialogResult.OK Then

                    Dim sb As New StringBuilder()


                    sb.AppendLine("BARANGAY HEALTH CLINIC")
                    sb.AppendLine("CLINIC REPORT")
                    sb.AppendLine("Generated: " &
                           DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"))
                    sb.AppendLine()


                    For i As Integer = 0 To Guna2DataGridView1.Columns.Count - 1

                        Dim header As String =
                    Guna2DataGridView1.Columns(i).HeaderText

                        sb.Append("""" & header.Replace("""", """""") & """")

                        If i < Guna2DataGridView1.Columns.Count - 1 Then
                            sb.Append(",")
                        End If

                    Next

                    sb.AppendLine()


                    For Each row As DataGridViewRow In Guna2DataGridView1.Rows

                        If Not row.IsNewRow Then

                            For i As Integer = 0 To Guna2DataGridView1.Columns.Count - 1

                                Dim value As String = ""

                                If row.Cells(i).Value IsNot Nothing Then
                                    value = row.Cells(i).Value.ToString()
                                End If


                                sb.Append("""" & value.Replace("""", """""") & """")

                                If i < Guna2DataGridView1.Columns.Count - 1 Then
                                    sb.Append(",")
                                End If

                            Next

                            sb.AppendLine()

                        End If

                    Next


                    File.WriteAllText(
                save.FileName,
                sb.ToString(),
                New UTF8Encoding(True)
            )

                    MessageBox.Show(
                "Report successfully generated!" &
                vbCrLf & vbCrLf &
                "Saved Location: " & save.FileName,
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )


                    Process.Start(New ProcessStartInfo() With {
                .FileName = save.FileName,
                .UseShellExecute = True
            })

                End If

            End Using

        Catch ex As Exception

            MessageBox.Show(
        "Error generating report: " & ex.Message,
        "Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error
    )

        End Try

    End Sub
End Class