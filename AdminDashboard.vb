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

        With Guna2DataGridView2.Rows
            .Add("Karl Benedict Palomo added a new patient")
            .Add("Maria Santos registered a new consultation")
            .Add("John Cruz updated a patient record")
            .Add("Ana Reyes added a new appointment")
            .Add("Mark Dela Cruz completed a consultation")
            .Add("Sofia Garcia cancelled an appointment")
            .Add("James Flores updated patient information")
            .Add("Angela Ramos added a new medical record")
            .Add("Daniel Mendoza registered a new patient")
        End With
    End Sub


End Class
