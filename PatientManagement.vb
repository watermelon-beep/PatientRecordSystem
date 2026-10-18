Public Class PatientManagement
    Private Sub PatientManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        pattable.Rows.Add("PAT-0001", "Kian", "Udani", "Abante", "None", "04/24/2004", "22", "Antipolo city", "Single", "Male", "09949068240", "", "", "Edit", "Delete", "View")
    End Sub

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        If pattable.ScrollBars = ScrollBars.Both Then
            pattable.ScrollBars = ScrollBars.None
        Else
            pattable.ScrollBars = ScrollBars.Both
        End If
    End Sub


End Class
