Imports System.Data.SqlClient
Imports Guna.UI2.WinForms

Public Class Appointment

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private WithEvents apptRefreshTimer As New Timer()
    Private actionToolbar As Panel
    Private btnAddAppt As Button
    Private btnEditAppt As Button
    Private btnDeleteAppt As Button

    Private Sub Appointment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnsureActionControls()
        SetupGrid()
        RefreshAllAppointmentData()

        apptRefreshTimer.Interval = 3000
        apptRefreshTimer.Start()
    End Sub

    Private Sub apptRefreshTimer_Tick(sender As Object, e As EventArgs) Handles apptRefreshTimer.Tick
        If Me.Visible AndAlso Not Me.IsDisposed Then
            RefreshAllAppointmentData()
        End If
    End Sub

    Public Sub RefreshAllAppointmentData()
        LoadAppointments()
        LoadCounts()
    End Sub

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

    Private Sub HookExistingDesignerButtons()
        For Each ctrl As Control In GetAllControlsRecursive(Me)
            If TypeOf ctrl Is Button OrElse TypeOf ctrl Is Guna2Button Then
                Dim combined As String = (ctrl.Name & " " & ctrl.Text).ToLowerInvariant()
                If combined.Contains("add") OrElse combined.Contains("new") OrElse combined.Contains("create") Then
                    RemoveHandler ctrl.Click, AddressOf HandleAddClick
                    AddHandler ctrl.Click, AddressOf HandleAddClick
                ElseIf combined.Contains("edit") OrElse combined.Contains("update") Then
                    RemoveHandler ctrl.Click, AddressOf HandleEditClick
                    AddHandler ctrl.Click, AddressOf HandleEditClick
                ElseIf combined.Contains("delete") OrElse combined.Contains("remove") Then
                    RemoveHandler ctrl.Click, AddressOf HandleDeleteClick
                    AddHandler ctrl.Click, AddressOf HandleDeleteClick
                End If
            End If
        Next
    End Sub

    Private Sub EnsureActionControls()
        HookExistingDesignerButtons()

        If appoint Is Nothing OrElse appoint.Parent Is Nothing Then Return
        If actionToolbar IsNot Nothing AndAlso Not actionToolbar.IsDisposed Then Return

        Dim parentCtrl As Control = appoint.Parent

        actionToolbar = New Panel() With {
            .Name = "pnlApptActions",
            .Height = 40,
            .BackColor = Color.Transparent,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }

        btnAddAppt = New Button() With {
            .Text = "+ Add Appointment",
            .Width = 145,
            .Height = 32,
            .Left = 0,
            .Top = 4,
            .BackColor = Color.FromArgb(76, 175, 80),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnAddAppt.FlatAppearance.BorderSize = 0
        AddHandler btnAddAppt.Click, AddressOf HandleAddClick

        btnEditAppt = New Button() With {
            .Text = "Edit Selected",
            .Width = 115,
            .Height = 32,
            .Left = 153,
            .Top = 4,
            .BackColor = Color.FromArgb(33, 150, 243),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnEditAppt.FlatAppearance.BorderSize = 0
        AddHandler btnEditAppt.Click, AddressOf HandleEditClick

        btnDeleteAppt = New Button() With {
            .Text = "Delete Selected",
            .Width = 125,
            .Height = 32,
            .Left = 276,
            .Top = 4,
            .BackColor = Color.FromArgb(211, 47, 47),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnDeleteAppt.FlatAppearance.BorderSize = 0
        AddHandler btnDeleteAppt.Click, AddressOf HandleDeleteClick

        actionToolbar.Controls.AddRange(New Control() {btnAddAppt, btnEditAppt, btnDeleteAppt})

        If TypeOf parentCtrl Is TableLayoutPanel Then
            Dim tlp As TableLayoutPanel = DirectCast(parentCtrl, TableLayoutPanel)
            Dim pos As TableLayoutPanelCellPosition = tlp.GetPositionFromControl(appoint)
            tlp.Controls.Remove(appoint)

            Dim wrapper As New Panel() With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.Transparent
            }
            actionToolbar.Dock = DockStyle.Top
            appoint.Dock = DockStyle.Fill
            wrapper.Controls.Add(appoint)
            wrapper.Controls.Add(actionToolbar)
            tlp.Controls.Add(wrapper, pos.Column, pos.Row)
        Else
            Dim origTop As Integer = appoint.Top
            Dim origLeft As Integer = appoint.Left
            Dim origWidth As Integer = appoint.Width
            Dim origHeight As Integer = appoint.Height

            actionToolbar.Left = origLeft
            actionToolbar.Top = Math.Max(0, origTop - actionToolbar.Height - 4)
            actionToolbar.Width = origWidth

            If actionToolbar.Top + actionToolbar.Height > origTop Then
                actionToolbar.Top = origTop
                appoint.Top = actionToolbar.Top + actionToolbar.Height + 4
                appoint.Height = Math.Max(120, origHeight - actionToolbar.Height - 4)
            End If

            parentCtrl.Controls.Add(actionToolbar)
            actionToolbar.BringToFront()
        End If
    End Sub

    Private Sub SetupGrid()
        With appoint
            .AutoGenerateColumns = False
            .Columns.Clear()
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colId",
                .HeaderText = "ID",
                .DataPropertyName = "AppointmentID",
                .FillWeight = 75
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colDateTime",
                .HeaderText = "Date & Time",
                .DataPropertyName = "DateTime",
                .FillWeight = 135
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colPatient",
                .HeaderText = "Patient Name",
                .DataPropertyName = "PatientName",
                .FillWeight = 130
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colService",
                .HeaderText = "Service",
                .DataPropertyName = "Service",
                .FillWeight = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = "colStatus",
                .HeaderText = "Status",
                .DataPropertyName = "Status",
                .FillWeight = 95
            })
            .Columns.Add(New DataGridViewButtonColumn With {
                .Name = "colActionEdit",
                .HeaderText = "Edit",
                .Text = "Edit",
                .UseColumnTextForButtonValue = True,
                .FillWeight = 65
            })
            .Columns.Add(New DataGridViewButtonColumn With {
                .Name = "colActionDelete",
                .HeaderText = "Delete",
                .Text = "Delete",
                .UseColumnTextForButtonValue = True,
                .FillWeight = 70
            })
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
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .RowTemplate.Height = 36
        End With
    End Sub

    Private Function GetSelectedAppointmentId(Optional rowIndex As Integer = -1) As Integer?
        Dim targetRow As DataGridViewRow = Nothing

        If rowIndex >= 0 AndAlso rowIndex < appoint.Rows.Count Then
            targetRow = appoint.Rows(rowIndex)
        ElseIf appoint.CurrentRow IsNot Nothing Then
            targetRow = appoint.CurrentRow
        ElseIf appoint.SelectedRows.Count > 0 Then
            targetRow = appoint.SelectedRows(0)
        End If

        If targetRow Is Nothing Then Return Nothing

        Dim drv As DataRowView = TryCast(targetRow.DataBoundItem, DataRowView)
        If drv IsNot Nothing AndAlso drv.Row.Table.Columns.Contains("RawAppointmentID") AndAlso Not IsDBNull(drv("RawAppointmentID")) Then
            Return Convert.ToInt32(drv("RawAppointmentID"))
        End If

        Dim rawText As String = Convert.ToString(targetRow.Cells(0).Value).Trim()
        If rawText.StartsWith("APT-", StringComparison.OrdinalIgnoreCase) Then
            rawText = rawText.Substring(4)
        End If

        Dim parsedId As Integer
        If Integer.TryParse(rawText, parsedId) Then
            Return parsedId
        End If

        Return Nothing
    End Function

    Private Function LoadPatientSuggestions() As List(Of String)
        Dim names As New List(Of String)()
        Try
            Using con As New SqlConnection(connStr)
                con.Open()
                Dim sql As String =
                    "SELECT DISTINCT LTRIM(RTRIM(ISNULL(first_Name,'') + ' ' + ISNULL(middle_Name,'') + ' ' + ISNULL(surname,''))) AS full_name " &
                    "FROM dbo.patient_information ORDER BY full_name"
                Using cmd As New SqlCommand(sql, con)
                    Using r As SqlDataReader = cmd.ExecuteReader()
                        While r.Read()
                            If Not IsDBNull(r("full_name")) Then
                                Dim cleanName As String = System.Text.RegularExpressions.Regex.Replace(r("full_name").ToString().Trim(), "\s+", " ")
                                If Not String.IsNullOrWhiteSpace(cleanName) AndAlso Not names.Contains(cleanName) Then
                                    names.Add(cleanName)
                                End If
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch
        End Try
        Return names
    End Function

    Private Sub ShowAppointmentEditor(Optional appointmentId As Integer? = Nothing)
        Dim isEdit As Boolean = appointmentId.HasValue
        Dim currentDateTime As DateTime = DateTime.Now
        Dim currentPatient As String = ""
        Dim currentService As String = "General Checkup"
        Dim currentStatus As String = "Pending"

        If isEdit Then
            Try
                Using con As New SqlConnection(connStr)
                    con.Open()
                    Using cmd As New SqlCommand(
                        "SELECT AppointmentDate, PatientName, Service, Status FROM dbo.Appointments WHERE AppointmentID = @id", con)
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = appointmentId.Value
                        Using r As SqlDataReader = cmd.ExecuteReader()
                            If Not r.Read() Then
                                MessageBox.Show("Selected appointment was not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                RefreshAllAppointmentData()
                                Return
                            End If
                            If Not IsDBNull(r("AppointmentDate")) Then currentDateTime = Convert.ToDateTime(r("AppointmentDate"))
                            currentPatient = If(IsDBNull(r("PatientName")), "", r("PatientName").ToString().Trim())
                            currentService = If(IsDBNull(r("Service")), "General Checkup", r("Service").ToString().Trim())
                            currentStatus = If(IsDBNull(r("Status")), "Pending", r("Status").ToString().Trim())
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Failed to load appointment: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End If

        Using dlg As New Form()
            dlg.Text = If(isEdit, "Edit Appointment - APT-" & appointmentId.Value.ToString(), "Add Appointment")
            dlg.Size = New Size(440, 290)
            dlg.StartPosition = FormStartPosition.CenterParent
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog
            dlg.MaximizeBox = False
            dlg.MinimizeBox = False
            dlg.BackColor = Color.White
            dlg.Font = New Font("Segoe UI", 9.5F)

            Dim lblDt As New Label() With {.Text = "Date & Time *:", .Left = 20, .Top = 24, .Width = 115}
            Dim dtpDate As New DateTimePicker() With {
                .Left = 142,
                .Top = 20,
                .Width = 145,
                .Format = DateTimePickerFormat.Short,
                .Value = currentDateTime
            }
            Dim dtpTime As New DateTimePicker() With {
                .Left = 294,
                .Top = 20,
                .Width = 115,
                .Format = DateTimePickerFormat.Time,
                .ShowUpDown = True,
                .Value = currentDateTime
            }

            Dim lblPat As New Label() With {.Text = "Patient Name *:", .Left = 20, .Top = 66, .Width = 115}
            Dim cmbPatient As New ComboBox() With {
                .Left = 142,
                .Top = 62,
                .Width = 267,
                .Text = currentPatient
            }
            For Each pName As String In LoadPatientSuggestions()
                cmbPatient.Items.Add(pName)
            Next

            Dim lblSrv As New Label() With {.Text = "Service *:", .Left = 20, .Top = 108, .Width = 115}
            Dim cmbService As New ComboBox() With {
                .Left = 142,
                .Top = 104,
                .Width = 267,
                .Text = currentService
            }
            cmbService.Items.AddRange(New Object() {"General Checkup", "Prenatal", "Immunization", "Consultation", "Dental"})

            Dim lblSt As New Label() With {.Text = "Status *:", .Left = 20, .Top = 150, .Width = 115}
            Dim cmbStatus As New ComboBox() With {
                .Left = 142,
                .Top = 146,
                .Width = 267,
                .DropDownStyle = ComboBoxStyle.DropDownList
            }
            cmbStatus.Items.AddRange(New Object() {"Pending", "Confirmed", "Cancelled"})
            If cmbStatus.Items.Contains(currentStatus) Then
                cmbStatus.SelectedItem = currentStatus
            Else
                cmbStatus.SelectedIndex = 0
            End If

            Dim btnSave As New Button() With {
                .Text = If(isEdit, "Save Changes", "Add Appointment"),
                .Left = 192,
                .Top = 198,
                .Width = 122,
                .Height = 34,
                .BackColor = Color.FromArgb(76, 175, 80),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }
            btnSave.FlatAppearance.BorderSize = 0

            Dim btnCancel As New Button() With {
                .Text = "Cancel",
                .Left = 322,
                .Top = 198,
                .Width = 87,
                .Height = 34,
                .BackColor = Color.FromArgb(130, 130, 130),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .DialogResult = DialogResult.Cancel
            }
            btnCancel.FlatAppearance.BorderSize = 0

            AddHandler btnSave.Click, Sub(s, ev)
                                          Dim patName As String = cmbPatient.Text.Trim()
                                          Dim srvName As String = cmbService.Text.Trim()
                                          Dim stName As String = If(cmbStatus.SelectedItem IsNot Nothing, cmbStatus.SelectedItem.ToString(), "Pending")

                                          If String.IsNullOrWhiteSpace(patName) Then
                                              MessageBox.Show("Please enter Patient Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                              cmbPatient.Focus()
                                              Return
                                          End If

                                          If String.IsNullOrWhiteSpace(srvName) Then
                                              MessageBox.Show("Please enter Service.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                              cmbService.Focus()
                                              Return
                                          End If

                                          Dim combinedDt As New DateTime(
                                              dtpDate.Value.Year, dtpDate.Value.Month, dtpDate.Value.Day,
                                              dtpTime.Value.Hour, dtpTime.Value.Minute, 0)

                                          Try
                                              Using con As New SqlConnection(connStr)
                                                  con.Open()
                                                  If isEdit Then
                                                      Using cmd As New SqlCommand(
                                                          "UPDATE dbo.Appointments " &
                                                          "SET AppointmentDate = @dt, PatientName = @pat, Service = @srv, Status = @st " &
                                                          "WHERE AppointmentID = @id", con)
                                                          cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = combinedDt
                                                          cmd.Parameters.Add("@pat", SqlDbType.VarChar, 150).Value = patName
                                                          cmd.Parameters.Add("@srv", SqlDbType.VarChar, 100).Value = srvName
                                                          cmd.Parameters.Add("@st", SqlDbType.VarChar, 50).Value = stName
                                                          cmd.Parameters.Add("@id", SqlDbType.Int).Value = appointmentId.Value
                                                          cmd.ExecuteNonQuery()
                                                      End Using
                                                  Else
                                                      Using cmd As New SqlCommand(
                                                          "INSERT INTO dbo.Appointments (AppointmentDate, PatientName, Service, Status) " &
                                                          "VALUES (@dt, @pat, @srv, @st)", con)
                                                          cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = combinedDt
                                                          cmd.Parameters.Add("@pat", SqlDbType.VarChar, 150).Value = patName
                                                          cmd.Parameters.Add("@srv", SqlDbType.VarChar, 100).Value = srvName
                                                          cmd.Parameters.Add("@st", SqlDbType.VarChar, 50).Value = stName
                                                          cmd.ExecuteNonQuery()
                                                      End Using
                                                  End If
                                              End Using

                                              Try
                                                  AdminFrm.LogNotification(
                                                      If(isEdit, "updated appointment for " & patName, "added appointment for " & patName))
                                              Catch
                                              End Try

                                              dlg.DialogResult = DialogResult.OK
                                              dlg.Close()
                                          Catch ex As Exception
                                              MessageBox.Show("Failed to save appointment: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                          End Try
                                      End Sub

            dlg.Controls.AddRange(New Control() {
                lblDt, dtpDate, dtpTime,
                lblPat, cmbPatient,
                lblSrv, cmbService,
                lblSt, cmbStatus,
                btnSave, btnCancel
            })
            dlg.AcceptButton = btnSave
            dlg.CancelButton = btnCancel

            If dlg.ShowDialog(Me) = DialogResult.OK Then
                RefreshAllAppointmentData()
            End If
        End Using
    End Sub

    Private Sub DeleteAppointmentById(appointmentId As Integer, patientName As String)
        Dim labelText As String = If(String.IsNullOrWhiteSpace(patientName),
                                     "APT-" & appointmentId.ToString(),
                                     "APT-" & appointmentId.ToString() & " (" & patientName & ")")

        Dim ans = MessageBox.Show(
            "Delete appointment " & labelText & "?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)

        If ans <> DialogResult.Yes Then Return

        Try
            Using con As New SqlConnection(connStr)
                con.Open()
                Using cmd As New SqlCommand("DELETE FROM dbo.Appointments WHERE AppointmentID = @id", con)
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = appointmentId
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Try
                AdminFrm.LogNotification("deleted appointment " & labelText)
            Catch
            End Try

            RefreshAllAppointmentData()
        Catch ex As Exception
            MessageBox.Show("Failed to delete appointment: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HandleAddClick(sender As Object, e As EventArgs)
        ShowAppointmentEditor(Nothing)
    End Sub

    Private Sub HandleEditClick(sender As Object, e As EventArgs)
        Dim apptId As Integer? = GetSelectedAppointmentId()
        If Not apptId.HasValue Then
            MessageBox.Show("Please select an appointment row to edit.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        ShowAppointmentEditor(apptId.Value)
    End Sub

    Private Sub HandleDeleteClick(sender As Object, e As EventArgs)
        Dim apptId As Integer? = GetSelectedAppointmentId()
        If Not apptId.HasValue Then
            MessageBox.Show("Please select an appointment row to delete.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim patName As String = If(appoint.CurrentRow IsNot Nothing, Convert.ToString(appoint.CurrentRow.Cells(2).Value), "")
        DeleteAppointmentById(apptId.Value, patName)
    End Sub

    Private Sub appoint_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles appoint.CellContentClick
        If e.RowIndex < 0 OrElse e.RowIndex >= appoint.Rows.Count Then Return

        Dim colName As String = appoint.Columns(e.ColumnIndex).Name
        Dim apptId As Integer? = GetSelectedAppointmentId(e.RowIndex)
        If Not apptId.HasValue Then Return

        If colName = "colActionEdit" Then
            ShowAppointmentEditor(apptId.Value)
        ElseIf colName = "colActionDelete" Then
            Dim patName As String = Convert.ToString(appoint.Rows(e.RowIndex).Cells(2).Value)
            DeleteAppointmentById(apptId.Value, patName)
        End If
    End Sub

    Private Sub LoadAppointments()
        Try
            Dim dt As New DataTable()
            Using con As New SqlConnection(connStr)
                Using da As New SqlDataAdapter(
                    "SELECT AppointmentID AS RawAppointmentID, " &
                    "'APT-' + CAST(AppointmentID AS VARCHAR(10)) AS AppointmentID, " &
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