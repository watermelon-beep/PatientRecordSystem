Imports System.Data.SqlClient

Public Class PatientRecord

    Private Sub SearchPatient()
        Try
            query = "SELECT * FROM patient_info WHERE FirstName LIKE @search"

            PatientTbl.Rows.Clear()

            Using comm As New SqlCommand(query, conn)
                With comm.Parameters
                    .AddWithValue("@search", "%" & searchtxbx.Text.Trim() & "%")
                End With

                Using dataRead = comm.ExecuteReader()
                    While dataRead.Read
                        PatientTbl.Rows.Add(dataRead("PatientID"),
                                      dataRead("FirstName"),
                                      dataRead("MiddleName"),
                                      dataRead("Surname"),
                                      dataRead("Age"),
                                      Convert.ToDateTime(dataRead("birthdate")).ToString("dd/MM/yyyy"),
                                      dataRead("Gender"),
                                      dataRead("Address"),
                                      dataRead("EmergencyContactInformation"),
                                      dataRead("PhoneNumber"),
                                      dataRead("civilstatus"),
                                      dataRead("Phonenumber"),
                                      dataRead("PatientImage"),
                                               "Edit",
                                               "Delete",
                                               "View")
                    End While

                End Using

            End Using

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub PatientRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PatientData
    End Sub
    Private Sub PatientData()

        PatientTbl.Rows.Clear()

        query = "SELECT * FROM Patient_info"

        Using comm As New SqlCommand(query, conn)
            Using dataRead = comm.ExecuteReader()

                While dataRead.Read
                    PatientTbl.Rows.Add(dataRead("PatientID"),
                                      dataRead("FirstName"),
                                      dataRead("MiddleName"),
                                      dataRead("Surname"),
                                      dataRead("Age"),
                                      Convert.ToDateTime(dataRead("birthdate")).ToString("dd/MM/yyyy"),
                                      dataRead("Gender"),
                                      dataRead("Address"),
                                      dataRead("EmergencyContactInformation"),
                                      dataRead("PhoneNumber"),
                                      dataRead("civilstatus"),
                                      dataRead("Phonenumber"),
                                      dataRead("PatientImage"),
                                               "Edit",
                                               "Delete",
                                               "View")
                End While

            End Using
        End Using
    End Sub

    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        PatientInfo.Show()
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged

    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        SearchPatient()

    End Sub
End Class
