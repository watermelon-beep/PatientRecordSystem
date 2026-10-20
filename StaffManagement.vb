Imports System.Data.SqlClient

Public Class StaffManagement

    Private Sub ViewPosition(role As String)
        Try
            query = "SELECT * FROM staff_information WHERE Position = @Position"

            stafftbl.Rows.Clear()

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.AddWithValue("Position", role)

                Using dataRead = comm.ExecuteReader()
                    While dataRead.Read
                        stafftbl.Rows.Add(dataRead("staff_code"),
                                   dataRead("first_Name"),
                                   dataRead("middle_Name"),
                                   dataRead("surname"),
                                   dataRead("name_extension"),
                                   Convert.ToDateTime(dataRead("birth_Date")).ToString("dd/MM/yyyy"),
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
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub SearchUser()
        Try
            query = "SELECT * FROM staff_information WHERE Position LIKE @search"

            stafftbl.Rows.Clear()

            Using comm As New SqlCommand(query, conn)
                With comm.Parameters
                    .AddWithValue("@search", "%" & searchtxbx.Text.Trim() & "%")
                End With

                Using dataRead = comm.ExecuteReader()
                    While dataRead.Read
                        stafftbl.Rows.Add(dataRead("staff_code"),
                                    dataRead("first_Name"),
                                    dataRead("middle_Name"),
                                    dataRead("surname"),
                                    dataRead("name_extension"),
                                    Convert.ToDateTime(dataRead("birth_Date")).ToString("dd/MM/yyyy"),
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

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ViewUserProfile(id As String)
        query = "SELECT * FROM staff_information WHERE staff_code = @staff_code"
        Using comm As New SqlCommand(query, conn)
            comm.Parameters.AddWithValue("@staff_code", id)
            Using dataRead = comm.ExecuteReader()
                If dataRead.Read Then
                    Label10.Text = dataRead("Position").ToString()
                    Label2.Text = dataRead("first_Name").ToString() & " " & dataRead("middle_Name").ToString() & " " & dataRead("surname").ToString()
                    Label3.Text = dataRead("age")
                    Label12.Text = Convert.ToDateTime(dataRead("created_account")).ToString("MMMM dd, yyyy")
                    Label6.Text = dataRead("gender").ToString()
                    Label7.Text = dataRead("civil_status").ToString()
                    Label5.Text = dataRead("user_address").ToString()
                    Label8.Text = dataRead("phone_number").ToString()
                    Label4.Text = dataRead("staff_code").ToString()
                    Label9.Text = Convert.ToDateTime(dataRead("birth_Date")).ToString("MMMM dd, yyyy")
                    Label11.Text = dataRead("email_address").ToString()

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
                    stafftbl.Rows.Add(dataRead("staff_code"),
                                      dataRead("first_Name"),
                                      dataRead("middle_Name"),
                                      dataRead("surname"),
                                      dataRead("name_extension"),
                                      Convert.ToDateTime(dataRead("birth_Date")).ToString("dd/MM/yyyy"),
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
        Dim value As String = stafftbl.Rows(0).Cells(0).Value.ToString()
        ViewUserProfile(value)
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
        Dim rowId As String = Convert.ToString(selectedRow.Cells("Column1").Value)


        Select Case columnName
            Case "viewbtn"
                ViewUserProfile(rowId)
        End Select
    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        SearchUser()
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged
        If searchtxbx.Text = "" Then
            StaffData()
        End If
    End Sub

    Private Sub selectionposcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectionposcmbx.SelectedIndexChanged
        Dim selectedPos As String = selectionposcmbx.SelectedItem

        Select Case selectedPos
            Case "Admin"
                ViewPosition("Admin")
            Case "Doctor"
                ViewPosition("Doctor")
            Case "Barangay assistance"
                ViewPosition("Barangay Assistance")
            Case Else
                StaffData()


        End Select
    End Sub
End Class
