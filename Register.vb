Imports System.Data.SqlClient

Public Class Register
    Public Function UsernameExists(username As String) As Boolean

        Dim query As String = "SELECT COUNT(*) FROM staff_information WHERE username = @username"

        Using sqlcom As New SqlCommand(query, conn)

            sqlcom.Parameters.AddWithValue("@username", username)

            Dim count As Integer = Convert.ToInt32(sqlcom.ExecuteScalar())

            Return count > 0

        End Using

    End Function

    Public Function UserPasswordExist(password As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM staff_information WHERE user_password = @user_password"

        Using sqlcom As New SqlCommand(query, conn)

            sqlcom.Parameters.AddWithValue("@user_password", password)

            Dim count As Integer = Convert.ToInt32(sqlcom.ExecuteScalar())

            Return count > 0

        End Using
    End Function

    Sub addStaff()

        If UsernameExists(regUsrnmTxbx.Text) Then
            MessageBox.Show("Username already exists.")
        Else
            query = "INSERT INTO staff_Information (
                        first_Name,
                        middle_Name,
                        surname,
                        gender,
                        username,
                        user_password
                    )
                      VALUES (
                        @first_Name,
                        @middle_Name,
                        @surname,
                        @gender,
                        @username,
                        @user_password
                        )"
            comm = New SqlClient.SqlCommand(query, conn)

            With comm.Parameters
                .AddWithValue("@first_Name", frstNmTxbx.Text)
                .AddWithValue("@middle_Name", mdlnmTxbx.Text)
                .AddWithValue("@surname", srnnmTxbx.Text)
                .AddWithValue("@gender", gendercmbx.Text)
                .AddWithValue("@username", regUsrnmTxbx.Text)
                .AddWithValue("@user_password", regPassTxbx.Text)
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
        positionCmbx.Items.Add("Barangay assistance")
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
        addStaff()
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles frstNmTxbx.TextChanged
        If frstNmTxbx.Text <> "" Then
            Guna2CircleProgressBar1.Value = 50
        End If
    End Sub

End Class
