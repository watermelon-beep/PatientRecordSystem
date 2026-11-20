Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting
Imports Guna.UI2.WinForms

Public Class DoctorFrm

    Private docDashControl As doctordbs
    Private WithEvents docSyncTimer As New Timer()

    Private ReadOnly fallbackConnStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private Function GetOpenDoctorConnection() As SqlConnection
        If conn Is Nothing OrElse String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            Try
                dbConnection()
            Catch
            End Try
        End If

        If conn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If
            Return conn
        End If

        Dim tempConn As New SqlConnection(fallbackConnStr)
        tempConn.Open()
        Return tempConn
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

    Private Function ExecuteIntScalar(activeConn As SqlConnection, sql As String) As Integer
        Using cmd As New SqlCommand(sql, activeConn)
            Dim res = cmd.ExecuteScalar()
            If res Is Nothing OrElse IsDBNull(res) Then Return 0
            Return Convert.ToInt32(res)
        End Using
    End Function

    Private Function GetAllControlsRecursive(parent As Control) As List(Of Control)
        Dim list As New List(Of Control)()
        If parent Is Nothing Then Return list

        For Each c As Control In parent.Controls
            list.Add(c)
            If c.HasChildren Then
                list.AddRange(GetAllControlsRecursive(c))
            End If
        Next
        Return list
    End Function

    Private Sub SetActiveSidebarButton(activeBtn As Guna2Button)
        Dim navButtons() As Guna2Button = {dashbtn, patientRecbtn, DrConsultation}
        For Each btn As Guna2Button In navButtons
            If btn Is Nothing Then Continue For
            If btn Is activeBtn Then
                btn.FillColor = Color.FromArgb(4, 196, 80)
                btn.ForeColor = Color.White
            Else
                btn.FillColor = Color.Transparent
                btn.ForeColor = Color.White
            End If
        Next
    End Sub

    Private Function FindStaffPhotoColumnName(activeConn As SqlConnection) As String
        Try
            Dim sql As String =
                "SELECT TOP 1 name FROM sys.columns " &
                "WHERE object_id = OBJECT_ID('dbo.staff_information') " &
                "AND name IN ('user_photo', 'staff_photo', 'profile_photo', 'photo', 'avatar')"
            Using cmd As New SqlCommand(sql, activeConn)
                Dim res = cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    Return res.ToString()
                End If
            End Using
        Catch
        End Try
        Return ""
    End Function

    Private Sub LoadDoctorPhoto(reader As SqlDataReader, colName As String)
        If String.IsNullOrWhiteSpace(colName) OrElse Guna2CirclePictureBox2 Is Nothing Then Return
        Try
            If Not IsDBNull(reader(colName)) Then
                Dim bytes() As Byte = TryCast(reader(colName), Byte())
                If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                    Using ms As New IO.MemoryStream(bytes)
                        Using tmp As Image = Image.FromStream(ms)
                            If Guna2CirclePictureBox2.Image IsNot Nothing Then
                                Guna2CirclePictureBox2.Image.Dispose()
                            End If
                            Guna2CirclePictureBox2.Image = New Bitmap(tmp)
                        End Using
                    End Using
                End If
            End If
        Catch
        End Try
    End Sub

    Public Sub doctorProfile()
        Try
            Dim activeConn As SqlConnection = GetOpenDoctorConnection()
            If Not TableExists(activeConn, "dbo.staff_information") Then Return

            Dim photoCol As String = FindStaffPhotoColumnName(activeConn)
            Dim photoSelect As String = If(String.IsNullOrWhiteSpace(photoCol), "", ", " & photoCol)

            Dim hasUsername As Boolean = (Not String.IsNullOrWhiteSpace(currentUsername))
            Dim sql As String = If(
                hasUsername,
                "SELECT TOP 1 first_Name, Position" & photoSelect & " FROM dbo.staff_information WHERE LTRIM(RTRIM(username)) = @username",
                "SELECT TOP 1 first_Name, Position" & photoSelect & " FROM dbo.staff_information ORDER BY CASE WHEN LOWER(LTRIM(RTRIM(Position))) = 'doctor' THEN 0 ELSE 1 END, staff_Id"
            )

            Using comm As New SqlCommand(sql, activeConn)
                If hasUsername Then
                    comm.Parameters.Add("@username", SqlDbType.VarChar, 100).Value = currentUsername.Trim()
                End If

                Using dataRead As SqlDataReader = comm.ExecuteReader()
                    If dataRead.Read() Then
                        Dim firstname As String = If(IsDBNull(dataRead("first_Name")), "Doctor", dataRead("first_Name").ToString().Trim())
                        Dim position As String = If(IsDBNull(dataRead("Position")), "Doctor", dataRead("Position").ToString().Trim())

                        Label3.Text = "Welcome, " & position & ", " & firstname & "!"
                        Label1.Text = firstname
                        Label2.Text = position
                        LoadDoctorPhoto(dataRead, photoCol)
                    Else
                        Label3.Text = "Welcome, Unknown user!"
                        Label1.Text = "Unknown user"
                        Label2.Text = "Unknown position"
                    End If
                End Using
            End Using
        Catch
        End Try
    End Sub

    Private Function GetStaffPasswordColumnName(activeConn As SqlConnection) As String
        Try
            Dim sql As String =
                "SELECT TOP 1 name FROM sys.columns " &
                "WHERE object_id = OBJECT_ID('dbo.staff_information') " &
                "AND name IN ('user_password', 'password')"
            Using cmd As New SqlCommand(sql, activeConn)
                Dim res = cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    Return res.ToString()
                End If
            End Using
        Catch
        End Try
        Return "user_password"
    End Function

    Public Sub OpenEditDoctorPasswordDialog()
        Try
            Dim activeConn As SqlConnection = GetOpenDoctorConnection()
            Dim passCol As String = GetStaffPasswordColumnName(activeConn)
            Dim hasUser As Boolean = (Not String.IsNullOrWhiteSpace(currentUsername))

            Dim selSql As String = If(
                hasUser,
                "SELECT TOP 1 staff_Id, username, " & passCol & " AS pass_val FROM dbo.staff_information WHERE LTRIM(RTRIM(username)) = @username",
                "SELECT TOP 1 staff_Id, username, " & passCol & " AS pass_val FROM dbo.staff_information ORDER BY CASE WHEN LOWER(LTRIM(RTRIM(Position))) = 'doctor' THEN 0 ELSE 1 END, staff_Id"
            )

            Dim staffId As Integer = 0
            Dim targetUsername As String = If(hasUser, currentUsername.Trim(), "")
            Dim currentPassVal As String = ""

            Using selCmd As New SqlCommand(selSql, activeConn)
                If hasUser Then
                    selCmd.Parameters.Add("@username", SqlDbType.VarChar, 100).Value = targetUsername
                End If
                Using r As SqlDataReader = selCmd.ExecuteReader()
                    If Not r.Read() Then
                        MessageBox.Show("Account record not found.", "Edit Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                    staffId = Convert.ToInt32(r("staff_Id"))
                    targetUsername = If(IsDBNull(r("username")), targetUsername, r("username").ToString().Trim())
                    currentPassVal = If(IsDBNull(r("pass_val")), "", r("pass_val").ToString())
                End Using
            End Using

            Using dlg As New Form()
                dlg.Text = "Edit Password - " & If(String.IsNullOrWhiteSpace(targetUsername), "Staff #" & staffId.ToString(), targetUsername)
                dlg.Size = New Size(470, 175)
                dlg.StartPosition = FormStartPosition.CenterParent
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog
                dlg.MaximizeBox = False
                dlg.MinimizeBox = False
                dlg.BackColor = Color.White
                dlg.Font = New Font("Segoe UI", 9.5F)

                Dim lblPass As New Label() With {.Text = "Password *:", .Left = 18, .Top = 26, .Width = 120}
                Dim txtPass As New TextBox() With {.Left = 142, .Top = 22, .Width = 295, .Text = currentPassVal}

                Dim btnSave As New Button() With {
                    .Text = "Save Changes",
                    .Left = 222,
                    .Top = 78,
                    .Width = 115,
                    .Height = 34,
                    .BackColor = Color.FromArgb(76, 175, 80),
                    .ForeColor = Color.White,
                    .FlatStyle = FlatStyle.Flat
                }
                Dim btnCancel As New Button() With {
                    .Text = "Cancel",
                    .Left = 345,
                    .Top = 78,
                    .Width = 92,
                    .Height = 34,
                    .BackColor = Color.FromArgb(130, 130, 130),
                    .ForeColor = Color.White,
                    .FlatStyle = FlatStyle.Flat,
                    .DialogResult = DialogResult.Cancel
                }

                AddHandler btnSave.Click, Sub(s, ev)
                                              If String.IsNullOrWhiteSpace(txtPass.Text) Then
                                                  MessageBox.Show("Password is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                  Return
                                              End If
                                              Try
                                                  Dim c As SqlConnection = GetOpenDoctorConnection()
                                                  Using updCmd As New SqlCommand("UPDATE dbo.staff_information SET " & passCol & " = @p WHERE staff_Id = @id", c)
                                                      updCmd.Parameters.Add("@p", SqlDbType.VarChar, 150).Value = txtPass.Text
                                                      updCmd.Parameters.Add("@id", SqlDbType.Int).Value = staffId
                                                      updCmd.ExecuteNonQuery()
                                                  End Using
                                                  dlg.DialogResult = DialogResult.OK
                                                  dlg.Close()
                                              Catch ex As Exception
                                                  MessageBox.Show("Failed to update password: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                              End Try
                                          End Sub

                dlg.Controls.AddRange(New Control() {lblPass, txtPass, btnSave, btnCancel})
                dlg.AcceptButton = btnSave
                dlg.CancelButton = btnCancel

                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    MessageBox.Show("Password updated successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error opening password editor: " & ex.Message, "Edit Password", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== REAL-TIME DOCTOR DASHBOARD (doctordbs) =====================

    Private Function FindDashboardNumberLabel(dashRoot As Control, titleKeyword As String) As Label
        If dashRoot Is Nothing Then Return Nothing
        Dim labels As List(Of Label) = GetAllControlsRecursive(dashRoot).OfType(Of Label)().ToList()

        For Each titleLbl As Label In labels
            If Not String.IsNullOrWhiteSpace(titleLbl.Text) AndAlso
               titleLbl.Text.IndexOf(titleKeyword, StringComparison.OrdinalIgnoreCase) >= 0 AndAlso
               titleLbl.Parent IsNot Nothing Then

                Dim bestLbl As Label = Nothing
                Dim bestDist As Integer = Integer.MaxValue

                For Each candidate As Label In GetAllControlsRecursive(titleLbl.Parent).OfType(Of Label)()
                    If candidate Is titleLbl Then Continue For
                    Dim txt As String = candidate.Text.Trim()
                    If IsNumeric(txt) OrElse txt = "0" OrElse txt = "-" Then
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

    Private Sub BindChartData(ctrl As Control, labels() As String, values() As Integer)
        If ctrl Is Nothing Then Return

        If TypeOf ctrl Is Chart Then
            Dim winChart As Chart = DirectCast(ctrl, Chart)
            If winChart.Series.Count = 0 Then
                winChart.Series.Add("Data")
            End If
            winChart.Series(0).Points.Clear()
            For i As Integer = 0 To Math.Min(labels.Length, values.Length) - 1
                winChart.Series(0).Points.AddXY(labels(i), values(i))
            Next
            winChart.Invalidate()
            Return
        End If

        Try
            Dim dsProp = ctrl.GetType().GetProperty("Datasets")
            If dsProp IsNot Nothing Then
                Dim datasets = TryCast(dsProp.GetValue(ctrl, Nothing), System.Collections.IEnumerable)
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

            Dim updateMethod = ctrl.GetType().GetMethod("Update", Type.EmptyTypes)
            If updateMethod IsNot Nothing Then updateMethod.Invoke(ctrl, Nothing)
            ctrl.Invalidate()
        Catch
        End Try
    End Sub

    Private Sub PopulateRecentPatientsPanel(dashRoot As Control, activeConn As SqlConnection)
        If dashRoot Is Nothing OrElse Not TableExists(activeConn, "dbo.patient_information") Then Return

        Dim viewButtons As List(Of Control) = GetAllControlsRecursive(dashRoot).Where(
            Function(c) (TypeOf c Is Button OrElse TypeOf c Is Guna2Button) AndAlso
                        c.Text.Trim().Equals("View", StringComparison.OrdinalIgnoreCase)).
            OrderBy(Function(c) c.PointToScreen(Point.Empty).Y).ToList()

        If viewButtons.Count = 0 Then Return

        Dim dt As New DataTable()
        Dim hasCreatedAcct As Boolean = ColumnExists(activeConn, "dbo.patient_information", "created_account")
        Dim dateExpr As String = If(hasCreatedAcct, "FORMAT(created_account, 'MMM dd, yyyy')", "FORMAT(GETDATE(), 'MMM dd, yyyy')")

        Dim sql As String =
            "SELECT TOP " & viewButtons.Count.ToString() & " " &
            "patient_Id, " &
            "LTRIM(RTRIM(ISNULL(first_Name,'') + ' ' + ISNULL(middle_Name,'') + ' ' + ISNULL(surname,''))) AS full_name, " &
            "ISNULL(CAST(age AS VARCHAR(10)) + ' years', 'N/A') AS age_text, " &
            "ISNULL(gender, 'N/A') AS gender_text, " &
            dateExpr & " AS visit_date " &
            "FROM dbo.patient_information ORDER BY patient_Id DESC"

        Using da As New SqlDataAdapter(sql, activeConn)
            da.Fill(dt)
        End Using

        For i As Integer = 0 To viewButtons.Count - 1
            Dim btn As Control = viewButtons(i)
            Dim container As Control = btn.Parent
            If container Is Nothing Then Continue For

            If i < dt.Rows.Count Then
                container.Visible = True
                Dim row As DataRow = dt.Rows(i)
                Dim fullName As String = System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row("full_name")).Trim(), "\s+", " ")
                Dim subtitle As String = Convert.ToString(row("age_text")) & " • " & Convert.ToString(row("gender_text"))
                Dim visitDate As String = Convert.ToString(row("visit_date"))

                Dim rowLabels As List(Of Label) = GetAllControlsRecursive(container).OfType(Of Label)().
                    OrderBy(Function(l) l.Left).ThenBy(Function(l) l.Top).ToList()

                If rowLabels.Count >= 1 Then rowLabels(0).Text = fullName
                If rowLabels.Count >= 2 Then rowLabels(1).Text = subtitle
                If rowLabels.Count >= 3 Then rowLabels(2).Text = visitDate

                btn.Tag = row("patient_Id")
                RemoveHandler btn.Click, AddressOf HandleRecentPatientViewClick
                AddHandler btn.Click, AddressOf HandleRecentPatientViewClick
            Else
                container.Visible = False
            End If
        Next
    End Sub

    Private Sub HandleRecentPatientViewClick(sender As Object, e As EventArgs)
        Dim btn As Control = TryCast(sender, Control)
        If btn Is Nothing OrElse btn.Tag Is Nothing Then Return

        Dim patId As String = btn.Tag.ToString()
        Try
            Dim activeConn As SqlConnection = GetOpenDoctorConnection()
            Using cmd As New SqlCommand(
                "SELECT TOP 1 first_Name, middle_Name, surname, age, gender, phone_number, user_address " &
                "FROM dbo.patient_information WHERE patient_Id = @id", activeConn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(patId)
                Using r As SqlDataReader = cmd.ExecuteReader()
                    If r.Read() Then
                        Dim info As String =
                            "Patient ID: " & patId & Environment.NewLine &
                            "Name: " & Convert.ToString(r("first_Name")) & " " & Convert.ToString(r("surname")) & Environment.NewLine &
                            "Age / Gender: " & Convert.ToString(r("age")) & " / " & Convert.ToString(r("gender")) & Environment.NewLine &
                            "Phone: " & Convert.ToString(r("phone_number")) & Environment.NewLine &
                            "Address: " & Convert.ToString(r("user_address"))
                        MessageBox.Show(info, "Patient Record Details", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Unable to load patient details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub RefreshDoctorDashboardRealtime()
        If docDashControl Is Nothing OrElse docDashControl.IsDisposed Then Return

        Try
            Dim activeConn As SqlConnection = GetOpenDoctorConnection()
            Dim hasPatients As Boolean = TableExists(activeConn, "dbo.patient_information")
            Dim hasAppointments As Boolean = TableExists(activeConn, "dbo.Appointments")

            Dim totalPatients As Integer = 0
            If hasPatients Then
                totalPatients = ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.patient_information")
            End If

            Dim todaysAppointments As Integer = 0
            Dim pendingReports As Integer = 0
            If hasAppointments Then
                todaysAppointments = ExecuteIntScalar(
                    activeConn,
                    "SELECT COUNT(*) FROM dbo.Appointments WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)")
                If todaysAppointments = 0 Then
                    todaysAppointments = ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM dbo.Appointments")
                End If

                If ColumnExists(activeConn, "dbo.Appointments", "Status") Then
                    pendingReports = ExecuteIntScalar(
                        activeConn,
                        "SELECT COUNT(*) FROM dbo.Appointments WHERE LOWER(LTRIM(RTRIM(ISNULL(Status,'')))) = 'pending'")
                End If
            End If

            Dim medicalRecords As Integer = 0
            For Each tblName As String In {"dbo.medical_records", "dbo.patient_consultations", "dbo.consultations"}
                If TableExists(activeConn, tblName) Then
                    medicalRecords += ExecuteIntScalar(activeConn, "SELECT COUNT(*) FROM " & tblName)
                End If
            Next

            Dim lblTotalPat As Label = FindDashboardNumberLabel(docDashControl, "Total Patients")
            Dim lblTodayAppt As Label = FindDashboardNumberLabel(docDashControl, "Appointment")
            Dim lblMedRec As Label = FindDashboardNumberLabel(docDashControl, "Medical")
            Dim lblPendingRep As Label = FindDashboardNumberLabel(docDashControl, "Pending")

            If lblTotalPat IsNot Nothing Then lblTotalPat.Text = totalPatients.ToString()
            If lblTodayAppt IsNot Nothing Then lblTodayAppt.Text = todaysAppointments.ToString()
            If lblMedRec IsNot Nothing Then lblMedRec.Text = medicalRecords.ToString()
            If lblPendingRep IsNot Nothing Then lblPendingRep.Text = pendingReports.ToString()

            PopulateRecentPatientsPanel(docDashControl, activeConn)

            ' Patient Records Donut Chart (New Patient, Follow-Up, Completed)
            Dim newCount As Integer = Math.Max(0, totalPatients - medicalRecords)
            Dim followUpCount As Integer = pendingReports
            Dim completedCount As Integer = medicalRecords
            Dim donutLabels() As String = {"New Patient", "Follow-Up", "Completed"}
            Dim donutValues() As Integer = {newCount, followUpCount, completedCount}

            ' Patient Statistics Monthly Chart (Jan - Dec)
            Dim monthsOrder() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
            Dim monthMap As New Dictionary(Of Integer, Integer)()
            For m As Integer = 1 To 12
                monthMap(m) = 0
            Next

            If hasPatients AndAlso ColumnExists(activeConn, "dbo.patient_information", "created_account") Then
                Using mCmd As New SqlCommand(
                    "SELECT MONTH(created_account) AS m_idx, COUNT(*) AS cnt " &
                    "FROM dbo.patient_information WHERE created_account IS NOT NULL GROUP BY MONTH(created_account)", activeConn)
                    Using r As SqlDataReader = mCmd.ExecuteReader()
                        While r.Read()
                            If Not IsDBNull(r("m_idx")) Then
                                Dim mNum As Integer = Convert.ToInt32(r("m_idx"))
                                If monthMap.ContainsKey(mNum) Then
                                    monthMap(mNum) = Convert.ToInt32(r("cnt"))
                                End If
                            End If
                        End While
                    End Using
                End Using
            End If

            Dim monthValues(11) As Integer
            For idx As Integer = 0 To 11
                monthValues(idx) = monthMap(idx + 1)
            Next

            Dim chartControls As List(Of Control) = GetAllControlsRecursive(docDashControl).Where(
                Function(c) TypeOf c Is Chart OrElse
                            c.GetType().FullName.IndexOf("GunaChart", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                            c.Name.IndexOf("Chart", StringComparison.OrdinalIgnoreCase) >= 0).ToList()

            For Each chCtrl As Control In chartControls
                Dim isStats As Boolean = (chCtrl.Name.IndexOf("Stat", StringComparison.OrdinalIgnoreCase) >= 0) OrElse
                                         (chCtrl.Parent IsNot Nothing AndAlso GetAllControlsRecursive(chCtrl.Parent).OfType(Of Label)().Any(
                                             Function(l) l.Text.IndexOf("Statistics", StringComparison.OrdinalIgnoreCase) >= 0))
                If isStats Then
                    BindChartData(chCtrl, monthsOrder, monthValues)
                Else
                    BindChartData(chCtrl, donutLabels, donutValues)
                End If
            Next

        Catch
        End Try
    End Sub

    Private Sub ShowDoctorDashboard()
        If docDashControl Is Nothing OrElse docDashControl.IsDisposed Then
            docDashControl = New doctordbs()
        End If
        ChangeUsrCnrl(docDashControl, Guna2Panel3)
        SetActiveSidebarButton(dashbtn)
        RefreshDoctorDashboardRealtime()
    End Sub

    Private Sub DoctorFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            GetOpenDoctorConnection()
        Catch
        End Try

        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
        Label4.Text = "Here's what's happening at the clinic today."
        Label6.Text = "Barangay" & Environment.NewLine & "Health Clinic"

        doctorProfile()
        ShowDoctorDashboard()

        Timer1.Interval = 1000
        Timer1.Start()

        docSyncTimer.Interval = 3000
        docSyncTimer.Start()
    End Sub

    Private Sub docSyncTimer_Tick(sender As Object, e As EventArgs) Handles docSyncTimer.Tick
        If Not Me.IsDisposed AndAlso Me.Visible Then
            doctorProfile()
            RefreshDoctorDashboardRealtime()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
    End Sub

    Private Sub dashbtn_Click(sender As Object, e As EventArgs) Handles dashbtn.Click
        doctorProfile()
        ShowDoctorDashboard()
    End Sub

    Private Sub patientRecbtn_Click(sender As Object, e As EventArgs) Handles patientRecbtn.Click
        Dim PatientRecordDctr As New PatientRecord()
        ChangeUsrCnrl(PatientRecordDctr, Guna2Panel3)
        SetActiveSidebarButton(patientRecbtn)
    End Sub

    Private Sub DrConsultation_Click(sender As Object, e As EventArgs) Handles DrConsultation.Click
        Dim consultationForm As New DrConsultation()
        ChangeUsrCnrl(consultationForm, Guna2Panel3)
        SetActiveSidebarButton(DrConsultation)
    End Sub

    Private Sub Label1_DoubleClick(sender As Object, e As EventArgs) Handles Label1.DoubleClick, Label2.DoubleClick
        OpenEditDoctorPasswordDialog()
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        docSyncTimer.Stop()
        Timer1.Stop()
        currentUsername = ""
        LogForm.Show()
        Me.Hide()
    End Sub

    Private Sub DoctorFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        docSyncTimer.Stop()
        Timer1.Stop()
        Application.Exit()
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

    Private Sub Guna2ShadowPanel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2ShadowPanel2.Paint

    End Sub

End Class