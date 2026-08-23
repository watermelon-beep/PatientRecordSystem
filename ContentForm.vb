Imports System.Drawing.Drawing2D

Public Class ContentForm
    Public newUsrContrl As New UserControl
    Public dashboardPanel As New Dashboard()
    Public logout As New Logout()
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles dashboardBtn.Click

        contentPanel.Controls.Clear()
        dashboardPanel.Dock = DockStyle.Fill
        contentPanel.Controls.Add(dashboardPanel)
        newUsrContrl = dashboardPanel

    End Sub

    Private Sub logoutBtn_Click(sender As Object, e As EventArgs) Handles logoutbtn.Click

        contentPanel.Controls.Add(logout)

        logout.BringToFront()

        logout.Location = New Point(
            (contentPanel.Width - logout.Width) \ 2,
            (contentPanel.Height - logout.Height) \ 2
        )

        logout.Visible = True

        AddHandler logout.logoutConfirmed,
            Sub()
                Me.Hide()
                LogForm.Show()
            End Sub

        AddHandler logout.logoutCanceled,
            Sub()
                logout.Hide()
                newUsrContrl.Show()
            End Sub

    End Sub

    Private Sub ContentForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        Application.Exit()

    End Sub
End Class
