Imports System.Drawing.Drawing2D

Public Class ContentForm

    Public dashboardPanel As New Dashboard()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles dashboardBtn.Click

        contentPanel.Controls.Clear()
        dashboardPanel.Dock = DockStyle.Fill
        contentPanel.Controls.Add(dashboardPanel)
    End Sub

    Private Sub logoutBtn_Click(sender As Object, e As EventArgs) Handles logoutbtn.Click
        Me.Hide()
        LogForm.Show()
    End Sub

    Private Sub ContentForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.Exit()
    End Sub
End Class
