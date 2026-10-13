Imports System.Data.SqlClient

Class BarangayAssistanceFrm

    Sub BarAssProfile()
        query = "SELECT first_Name, Position FROM staff_information WHERE username = @username"
        Using comm As New SqlClient.SqlCommand(query, conn)
            comm.Parameters.AddWithValue("@username", currentUsername)

            Using dataRead As SqlClient.SqlDataReader = comm.ExecuteReader()
                If dataRead.Read Then
                    Dim firstName As String = dataRead("first_Name").ToString()
                    Dim position As String = dataRead("Position").ToString()

                    Label3.Text = "Welcome, " & position & ", " & firstName & "!"
                    Label1.Text = firstName
                    Label2.Text = position
                Else
                    Label3.Text = "Welcome, Unknown user!"
                    Label1.Text = "Unknown user"
                    Label2.Text = "Unknown position"
                End If
            End Using
        End Using
    End Sub

    Private Sub BarangayAssistanceFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()

        Dim bardb As New Barangay_Assistance_Dashboard
        Guna2Panel3.Controls.Clear()
        bardb.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(bardb)

        BarAssProfile()

        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm:tt")
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        LogForm.Show()
        Me.Hide()
    End Sub
End Class