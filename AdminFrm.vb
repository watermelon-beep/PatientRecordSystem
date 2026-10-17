Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports Guna.UI2.WinForms

Public Class AdminFrm

    Private adminDashboard As New AdminDashboard
    Private Sub ProfileAdmin()

        query = "SELECT first_Name, Position FROM staff_information WHERE username = @username"

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
                    Label3.Text = "Welcome, Unknown position, Unknown user!"
                End If
            End Using
        End Using
    End Sub

    Private Sub AdminFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()

        ChangeUsrCnrl(adminDashboard, Guna2Panel3)

        ProfileAdmin()

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
    End Sub

    Private Sub AdminFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Application.Exit()
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        LogForm.Show()
        Me.Hide()

    End Sub

    Private Sub dashboardbtn_Click(sender As Object, e As EventArgs) Handles dashboardbtn.Click
        ChangeUsrCnrl(adminDashboard, Guna2Panel3)
        dashboardbtn.BackColor = Color.FromArgb(4, 196, 80)
    End Sub

    Private Sub patmangementbtn_Click(sender As Object, e As EventArgs) Handles patmangementbtn.Click
        patmangementbtn.BackColor = Color.FromArgb(4, 196, 80)
        Dim patManagement As New PatientManagement
        ChangeUsrCnrl(patManagement, Guna2Panel3)
    End Sub

    Private Sub staffmanagementbtn_Click(sender As Object, e As EventArgs) Handles staffmanagementbtn.Click
        staffmanagementbtn.BackColor = Color.FromArgb(4, 196, 80)
        Dim staffManagement As New StaffManagement
        ChangeUsrCnrl(staffManagement, Guna2Panel3)
    End Sub

    Private Sub appointmentbtn_Click(sender As Object, e As EventArgs) Handles appointmentbtn.Click
        appointmentbtn.BackColor = Color.FromArgb(4, 196, 80)
        Dim appointment As New Appointment
        ChangeUsrCnrl(appointment, Guna2Panel3)
    End Sub

    Private Sub dashboardbtn_LostFocus(sender As Object, e As EventArgs) Handles dashboardbtn.LostFocus
        dashboardbtn.BackColor = Color.Transparent
    End Sub

    Private Sub patmangementbtn_LostFocus(sender As Object, e As EventArgs) Handles patmangementbtn.LostFocus
        patmangementbtn.BackColor = Color.Transparent
    End Sub

    Private Sub staffmanagementbtn_LostFocus(sender As Object, e As EventArgs) Handles staffmanagementbtn.LostFocus
        staffmanagementbtn.BackColor = Color.Transparent
    End Sub

    Private Sub appointmentbtn_LostFocus(sender As Object, e As EventArgs) Handles appointmentbtn.LostFocus
        appointmentbtn.BackColor = Color.Transparent
    End Sub
End Class