Public Class RegisterStaff

    Private Sub gtloginbtn_Click(sender As Object, e As EventArgs) Handles gtloginbtn.Click
        Dim staffmanage As New StaffManagement()
        AdminFrm.Guna2Panel3.Controls.Clear()
        staffmanage.Dock = DockStyle.Fill
        AdminFrm.Guna2Panel3.Controls.Add(staffmanage)
    End Sub

    Private Sub Guna2Panel4_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel4.Paint

    End Sub

    Private Sub srnmtxbx_TextChanged(sender As Object, e As EventArgs) Handles srnmtxbx.TextChanged

    End Sub

    Private Sub Label16_Click(sender As Object, e As EventArgs) Handles Label16.Click

    End Sub

    Private Sub excbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles excbx.SelectedIndexChanged

    End Sub
End Class
