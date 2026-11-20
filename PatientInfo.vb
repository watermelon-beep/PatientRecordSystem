Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Public Class PatientInfo

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

        Me.AutoScaleMode = AutoScaleMode.None
        Me.AutoSize = False
        Me.DoubleBuffered = True

        Guna2Panel1.MaximumSize = Size.Empty

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

        If isScaling Then Return
        If designSize.Width = 0 OrElse designSize.Height = 0 Then Return
        If Me.WindowState = FormWindowState.Minimized Then Return
        If Me.ClientSize.Width = 0 OrElse Me.ClientSize.Height = 0 Then Return

        isScaling = True

        Try

            Me.PerformLayout()

            Dim sf As Single = Math.Min(
                Me.ClientSize.Width / CSng(designSize.Width),
                Me.ClientSize.Height / CSng(designSize.Height)
            )

            sf = Math.Max(0.6F, Math.Min(3.0F, sf))

            For Each info As LayoutInfo In layoutList

                Dim c As Control = info.Ctl
                Dim p As Control = c.Parent

                If p Is Nothing Then Continue For

                If c.Dock = DockStyle.None AndAlso
                   info.ParentDesign.Width > 0 AndAlso
                   info.ParentDesign.Height > 0 Then

                    Dim sx As Single =
                        p.ClientSize.Width / CSng(info.ParentDesign.Width)

                    Dim sy As Single =
                        p.ClientSize.Height / CSng(info.ParentDesign.Height)

                    c.Bounds = New Rectangle(
                        CInt(info.Bounds.X * sx),
                        CInt(info.Bounds.Y * sy),
                        CInt(info.Bounds.Width * sx),
                        CInt(info.Bounds.Height * sy)
                    )

                End If

                If info.OrigFont IsNot Nothing Then

                    Dim newSize As Single =
                        Math.Max(6.0F, info.OrigFont.Size * sf)

                    If Math.Abs(c.Font.Size - newSize) > 0.1F Then

                        c.Font = New Font(
                            info.OrigFont.FontFamily,
                            newSize,
                            info.OrigFont.Style,
                            info.OrigFont.Unit
                        )

                    End If

                End If

                If info.ItemHeight > 0 Then

                    Try
                        DirectCast(c, ComboBox).ItemHeight =
                            Math.Max(10, CInt(info.ItemHeight * sf))
                    Catch
                    End Try

                End If

            Next

        Finally
            isScaling = False
        End Try

    End Sub

    Private Sub PatientInfo_SizeChanged(
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

        badChars(Guna2TextBox11) = nameBad
        badChars(Guna2TextBox10) = nameBad
        badChars(Guna2TextBox9) = nameBad

        badChars(Guna2TextBox7) = "[^\p{L}\d ()\-.,/]"
        badChars(Guna2TextBox6) = "[^\p{L}\d .,'#\-/]"
        badChars(Guna2TextBox8) = "\D"
        badChars(Guna2TextBox5) = "\D"
        badChars(Guna2TextBox4) = "[^A-Za-z0-9@._+\-]"
        badChars(Guna2TextBox3) = "[^A-Za-z0-9._\-]"

        Guna2TextBox8.MaxLength = 2
        Guna2TextBox5.MaxLength = 11

        For Each tb As Guna.UI2.WinForms.Guna2TextBox In badChars.Keys

            AddHandler tb.KeyPress, AddressOf RestrictKey
            AddHandler tb.TextChanged, AddressOf CleanText

        Next

        For Each cb As ComboBox In New ComboBox() {
            Guna2ComboBox6,
            Guna2ComboBox1,
            Guna2ComboBox2,
            rolecbx,
            Guna2ComboBox5,
            Guna2ComboBox4,
            Guna2ComboBox3
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

        Guna2TextBox11.Clear()
        Guna2TextBox10.Clear()
        Guna2TextBox9.Clear()

        Guna2ComboBox6.SelectedIndex = -1

        Guna2ComboBox5.SelectedIndex = -1
        Guna2ComboBox4.SelectedIndex = -1
        Guna2ComboBox3.SelectedIndex = -1

        Guna2TextBox8.Clear()

        Guna2ComboBox1.SelectedIndex = -1
        Guna2ComboBox2.SelectedIndex = -1

        Guna2TextBox7.Clear()
        Guna2TextBox6.Clear()

        rolecbx.SelectedIndex = -1

        Guna2TextBox5.Clear()
        Guna2TextBox4.Clear()

        Guna2TextBox3.Clear()
        Guna2TextBox2.Clear()
        Guna2TextBox1.Clear()

        If Guna2PictureBox7.Image IsNot Nothing Then

            Guna2PictureBox7.Image.Dispose()
            Guna2PictureBox7.Image = Nothing

        End If

        Label19.Visible = False

        For Each lbl As Label In New Label() {
            Label21, Label22, Label23, Label24,
            Label25, Label26, Label27
        }

            lbl.Visible = True

        Next

        UpdatePasswordProgress()

    End Sub

    ' ===================== COMBOBOX PLACEHOLDER LABELS =====================

    Private Sub cmbxLblVisible(cmbx As ComboBox, lbl As Label)

        lbl.Visible = (cmbx.SelectedIndex = -1)

    End Sub

    Private Sub Guna2ComboBox6_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox6.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox6, Label24)

    End Sub

    Private Sub Guna2ComboBox5_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox5.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox5, Label27)

    End Sub

    Private Sub Guna2ComboBox4_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox4.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox4, Label26)

    End Sub

    Private Sub Guna2ComboBox3_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox3.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox3, Label25)

    End Sub

    Private Sub Guna2ComboBox1_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox1.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox1, Label23)

    End Sub

    Private Sub Guna2ComboBox2_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2ComboBox2.SelectedIndexChanged

        cmbxLblVisible(Guna2ComboBox2, Label22)

    End Sub

    Private Sub rolecbx_SelectedIndexChanged(
        sender As Object,
        e As EventArgs
    ) Handles rolecbx.SelectedIndexChanged

        cmbxLblVisible(rolecbx, Label21)

    End Sub

    ' ===================== FORM LOAD =====================

    Private Sub PatientInfo_Load(
        sender As Object,
        e As EventArgs
    ) Handles MyBase.Load

        dbConnection()

        Label3.Text = "Barangay health clinic: Patient Registration"
        Label21.Text = "Job title"

        Guna2ComboBox1.Items.Clear()
        Guna2ComboBox1.Items.AddRange(
            New Object() {"Male", "Female", "Prefer not to say"}
        )

        Guna2ComboBox2.Items.Clear()
        Guna2ComboBox2.Items.AddRange(
            New Object() {
                "Single", "Married", "Widowed",
                "Divorced", "Separated"
            }
        )

        Guna2ComboBox6.Items.Clear()
        Guna2ComboBox6.Items.AddRange(
            New Object() {"None", "Jr.", "Sr.", "II", "III", "IV", "V"}
        )

        rolecbx.Items.Clear()
        rolecbx.Items.AddRange(
            New Object() {
                "Student", "Employed", "Self-employed",
                "Unemployed", "Retired", "Other"
            }
        )

        Guna2ComboBox5.Items.Clear()

        For i As Integer = 1 To 31
            Guna2ComboBox5.Items.Add(i.ToString("00"))
        Next

        Guna2ComboBox4.Items.Clear()
        Guna2ComboBox4.Items.AddRange(
            New Object() {
                "January", "February", "March", "April",
                "May", "June", "July", "August",
                "September", "October", "November", "December"
            }
        )

        Guna2ComboBox3.Items.Clear()

        For y As Integer = Date.Now.Year To 1900 Step -1
            Guna2ComboBox3.Items.Add(y.ToString())
        Next

        Guna2CircleProgressBar1.Minimum = 0
        Guna2CircleProgressBar1.Maximum = 100
        Guna2CircleProgressBar1.Value = 0

        Guna2ProgressBar1.Minimum = 0
        Guna2ProgressBar1.Maximum = 100
        Guna2ProgressBar1.Value = 0

        Label28.Text = "empty"
        Label28.ForeColor = Color.Silver
        Label19.Visible = False

        For Each lbl As Label In New Label() {
            Label21, Label22, Label23, Label24,
            Label25, Label26, Label27
        }

            lbl.Visible = True

        Next

        SetupValidation()
        UpdatePasswordProgress()
        ScaleLayout()

    End Sub

    ' ===================== PASSWORD PROGRESS =====================

    Private Sub UpdatePasswordProgress()

        Dim pass As String = Guna2TextBox2.Text
        Dim score As Integer = 0

        Dim hasLen As Boolean = pass.Length >= 6
        Dim hasCap As Boolean = Regex.IsMatch(pass, "[A-Z]")
        Dim hasNum As Boolean = Regex.IsMatch(pass, "[0-9]")

        If hasLen Then score += 34
        If hasCap Then score += 33
        If hasNum Then score += 33

        score = Math.Min(100, Math.Max(0, score))

        Label31.ForeColor =
            If(hasLen, Color.ForestGreen, Color.Gray)

        Label30.ForeColor =
            If(hasCap, Color.ForestGreen, Color.Gray)

        Label29.ForeColor =
            If(hasNum, Color.ForestGreen, Color.Gray)

        Guna2CircleProgressBar1.Value = score
        Guna2CircleProgressBar1.Refresh()

        Guna2ProgressBar1.Value = score

        If pass.Length = 0 Then

            Label28.Text = "empty"
            Label28.ForeColor = Color.Silver

            Guna2ProgressBar1.ProgressColor = Color.LightGray
            Guna2ProgressBar1.ProgressColor2 = Color.LightGray

        ElseIf score < 67 Then

            Label28.Text = "weak"
            Label28.ForeColor = Color.Orange

            Guna2ProgressBar1.ProgressColor = Color.Orange
            Guna2ProgressBar1.ProgressColor2 = Color.DarkOrange

        ElseIf score < 100 Then

            Label28.Text = "strong"
            Label28.ForeColor = Color.LimeGreen

            Guna2ProgressBar1.ProgressColor = Color.LimeGreen
            Guna2ProgressBar1.ProgressColor2 = Color.Green

        Else

            Label28.Text = "very strong"
            Label28.ForeColor = Color.DarkGreen

            Guna2ProgressBar1.ProgressColor = Color.Green
            Guna2ProgressBar1.ProgressColor2 = Color.DarkGreen

        End If

    End Sub

    Private Sub Guna2TextBox2_TextChanged(
        sender As Object,
        e As EventArgs
    ) Handles Guna2TextBox2.TextChanged

        UpdatePasswordProgress()

    End Sub

    ' ===================== RANDOM / COPY PASSWORD =====================

    Private Sub randpassbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles randpassbtn.Click

        Dim rnd As New Random()

        Dim capital As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim small As String = "abcdefghijklmnopqrstuvwxyz"
        Dim number As String = "0123456789"
        Dim special As String = "!@#$%&*_-^"

        Dim allChars As String =
            capital & small & number & special

        Dim password As String = ""

        password &= capital(rnd.Next(capital.Length))
        password &= small(rnd.Next(small.Length))
        password &= number(rnd.Next(number.Length))
        password &= special(rnd.Next(special.Length))

        For i As Integer = 1 To rnd.Next(4, 12)
            password &= allChars(rnd.Next(allChars.Length))
        Next

        Guna2TextBox2.Text = password
        Guna2TextBox1.Text = password

    End Sub

    Private Sub cpybtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles cpybtn.Click

        If Guna2TextBox2.Text <> "" Then
            Clipboard.SetText(Guna2TextBox2.Text)
        End If

    End Sub

    ' ===================== UPLOAD PHOTO =====================

    Private Sub Guna2Button3_Click(
        sender As Object,
        e As EventArgs
    ) Handles Guna2Button3.Click

        Using ofd As New OpenFileDialog()

            ofd.Title = "Select Patient Photo"
            ofd.Filter = "Image files|*.jpeg;*.jpg;*.png;*.bmp"

            If ofd.ShowDialog() = DialogResult.OK Then

                Try

                    Using fs As New FileStream(
                        ofd.FileName,
                        FileMode.Open,
                        FileAccess.Read
                    )

                        Using tmp As Image = Image.FromStream(fs)

                            If Guna2PictureBox7.Image IsNot Nothing Then
                                Guna2PictureBox7.Image.Dispose()
                            End If

                            Guna2PictureBox7.Image = New Bitmap(tmp)

                        End Using

                    End Using

                Catch ex As Exception

                    MessageBox.Show(
                        "Cannot load image: " & ex.Message
                    )

                End Try

            End If

        End Using

    End Sub

    Private Function GetPatientImage() As Byte()

        If Guna2PictureBox7.Image Is Nothing Then
            Return Nothing
        End If

        Using ms As New MemoryStream()

            Using bmp As New Bitmap(Guna2PictureBox7.Image)

                bmp.Save(
                    ms,
                    System.Drawing.Imaging.ImageFormat.Jpeg
                )

            End Using

            Return ms.ToArray()

        End Using

    End Function

    ' ===================== RANDOM UNIQUE PATIENT ID =====================

    Private Function GenerateRandomPatientId() As Integer

        Dim rnd As New Random()
        Dim candidateId As Integer = 0
        Dim exists As Boolean = True

        While exists
            candidateId = rnd.Next(10000, 100000)

            Dim checkSql As String =
                "SELECT COUNT(*) FROM dbo.patient_information WHERE patient_Id = @id"

            Using checkCmd As New SqlCommand(checkSql, conn)
                checkCmd.Parameters.Add("@id", SqlDbType.Int).Value = candidateId
                exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0
            End Using
        End While

        Return candidateId

    End Function

    ' ===================== ADD PATIENT =====================

    Private Sub addPat()

        If Guna2TextBox11.Text.Trim() = "" OrElse
           Guna2TextBox9.Text.Trim() = "" OrElse
           Guna2TextBox8.Text.Trim() = "" OrElse
           Guna2ComboBox5.SelectedIndex = -1 OrElse
           Guna2ComboBox4.SelectedIndex = -1 OrElse
           Guna2ComboBox3.SelectedIndex = -1 OrElse
           Guna2ComboBox1.SelectedIndex = -1 OrElse
           Guna2ComboBox2.SelectedIndex = -1 OrElse
           Guna2TextBox6.Text.Trim() = "" OrElse
           Guna2TextBox5.Text.Trim() = "" Then

            MessageBox.Show("Please complete all required fields.")
            Return

        End If

        If Guna2TextBox5.Text.Length <> 11 Then

            MessageBox.Show("Phone number must contain 11 digits.")
            Return

        End If

        If Guna2TextBox4.Text.Trim() <> "" AndAlso
           Not Regex.IsMatch(
               Guna2TextBox4.Text.Trim(),
               "^[^@\s]+@[^@\s]+\.[^@\s]+$"
           ) Then

            MessageBox.Show("Invalid email address.")
            Return

        End If

        Dim birthDate As DateTime

        Dim dateString As String =
            Guna2ComboBox5.Text & " " &
            Guna2ComboBox4.Text & " " &
            Guna2ComboBox3.Text

        If Not DateTime.TryParseExact(
            dateString,
            "dd MMMM yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            birthDate
        ) Then

            MessageBox.Show("Invalid date of birth.")
            Return

        End If

        If birthDate.Date > Date.Today Then

            MessageBox.Show(
                "Date of birth cannot be in the future."
            )

            Return

        End If

        Dim actualAge As Integer =
            Date.Today.Year - birthDate.Year

        If birthDate.Date > Date.Today.AddYears(-actualAge) Then
            actualAge -= 1
        End If

        If actualAge < 0 OrElse actualAge > 99 Then

            MessageBox.Show("Invalid patient age.")
            Return

        End If

        If Val(Guna2TextBox8.Text) <> actualAge Then

            MessageBox.Show(
                "Age does not match the birthday. Correct age: " &
                actualAge.ToString()
            )

            Return

        End If

        Try

            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If

            Dim randomPatientId As Integer = GenerateRandomPatientId()
            Dim photo As Byte() = GetPatientImage()

            Dim extVal As String =
                If(Guna2ComboBox6.SelectedIndex = -1 OrElse Guna2ComboBox6.Text = "None",
                   "",
                   Guna2ComboBox6.Text.Trim())

            Dim jobVal As String =
                If(rolecbx.SelectedIndex = -1, "", rolecbx.Text.Trim())

            Dim emergencyVal As String =
                If(Guna2TextBox7.Text.Trim() = "", "N/A", Guna2TextBox7.Text.Trim())

            Using setOnCmd As New SqlCommand("SET IDENTITY_INSERT dbo.patient_information ON", conn)
                setOnCmd.ExecuteNonQuery()
            End Using

            Try
                Dim sql As String =
                    "INSERT INTO dbo.patient_information (" &
                    "patient_Id, first_Name, middle_Name, surname, name_extension, age, birth_Date, gender, " &
                    "user_address, emergency_info, job_title, civil_status, phone_number, email_address, " &
                    "username, patient_photo, created_account) " &
                    "VALUES (" &
                    "@patient_Id, @first_Name, @middle_Name, @surname, @name_extension, @age, @birth_Date, @gender, " &
                    "@user_address, @emergency_info, @job_title, @civil_status, @phone_number, @email_address, " &
                    "@username, @patient_photo, @created_account)"

                Using comm As New SqlCommand(sql, conn)

                    With comm.Parameters

                        .Add("@patient_Id", SqlDbType.Int).Value = randomPatientId
                        .Add("@first_Name", SqlDbType.VarChar, 100).Value = Guna2TextBox11.Text.Trim()
                        .Add("@middle_Name", SqlDbType.VarChar, 100).Value = Guna2TextBox10.Text.Trim()
                        .Add("@surname", SqlDbType.VarChar, 100).Value = Guna2TextBox9.Text.Trim()
                        .Add("@name_extension", SqlDbType.VarChar, 20).Value = extVal
                        .Add("@age", SqlDbType.Int).Value = actualAge
                        .Add("@birth_Date", SqlDbType.Date).Value = birthDate.Date
                        .Add("@gender", SqlDbType.VarChar, 50).Value = Guna2ComboBox1.Text
                        .Add("@user_address", SqlDbType.VarChar, 255).Value = Guna2TextBox6.Text.Trim()
                        .Add("@emergency_info", SqlDbType.VarChar, 200).Value = emergencyVal
                        .Add("@job_title", SqlDbType.VarChar, 100).Value = jobVal
                        .Add("@civil_status", SqlDbType.VarChar, 50).Value = Guna2ComboBox2.Text
                        .Add("@phone_number", SqlDbType.VarChar, 20).Value = Guna2TextBox5.Text.Trim()
                        .Add("@email_address", SqlDbType.VarChar, 150).Value = Guna2TextBox4.Text.Trim()
                        .Add("@username", SqlDbType.VarChar, 100).Value = Guna2TextBox3.Text.Trim()
                        .Add("@created_account", SqlDbType.Date).Value = Date.Today

                        If photo Is Nothing Then
                            .Add("@patient_photo", SqlDbType.VarBinary, -1).Value = DBNull.Value
                        Else
                            .Add("@patient_photo", SqlDbType.VarBinary, -1).Value = photo
                        End If

                    End With

                    comm.ExecuteNonQuery()

                End Using
            Finally
                Using setOffCmd As New SqlCommand("SET IDENTITY_INSERT dbo.patient_information OFF", conn)
                    setOffCmd.ExecuteNonQuery()
                End Using
            End Try

            AdminFrm.LogNotification("registered a new patient")

            MessageBox.Show(
                "Patient successfully registered! ID: " & randomPatientId.ToString(),
                "Registration Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )

            clearData()

        Catch ex As Exception

            MessageBox.Show(
                "Registration failed: " & ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )

        End Try

    End Sub

    ' ===================== REGISTER / CLEAR BUTTONS =====================

    Private Sub Guna2Button2_Click(
        sender As Object,
        e As EventArgs
    ) Handles Guna2Button2.Click

        addPat()

    End Sub

    Private Sub Guna2Button1_Click(
        sender As Object,
        e As EventArgs
    ) Handles Guna2Button1.Click

        clearData()

    End Sub

    ' ===================== GO BACK =====================

    Private Sub gtloginbtn_Click(
        sender As Object,
        e As EventArgs
    ) Handles gtloginbtn.Click

        Dim rec As New PatientRecord()

        AdminFrm.Show()
        AdminFrm.Guna2Panel3.Controls.Clear()

        rec.Dock = DockStyle.Fill

        AdminFrm.Guna2Panel3.Controls.Add(rec)

        Me.Hide()

    End Sub

    Private Sub Guna2PictureBox7_Click(
        sender As Object,
        e As EventArgs
    ) Handles Guna2PictureBox7.Click

    End Sub

End Class