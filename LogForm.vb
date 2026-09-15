Imports System.Text.RegularExpressions

Public Class LogForm

    Private usernamePlaceholder As Boolean = True
    Private passwordPlaceholder As Boolean = True

    Private Sub logBtn_Click(sender As Object, e As EventArgs) Handles logBtn.Click
        If Regex.IsMatch(usrnmLogtxtbx.Text, "[A-Z]") AndAlso
           Regex.IsMatch(usrnmLogtxtbx.Text, "[0-9]") AndAlso
           Regex.IsMatch(usrnmLogtxtbx.Text, ".{8}") AndAlso
           Regex.IsMatch(passLogTxbx.Text, "[A-Z]") AndAlso
           Regex.IsMatch(passLogTxbx.Text, "[0-9]") AndAlso
           Regex.IsMatch(passLogTxbx.Text, ".{8}") Then
            ContentForm.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid username or password. Please ensure that both the username and password contain at least one uppercase letter, one number, and are at least 8 characters long.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub LogForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Application.Exit()

    End Sub

    Private Sub usrnmLogtxtbx_MouseClick(sender As Object, e As MouseEventArgs) Handles usrnmLogtxtbx.MouseClick

        If usernamePlaceholder Then

            usrnmLogtxtbx.Clear()
            usrnmLogtxtbx.ForeColor = Color.Black
            usernamePlaceholder = False

        End If

    End Sub


    Private Sub usrnmLogtxtbx_Leave(sender As Object, e As EventArgs) Handles usrnmLogtxtbx.Leave

        If String.IsNullOrWhiteSpace(usrnmLogtxtbx.Text) Then

            usrnmLogtxtbx.Text = "Username"
            usrnmLogtxtbx.ForeColor = Color.Gray
            usernamePlaceholder = True

        End If

    End Sub


    Private Sub passLogTxbx_MouseClick(sender As Object, e As MouseEventArgs) Handles passLogTxbx.MouseClick

        If passwordPlaceholder Then

            passLogTxbx.Clear()
            passLogTxbx.ForeColor = Color.Black
            passLogTxbx.PasswordChar = "•"
            passwordPlaceholder = False

        End If

        If showPassCb.Checked Then
            passLogTxbx.PasswordChar = ""
        End If

    End Sub


    Private Sub passLogTxbx_Leave(sender As Object, e As EventArgs) Handles passLogTxbx.Leave

        If String.IsNullOrWhiteSpace(passLogTxbx.Text) Then

            passLogTxbx.Text = "Password"
            passLogTxbx.ForeColor = Color.Gray
            passLogTxbx.PasswordChar = ""
            passwordPlaceholder = True

        End If

    End Sub


    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles Me.Load

        RoundPanel(card, 50)
        RoundPanel(userLogPnl, 40)
        RoundPanel(passLogPnl, 40)
        Roundbtn(logBtn, 50)

        usrnmLogtxtbx.Text = "Username"
        passLogTxbx.Text = "Password"

        usrnmLogtxtbx.ForeColor = Color.Gray
        passLogTxbx.ForeColor = Color.Gray

        passLogTxbx.PasswordChar = ""

    End Sub

    Private Sub showPassCb_CheckedChanged(sender As Object, e As EventArgs) Handles showPassCb.CheckedChanged

        If Not showPassCb.Checked And Not passwordPlaceholder Then
            passLogTxbx.PasswordChar = "•"
        Else
            passLogTxbx.PasswordChar = ""
        End If

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Me.Hide()
        RegisterForm.Show()
    End Sub
End Class