Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Text.RegularExpressions

Public Class StaffManagement

    Private isLoading As Boolean = True

    ' ===================== PHOTO & PROFILE BANNER =====================
    Private Sub ViewPhoto(id As String)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            query = "SELECT user_photo FROM dbo.staff_information WHERE staff_code = @staff_code"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@staff_code", SqlDbType.VarChar, 50).Value = id
                Using reader As SqlDataReader = comm.ExecuteReader()
                    If reader.Read() AndAlso Not IsDBNull(reader("user_photo")) Then
                        Dim img() As Byte = CType(reader("user_photo"), Byte())
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
        Catch
            profileimg.Image = Nothing
        End Try
    End Sub

    Private Sub ClearBanner()
        Label10.Text = ""
        Label2.Text = "No staff found"
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
    End Sub

    Private Sub ShowFirstStaff()
        If stafftbl.Rows.Count > 0 Then
            Dim code As String = Convert.ToString(stafftbl.Rows(0).Cells(0).Value)
            ViewUserProfile(code)
            ViewPhoto(code)
        Else
            ClearBanner()
        End If
    End Sub

    Private Sub ViewUserProfile(id As String)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            query = "SELECT * FROM dbo.staff_information WHERE staff_code = @staff_code"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@staff_code", SqlDbType.VarChar, 50).Value = id
                Using reader As SqlDataReader = comm.ExecuteReader()
                    If reader.Read() Then
                        Label10.Text = reader("Position").ToString()
                        Label2.Text = (reader("first_Name").ToString() & " " &
                                       reader("middle_Name").ToString() & " " &
                                       reader("surname").ToString()).Replace("  ", " ").Trim()
                        Label3.Text = reader("age").ToString()
                        Label12.Text = If(IsDBNull(reader("created_account")),
                                          "N/A",
                                          Convert.ToDateTime(reader("created_account")).ToString("MMMM dd, yyyy"))
                        Label6.Text = reader("gender").ToString()
                        Label7.Text = reader("civil_status").ToString()
                        Label5.Text = reader("user_address").ToString()
                        Label8.Text = reader("phone_number").ToString()
                        Label4.Text = reader("staff_code").ToString()
                        Label9.Text = Convert.ToDateTime(reader("birth_Date")).ToString("MMMM dd, yyyy")
                        Label11.Text = reader("email_address").ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error viewing profile: " & ex.Message, "Staff Management",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== LOAD / SEARCH / FILTER =====================
    Private Sub LoadStaff(Optional search As String = "", Optional roleFilter As String = "All")
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            stafftbl.Rows.Clear()

            query = "SELECT * FROM dbo.staff_information " &
                    "WHERE (staff_code LIKE @search " &
                    "OR first_Name LIKE @search " &
                    "OR middle_Name LIKE @search " &
                    "OR surname LIKE @search " &
                    "OR username LIKE @search) " &
                    "AND (@role = 'All' OR Position = @role) " &
                    "ORDER BY staff_Id"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@search", SqlDbType.VarChar, 150).Value = "%" & search.Trim() & "%"
                comm.Parameters.Add("@role", SqlDbType.VarChar, 100).Value = roleFilter

                Using reader As SqlDataReader = comm.ExecuteReader()
                    While reader.Read()
                        stafftbl.Rows.Add(reader("staff_code"),
                                          reader("first_Name"),
                                          reader("middle_Name"),
                                          reader("surname"),
                                          reader("name_extension"),
                                          Convert.ToDateTime(reader("birth_Date")).ToString("dd/MM/yyyy"),
                                          reader("age"),
                                          reader("birth_place"),
                                          reader("user_address"),
                                          reader("gender"),
                                          reader("civil_status"),
                                          reader("Position"),
                                          reader("phone_number"),
                                          reader("email_address"),
                                          reader("username"),
                                          reader("user_password"),
                                          "Edit",
                                          "Delete",
                                          "View")
                    End While
                End Using
            End Using

            ColorActionButtons()
            stafftbl.CurrentCell = Nothing
            stafftbl.ClearSelection()

        Catch ex As Exception
            MessageBox.Show("Error loading staff: " & ex.Message, "Staff Management",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSelectedRoleFilter() As String
        If selectionposcmbx.SelectedItem Is Nothing Then Return "All"
        Dim selectedPos As String = selectionposcmbx.SelectedItem.ToString()
        Select Case selectedPos.Trim().ToLower()
            Case "admin"
                Return "Admin"
            Case "doctor"
                Return "Doctor"
            Case "barangay assistance"
                Return "Barangay Assistance"
            Case Else
                Return "All"
        End Select
    End Function

    Private Sub ColorActionButtons()
        Dim colors() As Color = {
            Color.FromArgb(76, 175, 80),   ' Edit (Green)
            Color.FromArgb(211, 74, 74),   ' Delete (Red)
            Color.FromArgb(52, 120, 190)   ' View (Blue)
        }

        For Each r As DataGridViewRow In stafftbl.Rows
            For k As Integer = 0 To 2
                Dim idx As Integer = 16 + k
                If idx < stafftbl.Columns.Count Then
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

    ' ===================== FORM LOAD & EVENTS =====================
    Private Sub StaffManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()

        If selectionposcmbx.Items.Count = 0 Then
            selectionposcmbx.Items.AddRange(New Object() {"All", "Admin", "Doctor", "Barangay Assistance"})
        End If
        If selectionposcmbx.SelectedIndex = -1 Then
            selectionposcmbx.SelectedIndex = 0
        End If

        LoadStaff("", GetSelectedRoleFilter())
        isLoading = False
        ShowFirstStaff()
    End Sub

    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        RegisterFrm.Show()
    End Sub

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        isLoading = True
        searchtxbx.Clear()
        If selectionposcmbx.Items.Count > 0 Then selectionposcmbx.SelectedIndex = 0
        isLoading = False

        If stafftbl.ScrollBars = ScrollBars.Horizontal Then
            stafftbl.ScrollBars = ScrollBars.Both
        End If

        LoadStaff("", "All")
        ShowFirstStaff()
    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        LoadStaff(searchtxbx.Text, GetSelectedRoleFilter())
        ShowFirstStaff()
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged
        If isLoading Then Return
        LoadStaff(searchtxbx.Text, GetSelectedRoleFilter())
        ShowFirstStaff()
    End Sub

    Private Sub selectionposcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectionposcmbx.SelectedIndexChanged
        If isLoading Then Return
        LoadStaff(searchtxbx.Text, GetSelectedRoleFilter())
        ShowFirstStaff()
    End Sub

    ' ===================== EDIT / DELETE / VIEW =====================
    Private Sub stafftbl_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles stafftbl.CellContentClick
        If e.RowIndex < 0 Then Return

        Dim selectedRow As DataGridViewRow = stafftbl.Rows(e.RowIndex)
        Dim staffCode As String = Convert.ToString(selectedRow.Cells(0).Value)
        Dim fullName As String = (Convert.ToString(selectedRow.Cells(1).Value) & " " &
                                  Convert.ToString(selectedRow.Cells(3).Value)).Trim()

        Dim colName As String = stafftbl.Columns(e.ColumnIndex).Name.ToLower()

        ' Support parehong column index (16=Edit, 17=Delete, 18=View) at button column names
        If e.ColumnIndex = 18 OrElse colName.Contains("view") Then
            ViewUserProfile(staffCode)
            ViewPhoto(staffCode)

        ElseIf e.ColumnIndex = 17 OrElse colName.Contains("del") Then
            DeleteStaff(staffCode, fullName)

        ElseIf e.ColumnIndex = 16 OrElse colName.Contains("edit") Then
            OpenEditStaffDialog(staffCode)
        End If
    End Sub

    Private Sub DeleteStaff(staffCode As String, fullName As String)
        Dim confirm As DialogResult = MessageBox.Show(
            "Are you sure you want to delete " & fullName & " (" & staffCode & ")?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        )

        If confirm <> DialogResult.Yes Then Return

        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            query = "DELETE FROM dbo.staff_information WHERE staff_code = @staff_code"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@staff_code", SqlDbType.VarChar, 50).Value = staffCode
                comm.ExecuteNonQuery()
            End Using

            LoadStaff(searchtxbx.Text, GetSelectedRoleFilter())
            ShowFirstStaff()

            MessageBox.Show("Staff record deleted successfully.", "Delete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Delete failed: " & ex.Message, "Database Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== BUILT-IN EDIT DIALOG =====================
    Private Sub OpenEditStaffDialog(staffCode As String)
        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            Dim fName As String = ""
            Dim mName As String = ""
            Dim sName As String = ""
            Dim ext As String = "None"
            Dim bDate As Date = Date.Today
            Dim bPlace As String = ""
            Dim addr As String = ""
            Dim gdr As String = "Male"
            Dim civil As String = "Single"
            Dim pos As String = "Admin"
            Dim phone As String = ""
            Dim email As String = ""
            Dim uName As String = ""
            Dim uPass As String = ""

            query = "SELECT * FROM dbo.staff_information WHERE staff_code = @staff_code"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.Add("@staff_code", SqlDbType.VarChar, 50).Value = staffCode
                Using reader As SqlDataReader = comm.ExecuteReader()
                    If Not reader.Read() Then
                        MessageBox.Show("Staff record not found.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    fName = reader("first_Name").ToString()
                    mName = reader("middle_Name").ToString()
                    sName = reader("surname").ToString()
                    ext = reader("name_extension").ToString()
                    bDate = Convert.ToDateTime(reader("birth_Date"))
                    bPlace = reader("birth_place").ToString()
                    addr = reader("user_address").ToString()
                    gdr = reader("gender").ToString()
                    civil = reader("civil_status").ToString()
                    pos = reader("Position").ToString()
                    phone = reader("phone_number").ToString()
                    email = reader("email_address").ToString()
                    uName = reader("username").ToString()
                    uPass = reader("user_password").ToString()
                End Using
            End Using

            Using dlg As New Form()
                dlg.Text = "Edit Staff - " & staffCode
                dlg.StartPosition = FormStartPosition.CenterParent
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog
                dlg.MaximizeBox = False
                dlg.MinimizeBox = False
                dlg.ClientSize = New Size(520, 560)
                dlg.BackColor = Color.White
                dlg.Font = New Font("Segoe UI", 9.5F)

                Dim y As Integer = 18
                Dim AddField = Function(caption As String, ctl As Control) As Control
                                   Dim lbl As New Label With {
                                       .Text = caption,
                                       .Location = New Point(20, y + 3),
                                       .Size = New Size(140, 22),
                                       .ForeColor = Color.FromArgb(40, 40, 40)
                                   }
                                   ctl.Location = New Point(170, y)
                                   ctl.Size = New Size(320, 26)
                                   dlg.Controls.Add(lbl)
                                   dlg.Controls.Add(ctl)
                                   y += 34
                                   Return ctl
                               End Function

                Dim txtFirst As New TextBox With {.Text = fName}
                Dim txtMid As New TextBox With {.Text = mName}
                Dim txtSur As New TextBox With {.Text = sName}

                Dim cmbExt As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbExt.Items.AddRange(New Object() {"None", "Jr.", "Sr.", "II", "III", "IV", "V"})
                If cmbExt.Items.Contains(ext) Then cmbExt.SelectedItem = ext Else cmbExt.SelectedIndex = 0

                Dim dtpBirth As New DateTimePicker With {
                    .Format = DateTimePickerFormat.Short,
                    .Value = If(bDate < New Date(1900, 1, 1) OrElse bDate > Date.Today, Date.Today, bDate)
                }

                Dim txtPlace As New TextBox With {.Text = bPlace}
                Dim txtAddr As New TextBox With {.Text = addr}

                Dim cmbGender As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbGender.Items.AddRange(New Object() {"Male", "Female", "Prefer not to say"})
                If cmbGender.Items.Contains(gdr) Then cmbGender.SelectedItem = gdr Else cmbGender.SelectedIndex = 0

                Dim cmbCivil As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbCivil.Items.AddRange(New Object() {"Single", "Married", "Widowed", "Divorced", "Separated", "Prefer not to say"})
                If cmbCivil.Items.Contains(civil) Then cmbCivil.SelectedItem = civil Else cmbCivil.SelectedIndex = 0

                Dim cmbPos As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                cmbPos.Items.AddRange(New Object() {"Admin", "Barangay Assistance", "Doctor"})
                If cmbPos.Items.Contains(pos) Then cmbPos.SelectedItem = pos Else cmbPos.SelectedIndex = 0

                Dim txtPhone As New TextBox With {.Text = phone, .MaxLength = 11}
                Dim txtEmail As New TextBox With {.Text = email}
                Dim txtUser As New TextBox With {.Text = uName}
                Dim txtPass As New TextBox With {.Text = uPass}

                AddField("First Name *", txtFirst)
                AddField("Middle Name", txtMid)
                AddField("Surname *", txtSur)
                AddField("Extension", cmbExt)
                AddField("Birth Date *", dtpBirth)
                AddField("Birth Place *", txtPlace)
                AddField("Address *", txtAddr)
                AddField("Gender *", cmbGender)
                AddField("Civil Status *", cmbCivil)
                AddField("Position *", cmbPos)
                AddField("Phone (11 digits) *", txtPhone)
                AddField("Email", txtEmail)
                AddField("Username *", txtUser)
                AddField("Password *", txtPass)

                Dim btnSave As New Button With {
                    .Text = "Save Changes",
                    .BackColor = Color.FromArgb(76, 175, 80),
                    .ForeColor = Color.White,
                    .FlatStyle = FlatStyle.Flat,
                    .Location = New Point(260, y + 10),
                    .Size = New Size(115, 34)
                }
                btnSave.FlatAppearance.BorderSize = 0

                Dim btnCancel As New Button With {
                    .Text = "Cancel",
                    .DialogResult = DialogResult.Cancel,
                    .Location = New Point(385, y + 10),
                    .Size = New Size(105, 34)
                }

                AddHandler btnSave.Click,
                    Sub()
                        If txtFirst.Text.Trim() = "" OrElse txtSur.Text.Trim() = "" OrElse
                           txtPlace.Text.Trim() = "" OrElse txtAddr.Text.Trim() = "" OrElse
                           txtPhone.Text.Trim() = "" OrElse txtUser.Text.Trim() = "" OrElse
                           txtPass.Text.Trim() = "" Then
                            MessageBox.Show("Please complete all required fields.", "Validation",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If

                        If Not Regex.IsMatch(txtPhone.Text.Trim(), "^\d{11}$") Then
                            MessageBox.Show("Phone number must be 11 digits.", "Validation",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If

                        If Not Regex.IsMatch(txtPass.Text, "^(?=.*[A-Z])(?=.*[0-9]).{6,}$") Then
                            MessageBox.Show("Password must be at least 6 characters with 1 uppercase letter and 1 number.",
                                            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If

                        Dim age As Integer = Date.Today.Year - dtpBirth.Value.Year
                        If dtpBirth.Value.Date > Date.Today.AddYears(-age) Then age -= 1
                        If age < 0 OrElse age > 99 Then
                            MessageBox.Show("Invalid birth date / age.", "Validation",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If

                        ' Check duplicate username excluding current staffCode
                        Dim dupSql As String = "SELECT COUNT(*) FROM dbo.staff_information WHERE username = @u AND staff_code <> @code"
                        Using dupCmd As New SqlCommand(dupSql, conn)
                            dupCmd.Parameters.Add("@u", SqlDbType.VarChar, 100).Value = txtUser.Text.Trim()
                            dupCmd.Parameters.Add("@code", SqlDbType.VarChar, 50).Value = staffCode
                            If Convert.ToInt32(dupCmd.ExecuteScalar()) > 0 Then
                                MessageBox.Show("Username is already taken by another staff.", "Validation",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                            End If
                        End Using

                        Dim updSql As String =
                            "UPDATE dbo.staff_information SET " &
                            "first_Name = @first, middle_Name = @mid, surname = @sur, name_extension = @ext, " &
                            "birth_Date = @bdate, age = @age, birth_place = @bplace, user_address = @addr, " &
                            "gender = @gender, civil_status = @civil, Position = @pos, " &
                            "phone_number = @phone, email_address = @email, username = @user, user_password = @pass " &
                            "WHERE staff_code = @code"

                        Using updCmd As New SqlCommand(updSql, conn)
                            updCmd.Parameters.Add("@first", SqlDbType.VarChar, 100).Value = txtFirst.Text.Trim()
                            updCmd.Parameters.Add("@mid", SqlDbType.VarChar, 100).Value = txtMid.Text.Trim()
                            updCmd.Parameters.Add("@sur", SqlDbType.VarChar, 100).Value = txtSur.Text.Trim()
                            updCmd.Parameters.Add("@ext", SqlDbType.VarChar, 20).Value = cmbExt.Text
                            updCmd.Parameters.Add("@bdate", SqlDbType.Date).Value = dtpBirth.Value.Date
                            updCmd.Parameters.Add("@age", SqlDbType.Int).Value = age
                            updCmd.Parameters.Add("@bplace", SqlDbType.VarChar, 150).Value = txtPlace.Text.Trim()
                            updCmd.Parameters.Add("@addr", SqlDbType.VarChar, 255).Value = txtAddr.Text.Trim()
                            updCmd.Parameters.Add("@gender", SqlDbType.VarChar, 50).Value = cmbGender.Text
                            updCmd.Parameters.Add("@civil", SqlDbType.VarChar, 50).Value = cmbCivil.Text
                            updCmd.Parameters.Add("@pos", SqlDbType.VarChar, 100).Value = cmbPos.Text
                            updCmd.Parameters.Add("@phone", SqlDbType.VarChar, 20).Value = txtPhone.Text.Trim()
                            updCmd.Parameters.Add("@email", SqlDbType.VarChar, 150).Value = txtEmail.Text.Trim()
                            updCmd.Parameters.Add("@user", SqlDbType.VarChar, 100).Value = txtUser.Text.Trim()
                            updCmd.Parameters.Add("@pass", SqlDbType.VarChar, 255).Value = txtPass.Text
                            updCmd.Parameters.Add("@code", SqlDbType.VarChar, 50).Value = staffCode
                            updCmd.ExecuteNonQuery()
                        End Using

                        dlg.DialogResult = DialogResult.OK
                        dlg.Close()
                    End Sub

                dlg.Controls.Add(btnSave)
                dlg.Controls.Add(btnCancel)
                dlg.AcceptButton = btnSave
                dlg.CancelButton = btnCancel

                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    LoadStaff(searchtxbx.Text, GetSelectedRoleFilter())
                    ViewUserProfile(staffCode)
                    ViewPhoto(staffCode)
                    MessageBox.Show("Staff record updated successfully.", "Updated",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error editing staff: " & ex.Message, "Database Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

End Class