Imports System.Data.Common
Imports System.Data.SqlClient

Public Class AdminDashboard

    Private Sub DashbooardTable()

        admintbl.Rows.Clear()

        query = "SELECT first_Name, middle_Name, surname, name_extension, age, gender, Position FROM staff_information"

        Using comm As New SqlCommand(query, conn)
            dataRead = comm.ExecuteReader
            While dataRead.Read()
                admintbl.Rows.Add(dataRead("first_Name"),
                                  dataRead("middle_Name"),
                                  dataRead("surname"),
                                  dataRead("name_extension"),
                                  dataRead("age"),
                                  dataRead("gender"),
                                  dataRead("Position")
                    )
            End While
        End Using
        dataRead.Close()

    End Sub

    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()
        DashbooardTable()
    End Sub


End Class
