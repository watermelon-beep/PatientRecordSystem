Imports System.Text.RegularExpressions
Imports Guna.UI2.WinForms

Public Class UserLog

    Private passVisible As Boolean = False
    Sub incorrectLogInfo(text As String, gunatext As Guna2TextBox)
        Label2.Text = text
        gunatext.BorderColor = Color.Red
        Label2.ForeColor = Color.Red
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        If usrnlogtxbx.Text = "" And passlogtxbx.Text = "" Then
            Label2.Text = "username and password cannot be empty"
            usrnlogtxbx.BorderColor = Color.Red
            passlogtxbx.BorderColor = Color.Red
            Label2.ForeColor = Color.Red
        ElseIf usrnlogtxbx.Text = "" Then
            incorrectLogInfo("username cannot be empty", usrnlogtxbx)
        ElseIf Not Regex.IsMatch(usrnlogtxbx.Text, ".{6}") Then
            incorrectLogInfo("username must be at least 6 characters", usrnlogtxbx)
        ElseIf Not Regex.IsMatch(usrnlogtxbx.Text, "[A-Z]") Then
            incorrectLogInfo("username must contain at least one uppercase letter", usrnlogtxbx)
        ElseIf Not Regex.IsMatch(usrnlogtxbx.Text, "[0-9]") Then
            incorrectLogInfo("username must contain at least one number", usrnlogtxbx)
        ElseIf passlogtxbx.Text = "" Then
            incorrectLogInfo("password cannot be empty", passlogtxbx)
            passlogtxbx.Clear()
        ElseIf Not Regex.IsMatch(passlogtxbx.Text, ".{6}") Then
            incorrectLogInfo("password must be at least 6 characters", passlogtxbx)
            passlogtxbx.Clear()
        ElseIf Not Regex.IsMatch(passlogtxbx.Text, "[A-Z]") Then
            incorrectLogInfo("password must contain at least one uppercase letter", passlogtxbx)
            passlogtxbx.Clear()
        ElseIf Not Regex.IsMatch(passlogtxbx.Text, "[0-9]") Then
            incorrectLogInfo("password must contain at least one number", passlogtxbx)
            passlogtxbx.Clear()
        Else
            usrnlogtxbx.BorderColor = Color.FromArgb(0, 64, 0)
            passlogtxbx.BorderColor = Color.FromArgb(0, 64, 0)
            Label2.Text = "at least 6 characters/ contain number/ contain capital letter"
            Label2.ForeColor = Color.Gray
            MsgBox("Login successful!", MsgBoxStyle.Information, "Success")

            usrnlogtxbx.Clear()
            passlogtxbx.Clear()

            LogForm.Hide()
            Me.Hide()
            ContentForm.Show()
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

        Dim logForm As LogForm = Me.FindForm()

        logForm.ShowRegister()
    End Sub

    Private Sub usrnlogtxbx_TextChanged(sender As Object, e As EventArgs) Handles usrnlogtxbx.TextChanged

    End Sub
End Class
