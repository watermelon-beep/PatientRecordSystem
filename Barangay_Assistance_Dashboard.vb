Public Class Barangay_Assistance_Dashboard
    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub

    Private Sub Barangay_Assistance_Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load



        With Guna2DataGridView2.Rows
            .Add("09:00am", "Karl", "General Checkup", "Confirmed")
            .Add("09:30am", "Romero", "Prenatal", "Confirmed")
            .Add("10:15am", "Kian", "Immunization", "Pending")
            .Add("11:00am", "Thea", "Consultation", "Confirmed")
            .Add("01:30pm", "Arjay", "Dental", "Confirmed")


        End With
    End Sub


End Class
