Imports System.Drawing.Drawing2D

Public Class ContentForm

    Private dashboardPanel As New Dashboard()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        contentPanel.Controls.Clear()
        dashboardPanel.Dock = DockStyle.Fill
        contentPanel.Controls.Add(dashboardPanel)
    End Sub


    Public Sub Roundbtn(btn As Button, radius As Integer)

        Dim path As New GraphicsPath()

        path.StartFigure()
        path.AddArc(0, 0, radius, radius, 180, 90)
        path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90)
        path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90)
        path.AddArc(0, btn.Height - radius, radius, radius, 90, 90)
        path.CloseFigure()

        btn.Region = New Region(path)

    End Sub

    Public Sub RoundPanel(pnl As Panel, radius As Integer)

        Dim path As New GraphicsPath()

        path.StartFigure()
        path.AddArc(0, 0, radius, radius, 180, 90)
        path.AddArc(pnl.Width - radius, 0, radius, radius, 270, 90)
        path.AddArc(pnl.Width - radius, pnl.Height - radius, radius, radius, 0, 90)
        path.AddArc(0, pnl.Height - radius, radius, radius, 90, 90)
        path.CloseFigure()

        pnl.Region = New Region(path)

    End Sub

    Private Sub logoutBtn_Click(sender As Object, e As EventArgs) Handles logoutbtn.Click
        LogForm.Show()
        Me.Hide()
    End Sub
End Class
