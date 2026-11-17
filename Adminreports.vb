Imports System.Data.SqlClient
Imports System.IO
Imports Guna.UI2.WinForms
Imports iText = iTextSharp.text
Imports iPdf = iTextSharp.text.pdf

Public Class Adminreports

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private Sub Adminreports_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadReports()
    End Sub

    ' ---------- GRID SETUP ----------
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

    ' ---------- LOAD DATA ----------
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

    ' ---------- GENERATE REPORT BUTTON ----------
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Try
            If Guna2DataGridView1.Rows.Count = 0 Then
                MessageBox.Show("No report data available!", "Generate Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using save As New SaveFileDialog()
                save.Title = "Generate Clinic Report"
                save.Filter = "PDF file (*.pdf)|*.pdf"
                save.FileName = "Barangay_Health_Clinic_Report_" & DateTime.Now.ToString("yyyyMMdd")

                If save.ShowDialog() = DialogResult.OK Then
                    CreatePdf(save.FileName)
                    Process.Start(New ProcessStartInfo(save.FileName) With {.UseShellExecute = True})
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ---------- PDF ----------
    Private Sub CreatePdf(path As String)
        Dim titleFont = iText.FontFactory.GetFont(iText.FontFactory.TIMES_BOLD, 14)
        Dim subFont = iText.FontFactory.GetFont(iText.FontFactory.TIMES_BOLD, 11)
        Dim noteFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA, 8, New iText.BaseColor(90, 90, 90))
        Dim headFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_BOLD, 9, iText.BaseColor.BLACK)
        Dim bodyFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA, 9, iText.BaseColor.BLACK)
        Dim gray As New iText.BaseColor(128, 128, 128)
        Dim border As New iText.BaseColor(170, 170, 170)

        Dim doc As New iText.Document(iText.PageSize.A4, 40, 40, 40, 40)
        Using fs As New FileStream(path, FileMode.Create)
            iPdf.PdfWriter.GetInstance(doc, fs)
            doc.Open()

            ' Header
            doc.Add(New iText.Paragraph("BARANGAY HEALTH CLINIC", titleFont))
            doc.Add(New iText.Paragraph("PATIENT MANAGEMENT SYSTEM", titleFont))
            doc.Add(New iText.Paragraph(" ", noteFont))
            doc.Add(New iText.Paragraph("CLINIC REPORTS SUMMARY", subFont))
            doc.Add(New iText.Paragraph("Generated: " & DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"), noteFont))
            doc.Add(New iText.Chunk(New iPdf.draw.LineSeparator(1.0F, 100.0F, iText.BaseColor.BLACK, iText.Element.ALIGN_CENTER, -4)))
            doc.Add(New iText.Paragraph(" ", noteFont))
            doc.Add(New iText.Paragraph(" ", noteFont))

            ' Table galing sa grid
            Dim colCount As Integer = Guna2DataGridView1.Columns.Count
            Dim t As New iPdf.PdfPTable(colCount)
            t.WidthPercentage = 100
            t.SetWidths(New Single() {20, 40, 24, 16})
            t.HeaderRows = 1

            For Each col As DataGridViewColumn In Guna2DataGridView1.Columns
                Dim cell As New iPdf.PdfPCell(New iText.Phrase(col.HeaderText, headFont))
                cell.BackgroundColor = gray
                cell.BorderColor = border
                cell.Padding = 6
                cell.HorizontalAlignment = iText.Element.ALIGN_CENTER
                t.AddCell(cell)
            Next

            For Each row As DataGridViewRow In Guna2DataGridView1.Rows
                If row.IsNewRow Then Continue For
                For i As Integer = 0 To colCount - 1
                    Dim v As String = If(row.Cells(i).Value Is Nothing, "", row.Cells(i).Value.ToString())
                    Dim cell As New iPdf.PdfPCell(New iText.Phrase(v, bodyFont))
                    cell.BorderColor = border
                    cell.Padding = 6
                    t.AddCell(cell)
                Next
            Next

            doc.Add(t)

            ' Footer
            doc.Add(New iText.Paragraph(" ", noteFont))
            doc.Add(New iText.Paragraph("Total reports: " & Guna2DataGridView1.Rows.Count, noteFont))

            doc.Close()
        End Using
    End Sub

End Class