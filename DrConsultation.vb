Public Class DrConsultation
    Private Sub Guna2CirclePictureBox3_Click(sender As Object, e As EventArgs) Handles Guna2CirclePictureBox3.Click
        Using dataRead = comm.ExecuteReader()
            While dataRead.Read
                cons.Rows.Add(dataRead("Date & Time"),
                              dataRead("Patient Name"),
                              dataRead("Age"),
                              dataRead("Gender"),
                              dataRead("Complaints/Diagnos"),
                              dataRead("Action"))

            End While

        End Using
    End Sub
End Class
