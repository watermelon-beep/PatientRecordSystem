Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting

Public Class Barangay_Assistance_Dashboard

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private WithEvents baRefreshTimer As New Timer()

    Private Function GetOpenBaConnection() As SqlConnection
        If conn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            If conn.State <> ConnectionState.Open Then conn.Open()
            Return conn
        End If

        Dim localConn As New SqlConnection(connStr)
        localConn.Open()
        Return localConn
    End Function

    Private Function TableExists(activeConn As SqlConnection, tableName As String) As Boolean
        Using cmd As New SqlCommand("SELECT CASE WHEN OBJECT_ID(@tbl, 'U') IS NULL THEN 0 ELSE 1 END", activeConn)
            cmd.Parameters.Add("@tbl", SqlDbType.VarChar, 128).Value = tableName
            Dim res = cmd.ExecuteScalar()
            Return (res IsNot Nothing AndAlso Not IsDBNull(res) AndAlso Convert.ToInt32(res) = 1)
        End Using
    End Function

    Private Function ColumnExists(activeConn As SqlConnection, tableName As String, colName As String) As Boolean
        Using cmd As New SqlCommand(
            "SELECT CASE WHEN COL_LENGTH(@tbl, @col) IS NULL THEN 0 ELSE 1 END", activeConn)
            cmd.Parameters.Add("@tbl", SqlDbType.VarChar, 128).Value = tableName
            cmd.Parameters.Add("@col", SqlDbType.VarChar, 128).Value = colName
            Dim res = cmd.ExecuteScalar()
            Return (res IsNot Nothing AndAlso Not IsDBNull(res) AndAlso Convert.ToInt32(res) = 1)
        End Using
    End Function

    Private Function ExecuteIntScalar(activeConn As SqlConnection, sql As String, Optional paramName As String = "", Optional paramVal As Object = Nothing) As Integer
        Using cmd As New SqlCommand(sql, activeConn)
            If Not String.IsNullOrWhiteSpace(paramName) AndAlso paramVal IsNot Nothing Then
                cmd.Parameters.AddWithValue(paramName, paramVal)
            End If
            Dim res = cmd.ExecuteScalar()
            If res Is Nothing OrElse IsDBNull(res) Then Return 0
            Return Convert.ToInt32(res)
        End Using
    End Function

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

    Private Function FindCardNumberLabel(keyword As String) As Label
        Dim labels As List(Of Label) = GetAllLabelsRecursive(Me)

        For Each titleLbl As Label In labels
            If Not String.IsNullOrWhiteSpace(titleLbl.Text) AndAlso
               titleLbl.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 AndAlso
               titleLbl.Parent IsNot Nothing Then

                Dim bestLbl As Label = Nothing
                Dim bestDist As Integer = Integer.MaxValue

                For Each candidate As Label In GetAllLabelsRecursive(titleLbl.Parent)
                    If candidate Is titleLbl Then Continue For
                    Dim txt As String = candidate.Text.Trim()
                    If IsNumeric(txt) OrElse txt = "0" Then
                        Dim dist As Integer = Math.Abs(candidate.Top - titleLbl.Top) + Math.Abs(candidate.Left - titleLbl.Left)
                        If dist < bestDist Then
                            bestDist = dist
                            bestLbl = candidate
                        End If
                    End If
                Next

                If bestLbl IsNot Nothing Then Return bestLbl
            End If
        Next
        Return Nothing
    End Function

    Public Sub LoadDashboardCards()
        Try
            Dim activeConn As SqlConnection = GetOpenBaConnection()
            Dim hasPatients As Boolean = TableExists(activeConn, "dbo.patient_information")
            Dim hasAppointments As Boolean = TableExists(activeConn, "dbo.Appointments")

            ' 1. Todays Patient
            Dim todaysPatients As Integer = 0
            If hasPatients Then
                If ColumnExists(activeConn, "dbo.patient_information", "created_account") Then
                    todaysPatients = ExecuteIntScalar(
                        activeConn,
                        "SELECT COUNT(*) FROM dbo.patient_information WHERE CAST(created_account AS DATE) = CAST(GETDATE() AS DATE)")
                    If todaysPatients = 0 Then
                        todaysPatients = ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.patient_information")
                    End If
                Else
                    todaysPatients = ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.patient_information")
                End If
            End If

            Dim lblTodayPat As Label = FindCardNumberLabel("Todays Patient")
            If lblTodayPat Is Nothing Then lblTodayPat = FindCardNumberLabel("Today")
            If lblTodayPat IsNot Nothing Then lblTodayPat.Text = todaysPatients.ToString()

            ' 2. Appointments & 3. Pending Request
            Dim apptCount As Integer = 0
            Dim pendingCount As Integer = 0
            If hasAppointments Then
                apptCount = ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.Appointments")
                If ColumnExists(activeConn, "dbo.Appointments", "Status") Then
                    pendingCount = ExecuteIntScalar(
                        activeConn,
                        "SELECT COUNT(*) FROM dbo.Appointments WHERE LOWER(LTRIM(RTRIM(ISNULL(Status,'')))) = 'pending'")
                End If
            End If

            Dim lblAppts As Label = FindCardNumberLabel("Appointment")
            If lblAppts IsNot Nothing Then lblAppts.Text = apptCount.ToString()

            Dim lblPending As Label = FindCardNumberLabel("Pending")
            If lblPending IsNot Nothing Then lblPending.Text = pendingCount.ToString()

            ' 4. Reports
            Dim reportCount As Integer = 0
            If TableExists(activeConn, "dbo.patient_consultations") Then
                reportCount += ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.patient_consultations")
            End If
            If TableExists(activeConn, "dbo.medical_records") Then
                reportCount += ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.medical_records")
            End If
            If reportCount = 0 Then
                reportCount = apptCount + todaysPatients
            End If

            Dim lblReports As Label = FindCardNumberLabel("Reports")
            If lblReports IsNot Nothing Then lblReports.Text = reportCount.ToString()

        Catch
        End Try
    End Sub

    Private Function FindChartsInDashboard(parent As Control) As List(Of Control)
        Dim list As New List(Of Control)()
        If parent Is Nothing Then Return list

        For Each c As Control In parent.Controls
            Dim tName As String = c.GetType().FullName
            If TypeOf c Is Chart OrElse
               tName.IndexOf("GunaChart", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               c.Name.IndexOf("Chart", StringComparison.OrdinalIgnoreCase) >= 0 Then
                list.Add(c)
            End If
            If c.HasChildren Then
                list.AddRange(FindChartsInDashboard(c))
            End If
        Next
        Return list
    End Function

    Private Sub BindWinFormsChartSeries(ch As Chart, labels() As String, values() As Integer)
        If ch Is Nothing Then Return
        If ch.Series.Count = 0 Then
            ch.Series.Add("Visits")
        End If
        ch.Series(0).Points.Clear()
        For i As Integer = 0 To Math.Min(labels.Length, values.Length) - 1
            ch.Series(0).Points.AddXY(labels(i), values(i))
        Next
        ch.Invalidate()
    End Sub

    Private Sub BindGunaChartDataset(gunaCtrl As Control, labels() As String, values() As Integer)
        If gunaCtrl Is Nothing Then Return
        Try
            Dim dsProp = gunaCtrl.GetType().GetProperty("Datasets")
            If dsProp IsNot Nothing Then
                Dim datasets = TryCast(dsProp.GetValue(gunaCtrl, Nothing), System.Collections.IEnumerable)
                If datasets IsNot Nothing Then
                    For Each ds In datasets
                        Dim dpProp = ds.GetType().GetProperty("DataPoints")
                        If dpProp IsNot Nothing Then
                            Dim dpObj = dpProp.GetValue(ds, Nothing)
                            Dim clearMethod = dpObj.GetType().GetMethod("Clear", Type.EmptyTypes)
                            If clearMethod IsNot Nothing Then clearMethod.Invoke(dpObj, Nothing)

                            Dim addMethod = dpObj.GetType().GetMethod("Add", New Type() {GetType(String), GetType(Double)})
                            If addMethod IsNot Nothing Then
                                For i As Integer = 0 To Math.Min(labels.Length, values.Length) - 1
                                    addMethod.Invoke(dpObj, New Object() {labels(i), CDbl(values(i))})
                                Next
                            End If
                        End If
                    Next
                End If
            End If

            Dim updateMethod = gunaCtrl.GetType().GetMethod("Update", Type.EmptyTypes)
            If updateMethod IsNot Nothing Then updateMethod.Invoke(gunaCtrl, Nothing)
            gunaCtrl.Invalidate()
        Catch
        End Try
    End Sub

    Public Sub LoadVisitCharts()
        Try
            Dim activeConn As SqlConnection = GetOpenBaConnection()

            Dim monthsOrder() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
            Dim monthCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each m As String In monthsOrder
                monthCounts(m) = 0
            Next

            Dim daysOrder() As String = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
            Dim dayCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each d As String In daysOrder
                dayCounts(d) = 0
            Next

            If TableExists(activeConn, "dbo.patient_information") AndAlso ColumnExists(activeConn, "dbo.patient_information", "created_account") Then
                Using mCmd As New SqlCommand(
                    "SELECT MONTH(created_account) AS m_num, COUNT(*) AS total " &
                    "FROM dbo.patient_information " &
                    "WHERE created_account IS NOT NULL " &
                    "GROUP BY MONTH(created_account)", activeConn)
                    Using r As SqlDataReader = mCmd.ExecuteReader()
                        While r.Read()
                            If Not IsDBNull(r("m_num")) Then
                                Dim mIdx As Integer = Convert.ToInt32(r("m_num")) - 1
                                If mIdx >= 0 AndAlso mIdx < monthsOrder.Length Then
                                    monthCounts(monthsOrder(mIdx)) = Convert.ToInt32(r("total"))
                                End If
                            End If
                        End While
                    End Using
                End Using

                Using dCmd As New SqlCommand(
                    "SELECT DATENAME(WEEKDAY, created_account) AS d_name, COUNT(*) AS total " &
                    "FROM dbo.patient_information " &
                    "WHERE created_account IS NOT NULL " &
                    "GROUP BY DATENAME(WEEKDAY, created_account)", activeConn)
                    Using r As SqlDataReader = dCmd.ExecuteReader()
                        While r.Read()
                            If Not IsDBNull(r("d_name")) Then
                                Dim dName As String = r("d_name").ToString().Trim()
                                If dayCounts.ContainsKey(dName) Then
                                    dayCounts(dName) = Convert.ToInt32(r("total"))
                                End If
                            End If
                        End While
                    End Using
                End Using
            End If

            Dim mVals(monthsOrder.Length - 1) As Integer
            For i As Integer = 0 To monthsOrder.Length - 1
                mVals(i) = monthCounts(monthsOrder(i))
            Next

            Dim dVals(daysOrder.Length - 1) As Integer
            For i As Integer = 0 To daysOrder.Length - 1
                dVals(i) = dayCounts(daysOrder(i))
            Next

            If GunaChart1 IsNot Nothing Then
                BindGunaChartDataset(GunaChart1, monthsOrder, mVals)
            End If

            Dim charts As List(Of Control) = FindChartsInDashboard(Me)
            For Each chCtrl As Control In charts
                Dim isDaily As Boolean = (chCtrl.Name.IndexOf("Daily", StringComparison.OrdinalIgnoreCase) >= 0) OrElse
                                       (chCtrl.Parent IsNot Nothing AndAlso chCtrl.Parent.Controls.Cast(Of Control)().Any(
                                           Function(x) TypeOf x Is Label AndAlso x.Text.IndexOf("Daily", StringComparison.OrdinalIgnoreCase) >= 0))

                If TypeOf chCtrl Is Chart Then
                    Dim winChart As Chart = DirectCast(chCtrl, Chart)
                    If isDaily Then
                        BindWinFormsChartSeries(winChart, daysOrder, dVals)
                    Else
                        BindWinFormsChartSeries(winChart, monthsOrder, mVals)
                    End If
                ElseIf chCtrl IsNot GunaChart1 Then
                    If isDaily Then
                        BindGunaChartDataset(chCtrl, daysOrder, dVals)
                    Else
                        BindGunaChartDataset(chCtrl, monthsOrder, mVals)
                    End If
                End If
            Next
        Catch
        End Try
    End Sub

    Public Sub LoadAppointmentsGrid()
        If Guna2DataGridView2 Is Nothing Then Return

        Try
            Dim activeConn As SqlConnection = GetOpenBaConnection()
            If Not TableExists(activeConn, "dbo.Appointments") Then Return

            Dim selectedApptId As Object = Nothing
            If Guna2DataGridView2.CurrentRow IsNot Nothing Then
                selectedApptId = Guna2DataGridView2.CurrentRow.Tag
            End If
            Dim firstVisible As Integer = If(Guna2DataGridView2.FirstDisplayedScrollingRowIndex >= 0,
                                             Guna2DataGridView2.FirstDisplayedScrollingRowIndex, 0)

            Dim sql As String =
                "SELECT TOP 25 AppointmentID, AppointmentDate, AppointmentTime, PatientName, ServiceType, Status " &
                "FROM dbo.Appointments " &
                "ORDER BY AppointmentDate DESC, AppointmentID DESC"

            Using cmd As New SqlCommand(sql, activeConn)
                Using r As SqlDataReader = cmd.ExecuteReader()
                    Guna2DataGridView2.Rows.Clear()

                    While r.Read()
                        Dim timeStr As String = ""
                        If Not IsDBNull(r("AppointmentTime")) AndAlso Not String.IsNullOrWhiteSpace(r("AppointmentTime").ToString()) Then
                            timeStr = r("AppointmentTime").ToString().Trim()
                        ElseIf Not IsDBNull(r("AppointmentDate")) Then
                            timeStr = Convert.ToDateTime(r("AppointmentDate")).ToString("hh:mm tt")
                        Else
                            timeStr = "N/A"
                        End If

                        Dim patName As String = If(IsDBNull(r("PatientName")), "Unknown", r("PatientName").ToString().Trim())
                        Dim serviceStr As String = If(IsDBNull(r("ServiceType")), "General Checkup", r("ServiceType").ToString().Trim())
                        Dim statusStr As String = If(IsDBNull(r("Status")), "Pending", r("Status").ToString().Trim())

                        Dim rowIdx As Integer = Guna2DataGridView2.Rows.Add(timeStr, patName, serviceStr, statusStr)
                        Guna2DataGridView2.Rows(rowIdx).Tag = r("AppointmentID")

                        If selectedApptId IsNot Nothing AndAlso selectedApptId.ToString() = r("AppointmentID").ToString() Then
                            Guna2DataGridView2.CurrentCell = Guna2DataGridView2.Rows(rowIdx).Cells(0)
                        End If
                    End While
                End Using
            End Using

            If Guna2DataGridView2.Rows.Count > 0 AndAlso firstVisible < Guna2DataGridView2.Rows.Count Then
                Guna2DataGridView2.FirstDisplayedScrollingRowIndex = firstVisible
            End If
        Catch
        End Try
    End Sub

    Public Sub RefreshBarangayDashboard()
        LoadDashboardCards()
        LoadVisitCharts()
        LoadAppointmentsGrid()
    End Sub

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub

    Private Sub Barangay_Assistance_Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshBarangayDashboard()

        baRefreshTimer.Interval = 3000
        baRefreshTimer.Start()
    End Sub

    Private Sub baRefreshTimer_Tick(sender As Object, e As EventArgs) Handles baRefreshTimer.Tick
        If Not Me.IsDisposed AndAlso Me.Visible Then
            RefreshBarangayDashboard()
        End If
    End Sub

    Private Sub Guna2DataGridView2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView2.CellDoubleClick
        If e.RowIndex < 0 OrElse e.RowIndex >= Guna2DataGridView2.Rows.Count Then Return

        Dim row As DataGridViewRow = Guna2DataGridView2.Rows(e.RowIndex)
        If row.Tag Is Nothing OrElse Not IsNumeric(row.Tag.ToString()) Then Return

        Dim apptId As Integer = Convert.ToInt32(row.Tag)
        Dim patName As String = Convert.ToString(row.Cells(1).Value)
        Dim currentStatus As String = Convert.ToString(row.Cells(3).Value)

        Dim nextStatus As String = If(currentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase),
                                      "Confirmed",
                                      If(currentStatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase),
                                         "Completed",
                                         "Pending"))

        Dim ans = MessageBox.Show(
            "Update status for " & patName & " from '" & currentStatus & "' to '" & nextStatus & "'?",
            "Update Appointment Status",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If ans = DialogResult.Yes Then
            Try
                Dim activeConn As SqlConnection = GetOpenBaConnection()
                Using cmd As New SqlCommand("UPDATE dbo.Appointments SET Status = @st WHERE AppointmentID = @id", activeConn)
                    cmd.Parameters.Add("@st", SqlDbType.VarChar, 50).Value = nextStatus
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = apptId
                    cmd.ExecuteNonQuery()
                End Using

                Try
                    AdminFrm.LogNotification("updated appointment #" & apptId.ToString() & " to " & nextStatus)
                Catch
                End Try

                RefreshBarangayDashboard()
            Catch ex As Exception
                MessageBox.Show("Failed to update status: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub GunaChart1_Load(sender As Object, e As EventArgs) Handles GunaChart1.Load

    End Sub

End Class