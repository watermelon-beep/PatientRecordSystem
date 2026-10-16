Imports System.Data.SqlClient

Public Class DoctorFrm
    Private Sub doctorProfile()
        query = "SELECT first_Name, Position FROM staff_information WHERE username = @username"
        Using comm As New SqlClient.SqlCommand(query, conn)

            comm.Parameters.AddWithValue("@username", currentUsername)

            Using dataRead As SqlClient.SqlDataReader = comm.ExecuteReader
                If dataRead.Read Then
                    Dim firstname As String = dataRead("first_Name").ToString()
                    Dim position As String = dataRead("Position").ToString()

                    Label3.Text = "Welcome, " & position & ", " & firstname & "!"
                    Label1.Text = firstname
                    Label2.Text = position
                Else
                    Label3.Text = "Welcome, Unknown user!"
                    Label1.Text = "Unknown user"
                    Label2.Text = "Unknown position"
                End If
            End Using
        End Using
    End Sub

    Private Sub DoctorFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()
        Dim doctordbs As New doctordbs
        Guna2Panel3.Controls.Clear()
        doctordbs.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(doctordbs)
        doctorProfile()
        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        LogForm.Show()
        Me.Hide()
    End Sub
End Class