Imports System.Data.SqlClient
Imports System.IO

Public Class PatientInfo
    Sub addPat()

        Dim ms As New MemoryStream

        If UsernameExists(regusrnmtxbx.Text) Then

            MessageBox.Show("Username already exists.")
        Else
            query = "INSERT INTO patient_info (
                        FirstName, 
                        MiddleName,
                        Surname,
                        age,
                        gender,
                        Address,
                        phonenumber,
                        PatientImage
                    )
                      VALUES (
                        @FirstName, 
                        @MiddleName,
                        @Surname,
                        @age,
                        @gender,
                        @Address,
                        @phonenumber,
                        @PatientImage
                        )"

            Using comm As New SqlClient.SqlCommand(query, conn)

                With comm.Parameters

                    .AddWithValue("@FirstName", frstnmtxbx.Text)
                    .AddWithValue("@MiddleName", mdlnmtxbx.Text)
                    .AddWithValue("@Surname", srnmtxbx.Text)
                    .AddWithValue("@age", agetxbx.Text)
                    .AddWithValue("@name_extension", excbx.Text)
                    .AddWithValue("@gender", gdrcmbx.Text)
                    .AddWithValue("@Address", addrtxbx.Text)
                    .AddWithValue("@phonenumber", Convert.ToInt32(phnumtxbx.Text))
                    .AddWithValue("@PatientImage", ms.ToArray)

                End With

                comm.ExecuteNonQuery()

                MsgBox("added")
            End Using
        End If

    End Sub

    Private Sub PatientInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()
    End Sub

    Private Sub regbtn_Click(sender As Object, e As EventArgs) Handles regbtn.Click
        addPat()
    End Sub
End Class
