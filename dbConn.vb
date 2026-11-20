
Imports System.Data.SqlClient

Module dbConn

    Public comm As New SqlCommand
    Public conn As New SqlConnection
    Public dataRead As SqlDataReader
    Public query As String

    ' ==========================================
    ' DATABASE CONNECTION
    ' ==========================================
    Public Sub dbConnection()

        Try
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            conn.ConnectionString =
                "Server=.\SQLEXPRESS;" &
                "Initial Catalog=two_plus_two_s5c2;" &
                "Integrated Security=True;" &
                "MultipleActiveResultSets=True"

            conn.Open()

        Catch ex As Exception
            MessageBox.Show(
                "Database Connection Failed: " & ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            )
        End Try

    End Sub

    ' ==========================================
    ' CHECK EXISTING USERNAME
    ' ==========================================
    Public Function UsernameExists(username As String) As Boolean

        Try
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If

            Dim sql As String =
                "SELECT COUNT(*) FROM dbo.staff_information " &
                "WHERE [username] = @username"

            Using sqlcom As New SqlCommand(sql, conn)

                sqlcom.Parameters.Add(
                    "@username", SqlDbType.VarChar, 255
                ).Value = username

                Dim count As Integer =
                    Convert.ToInt32(sqlcom.ExecuteScalar())

                Return count > 0

            End Using

        Catch ex As Exception

            Throw New Exception(
                "Username checking failed: " & ex.Message
            )

        End Try

    End Function

    ' ==========================================
    ' CHECK EXISTING PASSWORD
    ' ==========================================
    Public Function UserPasswordExist(password As String) As Boolean

        Try
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If

            Dim sql As String =
                "SELECT COUNT(*) FROM dbo.staff_information " &
                "WHERE [user_password] = @user_password"

            Using sqlcom As New SqlCommand(sql, conn)

                sqlcom.Parameters.Add(
                    "@user_password", SqlDbType.VarChar, 255
                ).Value = password

                Dim count As Integer =
                    Convert.ToInt32(sqlcom.ExecuteScalar())

                Return count > 0

            End Using

        Catch ex As Exception

            Throw New Exception(
                "Password checking failed: " & ex.Message
            )

        End Try

    End Function

End Module
