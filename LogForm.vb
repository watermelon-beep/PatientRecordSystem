Public Class LogForm
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

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
End Class