Public Class BarangayAssistanceFrm
    Private Sub Guna2Panel3_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel3.Paint

    End Sub

    Private Sub BarangayAssistanceFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim bardb As New Barangay_Assistance_Dashboard
        Guna2Panel3.Controls.Clear()
        bardb.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(bardb)
    End Sub
End Class