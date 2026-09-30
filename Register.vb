Imports System.Data.SqlClient

Public Class Register

    Function ComputeAge(birthDate As Date) As Integer
        Dim today As Date = Date.Today
        Dim age As Integer = today.Year - birthDate.Year

        If birthDate.Date > today.AddYears(-age) Then
            age -= 1
        End If

        Return age
    End Function

    Function TryGetBirthDate(ByRef birthDate As Date) As Boolean

        If dayCmbx.SelectedIndex < 0 OrElse monthCmbx.SelectedIndex < 0 OrElse yearCmbx.SelectedIndex < 0 Then
            Return False
        End If

        Dim d, y As Integer

        If Not Integer.TryParse(dayCmbx.SelectedItem.ToString(), d) Then Return False
        If Not Integer.TryParse(yearCmbx.SelectedItem.ToString(), y) Then Return False


        Dim m As Integer = monthCmbx.SelectedIndex + 1

        If d > Date.DaysInMonth(y, m) Then Return False

        birthDate = New Date(y, m, d)

        Return True

    End Function

    Sub addStaff()

        If UsernameExists(regUsrnmTxbx.Text) Then

            MessageBox.Show("Username already exists.")
        ElseIf UserPasswordExist(regPassTxbx.text) Then
            MessageBox.Show("Password already exists.")
        Else
            query = "INSERT INTO staff_information (
                        first_Name,
                        middle_Name,
                        surname,
                        gender,
                        username,
                        user_password,
                        Position
                    )
                      VALUES (
                        @first_Name,
                        @middle_Name,
                        @surname,
                        @gender,
                        @username,
                        @user_password,
                        @Position
                        )"

            comm = New SqlClient.SqlCommand(query, conn)
            Dim birthDate As Date
            With comm.Parameters
                .AddWithValue("@first_Name", frstNmTxbx.Text)
                .AddWithValue("@middle_Name", mdlnmTxbx.Text)
                .AddWithValue("@surname", srnnmTxbx.Text)
                .AddWithValue("@gender", gendercmbx.Text)
                .AddWithValue("@username", regUsrnmTxbx.Text)
                .AddWithValue("@user_password", regPassTxbx.Text)
                .AddWithValue("@Position", positionCmbx.Text)
            End With

            comm.ExecuteNonQuery()
            comm.Dispose()

            MsgBox("added")

        End If

    End Sub

    Private Sub dayCmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dayCmbx.SelectedIndexChanged

        If Not dayCmbx.SelectedIndex = -1 Then
            Label1.Visible = False
        End If

    End Sub

    Private Sub monthCmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles monthCmbx.SelectedIndexChanged

        If Not monthCmbx.SelectedIndex = -1 Then
            Label2.Visible = False
        End If

    End Sub

    Private Sub yearCmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles yearCmbx.SelectedIndexChanged

        If Not yearCmbx.SelectedIndex = -1 Then
            Label3.Visible = False
        End If

    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click

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

        regPassTxbx.Text = password
        cnfrmPasstxbx.Text = password

    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Clipboard.SetText(regPassTxbx.Text)
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click

        Dim logForm As LogForm = Me.FindForm()

        logForm.ShowLogin()

    End Sub

    Private Sub Register_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        For i As Integer = 1 To 31
            dayCmbx.Items.Add(i.ToString("00"))
        Next

        For i As Integer = 1 To 12
            monthCmbx.Items.Add(i.ToString("00"))
        Next

        For i As Integer = 1900 To DateTime.Now.Year
            yearCmbx.Items.Add(i.ToString())
        Next

        gendercmbx.Items.Add("Male")
        gendercmbx.Items.Add("Female")
        gendercmbx.Items.Add("Prefer not to say")

        positionCmbx.Items.Add("Admin")
        positionCmbx.Items.Add("Barangay Assistance")
        positionCmbx.Items.Add("Doctor")

        dbConnection()
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        LogForm.ShowLogin()
    End Sub

    Private Sub gendercmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gendercmbx.SelectedIndexChanged

        If Not gendercmbx.SelectedIndex = -1 Then
            Label4.Visible = False
        End If

    End Sub

    Private Sub positionCmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles positionCmbx.SelectedIndexChanged

        If Not positionCmbx.SelectedIndex = -1 Then
            Label5.Visible = False
        End If

    End Sub

    Private Sub addDataBtn_Click(sender As Object, e As EventArgs) Handles addDataBtn.Click
        Dim birthDate As Date

        If TryGetBirthDate(birthDate) Then
            If birthDate > Date.Today Then
                MsgBox("Birthdate cannot be in the future.")
                Exit Sub
            End If
        End If
        addStaff()
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles frstNmTxbx.TextChanged

        If frstNmTxbx.Text <> "" Then
            Guna2CircleProgressBar1.Value = 50
        End If

    End Sub
End Class
