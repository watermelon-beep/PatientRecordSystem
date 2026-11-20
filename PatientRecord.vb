Imports System.Data.SqlClient

Public Class PatientRecord

    ' 0 ID, 1 First, 2 Middle, 3 Surname, 4 Extension, 5 Age, 6 Birthdate, 7 Gender,
    ' 8 Civil, 9 Emergency, 10 Address, 11 Job Title, 12 Phone, 13 Email, 14 Username,
    ' 15 Password, 16 Image, 17 Edit, 18 Delete, 19 View
    Private ReadOnly ratios() As Single = {
        65, 85, 85, 90, 75, 50, 95, 70,
        80, 145, 160, 85, 105, 120, 90,
        85, 75, 65, 65, 65
    }
    Private isLoading As Boolean = True

    Private Function CreateBlankImage(w As Integer, h As Integer) As Bitmap
        Dim bmp As New Bitmap(Math.Max(1, w), Math.Max(1, h))
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Transparent)
        End Using
        Return bmp
    End Function

    Private Sub PatientTbl_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles PatientTbl.DataError
        e.ThrowException = False
    End Sub

    Private Sub PatientRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()

        selectionposcmbx.Items.Clear()
        selectionposcmbx.Items.AddRange({"All", "Male", "Female", "Single", "Married", "Widowed", "Divorced", "Separated"})
        selectionposcmbx.SelectedIndex = 0

        StyleGrid()
        LoadPatients("")
        isLoading = False
        ShowFirstPatient()
        FormatBannerLayout()
    End Sub

    ' ---------- LOAD / SEARCH / FILTER ----------
    Private Sub LoadPatients(search As String)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            PatientTbl.Rows.Clear()

            Dim filter As String = "All"
            If selectionposcmbx.SelectedItem IsNot Nothing Then filter = selectionposcmbx.SelectedItem.ToString()

            query = "SELECT patient_Id, first_Name, middle_Name, surname, name_extension, age, birth_Date, " &
                    "gender, civil_status, emergency_info, user_address, job_title, phone_number, " &
                    "email_address, username, patient_photo " &
                    "FROM dbo.patient_information " &
                    "WHERE (CAST(patient_Id AS VARCHAR(20)) LIKE @search " &
                    "OR first_Name LIKE @search OR middle_Name LIKE @search OR surname LIKE @search " &
                    "OR name_extension LIKE @search OR user_address LIKE @search OR emergency_info LIKE @search " &
                    "OR phone_number LIKE @search OR email_address LIKE @search OR job_title LIKE @search " &
                    "OR username LIKE @search) " &
                    "AND (@filter = 'All' OR gender = @filter OR civil_status = @filter) " &
                    "ORDER BY patient_Id"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@search", SqlDbType.VarChar, 150).Value = "%" & search.Trim() & "%"
                comm.Parameters.Add("@filter", SqlDbType.VarChar, 50).Value = filter

                Using dataRead = comm.ExecuteReader()
                    While dataRead.Read()
                        Dim imgObj As Object
                        If IsDBNull(dataRead("patient_photo")) Then
                            imgObj = CreateBlankImage(48, 48)
                        Else
                            Dim rawBytes() As Byte = CType(dataRead("patient_photo"), Byte())
                            Using ms As New IO.MemoryStream(rawBytes)
                                Using tmp As Image = Image.FromStream(ms)
                                    imgObj = New Bitmap(tmp)
                                End Using
                            End Using
                        End If

                        Dim middleText As String = If(IsDBNull(dataRead("middle_Name")) OrElse
                                                      dataRead("middle_Name").ToString().Trim() = "",
                                                      "N/A",
                                                      dataRead("middle_Name").ToString().Trim())
                        Dim extText As String = If(IsDBNull(dataRead("name_extension")) OrElse
                                                   dataRead("name_extension").ToString().Trim() = "",
                                                   "N/A",
                                                   dataRead("name_extension").ToString().Trim())
                        Dim emergText As String = If(IsDBNull(dataRead("emergency_info")) OrElse
                                                     dataRead("emergency_info").ToString().Trim() = "",
                                                     "N/A",
                                                     dataRead("emergency_info").ToString().Trim())
                        Dim addrText As String = If(IsDBNull(dataRead("user_address")) OrElse
                                                    dataRead("user_address").ToString().Trim() = "",
                                                    "N/A",
                                                    dataRead("user_address").ToString().Trim())
                        Dim jobText As String = If(IsDBNull(dataRead("job_title")) OrElse
                                                   dataRead("job_title").ToString().Trim() = "",
                                                   "N/A",
                                                   dataRead("job_title").ToString().Trim())
                        Dim emailText As String = If(IsDBNull(dataRead("email_address")) OrElse
                                                     dataRead("email_address").ToString().Trim() = "",
                                                     "N/A",
                                                     dataRead("email_address").ToString().Trim())
                        Dim userText As String = If(IsDBNull(dataRead("username")) OrElse
                                                   dataRead("username").ToString().Trim() = "",
                                                   "N/A",
                                                   dataRead("username").ToString().Trim())

                        PatientTbl.Rows.Add(dataRead("patient_Id"),
                                            dataRead("first_Name"),
                                            middleText,
                                            dataRead("surname"),
                                            extText,
                                            dataRead("age"),
                                            Convert.ToDateTime(dataRead("birth_Date")).ToString("dd/MM/yyyy"),
                                            dataRead("gender"),
                                            dataRead("civil_status"),
                                            emergText,
                                            addrText,
                                            jobText,
                                            dataRead("phone_number"),
                                            emailText,
                                            userText,
                                            "••••••••",
                                            imgObj,
                                            "Edit",
                                            "Delete",
                                            "View")
                    End While
                End Using
            End Using

            For Each r As DataGridViewRow In PatientTbl.Rows
                r.Height = 76
            Next

            ColorButtons()
            PatientTbl.CurrentCell = Nothing
            PatientTbl.ClearSelection()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ' ---------- BANNER ----------
    Private Sub ShowFirstPatient()
        If PatientTbl.Rows.Count > 0 Then
            Dim id As Integer = Convert.ToInt32(PatientTbl.Rows(0).Cells(0).Value)
            ViewPatientProfile(id)
            ViewPhoto(id)
        Else
            ClearBanner()
        End If
    End Sub

    Private Sub ClearBanner()
        Label10.Text = ""
        Label2.Text = "No patient found"
        Label3.Text = ""
        Label12.Text = ""
        Label6.Text = ""
        Label7.Text = ""
        Label5.Text = ""
        Label8.Text = ""
        Label4.Text = ""
        Label9.Text = ""
        Label11.Text = ""
        If profileimg.Image IsNot Nothing Then profileimg.Image.Dispose()
        profileimg.Image = Nothing
        FormatBannerLayout()
    End Sub

    Private Sub ViewPatientProfile(id As Integer)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            query = "SELECT patient_Id, first_Name, middle_Name, surname, name_extension, age, birth_Date, " &
                    "gender, user_address, emergency_info, job_title, civil_status, phone_number, " &
                    "email_address, username, created_account " &
                    "FROM dbo.patient_information WHERE patient_Id = @id"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@id", SqlDbType.Int).Value = id
                Using dataRead = comm.ExecuteReader()
                    If dataRead.Read() Then
                        Dim jobText As String = If(IsDBNull(dataRead("job_title")) OrElse
                                                   dataRead("job_title").ToString().Trim() = "",
                                                   "Patient",
                                                   dataRead("job_title").ToString().Trim())

                        Dim extText As String = If(IsDBNull(dataRead("name_extension")) OrElse
                                                   dataRead("name_extension").ToString().Trim() = "N/A",
                                                  "",
                                                  dataRead("name_extension").ToString().Trim())

                        Dim fullName As String = (dataRead("first_Name").ToString() & " " &
                                                dataRead("middle_Name").ToString() & " " &
                                                dataRead("surname").ToString() & " " &
                                                extText).Replace("  ", " ").Trim()

                        Dim userText As String = If(IsDBNull(dataRead("username")) OrElse
                                                   dataRead("username").ToString().Trim() = "",
                                                   "",
                                                   " (@" & dataRead("username").ToString().Trim() & ")")

                        Dim emailText As String = If(IsDBNull(dataRead("email_address")) OrElse
                                                    dataRead("email_address").ToString().Trim() = "",
                                                    "N/A",
                                                    dataRead("email_address").ToString().Trim())

                        Label10.Text = jobText
                        Label2.Text = fullName & userText
                        Label3.Text = dataRead("age").ToString()
                        Label12.Text = If(IsDBNull(dataRead("created_account")),
                                          "Registered patient",
                                          Convert.ToDateTime(dataRead("created_account")).ToString("MMMM dd, yyyy"))
                        Label6.Text = dataRead("gender").ToString()
                        Label7.Text = dataRead("civil_status").ToString()
                        Label5.Text = dataRead("user_address").ToString()
                        Label8.Text = dataRead("phone_number").ToString()
                        Label4.Text = dataRead("patient_Id").ToString()
                        Label9.Text = Convert.ToDateTime(dataRead("birth_Date")).ToString("MMMM dd, yyyy")
                        Label11.Text = emailText
                        FormatBannerLayout()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ViewPhoto(id As Integer)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            query = "SELECT patient_photo FROM dbo.patient_information WHERE patient_Id = @id"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@id", SqlDbType.Int).Value = id
                Using dataRead = comm.ExecuteReader()
                    If dataRead.Read() AndAlso Not IsDBNull(dataRead("patient_photo")) Then
                        Dim img() As Byte = CType(dataRead("patient_photo"), Byte())
                        Using ms As New IO.MemoryStream(img)
                            If profileimg.Image IsNot Nothing Then profileimg.Image.Dispose()
                            profileimg.Image = New Bitmap(Image.FromStream(ms))
                        End Using
                    Else
                        If profileimg.Image IsNot Nothing Then profileimg.Image.Dispose()
                        profileimg.Image = Nothing
                    End If
                End Using
            End Using
        Catch ex As Exception
            profileimg.Image = Nothing
        End Try
    End Sub

    ' ---------- BANNER FONT & SPACING (LARGER FONTS + NO OVERLAP/CLIPPING) ----------
    Private Sub FormatBannerLayout()
        Try
            ' Name (Label2) - malaki at malinaw, sapat ang height para hindi maputol sa ibaba
            If Label2 IsNot Nothing Then
                Label2.Font = New Font("Segoe UI", 14.5F, FontStyle.Bold)
                Label2.ForeColor = Color.FromArgb(15, 72, 40)
                Label2.AutoSize = False
                Label2.Height = 36
            End If

            ' Position (Label10)
            If Label10 IsNot Nothing Then
                Label10.Font = New Font("Segoe UI", 11.5F, FontStyle.Bold)
                Label10.ForeColor = Color.FromArgb(34, 139, 84)
                Label10.AutoSize = False
                Label10.Height = 25
            End If

            ' Primary fields: Gender (Label6), Civil Status (Label7), Address (Label5), Phone Number (Label8)
            Dim primaryLabels() As Label = {Label6, Label7, Label5, Label8}
            For Each lbl As Label In primaryLabels
                If lbl IsNot Nothing Then
                    lbl.Font = New Font("Segoe UI", 11.5F, FontStyle.Bold)
                    lbl.ForeColor = Color.FromArgb(26, 44, 34)
                    lbl.AutoSize = False
                    lbl.Height = 26
                End If
            Next

            ' Secondary fields: Age (Label3), ID (Label4), Created Account (Label12), Birthdate (Label9), Email (Label11)
            Dim secondaryLabels() As Label = {Label3, Label4, Label12, Label9, Label11}
            For Each lbl As Label In secondaryLabels
                If lbl IsNot Nothing Then
                    lbl.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular)
                    lbl.ForeColor = Color.FromArgb(65, 85, 75)
                    lbl.AutoSize = False
                    lbl.Height = 25
                End If
            Next

            ' 3-column horizontal spacing (hindi binabago ang Top para walang vertical overlap)
            Dim startX As Integer = If(profileimg IsNot Nothing, profileimg.Right + 22, 130)
            Dim parentW As Integer = If(Label2 IsNot Nothing AndAlso Label2.Parent IsNot Nothing,
                                        Label2.Parent.ClientSize.Width,
                                        Me.ClientSize.Width)
            Dim usableW As Integer = Math.Max(600, parentW - startX - 24)

            Dim col1W As Integer = CInt(usableW * 0.45F)
            Dim col2X As Integer = startX + col1W + 20
            Dim col2W As Integer = CInt(usableW * 0.22F)
            Dim col3X As Integer = col2X + col2W + 20
            Dim col3W As Integer = Math.Max(140, (startX + usableW) - col3X)

            For Each c1 As Control In New Control() {Label10, Label2, Label3, Label12, Label4}
                If c1 IsNot Nothing Then
                    c1.Left = startX
                    c1.Width = col1W
                End If
            Next

            For Each c2 As Control In New Control() {Label6, Label7, Label9}
                If c2 IsNot Nothing Then
                    c2.Left = col2X
                    c2.Width = col2W
                End If
            Next

            For Each c3 As Control In New Control() {Label5, Label8, Label11}
                If c3 IsNot Nothing Then
                    c3.Left = col3X
                    c3.Width = col3W
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub

    ' ---------- QUICK EDIT DIALOG ----------
    Private Sub OpenEditPatientDialog(patientId As Integer)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            Dim firstName As String = ""
            Dim middleName As String = ""
            Dim surname As String = ""
            Dim nameExt As String = ""
            Dim ageVal As Integer = 0
            Dim birthDateVal As Date = Date.Today
            Dim genderVal As String = "Male"
            Dim civilVal As String = "Single"
            Dim emergencyVal As String = ""
            Dim addressVal As String = ""
            Dim jobVal As String = ""
            Dim phoneVal As String = ""
            Dim emailVal As String = ""
            Dim usernameVal As String = ""

            Dim selSql As String =
                "SELECT first_Name, middle_Name, surname, name_extension, age, birth_Date, " &
                "gender, civil_status, emergency_info, user_address, job_title, phone_number, " &
                "email_address, username " &
                "FROM dbo.patient_information WHERE patient_Id = @id"

            Using selCmd As New SqlCommand(selSql, conn)
                selCmd.Parameters.Add("@id", SqlDbType.Int).Value = patientId
                Using r As SqlDataReader = selCmd.ExecuteReader()
                    If Not r.Read() Then
                        MessageBox.Show("Patient record not found.", "Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    firstName = If(IsDBNull(r("first_Name")), "", r("first_Name").ToString().Trim())
                    middleName = If(IsDBNull(r("middle_Name")), "", r("middle_Name").ToString().Trim())
                    surname = If(IsDBNull(r("surname")), "", r("surname").ToString().Trim())
                    nameExt = If(IsDBNull(r("name_extension")), "", r("name_extension").ToString().Trim())
                    ageVal = If(IsDBNull(r("age")), 0, Convert.ToInt32(r("age")))
                    birthDateVal = If(IsDBNull(r("birth_Date")), Date.Today, Convert.ToDateTime(r("birth_Date")))
                    genderVal = If(IsDBNull(r("gender")), "Male", r("gender").ToString().Trim())
                    civilVal = If(IsDBNull(r("civil_status")), "Single", r("civil_status").ToString().Trim())
                    emergencyVal = If(IsDBNull(r("emergency_info")), "", r("emergency_info").ToString().Trim())
                    addressVal = If(IsDBNull(r("user_address")), "", r("user_address").ToString().Trim())
                    jobVal = If(IsDBNull(r("job_title")), "", r("job_title").ToString().Trim())
                    phoneVal = If(IsDBNull(r("phone_number")), "", r("phone_number").ToString().Trim())
                    emailVal = If(IsDBNull(r("email_address")), "", r("email_address").ToString().Trim())
                    usernameVal = If(IsDBNull(r("username")), "", r("username").ToString().Trim())
                End Using
            End Using

            Using dlg As New Form()
                dlg.Text = "Edit Patient Information - ID #" & patientId.ToString()
                dlg.Size = New Size(520, 650)
                dlg.StartPosition = FormStartPosition.CenterParent
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog
                dlg.MaximizeBox = False
                dlg.MinimizeBox = False
                dlg.BackColor = Color.White
                dlg.Font = New Font("Segoe UI", 9.5F)

                Dim y As Integer = 16
                Dim labelW As Integer = 145
                Dim inputX As Integer = 165
                Dim inputW As Integer = 315

                Dim addRow = Function(caption As String, ctrl As Control) As Control
                                 Dim lbl As New Label() With {
                                     .Text = caption,
                                     .Left = 18,
                                     .Top = y + 4,
                                     .Width = labelW,
                                     .ForeColor = Color.FromArgb(35, 35, 35)
                                 }
                                 ctrl.Left = inputX
                                 ctrl.Top = y
                                 ctrl.Width = inputW
                                 dlg.Controls.Add(lbl)
                                 dlg.Controls.Add(ctrl)
                                 y += 38
                                 Return ctrl
                             End Function

                Dim txtFirst As TextBox = DirectCast(addRow("First Name *:", New TextBox() With {.Text = firstName}), TextBox)
                Dim txtMiddle As TextBox = DirectCast(addRow("Middle Name:", New TextBox() With {.Text = middleName}), TextBox)
                Dim txtSurname As TextBox = DirectCast(addRow("Surname *:", New TextBox() With {.Text = surname}), TextBox)

                Dim cmbExt As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbExt.Items.AddRange(New Object() {"None", "Jr.", "Sr.", "II", "III", "IV", "V"})
                cmbExt.SelectedItem = If(cmbExt.Items.Contains(nameExt) AndAlso nameExt <> "", nameExt, "None")
                addRow("Name Extension:", cmbExt)

                Dim txtAge As TextBox = DirectCast(addRow("Age *:", New TextBox() With {.Text = ageVal.ToString(), .MaxLength = 2}), TextBox)

                Dim dtpBirth As New DateTimePicker() With {
                    .Format = DateTimePickerFormat.Short
                }
                If birthDateVal < dtpBirth.MinDate OrElse birthDateVal > Date.Today Then
                    dtpBirth.Value = Date.Today
                Else
                    dtpBirth.Value = birthDateVal
                End If
                addRow("Birth Date *:", dtpBirth)

                Dim cmbGender As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbGender.Items.AddRange(New Object() {"Male", "Female", "Prefer not to say"})
                cmbGender.SelectedItem = If(cmbGender.Items.Contains(genderVal), genderVal, "Male")
                addRow("Gender *:", cmbGender)

                Dim cmbCivil As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbCivil.Items.AddRange(New Object() {"Single", "Married", "Widowed", "Divorced", "Separated"})
                cmbCivil.SelectedItem = If(cmbCivil.Items.Contains(civilVal), civilVal, "Single")
                addRow("Civil Status *:", cmbCivil)

                Dim txtEmergency As TextBox = DirectCast(addRow("Emergency Contact:", New TextBox() With {.Text = emergencyVal}), TextBox)
                Dim txtAddress As TextBox = DirectCast(addRow("Address *:", New TextBox() With {.Text = addressVal}), TextBox)

                Dim cmbJob As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbJob.Items.AddRange(New Object() {"Student", "Employed", "Self-employed", "Unemployed", "Retired", "Other"})
                cmbJob.SelectedItem = If(cmbJob.Items.Contains(jobVal), jobVal, "Other")
                addRow("Job Title:", cmbJob)

                Dim txtPhone As TextBox = DirectCast(addRow("Phone (11 digits) *:", New TextBox() With {.Text = phoneVal, .MaxLength = 11}), TextBox)
                Dim txtEmail As TextBox = DirectCast(addRow("Email Address:", New TextBox() With {.Text = emailVal}), TextBox)
                Dim txtUsername As TextBox = DirectCast(addRow("Username:", New TextBox() With {.Text = usernameVal}), TextBox)

                Dim btnSave As New Button() With {
                    .Text = "Save Changes",
                    .Left = 240,
                    .Top = y + 8,
                    .Width = 120,
                    .Height = 34,
                    .BackColor = Color.FromArgb(76, 175, 80),
                    .ForeColor = Color.White,
                    .FlatStyle = FlatStyle.Flat
                }

                Dim btnCancel As New Button() With {
                    .Text = "Cancel",
                    .Left = 368,
                    .Top = y + 8,
                    .Width = 112,
                    .Height = 34,
                    .BackColor = Color.FromArgb(140, 140, 140),
                    .ForeColor = Color.White,
                    .FlatStyle = FlatStyle.Flat,
                    .DialogResult = DialogResult.Cancel
                }

                AddHandler btnSave.Click, Sub(s, ev)
                                              If String.IsNullOrWhiteSpace(txtFirst.Text) OrElse
                                                 String.IsNullOrWhiteSpace(txtSurname.Text) OrElse
                                                 String.IsNullOrWhiteSpace(txtAddress.Text) OrElse
                                                 String.IsNullOrWhiteSpace(txtPhone.Text) Then
                                                  MessageBox.Show("Please fill in all required fields (*).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                  Return
                                              End If

                                              Dim parsedAge As Integer
                                              If Not Integer.TryParse(txtAge.Text.Trim(), parsedAge) OrElse parsedAge < 0 OrElse parsedAge > 99 Then
                                                  MessageBox.Show("Please enter a valid age (0-99).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                  Return
                                              End If

                                              If txtPhone.Text.Trim().Length <> 11 OrElse Not IsNumeric(txtPhone.Text.Trim()) Then
                                                  MessageBox.Show("Phone number must be 11 digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                  Return
                                              End If

                                              Try
                                                  If conn.State <> ConnectionState.Open Then conn.Open()

                                                  Dim extSave As String = If(cmbExt.SelectedItem Is Nothing OrElse cmbExt.SelectedItem.ToString() = "None",
                                                                           "",
                                                                           cmbExt.SelectedItem.ToString().Trim())
                                                  Dim jobSave As String = If(cmbJob.SelectedItem Is Nothing, "", cmbJob.SelectedItem.ToString().Trim())
                                                  Dim emergSave As String = If(String.IsNullOrWhiteSpace(txtEmergency.Text), "N/A", txtEmergency.Text.Trim())

                                                  Dim updSql As String =
                                                      "UPDATE dbo.patient_information SET " &
                                                      "first_Name = @first_Name, " &
                                                      "middle_Name = @middle_Name, " &
                                                      "surname = @surname, " &
                                                      "name_extension = @name_extension, " &
                                                      "age = @age, " &
                                                      "birth_Date = @birth_Date, " &
                                                      "gender = @gender, " &
                                                      "civil_status = @civil_status, " &
                                                      "emergency_info = @emergency_info, " &
                                                      "user_address = @user_address, " &
                                                      "job_title = @job_title, " &
                                                      "phone_number = @phone_number, " &
                                                      "email_address = @email_address, " &
                                                      "username = @username " &
                                                      "WHERE patient_Id = @id"

                                                  Using updCmd As New SqlCommand(updSql, conn)
                                                      With updCmd.Parameters
                                                          .Add("@first_Name", SqlDbType.VarChar, 100).Value = txtFirst.Text.Trim()
                                                          .Add("@middle_Name", SqlDbType.VarChar, 100).Value = txtMiddle.Text.Trim()
                                                          .Add("@surname", SqlDbType.VarChar, 100).Value = txtSurname.Text.Trim()
                                                          .Add("@name_extension", SqlDbType.VarChar, 20).Value = extSave
                                                          .Add("@age", SqlDbType.Int).Value = parsedAge
                                                          .Add("@birth_Date", SqlDbType.Date).Value = dtpBirth.Value.Date
                                                          .Add("@gender", SqlDbType.VarChar, 50).Value = cmbGender.SelectedItem.ToString()
                                                          .Add("@civil_status", SqlDbType.VarChar, 50).Value = cmbCivil.SelectedItem.ToString()
                                                          .Add("@emergency_info", SqlDbType.VarChar, 200).Value = emergSave
                                                          .Add("@user_address", SqlDbType.VarChar, 255).Value = txtAddress.Text.Trim()
                                                          .Add("@job_title", SqlDbType.VarChar, 100).Value = jobSave
                                                          .Add("@phone_number", SqlDbType.VarChar, 20).Value = txtPhone.Text.Trim()
                                                          .Add("@email_address", SqlDbType.VarChar, 150).Value = txtEmail.Text.Trim()
                                                          .Add("@username", SqlDbType.VarChar, 100).Value = txtUsername.Text.Trim()
                                                          .Add("@id", SqlDbType.Int).Value = patientId
                                                      End With
                                                      updCmd.ExecuteNonQuery()
                                                  End Using

                                                  AdminFrm.LogNotification("updated patient ID " & patientId.ToString())
                                                  dlg.DialogResult = DialogResult.OK
                                                  dlg.Close()
                                              Catch ex As Exception
                                                  MessageBox.Show("Failed to update patient: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                              End Try
                                          End Sub

                dlg.Controls.Add(btnSave)
                dlg.Controls.Add(btnCancel)
                dlg.AcceptButton = btnSave
                dlg.CancelButton = btnCancel

                If dlg.ShowDialog(Me.FindForm()) = DialogResult.OK Then
                    LoadPatients(searchtxbx.Text.Trim())
                    ViewPatientProfile(patientId)
                    ViewPhoto(patientId)
                    MessageBox.Show("Patient record updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error opening editor: " & ex.Message, "Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ---------- EVENTS ----------
    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        PatientInfo.Show()
    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged
        If isLoading Then Return
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub selectionposcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectionposcmbx.SelectedIndexChanged
        If isLoading Then Return
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        isLoading = True
        searchtxbx.Clear()
        selectionposcmbx.SelectedIndex = 0
        isLoading = False
        LoadPatients("")
        ShowFirstPatient()
    End Sub

    ' ---------- EDIT / DELETE / VIEW ----------
    Private Sub PatientTbl_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PatientTbl.CellContentClick
        If e.RowIndex < 0 Then Return

        Dim id As Integer = Convert.ToInt32(PatientTbl.Rows(e.RowIndex).Cells(0).Value)
        Dim fullName As String = Convert.ToString(PatientTbl.Rows(e.RowIndex).Cells(1).Value) & " " &
                                 Convert.ToString(PatientTbl.Rows(e.RowIndex).Cells(3).Value)

        Select Case e.ColumnIndex
            Case 19     ' VIEW
                ViewPatientProfile(id)
                ViewPhoto(id)

            Case 18     ' DELETE
                If MessageBox.Show("Are you sure you want to delete " & fullName & "?",
                                   "Confirm Delete", MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning) = DialogResult.Yes Then
                    Try
                        If conn.State <> ConnectionState.Open Then conn.Open()

                        query = "DELETE FROM dbo.patient_information WHERE patient_Id = @id"
                        Using comm As New SqlCommand(query, conn)
                            comm.Parameters.Add("@id", SqlDbType.Int).Value = id
                            comm.ExecuteNonQuery()
                        End Using

                        AdminFrm.LogNotification("deleted a patient record")

                        LoadPatients(searchtxbx.Text.Trim())
                        ShowFirstPatient()
                        MessageBox.Show("Patient deleted.", "Delete",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MsgBox(ex.Message)
                    End Try
                End If

            Case 17     ' EDIT
                OpenEditPatientDialog(id)
        End Select
    End Sub

    Private Sub PatientRecord_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        FormatBannerLayout()
    End Sub

    Private Sub PatientTbl_Resize(sender As Object, e As EventArgs) Handles PatientTbl.Resize
        ApplyColumnWidths()
    End Sub

    ' ---------- COLUMN WIDTHS ----------
    Private Sub ApplyColumnWidths()
        If PatientTbl.Columns.Count = 0 Then Return

        Dim total As Single = 0
        For Each r In ratios
            total += r
        Next

        Dim avail As Integer = Math.Max(PatientTbl.ClientSize.Width, 1450)
        PatientTbl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        Dim colCount As Integer = Math.Min(ratios.Length, PatientTbl.Columns.Count)
        Dim usedWidth As Integer = 0

        For i As Integer = 0 To colCount - 1
            With PatientTbl.Columns(i)
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .MinimumWidth = 45
                If i = colCount - 1 Then
                    .Width = Math.Max(60, avail - usedWidth)
                Else
                    Dim w As Integer = CInt(avail * ratios(i) / total)
                    .Width = w
                    usedWidth += w
                End If
                .Resizable = DataGridViewTriState.False
                .SortMode = DataGridViewColumnSortMode.NotSortable
            End With
        Next
    End Sub

    ' ---------- STYLE ----------
    Private Sub StyleGrid()
        With PatientTbl
            ApplyColumnWidths()

            For Each i In {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}
                If i < .Columns.Count Then
                    .Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Columns(i).DefaultCellStyle.Padding = Padding.Empty
                End If
            Next

            If .Columns.Count > 16 AndAlso TypeOf .Columns(16) Is DataGridViewImageColumn Then
                CType(.Columns(16), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            End If

            For i As Integer = 17 To 19
                If i < .Columns.Count AndAlso TypeOf .Columns(i) Is DataGridViewButtonColumn Then
                    CType(.Columns(i), DataGridViewButtonColumn).FlatStyle = FlatStyle.Flat
                End If
            Next

            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .RowTemplate.Height = 76
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToOrderColumns = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 48

            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.FromArgb(225, 235, 228)
            .BackgroundColor = Color.White
            .ScrollBars = ScrollBars.Both

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            Dim row1 As Color = Color.FromArgb(236, 247, 239)
            Dim row2 As Color = Color.White

            .DefaultCellStyle.BackColor = row2
            .DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9)
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DefaultCellStyle.SelectionBackColor = row2
            .DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30)
            .AlternatingRowsDefaultCellStyle.BackColor = row1
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = row1
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30)

            .ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(76, 175, 80)
            .ThemeStyle.HeaderStyle.ForeColor = Color.White
            .ThemeStyle.RowsStyle.BackColor = row2
            .ThemeStyle.RowsStyle.SelectionBackColor = row2
            .ThemeStyle.AlternatingRowsStyle.BackColor = row1
            .ThemeStyle.AlternatingRowsStyle.SelectionBackColor = row1
        End With
    End Sub

    ' ---------- EDIT / DELETE / VIEW COLORS ----------
    Private Sub ColorButtons()
        Dim colors() As Color = {Color.FromArgb(76, 175, 80),
                                 Color.FromArgb(211, 74, 74),
                                 Color.FromArgb(52, 120, 190)}

        For Each r As DataGridViewRow In PatientTbl.Rows
            For k As Integer = 0 To 2
                Dim idx As Integer = 17 + k
                If idx < PatientTbl.Columns.Count Then
                    With r.Cells(idx).Style
                        .BackColor = colors(k)
                        .ForeColor = Color.White
                        .SelectionBackColor = colors(k)
                        .SelectionForeColor = Color.White
                        .Alignment = DataGridViewContentAlignment.MiddleCenter
                    End With
                End If
            Next
        Next
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint
    End Sub

End Class