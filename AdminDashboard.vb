Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting

Public Class AdminDashboard

    Private WithEvents dashRefreshTimer As New Timer()

    Private Sub EnsureDashConnection()
        If conn Is Nothing OrElse String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            dbConnection()
        End If
        If conn.State <> ConnectionState.Open Then
            conn.Open()
        End If
    End Sub

    Private Function FindChartRecursive(parent As Control, nameKeyword As String) As Chart
        If parent Is Nothing Then Return Nothing

        For Each c As Control In parent.Controls
            If TypeOf c Is Chart Then
                If String.IsNullOrEmpty(nameKeyword) OrElse
                   c.Name.IndexOf(nameKeyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return DirectCast(c, Chart)
                End If
            End If

            Dim found As Chart = FindChartRecursive(c, nameKeyword)
            If found IsNot Nothing Then Return found
        Next

        Return Nothing
    End Function

    ' ===================== TABLE NG STAFF =====================
    Private Sub DashbooardTable()
        Try
            EnsureDashConnection()
            admintbl.DataSource = Nothing
            admintbl.Rows.Clear()

            query = "SELECT first_Name, middle_Name, surname, name_extension, age, gender, Position FROM dbo.staff_information"

            Using comm As New SqlCommand(query, conn)
                Using reader As SqlDataReader = comm.ExecuteReader()
                    While reader.Read()
                        admintbl.Rows.Add(reader("first_Name"),
                                          reader("middle_Name"),
                                          reader("surname"),
                                          reader("name_extension"),
                                          reader("age"),
                                          reader("gender"),
                                          reader("Position"))
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading staff table: " & ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== HELPER: KUMUHA NG BILANG =====================
    Private Function GetCount(sql As String) As Integer
        Using comm As New SqlCommand(sql, conn)
            Dim result = comm.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then Return 0
            Return Convert.ToInt32(result)
        End Using
    End Function

    ' ===================== HELPER: I-SET ANG CARD =====================
    Private Sub SetCard(lblValue As Label, picTrend As Guna.UI2.WinForms.Guna2PictureBox, current As Integer, previous As Integer)
        lblValue.Text = current.ToString()

        If current >= previous Then
            picTrend.Image = My.Resources.trending_up__1_     ' green arrow
        Else
            picTrend.Image = My.Resources.trending_down       ' red arrow
        End If
    End Sub

    ' ===================== LOAD NG DASHBOARD CARDS =====================
    Private Sub LoadDashboardCards()
        Try
            EnsureDashConnection()
            Dim today As String = "CAST(GETDATE() AS DATE)"

            ' 1. Guna2ShadowPanel1 - TOTAL PATIENTS (vs last week)
            Dim totalNow = GetCount("SELECT COUNT(*) FROM dbo.patient_information")
            Dim totalPrev = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE created_account < DATEADD(DAY, -7, " & today & ")")
            SetCard(Label2, Guna2PictureBox1, totalNow, totalPrev)

            ' 2. Guna2ShadowPanel2 - TODAY'S APPOINTMENT (vs yesterday)
            Dim apptNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = " & today)
            Dim apptPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = DATEADD(DAY, -1, " & today & ")")
            SetCard(Label5, Guna2PictureBox2, apptNow, apptPrev)

            ' 3. Guna2ShadowPanel3 - NEW PATIENTS TODAY (vs yesterday)
            Dim newNow = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE CAST(created_account AS DATE) = " & today)
            Dim newPrev = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE CAST(created_account AS DATE) = DATEADD(DAY, -1, " & today & ")")
            SetCard(Label8, Guna2PictureBox3, newNow, newPrev)

            ' 4. Guna2ShadowPanel4 - TOTAL STAFF (vs last month)
            Dim staffNow = GetCount("SELECT COUNT(*) FROM dbo.staff_information")
            Dim staffPrev = staffNow
            If GetCount("SELECT CASE WHEN COL_LENGTH('dbo.staff_information','DateAdded') IS NULL THEN 0 ELSE 1 END") = 1 Then
                staffPrev = GetCount("SELECT COUNT(*) FROM dbo.staff_information WHERE DateAdded < DATEADD(MONTH, -1, " & today & ")")
            End If
            SetCard(Label11, Guna2PictureBox4, staffNow, staffPrev)

            ' 5. Guna2ShadowPanel5 - TOTAL MALE PATIENTS (vs last month)
            Dim maleNow = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE gender = 'Male'")
            Dim malePrev = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE gender = 'Male' AND created_account < DATEADD(MONTH, -1, " & today & ")")
            SetCard(Label14, Guna2PictureBox5, maleNow, malePrev)

            ' 6. Guna2ShadowPanel6 - TOTAL FEMALE PATIENTS (vs last month)
            Dim femNow = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE gender = 'Female'")
            Dim femPrev = GetCount("SELECT COUNT(*) FROM dbo.patient_information WHERE gender = 'Female' AND created_account < DATEADD(MONTH, -1, " & today & ")")
            SetCard(Label17, Guna2PictureBox6, femNow, femPrev)

            ' 7. Guna2ShadowPanel7 - PENDING APPOINTMENTS (ngayon, vs parehong araw last week)
            Dim pendNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Pending' AND CAST(AppointmentDate AS DATE) = " & today)
            Dim pendPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Pending' AND CAST(AppointmentDate AS DATE) = DATEADD(DAY, -7, " & today & ")")
            SetCard(Label20, Guna2PictureBox7, pendNow, pendPrev)

            ' 8. Guna2ShadowPanel8 - COMPLETE TODAY (ngayon, vs parehong araw last week)
            Dim compNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed' AND CAST(AppointmentDate AS DATE) = " & today)
            Dim compPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed' AND CAST(AppointmentDate AS DATE) = DATEADD(DAY, -7, " & today & ")")
            SetCard(Label23, Guna2PictureBox8, compNow, compPrev)

        Catch ex As Exception
            MessageBox.Show("Error loading dashboard: " & ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== REAL-TIME NOTIFICATIONS (Guna2DataGridView2) =====================
    Private Sub LoadNotifications()
        Try
            EnsureDashConnection()

            If GetCount("SELECT CASE WHEN OBJECT_ID('dbo.system_notifications', 'U') IS NULL THEN 0 ELSE 1 END") = 0 Then
                Return
            End If

            Dim selectedRowIdx As Integer = If(Guna2DataGridView2.CurrentRow IsNot Nothing, Guna2DataGridView2.CurrentRow.Index, -1)
            Guna2DataGridView2.Rows.Clear()

            Dim sql As String =
                "SELECT TOP 20 actor_name, action_text " &
                "FROM dbo.system_notifications " &
                "ORDER BY notif_id DESC"

            Using comm As New SqlCommand(sql, conn)
                Using reader As SqlDataReader = comm.ExecuteReader()
                    While reader.Read()
                        Dim msg As String = reader("actor_name").ToString().Trim() & " " &
                                          reader("action_text").ToString().Trim()
                        Guna2DataGridView2.Rows.Add(msg)
                    End While
                End Using
            End Using

            If selectedRowIdx >= 0 AndAlso selectedRowIdx < Guna2DataGridView2.Rows.Count Then
                Guna2DataGridView2.CurrentCell = Guna2DataGridView2.Rows(selectedRowIdx).Cells(0)
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' ===================== ACCURATE DAILY VISIT CHART (MONDAY - SUNDAY) =====================
    Private Sub LoadDailyVisitChartAccurate(visitChart As Chart)
        If visitChart Is Nothing Then Return

        Try
            EnsureDashConnection()

            Dim daysOrder() As String = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
            Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each d As String In daysOrder
                counts(d) = 0
            Next

            Dim sql As String =
                "SELECT DATENAME(WEEKDAY, created_account) AS day_name, COUNT(*) AS total " &
                "FROM dbo.patient_information " &
                "WHERE created_account IS NOT NULL " &
                "GROUP BY DATENAME(WEEKDAY, created_account)"

            Using cmd As New SqlCommand(sql, conn)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim dayName As String = reader("day_name").ToString().Trim()
                        If counts.ContainsKey(dayName) Then
                            counts(dayName) = Convert.ToInt32(reader("total"))
                        End If
                    End While
                End Using
            End Using

            If visitChart.Series.Count > 0 Then
                Dim s As Series = visitChart.Series(0)
                s.Points.Clear()
                For Each d As String In daysOrder
                    Dim ptIdx As Integer = s.Points.AddXY(d, counts(d))
                    s.Points(ptIdx).AxisLabel = d
                    s.Points(ptIdx).YValues = New Double() {CDbl(counts(d))}
                Next
                If visitChart.ChartAreas.Count > 0 Then
                    visitChart.ChartAreas(0).RecalculateAxesScale()
                End If
                visitChart.Invalidate()
                visitChart.Update()
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' ===================== REAL-TIME CHARTS =====================
    Private Sub LoadDashboardCharts()
        Dim visitChart As Chart = FindChartRecursive(Me, "visit")
        If visitChart Is Nothing Then visitChart = FindChartRecursive(Me, "daily")
        If visitChart Is Nothing Then visitChart = FindChartRecursive(Me, "")

        LoadDailyVisitChartAccurate(visitChart)

        Dim parentAdmin As AdminFrm = TryCast(Me.FindForm(), AdminFrm)
        If parentAdmin Is Nothing Then Return

        Dim staffChart As Chart = FindChartRecursive(Me, "staff")
        Dim genderChart As Chart = FindChartRecursive(Me, "gender")

        parentAdmin.RefreshDashboardCharts(Nothing, staffChart, genderChart)
    End Sub

    Public Sub RefreshDashboardData()
        DashbooardTable()
        LoadDashboardCards()
        LoadNotifications()
        LoadDashboardCharts()
    End Sub

    ' ===================== FORM LOAD & REAL-TIME TIMER =====================
    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AdminFrm.dashboardbtn.BackColor = Color.FromArgb(4, 196, 80)

        dbConnection()
        RefreshDashboardData()

        dashRefreshTimer.Interval = 3000
        dashRefreshTimer.Start()
    End Sub

    Private Sub dashRefreshTimer_Tick(sender As Object, e As EventArgs) Handles dashRefreshTimer.Tick
        If Me.Visible Then
            RefreshDashboardData()
        End If
    End Sub

End Class