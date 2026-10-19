Imports System.Data.SqlClient

Public Class StaffManagement

    Private Sub ViewUserProfile(username As String)
        query = "SELECT * FROM staff_information WHERE username = @username"
        Using comm As New SqlCommand(query, conn)
            comm.Parameters.AddWithValue("@username", username)
            Using dataRead = comm.ExecuteReader()
                If dataRead.Read Then
                    Label10.Text = dataRead("Position").ToString()
                    Label2.Text = dataRead("first_Name").ToString() & " " & dataRead("middle_Name").ToString() & " " & dataRead("surname").ToString()
                    Label3.Text = dataRead("age")
                    Label12.Text = dataRead("created_account").ToString()
                    Label6.Text = dataRead("gender").ToString()
                    Label7.Text = dataRead("civil_status").ToString()
                    Label5.Text = dataRead("user_address").ToString()
                    Label8.Text = dataRead("phone_number").ToString()

                End If
            End Using
        End Using
    End Sub

    Private Sub StaffData()

        stafftbl.Rows.Clear()

        query = "SELECT * FROM staff_information"

        Using comm As New SqlCommand(query, conn)
            Using dataRead = comm.ExecuteReader()

                While dataRead.Read
                    stafftbl.Rows.Add(dataRead("staff_Id"),
                                      dataRead("first_Name"),
                                      dataRead("middle_Name"),
                                      dataRead("surname"),
                                      dataRead("name_extension"),
                                      dataRead("birth_Date"),
                                      dataRead("age"),
                                      dataRead("birth_place"),
                                      dataRead("user_address"),
                                      dataRead("gender"),
                                      dataRead("civil_status"),
                                      dataRead("Position"),
                                      dataRead("phone_number"),
                                      dataRead("email_address"),
                                      dataRead("username"),
                                      dataRead("user_password"),
                                               "Edit",
                                               "Delete",
                                               "View")
                End While

            End Using
        End Using
    End Sub


    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        RegisterFrm.Show()
    End Sub

    Private Sub StaffManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()
        StaffData()
    End Sub

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        If stafftbl.ScrollBars = ScrollBars.Horizontal Then
            stafftbl.ScrollBars = ScrollBars.Both
        Else
            stafftbl.ScrollBars = ScrollBars.Horizontal
        End If
    End Sub

    Private Sub stafftbl_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles stafftbl.CellContentClick
        If e.RowIndex < 0 Then Return

        Dim columnName As String = stafftbl.Columns(e.ColumnIndex).Name
        Dim selectedRow As DataGridViewRow = stafftbl.Rows(e.RowIndex)
        Dim rowUsrname As String = Convert.ToString(selectedRow.Cells("Column10").Value)


        Select Case columnName
            Case "viewbtn"
                ViewUserProfile(rowUsrname)
        End Select
    End Sub
End Class
