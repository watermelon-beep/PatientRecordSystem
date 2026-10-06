Imports System.Data.SqlClient

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

    Public Function UsernameExists(username As String) As Boolean

        Dim query As String = "SELECT COUNT(*) FROM staff_information WHERE username = @username"

        Using sqlcom As New SqlCommand(query, conn)

            sqlcom.Parameters.AddWithValue("@username", username)

            Dim count As Integer = Convert.ToInt32(sqlcom.ExecuteScalar())

            Return count > 0

        End Using

    End Function

    Public Function UserPasswordExist(password As String) As Boolean

        Dim query As String = "SELECT COUNT(*) FROM staff_information WHERE user_password = @user_password"

        Using sqlcom As New SqlCommand(query, conn)

            sqlcom.Parameters.AddWithValue("@user_password", password)

            Dim count As Integer = Convert.ToInt32(sqlcom.ExecuteScalar())

            Return count > 0

        End Using
    End Function

End Module
