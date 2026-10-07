Public Class DoctorFrm
    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub DoctorFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim doctordbs As New doctordbs
        Guna2Panel3.Controls.Clear()
        doctordbs.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(doctordbs)



    End Sub
End Class