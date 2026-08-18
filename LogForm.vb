Public Class LogForm

    Private defaultPlaceholder As Boolean = True
    Private Sub userPlaceHolder()

        usrnmLogtxtbx.Text = "Username"
        passLogTxbx.Text = "Password"
        usrnmLogtxtbx.ForeColor = Color.Gray
        passLogTxbx.ForeColor = Color.Gray
    End Sub

    Private Sub defaultPlchldr(textbox As TextBox)

        If defaultPlaceholder Then
            textbox.ResetText()
            textbox.ForeColor = Color.Black
        Else
            defaultPlaceholder = False
        End If
    End Sub



    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        Me.Hide()
        RegisterForm.Show()
    End Sub

    Private Sub logBtn_Click(sender As Object, e As EventArgs) Handles logBtn.Click
        Me.Hide()
        ContentForm.Show()
        ContentForm.contentPanel.Controls.Clear()
        ContentForm.dashboardPanel.Dock = DockStyle.Fill
        ContentForm.contentPanel.Controls.Add(ContentForm.dashboardPanel)
    End Sub

    Private Sub LogForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Application.Exit()
    End Sub


    Private Sub usrnmLogtxtbx_MouseClick(sender As Object, e As MouseEventArgs) Handles usrnmLogtxtbx.MouseClick
        defaultPlchldr(usrnmLogtxtbx)
        usrnmLogtxtbx.ForeColor = Color.Black
    End Sub

    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        RoundPanel(card, 50)
        RoundPanel(userLogPnl, 20)
        RoundPanel(passLogPnl, 20)

        userPlaceHolder()

    End Sub

    Private Sub usrnmLogtxtbx_Leave(sender As Object, e As EventArgs) Handles usrnmLogtxtbx.Leave
        If usrnmLogtxtbx.Text = "" Then
            defaultPlaceholder = True
            userPlaceHolder()
        Else
            defaultPlaceholder = False
        End If
    End Sub

    Private Sub passLogTxbx_MouseClick(sender As Object, e As MouseEventArgs) Handles passLogTxbx.MouseClick
        defaultPlchldr(passLogTxbx)
        passLogTxbx.PasswordChar = "•"
    End Sub

    Private Sub passLogTxbx_Leave(sender As Object, e As EventArgs) Handles passLogTxbx.Leave
        If passLogTxbx.Text = "" Then
            defaultPlaceholder = True
            passLogTxbx.PasswordChar = ""
            userPlaceHolder()
        Else
            defaultPlaceholder = False
        End If
    End Sub
End Class