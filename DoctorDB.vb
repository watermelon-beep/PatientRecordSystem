Public Class DoctorDB
    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub

    Private Sub DoctorDB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim doctordbs As New doctordbs

        Guna2Panel3.Controls.Clear()
        doctordbs.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(doctordbs)
    End Sub
End Class