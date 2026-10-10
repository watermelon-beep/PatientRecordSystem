Imports System.Data.SqlClient
Imports System.Web.UI.WebControls

Public Class AdminFrm

    Private adminDashboard As New AdminDashboard

    Private Sub ProfileAdmin()
        Dim userlog As New UserLog
        Dim query As String = "SELECT first_Name, Position FROM staff_information WHERE username = @username"

        Using comm As New SqlCommand(query, conn)
            comm.Parameters.AddWithValue("@username", currentUsername)
            Using dataRead As SqlDataReader = comm.ExecuteReader()
                If dataRead.Read() Then
                    Dim firstName As String = dataRead("first_Name").ToString()
                    Dim position As String = dataRead("Position").ToString()
                    Label1.Text = firstName
                    Label3.Text = "Welcome, " & position & ", " & firstName & "!"
                    Label2.Text = position
                Else
                    Label1.Text = "Unknown user"
                    Label2.Text = "Unknown position"
                End If
            End Using
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

    Private Sub AdminFrm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim lastState As FormWindowState = FormWindowState.Maximized
        If WindowState <> lastState Then
            lastState = Me.WindowState
            If WindowState = FormWindowState.Maximized Then
                adminDashboard.Label21.Font = New Font(adminDashboard.Label21.Font.FontFamily, 16)
            ElseIf WindowState = FormWindowState.Normal Then
                adminDashboard.Label21.Font = New Font(adminDashboard.Label21.Font.FontFamily, 10)
            End If
        End If

    End Sub
End Class