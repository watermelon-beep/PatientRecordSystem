Imports System.Data.SqlClient
Imports System.Security.AccessControl

Module dbConn
    Public comm As New SqlCommand
    Public conn As New SqlConnection
    Public dataRead As SqlDataReader
    Public query As String

    Public Sub dbConnection()
        Try
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
            conn.ConnectionString = "Server= .\SQLEXPRESS; Database = two_plus_two_s5c2; Trusted_Connection = True; MultipleActiveResultSets = True"
            conn.Open()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Module
