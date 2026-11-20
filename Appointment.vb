Imports System.Data.SqlClient
Imports Guna.UI2.WinForms

Public Class Appointment

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private WithEvents apptRefreshTimer As New Timer()

    Private Sub Appointment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        RefreshAllAppointmentData()

        apptRefreshTimer.Interval = 3000
        apptRefreshTimer.Start()
    End Sub

    Private Sub apptRefreshTimer_Tick(sender As Object, e As EventArgs) Handles apptRefreshTimer.Tick
        If Me.Visible Then
            RefreshAllAppointmentData()
        End If
    End Sub

    Public Sub RefreshAllAppointmentData()
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

            Dim currentScroll As Integer = If(appoint.FirstDisplayedScrollingRowIndex >= 0, appoint.FirstDisplayedScrollingRowIndex, 0)
            appoint.DataSource = dt
            If appoint.Rows.Count > 0 AndAlso currentScroll < appoint.Rows.Count Then
                appoint.FirstDisplayedScrollingRowIndex = currentScroll
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function GetAllLabelsRecursive(parent As Control) As List(Of Label)
        Dim list As New List(Of Label)()
        If parent Is Nothing Then Return list

        For Each c As Control In parent.Controls
            If TypeOf c Is Label Then
                list.Add(DirectCast(c, Label))
            End If
            If c.HasChildren Then
                list.AddRange(GetAllLabelsRecursive(c))
            End If
        Next

        Return list
    End Function

    Private Function FindCardCountLabel(titleKeyword As String) As Label
        Dim allLabels As List(Of Label) = GetAllLabelsRecursive(Me)

        For Each titleLbl As Label In allLabels
            If Not String.IsNullOrWhiteSpace(titleLbl.Text) AndAlso
               titleLbl.Text.IndexOf(titleKeyword, StringComparison.OrdinalIgnoreCase) >= 0 AndAlso
               titleLbl.Parent IsNot Nothing Then

                Dim bestLbl As Label = Nothing
                Dim bestScore As Integer = Integer.MaxValue

                For Each candidate As Label In GetAllLabelsRecursive(titleLbl.Parent)
                    If candidate Is titleLbl Then Continue For

                    Dim txt As String = candidate.Text.Trim()
                    If IsNumeric(txt) OrElse txt = "" OrElse txt = "-" Then
                        Dim dist As Integer = Math.Abs(candidate.Top - titleLbl.Top) + Math.Abs(candidate.Left - titleLbl.Left)
                        If dist < bestScore Then
                            bestScore = dist
                            bestLbl = candidate
                        End If
                    End If
                Next

                If bestLbl IsNot Nothing Then Return bestLbl
            End If
        Next

        Return Nothing
    End Function

    Private Sub LoadCounts()
        Try
            Using con As New SqlConnection(connStr)
                con.Open()
                Using cmd As New SqlCommand(
                    "SELECT COUNT(*), " &
                    "ISNULL(SUM(CASE WHEN LOWER(LTRIM(RTRIM(ISNULL(Status,''))))='confirmed' THEN 1 ELSE 0 END),0), " &
                    "ISNULL(SUM(CASE WHEN LOWER(LTRIM(RTRIM(ISNULL(Status,''))))='pending' THEN 1 ELSE 0 END),0), " &
                    "ISNULL(SUM(CASE WHEN LOWER(LTRIM(RTRIM(ISNULL(Status,'')))) IN ('cancelled','canceled') THEN 1 ELSE 0 END),0) " &
                    "FROM Appointments", con)
                    Using r = cmd.ExecuteReader()
                        If r.Read() Then
                            Dim totalVal As String = r.GetInt32(0).ToString()
                            Dim confirmedVal As String = r.GetInt32(1).ToString()
                            Dim pendingVal As String = r.GetInt32(2).ToString()
                            Dim cancelledVal As String = r.GetInt32(3).ToString()

                            Dim lblTotal As Label = FindCardCountLabel("Total")
                            Dim lblConfirmed As Label = FindCardCountLabel("Confirmed")
                            Dim lblPending As Label = FindCardCountLabel("Pending")
                            Dim lblCancelled As Label = FindCardCountLabel("Cancel")

                            If lblTotal IsNot Nothing Then lblTotal.Text = totalVal
                            If lblConfirmed IsNot Nothing Then lblConfirmed.Text = confirmedVal
                            If lblPending IsNot Nothing Then lblPending.Text = pendingVal
                            If lblCancelled IsNot Nothing Then lblCancelled.Text = cancelledVal
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

End Class