Imports System.Data.SqlClient

Partial Public Class DrConsultation

    Private ReadOnly connStr As String =
        "Server=.\SQLEXPRESS;Database=two_plus_two_s5c2;Trusted_Connection=True;TrustServerCertificate=True;"

    Private WithEvents consRefreshTimer As New Timer()
    Private isInitializing As Boolean = True

    Private Function GetOpenConsConnection() As SqlConnection
        If conn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            If conn.State <> ConnectionState.Open Then conn.Open()
            Return conn
        End If

        Dim localConn As New SqlConnection(connStr)
        localConn.Open()
        Return localConn
    End Function

    Private Sub EnsureConsultationsTable(activeConn As SqlConnection)
        Dim sql As String =
            "IF OBJECT_ID('dbo.patient_consultations', 'U') IS NULL " &
            "BEGIN " &
            "   CREATE TABLE dbo.patient_consultations (" &
            "       consultation_id INT IDENTITY(1,1) PRIMARY KEY, " &
            "       patient_id INT NULL, " &
            "       consult_datetime DATETIME NOT NULL DEFAULT GETDATE(), " &
            "       patient_name VARCHAR(200) NOT NULL, " &
            "       age INT NOT NULL DEFAULT 0, " &
            "       gender VARCHAR(30) NOT NULL DEFAULT 'N/A', " &
            "       complaint_diagnosis VARCHAR(500) NOT NULL, " &
            "       action_taken VARCHAR(500) NOT NULL" &
            "   ); " &
            "END"

        Using cmd As New SqlCommand(sql, activeConn)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub ApplyConsultationTypography()
        Try
            If Label10 IsNot Nothing Then
                Label10.AutoSize = False
                Label10.Font = New Font("Segoe UI", 11.5F, FontStyle.Bold)
                Label10.ForeColor = Color.FromArgb(24, 94, 46)
                Label10.Height = 24
            End If

            If Label2 IsNot Nothing Then
                Label2.AutoSize = False
                Label2.Font = New Font("Segoe UI", 17.5F, FontStyle.Bold)
                Label2.ForeColor = Color.FromArgb(0, 64, 0)
                Label2.Height = 36
            End If

            Dim primaryBannerLabels() As Label = {Label3, Label6, Label7, Label5, Label8}
            For Each lbl As Label In primaryBannerLabels
                If lbl IsNot Nothing Then
                    lbl.AutoSize = False
                    lbl.Font = New Font("Segoe UI", 12.5F, FontStyle.Bold)
                    lbl.ForeColor = Color.FromArgb(20, 48, 30)
                    lbl.Height = 26
                End If
            Next

            Dim secondaryBannerLabels() As Label = {Label4, Label12, Label9, Label11}
            For Each lbl As Label In secondaryBannerLabels
                If lbl IsNot Nothing Then
                    lbl.AutoSize = False
                    lbl.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
                    lbl.ForeColor = Color.FromArgb(35, 62, 46)
                    lbl.Height = 24
                End If
            Next

            ' Ayusin ang lapad at horizontal alignment para hindi maputol ang Created account at Birthdate
            Dim panelW As Integer = If(Guna2Panel2 IsNot Nothing AndAlso Guna2Panel2.ClientSize.Width > 0,
                                     Guna2Panel2.ClientSize.Width,
                                     Me.ClientSize.Width)
            Dim leftStart As Integer = If(Guna2Panel8 IsNot Nothing, Guna2Panel8.Right + 22, 200)
            Dim usableW As Integer = Math.Max(660, panelW - leftStart - 20)

            Dim col1W As Integer = CInt(usableW * 0.34F)
            Dim col2X As Integer = leftStart + col1W + 24
            Dim col2W As Integer = CInt(usableW * 0.28F)
            Dim col3X As Integer = col2X + col2W + 24
            Dim col3W As Integer = Math.Max(180, (leftStart + usableW) - col3X)

            For Each c1 As Control In New Control() {Label10, Label2, Label3, Label4, Label12}
                If c1 IsNot Nothing Then
                    c1.Left = leftStart
                    c1.Width = col1W
                End If
            Next

            If Label4 IsNot Nothing AndAlso Label12 IsNot Nothing Then
                Label4.Top = Math.Min(Label4.Top, 102)
                Label12.Top = Math.Max(Label4.Bottom + 1, 124)
            End If

            For Each c2 As Control In New Control() {Label6, Label7, Label9}
                If c2 IsNot Nothing Then
                    c2.Left = col2X
                    c2.Width = col2W
                End If
            Next

            If Label9 IsNot Nothing Then
                Label9.Top = Math.Max(Label9.Top, 122)
            End If

            For Each c3 As Control In New Control() {Label5, Label8, Label11}
                If c3 IsNot Nothing Then
                    c3.Left = col3X
                    c3.Width = col3W
                End If
            Next

            If Label11 IsNot Nothing Then
                Label11.Top = Math.Max(Label11.Top, 122)
            End If

            If Label13 IsNot Nothing Then
                Label13.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
            End If

            If Label14 IsNot Nothing Then
                Label14.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
            End If

            If Guna2Button1 IsNot Nothing Then
                Guna2Button1.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
            End If

            If Guna2ComboBox1 IsNot Nothing Then
                Guna2ComboBox1.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
            End If

            If Guna2ComboBox2 IsNot Nothing Then
                Guna2ComboBox2.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
            End If

            If Guna2TextBox1 IsNot Nothing Then
                Guna2TextBox1.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
            End If

            If cons IsNot Nothing Then
                cons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
                cons.ColumnHeadersHeight = 46
                cons.RowTemplate.Height = 40
                cons.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
                cons.DefaultCellStyle.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
                cons.AlternatingRowsDefaultCellStyle.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
                cons.ThemeStyle.HeaderStyle.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
                cons.ThemeStyle.RowsStyle.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
                cons.ThemeStyle.AlternatingRowsStyle.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
            End If
        Catch
        End Try
    End Sub

    Private Sub InitializeFilterControls()
        isInitializing = True
        Try
            Guna2ComboBox1.Items.Clear()
            Guna2ComboBox1.Items.AddRange(New Object() {"All Dates", "Today", "Last 7 Days", "This Month"})
            Guna2ComboBox1.SelectedIndex = 0

            Guna2ComboBox2.Items.Clear()
            Guna2ComboBox2.Items.AddRange(New Object() {"All Patients", "Male", "Female"})
            Guna2ComboBox2.SelectedIndex = 0
        Finally
            isInitializing = False
        End Try
    End Sub

    Public Sub LoadConsultations()
        Try
            Dim activeConn As SqlConnection = GetOpenConsConnection()
            EnsureConsultationsTable(activeConn)

            Dim searchRaw As String = If(Guna2TextBox1.Text, "").Trim()
            If searchRaw.Equals("Search by name,ID, or complaints.....", StringComparison.OrdinalIgnoreCase) Then
                searchRaw = ""
            End If

            Dim dateFilter As String = If(Guna2ComboBox1.SelectedItem IsNot Nothing, Guna2ComboBox1.SelectedItem.ToString(), "All Dates")
            Dim patFilter As String = If(Guna2ComboBox2.SelectedItem IsNot Nothing, Guna2ComboBox2.SelectedItem.ToString(), "All Patients")

            Dim sql As String =
                "SELECT consultation_id, patient_id, consult_datetime, patient_name, age, gender, complaint_diagnosis, action_taken " &
                "FROM dbo.patient_consultations " &
                "WHERE (@search = '' OR patient_name LIKE @searchLike OR CAST(ISNULL(patient_id, 0) AS VARCHAR(20)) LIKE @searchLike OR complaint_diagnosis LIKE @searchLike OR action_taken LIKE @searchLike) " &
                "AND ( " &
                "   @patFilter = 'All Patients' " &
                "   OR (@patFilter = 'Male' AND LOWER(LTRIM(RTRIM(gender))) = 'male') " &
                "   OR (@patFilter = 'Female' AND LOWER(LTRIM(RTRIM(gender))) = 'female') " &
                ") " &
                "AND ( " &
                "   @dateFilter = 'All Dates' " &
                "   OR (@dateFilter = 'Today' AND CAST(consult_datetime AS DATE) = CAST(GETDATE() AS DATE)) " &
                "   OR (@dateFilter = 'Last 7 Days' AND consult_datetime >= DATEADD(DAY, -7, GETDATE())) " &
                "   OR (@dateFilter = 'This Month' AND MONTH(consult_datetime) = MONTH(GETDATE()) AND YEAR(consult_datetime) = YEAR(GETDATE())) " &
                ") " &
                "ORDER BY consult_datetime DESC, consultation_id DESC"

            Using cmd As New SqlCommand(sql, activeConn)
                cmd.Parameters.Add("@search", SqlDbType.VarChar, 150).Value = searchRaw
                cmd.Parameters.Add("@searchLike", SqlDbType.VarChar, 155).Value = "%" & searchRaw & "%"
                cmd.Parameters.Add("@patFilter", SqlDbType.VarChar, 50).Value = patFilter
                cmd.Parameters.Add("@dateFilter", SqlDbType.VarChar, 50).Value = dateFilter

                Using dataRead As SqlDataReader = cmd.ExecuteReader()
                    Dim selectedId As Object = Nothing
                    If cons.CurrentRow IsNot Nothing AndAlso cons.CurrentRow.Tag IsNot Nothing Then
                        selectedId = cons.CurrentRow.Tag
                    End If

                    cons.Rows.Clear()

                    While dataRead.Read()
                        Dim dtText As String = Convert.ToDateTime(dataRead("consult_datetime")).ToString("MMM dd, yyyy hh:mm tt")
                        Dim pName As String = dataRead("patient_name").ToString()
                        Dim pAge As String = dataRead("age").ToString()
                        Dim pGender As String = dataRead("gender").ToString()
                        Dim complaint As String = dataRead("complaint_diagnosis").ToString()
                        Dim actionText As String = dataRead("action_taken").ToString()

                        Dim idx As Integer = cons.Rows.Add(dtText, pName, pAge, pGender, complaint, actionText)
                        cons.Rows(idx).Tag = dataRead("consultation_id")
                        cons.Rows(idx).Cells(0).Tag = dataRead("patient_id")
                        cons.Rows(idx).Height = 40

                        If selectedId IsNot Nothing AndAlso selectedId.ToString() = dataRead("consultation_id").ToString() Then
                            cons.CurrentCell = cons.Rows(idx).Cells(0)
                        End If
                    End While
                End Using
            End Using

            ApplyConsultationTypography()

            If cons.Rows.Count > 0 Then
                Dim rowToShow As DataGridViewRow = If(cons.CurrentRow IsNot Nothing, cons.CurrentRow, cons.Rows(0))
                UpdateBannerFromRow(rowToShow)
            Else
                ResetBannerDefaults()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ResetBannerDefaults()
        Label10.Text = "Position"
        Label2.Text = "Name"
        Label3.Text = "Age"
        Label4.Text = "ID"
        Label12.Text = "Created account"
        Label6.Text = "Gender"
        Label7.Text = "Civil Status"
        Label9.Text = "Birthdate"
        Label5.Text = "Address"
        Label8.Text = "Phone number"
        Label11.Text = "Email"
        If profileimg.Image IsNot Nothing Then
            profileimg.Image.Dispose()
            profileimg.Image = Nothing
        End If
        ApplyConsultationTypography()
    End Sub

    Private Sub UpdateBannerFromRow(row As DataGridViewRow)
        If row Is Nothing Then Return
        Try
            Dim rowPatName As String = Convert.ToString(row.Cells(1).Value).Trim()
            Label2.Text = rowPatName
            Label3.Text = Convert.ToString(row.Cells(2).Value)
            Label6.Text = Convert.ToString(row.Cells(3).Value)

            Dim patIdObj As Object = row.Cells(0).Tag
            Dim hasNumericId As Boolean = (patIdObj IsNot Nothing AndAlso Not IsDBNull(patIdObj) AndAlso IsNumeric(patIdObj.ToString()))

            Dim activeConn As SqlConnection = GetOpenConsConnection()
            Dim sql As String = If(
                hasNumericId,
                "SELECT TOP 1 patient_Id, first_Name, middle_Name, surname, name_extension, age, birth_Date, " &
                "gender, civil_status, user_address, job_title, phone_number, email_address, created_account, patient_photo " &
                "FROM dbo.patient_information WHERE patient_Id = @id",
                "SELECT TOP 1 patient_Id, first_Name, middle_Name, surname, name_extension, age, birth_Date, " &
                "gender, civil_status, user_address, job_title, phone_number, email_address, created_account, patient_photo " &
                "FROM dbo.patient_information " &
                "WHERE LTRIM(RTRIM(REPLACE(ISNULL(first_Name,'') + ' ' + ISNULL(middle_Name,'') + ' ' + ISNULL(surname,'') + ' ' + CASE WHEN ISNULL(name_extension,'') IN ('','N/A') THEN '' ELSE name_extension END, '  ', ' '))) = @pName " &
                "OR LTRIM(RTRIM(ISNULL(first_Name,'') + ' ' + ISNULL(surname,''))) = @pName"
            )

            Using cmd As New SqlCommand(sql, activeConn)
                If hasNumericId Then
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(patIdObj)
                Else
                    cmd.Parameters.Add("@pName", SqlDbType.VarChar, 200).Value = rowPatName
                End If

                Using r As SqlDataReader = cmd.ExecuteReader()
                    If r.Read() Then
                        Dim ext As String = If(IsDBNull(r("name_extension")) OrElse r("name_extension").ToString().Trim() = "N/A", "", r("name_extension").ToString().Trim())
                        Dim fullName As String = (r("first_Name").ToString().Trim() & " " &
                                                r("middle_Name").ToString().Trim() & " " &
                                                r("surname").ToString().Trim() & " " & ext).Replace("  ", " ").Trim()

                        Label10.Text = If(IsDBNull(r("job_title")) OrElse r("job_title").ToString().Trim() = "", "Patient", r("job_title").ToString().Trim())
                        Label2.Text = fullName
                        Label3.Text = r("age").ToString()
                        Label4.Text = r("patient_Id").ToString()
                        Label12.Text = If(IsDBNull(r("created_account")), "Registered patient", Convert.ToDateTime(r("created_account")).ToString("MMMM dd, yyyy"))
                        Label6.Text = r("gender").ToString()
                        Label7.Text = If(IsDBNull(r("civil_status")), "N/A", r("civil_status").ToString())
                        Label9.Text = If(IsDBNull(r("birth_Date")), "N/A", Convert.ToDateTime(r("birth_Date")).ToString("MMMM dd, yyyy"))
                        Label5.Text = If(IsDBNull(r("user_address")), "N/A", r("user_address").ToString())
                        Label8.Text = If(IsDBNull(r("phone_number")), "N/A", r("phone_number").ToString())
                        Label11.Text = If(IsDBNull(r("email_address")) OrElse r("email_address").ToString().Trim() = "", "N/A", r("email_address").ToString())

                        If Not IsDBNull(r("patient_photo")) Then
                            Dim bytes() As Byte = TryCast(r("patient_photo"), Byte())
                            If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                                Using ms As New IO.MemoryStream(bytes)
                                    Using tmp As Image = Image.FromStream(ms)
                                        If profileimg.Image IsNot Nothing Then profileimg.Image.Dispose()
                                        profileimg.Image = New Bitmap(tmp)
                                    End Using
                                End Using
                            End If
                        End If
                    End If
                End Using
            End Using

            ApplyConsultationTypography()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OpenNewConsultationModal()
        Using dlg As New Form()
            dlg.Text = "New Consultation"
            dlg.Size = New Size(460, 340)
            dlg.StartPosition = FormStartPosition.CenterParent
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog
            dlg.MaximizeBox = False
            dlg.MinimizeBox = False
            dlg.BackColor = Color.White
            dlg.Font = New Font("Segoe UI", 9.5F)

            Dim lblPat As New Label() With {.Text = "Patient Name *:", .Left = 18, .Top = 22, .Width = 135}
            Dim cmbPat As New ComboBox() With {.Left = 160, .Top = 18, .Width = 265, .DropDownStyle = ComboBoxStyle.DropDown}

            Dim lblAge As New Label() With {.Text = "Age *:", .Left = 18, .Top = 62, .Width = 135}
            Dim txtAge As New TextBox() With {.Left = 160, .Top = 58, .Width = 90, .Text = "0"}

            Dim lblGen As New Label() With {.Text = "Gender *:", .Left = 265, .Top = 62, .Width = 65}
            Dim cmbGen As New ComboBox() With {.Left = 330, .Top = 58, .Width = 95, .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbGen.Items.AddRange(New Object() {"Male", "Female"})
            cmbGen.SelectedIndex = 0

            Dim lblComp As New Label() With {.Text = "Complaint/Diagnosis *:", .Left = 18, .Top = 104, .Width = 135}
            Dim txtComp As New TextBox() With {.Left = 160, .Top = 100, .Width = 265, .Height = 60, .Multiline = True}

            Dim lblAct As New Label() With {.Text = "Action *:", .Left = 18, .Top = 174, .Width = 135}
            Dim txtAct As New TextBox() With {.Left = 160, .Top = 170, .Width = 265, .Height = 55, .Multiline = True}

            Try
                Dim activeConn As SqlConnection = GetOpenConsConnection()
                Using cmd As New SqlCommand("SELECT first_Name, middle_Name, surname, name_extension FROM dbo.patient_information ORDER BY first_Name", activeConn)
                    Using r As SqlDataReader = cmd.ExecuteReader()
                        While r.Read()
                            Dim ext As String = If(IsDBNull(r("name_extension")) OrElse r("name_extension").ToString().Trim() = "N/A", "", r("name_extension").ToString().Trim())
                            Dim fullName As String = (r("first_Name").ToString().Trim() & " " &
                                                    r("middle_Name").ToString().Trim() & " " &
                                                    r("surname").ToString().Trim() & " " & ext).Replace("  ", " ").Trim()
                            If Not String.IsNullOrWhiteSpace(fullName) AndAlso Not cmbPat.Items.Contains(fullName) Then
                                cmbPat.Items.Add(fullName)
                            End If
                        End While
                    End Using
                End Using
            Catch
            End Try

            AddHandler cmbPat.SelectedIndexChanged, Sub(s, ev)
                                                        Try
                                                            Dim activeConn As SqlConnection = GetOpenConsConnection()
                                                            Dim sql As String =
                                                                "SELECT TOP 1 age, gender FROM dbo.patient_information " &
                                                                "WHERE LTRIM(RTRIM(REPLACE(ISNULL(first_Name,'') + ' ' + ISNULL(middle_Name,'') + ' ' + ISNULL(surname,'') + ' ' + CASE WHEN ISNULL(name_extension,'') IN ('','N/A') THEN '' ELSE name_extension END, '  ', ' '))) = @pName"
                                                            Using cmd As New SqlCommand(sql, activeConn)
                                                                cmd.Parameters.Add("@pName", SqlDbType.VarChar, 200).Value = cmbPat.Text.Trim()
                                                                Using r As SqlDataReader = cmd.ExecuteReader()
                                                                    If r.Read() Then
                                                                        txtAge.Text = r("age").ToString()
                                                                        Dim gVal As String = r("gender").ToString().Trim()
                                                                        If cmbGen.Items.Contains(gVal) Then cmbGen.SelectedItem = gVal
                                                                    End If
                                                                End Using
                                                            End Using
                                                        Catch
                                                        End Try
                                                    End Sub

            Dim btnSave As New Button() With {
                .Text = "Save",
                .Left = 245,
                .Top = 245,
                .Width = 100,
                .Height = 34,
                .BackColor = Color.FromArgb(0, 64, 0),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }

            Dim btnCancel As New Button() With {
                .Text = "Cancel",
                .Left = 352,
                .Top = 245,
                .Width = 73,
                .Height = 34,
                .BackColor = Color.Gray,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .DialogResult = DialogResult.Cancel
            }

            AddHandler btnSave.Click, Sub(s, ev)
                                          Dim pName As String = cmbPat.Text.Trim()
                                          Dim compVal As String = txtComp.Text.Trim()
                                          Dim actVal As String = txtAct.Text.Trim()
                                          Dim parsedAge As Integer

                                          If String.IsNullOrWhiteSpace(pName) OrElse String.IsNullOrWhiteSpace(compVal) OrElse String.IsNullOrWhiteSpace(actVal) Then
                                              MessageBox.Show("Please complete all required fields (*).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                              Return
                                          End If

                                          If Not Integer.TryParse(txtAge.Text.Trim(), parsedAge) OrElse parsedAge < 0 OrElse parsedAge > 130 Then
                                              MessageBox.Show("Please enter a valid age.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                              Return
                                          End If

                                          Try
                                              Dim activeConn As SqlConnection = GetOpenConsConnection()
                                              EnsureConsultationsTable(activeConn)

                                              Dim patIdObj As Object = DBNull.Value
                                              Dim idSql As String =
                                                  "SELECT TOP 1 patient_Id FROM dbo.patient_information " &
                                                  "WHERE LTRIM(RTRIM(REPLACE(ISNULL(first_Name,'') + ' ' + ISNULL(middle_Name,'') + ' ' + ISNULL(surname,'') + ' ' + CASE WHEN ISNULL(name_extension,'') IN ('','N/A') THEN '' ELSE name_extension END, '  ', ' '))) = @pName"
                                              Using idCmd As New SqlCommand(idSql, activeConn)
                                                  idCmd.Parameters.Add("@pName", SqlDbType.VarChar, 200).Value = pName
                                                  Dim foundId = idCmd.ExecuteScalar()
                                                  If foundId IsNot Nothing AndAlso Not IsDBNull(foundId) Then
                                                      patIdObj = Convert.ToInt32(foundId)
                                                  End If
                                              End Using

                                              Dim insSql As String =
                                                  "INSERT INTO dbo.patient_consultations (patient_id, consult_datetime, patient_name, age, gender, complaint_diagnosis, action_taken) " &
                                                  "VALUES (@patient_id, GETDATE(), @patient_name, @age, @gender, @complaint_diagnosis, @action_taken)"

                                              Using insCmd As New SqlCommand(insSql, activeConn)
                                                  insCmd.Parameters.Add("@patient_id", SqlDbType.Int).Value = patIdObj
                                                  insCmd.Parameters.Add("@patient_name", SqlDbType.VarChar, 200).Value = pName
                                                  insCmd.Parameters.Add("@age", SqlDbType.Int).Value = parsedAge
                                                  insCmd.Parameters.Add("@gender", SqlDbType.VarChar, 30).Value = cmbGen.Text
                                                  insCmd.Parameters.Add("@complaint_diagnosis", SqlDbType.VarChar, 500).Value = compVal
                                                  insCmd.Parameters.Add("@action_taken", SqlDbType.VarChar, 500).Value = actVal
                                                  insCmd.ExecuteNonQuery()
                                              End Using

                                              Try
                                                  AdminFrm.LogNotification("added consultation for " & pName)
                                              Catch
                                              End Try

                                              dlg.DialogResult = DialogResult.OK
                                              dlg.Close()
                                          Catch ex As Exception
                                              MessageBox.Show("Failed to save consultation: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                          End Try
                                      End Sub

            dlg.Controls.AddRange(New Control() {lblPat, cmbPat, lblAge, txtAge, lblGen, cmbGen, lblComp, txtComp, lblAct, txtAct, btnSave, btnCancel})
            dlg.AcceptButton = btnSave
            dlg.CancelButton = btnCancel

            If dlg.ShowDialog(Me.FindForm()) = DialogResult.OK Then
                LoadConsultations()
                MessageBox.Show("Consultation saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub DrConsultation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()
        ApplyConsultationTypography()
        InitializeFilterControls()
        LoadConsultations()

        consRefreshTimer.Interval = 3000
        consRefreshTimer.Start()
    End Sub

    Private Sub DrConsultation_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        ApplyConsultationTypography()
    End Sub

    Private Sub consRefreshTimer_Tick(sender As Object, e As EventArgs) Handles consRefreshTimer.Tick
        If Not Me.IsDisposed AndAlso Me.Visible Then
            LoadConsultations()
        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        OpenNewConsultationModal()
    End Sub

    Private Sub Guna2CirclePictureBox3_Click(sender As Object, e As EventArgs) Handles Guna2CirclePictureBox3.Click
        OpenNewConsultationModal()
    End Sub

    Private Sub Guna2ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox1.SelectedIndexChanged
        If isInitializing Then Return
        LoadConsultations()
    End Sub

    Private Sub Guna2ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox2.SelectedIndexChanged
        If isInitializing Then Return
        LoadConsultations()
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        If isInitializing Then Return
        LoadConsultations()
    End Sub

    Private Sub cons_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles cons.CellClick
        If e.RowIndex >= 0 AndAlso e.RowIndex < cons.Rows.Count Then
            UpdateBannerFromRow(cons.Rows(e.RowIndex))
        End If
    End Sub

End Class