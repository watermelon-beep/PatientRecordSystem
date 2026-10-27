Imports System.Data.SqlClient

Public Class PatientRecord


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
End Class
