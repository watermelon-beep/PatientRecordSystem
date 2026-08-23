Public Class ContentForm
    Private currentUsrContrl As New UserControl
    Public dashboardPanel As New Dashboard()
    Public logout As New Logout()

    Private Sub ContentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        contentPanel.Controls.Add(logout)
        logout.Visible = False

        AddHandler logout.logoutConfirmed,
            Sub()
                Me.Hide()
                LogForm.Show()
            End Sub

        AddHandler logout.logoutCanceled,
            Sub()
                logout.Hide()
                currentUsrContrl.Show()
            End Sub
        showControl(dashboardPanel)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles dashboardBtn.Click

        showControl(dashboardPanel)
        currentUsrContrl = dashboardPanel

    End Sub

    Private Sub logoutBtn_Click(sender As Object, e As EventArgs) Handles logoutbtn.Click

        logout.Location = New Point(
            (contentPanel.Width - logout.Width) \ 2,
            (contentPanel.Height - logout.Height) \ 2
        )

        logout.BringToFront()
        logout.Visible = True

    End Sub

    Private Sub ContentForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        Application.Exit()

    End Sub

    Public Sub showControl(cntrl As UserControl)
        contentPanel.Controls.Clear()
        cntrl.Dock = DockStyle.Fill
        contentPanel.Controls.Add(cntrl)
        currentUsrContrl = cntrl

        contentPanel.Controls.Add(logout)
        logout.BringToFront()
    End Sub

End Class
