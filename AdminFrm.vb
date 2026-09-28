Imports Guna.Charts.WinForms

Public Class AdminFrm

    Private adminDashboard As New AdminDashboard

    Private Sub AdminFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()

        Guna2Panel3.Controls.Clear()
        adminDashboard.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(adminDashboard)


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
    End Sub


End Class