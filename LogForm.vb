Public Class LogForm

    Private Sub userPlaceHolder()
        usrnmLogtxtbx.Text = "Username"
        usrnmLogtxtbx.ForeColor = Color.Gray
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
        usrnmLogtxtbx.Clear()
    End Sub

    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ContentForm.RoundPanel(card, 50)
        userPlaceHolder()
    End Sub

    Private Sub usrnmLogtxtbx_Leave(sender As Object, e As EventArgs) Handles usrnmLogtxtbx.Leave
        userPlaceHolder()

    End Sub

    Private Sub usrnmLogtxtbx_TextChanged(sender As Object, e As EventArgs) Handles usrnmLogtxtbx.TextChanged

    End Sub
End Class