Public Class LogForm

    Private usernamePlaceholder As Boolean = True
    Private passwordPlaceholder As Boolean = True

    Private Sub logBtn_Click(sender As Object, e As EventArgs) Handles logBtn.Click
        ContentForm.Show()
        Me.Hide()

        ContentForm.contentPanel.Controls.Clear()
        ContentForm.dashboardPanel.Dock = DockStyle.Fill
        ContentForm.contentPanel.Controls.Add(ContentForm.dashboardPanel)
        ContentForm.newUsrContrl = ContentForm.dashboardPanel

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
        Roundbtn(logBtn, 20)

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