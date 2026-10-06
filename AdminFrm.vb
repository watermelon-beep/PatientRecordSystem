Imports System.Data.SqlClient

Public Class AdminFrm

    Private adminDashboard As New AdminDashboard

    Private Sub ProfileAdmin()
        Dim userlog As New UserLog
        Dim query As String = "SELECT first_Name FROM staff_information WHERE username = @username"

        Using comm As New SqlCommand(query, conn)
            comm.Parameters.AddWithValue("@username", currentUsername)

            Dim result As Object = comm.ExecuteScalar()

            If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                Label1.Text = result.ToString()
                Label3.Text = "Welcome, " & result.ToString() & "!"
            Else
                Label1.Text = "Unknown user"
            End If
        End Using
    End Sub

    Private Sub AdminFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()

        Guna2Panel3.Controls.Clear()
        adminDashboard.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(adminDashboard)

        ProfileAdmin()

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
    End Sub

    Private Sub AdminFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Application.Exit()
    End Sub

End Class