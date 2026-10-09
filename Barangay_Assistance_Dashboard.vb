Public Class Barangay_Assistance_Dashboard
    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub

    Private Sub Barangay_Assistance_Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load



        With Guna2DataGridView1.Rows
            .Add("09:00am", "Karl", "General Checkup", "Confirmed")
            .Add("09:30am", "Romero", "Prenatal", "Confirmed")
            .Add("10:15am", "Kian", "Immunization", "Pending")
            .Add("11:00am", "Thea", "Consultation", "Confirmed")
            .Add("01:30pm", "Arjay", "Dental", "Confirmed")


        End With
    End Sub

    Private Sub Guna2DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellContentClick

    End Sub

    Private Sub Guna2PictureBox2_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox2.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub
End Class
