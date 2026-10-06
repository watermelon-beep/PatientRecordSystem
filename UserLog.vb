Imports System.Text.RegularExpressions
Imports Guna.UI2.WinForms
Public Class UserLog

    Private passVisible As Boolean = False

    Private Function ValidateField(fieldName As String, value As String) As String
        If value = "" Then Return fieldName & " cannot be empty"
        If value.Length < 6 Then Return fieldName & " must be at least 6 characters"
        If Not Regex.IsMatch(value, "[A-Z]") Then Return fieldName & " must contain at least one uppercase letter"
        If Not Regex.IsMatch(value, "[0-9]") Then Return fieldName & " must contain at least one number"
        Return ""
    End Function

    Sub incorrectLogInfo(text As String, gunatext As Guna2TextBox)
        Label2.Text = text
        gunatext.BorderColor = Color.Red
        Label2.ForeColor = Color.Red
    End Sub

    Sub usersPosition()

        query = "SELECT Position FROM staff_information WHERE username = @username"

        comm = New SqlClient.SqlCommand(query, conn)

        comm.Parameters.AddWithValue("@username", usrnlogtxbx.Text)


        Dim Position As Object = comm.ExecuteScalar()

        Select Case Position.ToString()
            Case "Admin"

                usrnlogtxbx.Clear()
                passlogtxbx.Clear()

                AdminFrm.Show()

                LogForm.Hide()
                Me.Hide()

            Case "Barangay Assistance"

                usrnlogtxbx.Clear()
                passlogtxbx.Clear()

                BarangayAssistant.Show()

                LogForm.Hide()
                Me.Hide()
            Case "Doctor"

                usrnlogtxbx.Clear()
                passlogtxbx.Clear()

                DoctorDB.Show()

                LogForm.Hide()
                Me.Hide()
            Case Else
                MsgBox("Invalid user position: " & Position.ToString())
        End Select

        comm.Dispose()

    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click

        Dim username As String = usrnlogtxbx.Text.Trim()
        Dim password As String = passlogtxbx.Text.Trim()

        If username = "" AndAlso password = "" Then
            Label2.Text = "username and password cannot be empty"
            Label2.ForeColor = Color.Red
            usrnlogtxbx.BorderColor = Color.Red
            passlogtxbx.BorderColor = Color.Red
            Return
        End If

        Dim userError As String = ValidateField("username", username)
        If userError <> "" Then
            incorrectLogInfo(userError, usrnlogtxbx)
            Return
        End If

        Dim passError As String = ValidateField("password", password)
        If passError <> "" Then
            incorrectLogInfo(passError, passlogtxbx)
            passlogtxbx.Clear()
            Return
        End If

        usrnlogtxbx.BorderColor = Color.FromArgb(0, 64, 0)
        passlogtxbx.BorderColor = Color.FromArgb(0, 64, 0)
        Label2.Text = "at least 6 characters, contain number, contain capital letter"
        Label2.ForeColor = Color.Gray

        If UsernameExists(username) AndAlso UserPasswordExist(password) Then
            currentUsername = username
            usersPosition()
        Else
            MsgBox("Incorrect username or password.", MsgBoxStyle.Exclamation, "Login failed")
        End If

    End Sub

    Private Sub passlogtxbx_IconRightClick(sender As Object, e As EventArgs) Handles passlogtxbx.IconRightClick

        If passlogtxbx.Text = "" Then Exit Sub

        If passVisible Then
            passlogtxbx.IconRight = My.Resources.eye_closed
            passlogtxbx.PasswordChar = "•"
            passVisible = False
        Else
            passlogtxbx.IconRight = My.Resources.eye
            passlogtxbx.PasswordChar = ""
            passVisible = True
        End If
    End Sub

    Private Sub passlogtxbx_TextChanged(sender As Object, e As EventArgs) Handles passlogtxbx.TextChanged

        If passlogtxbx.Text = "" Then
            passlogtxbx.IconRight = My.Resources.lock_keyhole
        ElseIf passVisible Then
            passlogtxbx.IconRight = My.Resources.eye
        Else
            passlogtxbx.IconRight = My.Resources.eye_closed
        End If

    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click

        LogForm.Hide()
        RegisterFrm.Show()
    End Sub

    Private Sub UserLog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        dbConnection()

    End Sub


End Class
