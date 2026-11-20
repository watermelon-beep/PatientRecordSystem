Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Public Class RegisterFrm

    ' ===================== FULLSCREEN / AUTO-SCALE =====================
    Private Class LayoutInfo
        Public Ctl As Control
        Public Bounds As Rectangle
        Public OrigFont As Font
        Public ItemHeight As Integer
        Public ParentDesign As Size
    End Class

    Private layoutList As New List(Of LayoutInfo)
    Private designSize As Size
    Private isScaling As Boolean = False

    Public Sub New()
        InitializeComponent()

        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.MaximumSize = New Size(0, 0)

        Me.AutoScaleMode = AutoScaleMode.None
        Me.AutoSize = False
        Me.DoubleBuffered = True


        designSize = Me.ClientSize
        CaptureLayout(Me)
    End Sub

    Private Sub CaptureLayout(parent As Control)
        Dim pw As Integer = parent.ClientSize.Width
        Dim ph As Integer = parent.ClientSize.Height
        For Each c As Control In parent.Controls
            If c.Dock = DockStyle.None Then
                pw = Math.Max(pw, c.Right + 8)
                ph = Math.Max(ph, c.Bottom + 8)
            End If
        Next

        For Each c As Control In parent.Controls
            Dim info As New LayoutInfo With {
                .Ctl = c,
                .Bounds = c.Bounds,
                .OrigFont = c.Font,
                .ParentDesign = New Size(pw, ph)
            }
            If TypeOf c Is ComboBox Then info.ItemHeight = DirectCast(c, ComboBox).ItemHeight
            If c.Dock = DockStyle.None Then c.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            layoutList.Add(info)
            CaptureLayout(c)
        Next
    End Sub

    Private Sub ScaleLayout()
        If isScaling Then Return
        If designSize.Width = 0 OrElse designSize.Height = 0 Then Return
        If Me.WindowState = FormWindowState.Minimized Then Return
        If Me.ClientSize.Width = 0 OrElse Me.ClientSize.Height = 0 Then Return

        isScaling = True
        Try
            Me.PerformLayout()

            Dim sf As Single = Math.Min(Me.ClientSize.Width / CSng(designSize.Width),
                                        Me.ClientSize.Height / CSng(designSize.Height))
            sf = Math.Max(0.6F, Math.Min(3.0F, sf))

            For Each info As LayoutInfo In layoutList
                Dim c As Control = info.Ctl
                Dim p As Control = c.Parent
                If p Is Nothing Then Continue For

                If c.Dock = DockStyle.None AndAlso info.ParentDesign.Width > 0 AndAlso info.ParentDesign.Height > 0 Then
                    Dim sx As Single = p.ClientSize.Width / CSng(info.ParentDesign.Width)
                    Dim sy As Single = p.ClientSize.Height / CSng(info.ParentDesign.Height)

                    c.Bounds = New Rectangle(CInt(info.Bounds.X * sx), CInt(info.Bounds.Y * sy),
                                             CInt(info.Bounds.Width * sx), CInt(info.Bounds.Height * sy))
                End If

                If info.OrigFont IsNot Nothing Then
                    Dim newSize As Single = Math.Max(6.0F, info.OrigFont.Size * sf)
                    If Math.Abs(c.Font.Size - newSize) > 0.1F Then
                        c.Font = New Font(info.OrigFont.FontFamily, newSize,
                                          info.OrigFont.Style, info.OrigFont.Unit)
                    End If
                End If

                If info.ItemHeight > 0 Then
                    Try
                        DirectCast(c, ComboBox).ItemHeight = Math.Max(10, CInt(info.ItemHeight * sf))
                    Catch
                    End Try
                End If
            Next
        Finally
            isScaling = False
        End Try
    End Sub

    Private Sub RegisterFrm_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        ScaleLayout()
    End Sub

    ' ===================== INPUT VALIDATION =====================
    Private ReadOnly badChars As New Dictionary(Of Guna.UI2.WinForms.Guna2TextBox, String)

    Private Sub SetupValidation()
        Dim nameBad As String = "[^\p{L} .'\-]"

        badChars(frstnmtxbx) = nameBad
        badChars(mdlnmtxbx) = nameBad
        badChars(srnmtxbx) = nameBad
        badChars(birthtxbx) = "[^\p{L} .,'\-]"
        badChars(addrtxbx) = "[^\p{L}\d .,'#\-/]"
        badChars(agetxbx) = "\D"
        badChars(phnumtxbx) = "\D"
        badChars(emailtxbx) = "[^A-Za-z0-9@._+\-]"
        badChars(regusrnmtxbx) = "[^A-Za-z0-9_.]"

        agetxbx.MaxLength = 2
        phnumtxbx.MaxLength = 11

        For Each tb As Guna.UI2.WinForms.Guna2TextBox In badChars.Keys
            AddHandler tb.KeyPress, AddressOf RestrictKey
            AddHandler tb.TextChanged, AddressOf CleanText
        Next

        For Each cb As ComboBox In New ComboBox() {excbx, gdrcmbx, cvlcbx, rolecbx, daycmbx, mnthcbx, yrcbx}
            cb.DropDownStyle = ComboBoxStyle.DropDownList
            AddHandler cb.KeyPress, AddressOf BlockTyping
        Next
    End Sub

    Private Sub RestrictKey(sender As Object, e As KeyPressEventArgs)
        If Char.IsControl(e.KeyChar) Then Return
        Dim tb As Guna.UI2.WinForms.Guna2TextBox = DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)
        If Regex.IsMatch(e.KeyChar.ToString(), badChars(tb)) Then e.Handled = True
    End Sub

    Private Sub CleanText(sender As Object, e As EventArgs)
        Dim tb As Guna.UI2.WinForms.Guna2TextBox = DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)
        Dim cleaned As String = Regex.Replace(tb.Text, badChars(tb), "")
        If cleaned <> tb.Text Then
            Dim pos As Integer = tb.SelectionStart - (tb.Text.Length - cleaned.Length)
            tb.Text = cleaned
            tb.SelectionStart = Math.Max(0, pos)
        End If
    End Sub

    Private Sub BlockTyping(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    ' ===================== CLEAR DATA =====================
    Private Sub clearData()
        frstnmtxbx.Clear()
        mdlnmtxbx.Clear()
        srnmtxbx.Clear()
        excbx.SelectedIndex = -1

        agetxbx.Clear()
        birthtxbx.Clear()
        addrtxbx.Clear()

        gdrcmbx.SelectedIndex = -1
        cvlcbx.SelectedIndex = -1

        regusrnmtxbx.Clear()
        regpasstxbx.Clear()
        cnfrmpasstxbx.Clear()

        rolecbx.SelectedIndex = -1

        phnumtxbx.Clear()
        emailtxbx.Clear()

        daycmbx.SelectedIndex = -1
        mnthcbx.SelectedIndex = -1
        yrcbx.SelectedIndex = -1

        If staffimgpcbx.Image IsNot Nothing Then
            staffimgpcbx.Image.Dispose()
            staffimgpcbx.Image = Nothing
        End If

        Label20.Visible = False

        Label13.Visible = True
        Label14.Visible = True
        Label15.Visible = True
        Label16.Visible = True
        Label17.Visible = True
        Label18.Visible = True
        Label19.Visible = True

        UpdatePasswordProgress()
    End Sub

    ' ===================== COMBOBOX LABEL =====================
    Private Sub cmbxLblVisible(cmbx As ComboBox, lbl As Label)
        lbl.Visible = (cmbx.SelectedIndex = -1)
    End Sub

    ' ===================== PHOTO TO BYTES =====================
    Private Function GetStaffImage() As Byte()
        If staffimgpcbx.Image Is Nothing Then Return Nothing

        Using ms As New MemoryStream()
            Using bmp As New Bitmap(staffimgpcbx.Image)
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
            End Using
            Return ms.ToArray()
        End Using
    End Function

    ' ===================== ADD STAFF =====================
    ' Returns True kung na-save, False kung may problema (hindi na buburahin ang form)
    Function addStaff() As Boolean

        ' --- required fields ---
        If frstnmtxbx.Text.Trim() = "" OrElse srnmtxbx.Text.Trim() = "" OrElse
           agetxbx.Text.Trim() = "" OrElse daycmbx.SelectedIndex = -1 OrElse
           mnthcbx.SelectedIndex = -1 OrElse yrcbx.SelectedIndex = -1 OrElse
           gdrcmbx.SelectedIndex = -1 OrElse cvlcbx.SelectedIndex = -1 OrElse
           birthtxbx.Text.Trim() = "" OrElse addrtxbx.Text.Trim() = "" OrElse
           phnumtxbx.Text.Trim() = "" OrElse regusrnmtxbx.Text.Trim() = "" OrElse
           rolecbx.SelectedIndex = -1 Then

            MessageBox.Show("Please complete all required fields.")
            Return False
        End If

        If phnumtxbx.Text.Length <> 11 Then
            MessageBox.Show("Phone number must contain 11 digits.")
            Return False
        End If

        If emailtxbx.Text.Trim() <> "" AndAlso
           Not Regex.IsMatch(emailtxbx.Text.Trim(), "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            MessageBox.Show("Invalid email address.")
            Return False
        End If

        ' --- birthdate and age ---
        Dim birthDate As DateTime
        Dim dateString As String = daycmbx.Text & " " & mnthcbx.Text & " " & yrcbx.Text

        If Not DateTime.TryParseExact(dateString, "dd MMMM yyyy", CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, birthDate) Then
            MessageBox.Show("Invalid date of birth.")
            Return False
        End If

        If birthDate.Date > Date.Today Then
            MessageBox.Show("Date of birth cannot be in the future.")
            Return False
        End If

        Dim actualAge As Integer = Date.Today.Year - birthDate.Year
        If birthDate.Date > Date.Today.AddYears(-actualAge) Then actualAge -= 1

        If actualAge < 0 OrElse actualAge > 99 Then
            MessageBox.Show("Invalid age.")
            Return False
        End If

        If Val(agetxbx.Text) <> actualAge Then
            MessageBox.Show("Age does not match the birthday. Correct age: " & actualAge.ToString())
            Return False
        End If

        ' --- password ---
        If Not Regex.IsMatch(regpasstxbx.Text, "^(?=.*[A-Z])(?=.*[0-9]).{6,}$") Then
            Label20.Visible = True
            MessageBox.Show("Password must contain at least 6 characters, one capital letter, and one number.")
            Return False
        End If

        If regpasstxbx.Text <> cnfrmpasstxbx.Text Then
            MessageBox.Show("Passwords do not match.")
            Return False
        End If

        Try
            If conn.State <> ConnectionState.Open Then conn.Open()

            If UsernameExists(regusrnmtxbx.Text.Trim()) Then
                MessageBox.Show("Username already exists.")
                Return False
            End If

            Dim photo As Byte() = GetStaffImage()

            query = "INSERT INTO staff_information (" &
                    "first_Name, middle_Name, surname, name_extension, age, birth_Date, birth_place, " &
                    "user_address, gender, civil_status, username, user_password, Position, " &
                    "phone_number, email_address, user_photo, created_account) " &
                    "VALUES (@first_Name, @middle_Name, @surname, @name_extension, @age, @birth_Date, @birth_place, " &
                    "@user_address, @gender, @civil_status, @username, @user_password, @Position, " &
                    "@phone_number, @email_address, @user_photo, @created_account)"

            Using comm As New SqlCommand(query, conn)
                With comm.Parameters
                    .AddWithValue("@first_Name", frstnmtxbx.Text.Trim())
                    .AddWithValue("@middle_Name", mdlnmtxbx.Text.Trim())
                    .AddWithValue("@surname", srnmtxbx.Text.Trim())
                    .AddWithValue("@name_extension", excbx.Text)
                    .AddWithValue("@age", actualAge)
                    .AddWithValue("@birth_Date", birthDate.Date)
                    .AddWithValue("@birth_place", birthtxbx.Text.Trim())
                    .AddWithValue("@user_address", addrtxbx.Text.Trim())
                    .AddWithValue("@gender", gdrcmbx.Text)
                    .AddWithValue("@civil_status", cvlcbx.Text)
                    .AddWithValue("@username", regusrnmtxbx.Text.Trim())
                    .AddWithValue("@user_password", regpasstxbx.Text)
                    .AddWithValue("@Position", rolecbx.Text)
                    .AddWithValue("@phone_number", phnumtxbx.Text)
                    .AddWithValue("@email_address", emailtxbx.Text.Trim())
                    .AddWithValue("@created_account", Date.Today)

                    If photo Is Nothing Then
                        .Add("@user_photo", SqlDbType.VarBinary, -1).Value = DBNull.Value
                    Else
                        .Add("@user_photo", SqlDbType.VarBinary, -1).Value = photo
                    End If
                End With

                comm.ExecuteNonQuery()
            End Using

            MessageBox.Show("Staff added successfully!", "Registration",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True

        Catch ex As Exception
            MessageBox.Show("Registration failed: " & ex.Message, "Database Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function

    ' ===================== REGISTER BUTTON =====================
    Private Sub regbtn_Click(sender As Object, e As EventArgs) Handles regbtn.Click
        If addStaff() Then clearData()
    End Sub

    ' ===================== FORM LOAD =====================
    Private Sub RegisterFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.MaximumSize = New Size(0, 0)

        dbConnection()

        gdrcmbx.Items.Clear()
        gdrcmbx.Items.AddRange(New Object() {"Male", "Female", "Prefer not to say"})

        cvlcbx.Items.Clear()
        cvlcbx.Items.AddRange(New Object() {"Single", "Married", "Widowed", "Divorced", "Separated", "Prefer not to say"})

        excbx.Items.Clear()
        excbx.Items.AddRange(New Object() {"None", "Jr.", "Sr.", "II", "III", "IV", "V"})

        rolecbx.Items.Clear()
        rolecbx.Items.AddRange(New Object() {"Admin", "Barangay Assistance", "Doctor"})

        daycmbx.Items.Clear()
        For i As Integer = 1 To 31
            daycmbx.Items.Add(i.ToString("00"))
        Next

        mnthcbx.Items.Clear()
        mnthcbx.Items.AddRange(New Object() {"January", "February", "March", "April", "May", "June",
                                             "July", "August", "September", "October", "November", "December"})

        yrcbx.Items.Clear()
        For y As Integer = Date.Now.Year To 1900 Step -1
            yrcbx.Items.Add(y.ToString())
        Next

        regprogress.Minimum = 0
        regprogress.Maximum = 100
        regprogress.Value = 0

        passbar.Minimum = 0
        passbar.Maximum = 100
        passbar.Value = 0

        Label4.Text = "empty"
        Label4.ForeColor = Color.Silver

        Label20.Visible = False

        Label13.Visible = True
        Label14.Visible = True
        Label15.Visible = True
        Label16.Visible = True
        Label17.Visible = True
        Label18.Visible = True
        Label19.Visible = True

        SetupValidation()
        UpdatePasswordProgress()
        ScaleLayout()
    End Sub

    ' ===================== COMBOBOX EVENTS =====================
    Private Sub excbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles excbx.SelectedIndexChanged
        cmbxLblVisible(excbx, Label16)
    End Sub

    Private Sub gdrcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gdrcmbx.SelectedIndexChanged
        cmbxLblVisible(gdrcmbx, Label17)
    End Sub

    Private Sub cvlcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cvlcbx.SelectedIndexChanged
        cmbxLblVisible(cvlcbx, Label18)
    End Sub

    Private Sub rolecbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rolecbx.SelectedIndexChanged
        cmbxLblVisible(rolecbx, Label19)
    End Sub

    Private Sub yrcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles yrcbx.SelectedIndexChanged
        cmbxLblVisible(yrcbx, Label15)
    End Sub

    Private Sub mnthcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles mnthcbx.SelectedIndexChanged
        Label8.Visible = (mnthcbx.SelectedIndex = -1)
    End Sub

    Private Sub daycmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles daycmbx.SelectedIndexChanged
        Label5.Visible = (daycmbx.SelectedIndex = -1)
    End Sub

    ' ===================== GO TO LOGIN =====================
    Private Sub gtloginbtn_Click(sender As Object, e As EventArgs) Handles gtloginbtn.Click
        Dim staffmanage As New StaffManagement()

        AdminFrm.Show()
        AdminFrm.Guna2Panel3.Controls.Clear()

        staffmanage.Dock = DockStyle.Fill
        AdminFrm.Guna2Panel3.Controls.Add(staffmanage)

        Me.Hide()
    End Sub

    ' ===================== RANDOM PASSWORD =====================
    Private Sub randpassbtn_Click(sender As Object, e As EventArgs) Handles randpassbtn.Click
        Dim rnd As New Random()
        Dim randomNumber As Integer = rnd.Next(2, 16)

        Dim capital As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim small As String = "abcdefghijklmnopqrstuvwxyz"
        Dim number As String = "0123456789"
        Dim special As String = "!@#$%&*_-^"

        Dim password As String = ""
        password &= capital(rnd.Next(capital.Length))
        password &= small(rnd.Next(small.Length))
        password &= number(rnd.Next(number.Length))
        password &= special(rnd.Next(special.Length))

        Dim allChars As String = capital & small & number & special
        For i As Integer = 1 To randomNumber
            password &= allChars(rnd.Next(allChars.Length))
        Next

        regpasstxbx.Text = password
        cnfrmpasstxbx.Text = password
    End Sub

    ' ===================== COPY PASSWORD =====================
    Private Sub cpybtn_Click(sender As Object, e As EventArgs) Handles cpybtn.Click
        If regpasstxbx.Text <> "" Then Clipboard.SetText(regpasstxbx.Text)
    End Sub

    ' ===================== PASSWORD PROGRESS =====================
    Private Sub UpdatePasswordProgress()
        Dim regpass As String = regpasstxbx.Text
        Dim score As Integer = 0

        If regpass.Length >= 6 Then score += 34
        If Regex.IsMatch(regpass, "[A-Z]") Then score += 33
        If Regex.IsMatch(regpass, "[0-9]") Then score += 33

        score = Math.Min(100, Math.Max(0, score))

        regprogress.Minimum = 0
        regprogress.Maximum = 100
        regprogress.Value = score
        regprogress.Refresh()

        passbar.Value = score

        If regpass.Length = 0 Then
            Label4.Text = "empty"
            Label4.ForeColor = Color.Silver
            passbar.ProgressColor = Color.LightGray
            passbar.ProgressColor2 = Color.LightGray
        ElseIf score < 67 Then
            Label4.Text = "weak"
            Label4.ForeColor = Color.Orange
            passbar.ProgressColor = Color.Orange
            passbar.ProgressColor2 = Color.DarkOrange
        ElseIf score < 100 Then
            Label4.Text = "strong"
            Label4.ForeColor = Color.LimeGreen
            passbar.ProgressColor = Color.LimeGreen
            passbar.ProgressColor2 = Color.Green
        Else
            Label4.Text = "very strong"
            Label4.ForeColor = Color.DarkGreen
            passbar.ProgressColor = Color.Green
            passbar.ProgressColor2 = Color.DarkGreen
        End If
    End Sub

    Private Sub regpasstxbx_TextChanged(sender As Object, e As EventArgs) Handles regpasstxbx.TextChanged
        UpdatePasswordProgress()
    End Sub

    ' ===================== CLEAR BUTTON =====================
    Private Sub clrbtn_Click(sender As Object, e As EventArgs) Handles clrbtn.Click
        clearData()
    End Sub

    ' ===================== FIRST NAME EVENT =====================
    Private Sub frstnmtxbx_TextChanged(sender As Object, e As EventArgs) Handles frstnmtxbx.TextChanged

    End Sub

    ' ===================== FILE DIALOG EVENT =====================
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk

    End Sub

    ' ===================== UPLOAD IMAGE =====================
    Private Sub uploadbtn_Click(sender As Object, e As EventArgs) Handles uploadbtn.Click

        With OpenFileDialog1
            .FileName = ""
            .Filter = "Image files|*.jpeg;*.jpg;*.png;*.bmp"
        End With

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Try
                Using fs As New FileStream(OpenFileDialog1.FileName, FileMode.Open, FileAccess.Read)
                    Using tmp As Image = Image.FromStream(fs)
                        If staffimgpcbx.Image IsNot Nothing Then staffimgpcbx.Image.Dispose()
                        staffimgpcbx.Image = New Bitmap(tmp)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Cannot load image: " & ex.Message)
            End Try
        End If

    End Sub

    ' ===================== PANEL PAINT =====================
    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

End Class