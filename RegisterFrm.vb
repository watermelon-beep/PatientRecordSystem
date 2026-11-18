
Imports System.IO
Imports System.Text.RegularExpressions

Public Class RegisterFrm

    ' ===================== FULLSCREEN / AUTO-SCALE =====================

    Private Class LayoutInfo
        Public Ctl As Control
        Public Bounds As Rectangle
        Public OrigFont As Font
        Public ItemHeight As Integer
    End Class

    Private layoutList As New List(Of LayoutInfo)
    Private designSize As Size

    Public Sub New()
        InitializeComponent()
        designSize = Me.ClientSize
        CaptureLayout(Me)
    End Sub

    Private Sub CaptureLayout(parent As Control)

        For Each c As Control In parent.Controls

            Dim info As New LayoutInfo With {
                .Ctl = c,
                .Bounds = c.Bounds,
                .OrigFont = c.Font
            }

            If TypeOf c Is ComboBox Then
                info.ItemHeight = DirectCast(c, ComboBox).ItemHeight
            End If

            If c.Dock = DockStyle.None Then
                c.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            End If

            layoutList.Add(info)
            CaptureLayout(c)

        Next

    End Sub

    Private Sub ScaleLayout()

        If designSize.Width = 0 OrElse designSize.Height = 0 Then Return
        If Me.WindowState = FormWindowState.Minimized Then Return

        Dim sx As Single = Me.ClientSize.Width / designSize.Width
        Dim sy As Single = Me.ClientSize.Height / designSize.Height
        Dim sf As Single = Math.Min(sx, sy)

        Me.SuspendLayout()

        For Each info As LayoutInfo In layoutList

            Dim c As Control = info.Ctl

            If c.Dock = DockStyle.None Then

                c.Bounds = New Rectangle(
                    CInt(info.Bounds.X * sx),
                    CInt(info.Bounds.Y * sy),
                    CInt(info.Bounds.Width * sx),
                    CInt(info.Bounds.Height * sy)
                )

            End If

            If info.OrigFont IsNot Nothing Then

                c.Font = New Font(
                    info.OrigFont.FontFamily,
                    Math.Max(6.0F, info.OrigFont.Size * sf),
                    info.OrigFont.Style,
                    info.OrigFont.Unit
                )

            End If

            If info.ItemHeight > 0 Then

                Try
                    DirectCast(c, ComboBox).ItemHeight =
                        Math.Max(10, CInt(info.ItemHeight * sy))
                Catch
                End Try

            End If

        Next

        Me.ResumeLayout(True)

    End Sub

    Private Sub RegisterFrm_SizeChanged(
        sender As Object,
        e As EventArgs
    ) Handles Me.SizeChanged

        ScaleLayout()

    End Sub


    ' ===================== INPUT VALIDATION =====================

    Private ReadOnly badChars As New Dictionary(
        Of Guna.UI2.WinForms.Guna2TextBox, String
    )

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

        ' AGE: TWO DIGITS ONLY
        agetxbx.MaxLength = 2

        ' PHONE NUMBER: 11 DIGITS ONLY
        phnumtxbx.MaxLength = 11

        For Each tb As Guna.UI2.WinForms.Guna2TextBox In badChars.Keys

            AddHandler tb.KeyPress, AddressOf RestrictKey
            AddHandler tb.TextChanged, AddressOf CleanText

        Next

        ' COMBOBOX SETTINGS

        For Each cb As ComboBox In New ComboBox() {
            excbx, gdrcmbx, cvlcbx, rolecbx,
            daycmbx, mnthcbx, yrcbx
        }

            cb.DropDownStyle = ComboBoxStyle.DropDownList
            AddHandler cb.KeyPress, AddressOf BlockTyping

        Next

    End Sub

    Private Sub RestrictKey(
        sender As Object,
        e As KeyPressEventArgs
    )

        If Char.IsControl(e.KeyChar) Then Return

        Dim tb As Guna.UI2.WinForms.Guna2TextBox =
            DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)

        If Regex.IsMatch(e.KeyChar.ToString(), badChars(tb)) Then
            e.Handled = True
        End If

    End Sub

    Private Sub CleanText(sender As Object, e As EventArgs)

        Dim tb As Guna.UI2.WinForms.Guna2TextBox =
            DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)

        Dim cleaned As String =
            Regex.Replace(tb.Text, badChars(tb), "")

        If cleaned <> tb.Text Then

            Dim pos As Integer =
                tb.SelectionStart - (tb.Text.Length - cleaned.Length)

            tb.Text = cleaned
            tb.SelectionStart = Math.Max(0, pos)

        End If

    End Sub

    Private Sub BlockTyping(
        sender As Object,
        e As KeyPressEventArgs
    )

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


    ' ===================== ADD STAFF =====================

    Sub addStaff()

        Dim ms As New MemoryStream

        If UsernameExists(regusrnmtxbx.Text) Then

            MessageBox.Show("Username already exists.")

        Else

            query = "INSERT INTO staff_information (
                        first_Name,
                        middle_Name,
                        surname,
                        name_extension,
                        age,
                        birth_place,
                        user_address,
                        gender,
                        civil_status,
                        username,
                        user_password,
                        Position,
                        phone_number,
                        email_address,
                        user_photo,
                        created_account
                    )
                    VALUES (
                        @first_Name,
                        @middle_Name,
                        @surname,
                        @name_extension,
                        @age,
                        @birth_place,
                        @user_address,
                        @gender,
                        @civil_status,
                        @username,
                        @user_password,
                        @Position,
                        @phone_number,
                        @email_address,
                        @user_photo,
                        @created_account
                    )"

            Using comm As New SqlClient.SqlCommand(query, conn)

                With comm.Parameters

                    .AddWithValue("@first_Name", frstnmtxbx.Text)
                    .AddWithValue("@middle_Name", mdlnmtxbx.Text)
                    .AddWithValue("@surname", srnmtxbx.Text)
                    .AddWithValue("@name_extension", excbx.Text)
                    .AddWithValue("@age", agetxbx.Text)
                    .AddWithValue("@birth_place", birthtxbx.Text)
                    .AddWithValue("@user_address", addrtxbx.Text)
                    .AddWithValue("@gender", gdrcmbx.Text)
                    .AddWithValue("@civil_status", cvlcbx.Text)
                    .AddWithValue("@username", regusrnmtxbx.Text)
                    .AddWithValue("@user_password", regpasstxbx.Text)
                    .AddWithValue("@Position", rolecbx.Text)
                    .AddWithValue("@phone_number", phnumtxbx.Text)
                    .AddWithValue("@email_address", emailtxbx.Text)
                    .AddWithValue("@user_photo", ms.ToArray())
                    .AddWithValue("@created_account",
                                  Date.Now.ToString("yyyy-MM-dd"))

                End With

                comm.ExecuteNonQuery()

                MsgBox("Added successfully!")

            End Using

        End If

    End Sub


    ' ===================== REGISTER BUTTON =====================

    Private Sub regbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles regbtn.Click

        If Not Regex.IsMatch(
            regpasstxbx.Text,
            "^(?=.*[A-Z])(?=.*[0-9]).{6,}$"
        ) Then

            Label20.Visible = True

            MessageBox.Show(
                "Password must contain at least 6 characters, " &
                "one capital letter, and one number."
            )

        ElseIf regpasstxbx.Text <> cnfrmpasstxbx.Text Then

            MessageBox.Show("Passwords do not match.")

        Else

            addStaff()

            clearData()

        End If

    End Sub


    ' ===================== FORM LOAD =====================

    Private Sub RegisterFrm_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        dbConnection()

        ' GENDER

        gdrcmbx.Items.Clear()

        gdrcmbx.Items.Add("Male")
        gdrcmbx.Items.Add("Female")
        gdrcmbx.Items.Add("Prefer not to say")


        ' CIVIL STATUS

        cvlcbx.Items.Clear()

        cvlcbx.Items.Add("Single")
        cvlcbx.Items.Add("Married")
        cvlcbx.Items.Add("Widowed")
        cvlcbx.Items.Add("Divorced")
        cvlcbx.Items.Add("Separated")
        cvlcbx.Items.Add("Prefer not to say")


        ' EXTENSION

        excbx.Items.Clear()

        excbx.Items.Add("None")
        excbx.Items.Add("Jr.")
        excbx.Items.Add("Sr.")
        excbx.Items.Add("II")
        excbx.Items.Add("III")
        excbx.Items.Add("IV")
        excbx.Items.Add("V")


        ' JOB TITLE

        rolecbx.Items.Clear()

        rolecbx.Items.Add("Admin")
        rolecbx.Items.Add("Barangay Assistance")
        rolecbx.Items.Add("Doctor")
        rolecbx.Items.Add("Nurse")
        rolecbx.Items.Add("Midwife")
        rolecbx.Items.Add("Barangay Health Worker")
        rolecbx.Items.Add("Secretary")


        ' DAY

        daycmbx.Items.Clear()

        For i As Integer = 1 To 31
            daycmbx.Items.Add(i.ToString("00"))
        Next


        ' MONTH

        mnthcbx.Items.Clear()

        Dim months As String() = {
            "January", "February", "March",
            "April", "May", "June",
            "July", "August", "September",
            "October", "November", "December"
        }

        For Each month As String In months
            mnthcbx.Items.Add(month)
        Next


        ' YEAR

        yrcbx.Items.Clear()

        For year As Integer = Date.Now.Year To 1900 Step -1
            yrcbx.Items.Add(year.ToString())
        Next


        ' CIRCULAR PROGRESS SETTINGS

        regprogress.Minimum = 0
        regprogress.Maximum = 100
        regprogress.Value = 0

        ' HORIZONTAL PROGRESS SETTINGS

        passbar.Minimum = 0
        passbar.Maximum = 100
        passbar.Value = 0

        Label4.Text = "empty"
        Label4.ForeColor = Color.Silver

        Label20.Visible = False

        ' INITIAL PLACEHOLDERS

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

    Private Sub excbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles excbx.SelectedIndexChanged

        cmbxLblVisible(excbx, Label16)

    End Sub

    Private Sub gdrcmbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles gdrcmbx.SelectedIndexChanged

        cmbxLblVisible(gdrcmbx, Label17)

    End Sub

    Private Sub cvlcbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles cvlcbx.SelectedIndexChanged

        cmbxLblVisible(cvlcbx, Label18)

    End Sub

    Private Sub rolecbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles rolecbx.SelectedIndexChanged

        cmbxLblVisible(rolecbx, Label19)

    End Sub

    Private Sub yrcbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles yrcbx.SelectedIndexChanged

        cmbxLblVisible(yrcbx, Label15)

    End Sub

    Private Sub mnthcbx_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles mnthcbx.SelectedIndexChanged

        Label8.Visible = (mnthcbx.SelectedIndex = -1)

    End Sub

    Private Sub daycmbx_SelectedIndexChanged(
    sender As Object,
    e As EventArgs
) Handles daycmbx.SelectedIndexChanged

        Label5.Visible = (daycmbx.SelectedIndex = -1)

    End Sub


    ' ===================== GO TO LOGIN =====================

    Private Sub gtloginbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles gtloginbtn.Click

        Dim staffmanage As New StaffManagement()

        AdminFrm.Show()

        AdminFrm.Guna2Panel3.Controls.Clear()

        staffmanage.Dock = DockStyle.Fill

        AdminFrm.Guna2Panel3.Controls.Add(staffmanage)

        Me.Hide()

    End Sub


    ' ===================== RANDOM PASSWORD =====================

    Private Sub randpassbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles randpassbtn.Click

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

        Dim allChars As String =
            capital & small & number & special

        For i As Integer = 1 To randomNumber

            password &= allChars(rnd.Next(allChars.Length))

        Next

        regpasstxbx.Text = password
        cnfrmpasstxbx.Text = password

    End Sub


    ' ===================== COPY PASSWORD =====================

    Private Sub cpybtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles cpybtn.Click

        If regpasstxbx.Text <> "" Then
            Clipboard.SetText(regpasstxbx.Text)
        End If

    End Sub


    ' ===================== PASSWORD PROGRESS FUNCTION =====================

    Private Sub UpdatePasswordProgress()

        Dim regpass As String = regpasstxbx.Text

        Dim score As Integer = 0

        ' REQUIREMENT 1: AT LEAST 6 CHARACTERS

        If regpass.Length >= 6 Then
            score += 34
        End If

        ' REQUIREMENT 2: CONTAINS CAPITAL LETTER

        If Regex.IsMatch(regpass, "[A-Z]") Then
            score += 33
        End If

        ' REQUIREMENT 3: CONTAINS NUMBER

        If Regex.IsMatch(regpass, "[0-9]") Then
            score += 33
        End If

        score = Math.Min(100, Math.Max(0, score))

        ' UPDATE CIRCULAR PROGRESS

        regprogress.Minimum = 0
        regprogress.Maximum = 100
        regprogress.Value = score

        regprogress.Refresh()

        ' UPDATE HORIZONTAL PROGRESS

        passbar.Value = score

        ' PASSWORD STRENGTH LABEL

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


    ' ===================== PASSWORD TEXT CHANGED =====================

    Private Sub regpasstxbx_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles regpasstxbx.TextChanged

        UpdatePasswordProgress()

    End Sub


    ' ===================== CLEAR BUTTON =====================

    Private Sub clrbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles clrbtn.Click

        clearData()

    End Sub


    ' ===================== FIRST NAME EVENT =====================

    Private Sub frstnmtxbx_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles frstnmtxbx.TextChanged

    End Sub


    ' ===================== FILE DIALOG EVENT =====================

    Private Sub OpenFileDialog1_FileOk(
        sender As Object,
        e As System.ComponentModel.CancelEventArgs
    ) Handles OpenFileDialog1.FileOk

    End Sub


    ' ===================== UPLOAD IMAGE =====================

    Private Sub uploadbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles uploadbtn.Click

        With OpenFileDialog1

            .FileName = ""
            .Filter = "Image files|*.jpeg;*.jpg;*.png;*.bmp"

        End With

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then

            staffimgpcbx.Image =
                Image.FromFile(OpenFileDialog1.FileName)

        End If

    End Sub


    ' ===================== PANEL PAINT =====================

    Private Sub Guna2Panel2_Paint(
        sender As Object,
        e As PaintEventArgs
    ) Handles Guna2Panel2.Paint

    End Sub

End Class
