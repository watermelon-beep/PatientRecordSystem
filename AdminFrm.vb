Imports System.Data.SqlClient
Imports Guna.UI2.WinForms

Public Class AdminFrm

    Private adminDashboard As New AdminDashboard
    Private WithEvents notifTimer As New Timer()

    Private Sub EnsureConnectionOpen()
        If conn Is Nothing OrElse String.IsNullOrWhiteSpace(conn.ConnectionString) Then
            dbConnection()
        End If
        If conn.State <> ConnectionState.Open Then
            conn.Open()
        End If
    End Sub

    Private Function HasStaffPhotoColumn() As String
        Try
            EnsureConnectionOpen()
            Dim sql As String =
                "SELECT TOP 1 name FROM sys.columns " &
                "WHERE object_id = OBJECT_ID('dbo.staff_information') " &
                "AND name IN ('staff_photo', 'profile_photo', 'photo', 'avatar', 'user_photo')"
            Using cmd As New SqlCommand(sql, conn)
                Dim res = cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    Return res.ToString()
                End If
            End Using
        Catch ex As Exception
        End Try
        Return ""
    End Function

    Private Function FindHeaderProfilePictureBox(parent As Control) As PictureBox
        If parent Is Nothing Then Return Nothing
        For Each c As Control In parent.Controls
            If c IsNot adminDashboard AndAlso c IsNot Guna2Panel3 Then
                If TypeOf c Is PictureBox Then
                    Dim pb As PictureBox = DirectCast(c, PictureBox)
                    If pb.Left > (Me.ClientSize.Width \ 2) Then
                        Return pb
                    End If
                End If
                If c.HasChildren Then
                    Dim found As PictureBox = FindHeaderProfilePictureBox(c)
                    If found IsNot Nothing Then Return found
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Sub LoadStaffHeaderPhoto(reader As SqlDataReader, photoColName As String)
        If String.IsNullOrWhiteSpace(photoColName) Then Return
        Dim pb As PictureBox = FindHeaderProfilePictureBox(Me)
        If pb Is Nothing Then Return

        Try
            If Not IsDBNull(reader(photoColName)) Then
                Dim rawBytes() As Byte = TryCast(reader(photoColName), Byte())
                If rawBytes IsNot Nothing AndAlso rawBytes.Length > 0 Then
                    Using ms As New IO.MemoryStream(rawBytes)
                        Using tmp As Image = Image.FromStream(ms)
                            If pb.Image IsNot Nothing Then pb.Image.Dispose()
                            pb.Image = New Bitmap(tmp)
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ProfileAdmin()
        Try
            EnsureConnectionOpen()

            Dim photoCol As String = HasStaffPhotoColumn()
            Dim photoSelect As String = If(String.IsNullOrWhiteSpace(photoCol), "", ", " & photoCol)

            Dim hasUser As Boolean = (Not String.IsNullOrWhiteSpace(currentUsername))
            Dim sql As String = If(
                hasUser,
                "SELECT TOP 1 first_Name, Position" & photoSelect & " FROM dbo.staff_information WHERE LTRIM(RTRIM(username)) = @username",
                "SELECT TOP 1 first_Name, Position" & photoSelect & " FROM dbo.staff_information ORDER BY CASE WHEN Position = 'Admin' THEN 0 ELSE 1 END, staff_Id"
            )

            Using comm As New SqlCommand(sql, conn)
                If hasUser Then
                    comm.Parameters.Add("@username", SqlDbType.VarChar, 100).Value = currentUsername.Trim()
                End If

                Using dataRead As SqlDataReader = comm.ExecuteReader()
                    If dataRead.Read() Then
                        Dim firstName As String = If(IsDBNull(dataRead("first_Name")), "Admin", dataRead("first_Name").ToString().Trim())
                        Dim position As String = If(IsDBNull(dataRead("Position")), "Staff", dataRead("Position").ToString().Trim())

                        Label1.Text = firstName
                        Label2.Text = position
                        Label3.Text = "Welcome, " & position & ", " & firstName & "!"

                        LoadStaffHeaderPhoto(dataRead, photoCol)
                    Else
                        Label1.Text = "Unknown user"
                        Label2.Text = "Unknown position"
                        Label3.Text = "Welcome, Unknown position, Unknown user!"
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    ' ===================== ACCURATE DASHBOARD CHARTS =====================

    Public Sub LoadStaffChart(staffChart As System.Windows.Forms.DataVisualization.Charting.Chart)
        If staffChart Is Nothing Then Return

        Try
            EnsureConnectionOpen()

            Dim roles() As String = {"Admin", "Barangay Assistance", "Doctor"}
            Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each r As String In roles
                counts(r) = 0
            Next

            Dim sql As String =
                "SELECT Position, COUNT(*) AS total " &
                "FROM dbo.staff_information " &
                "WHERE Position IS NOT NULL " &
                "GROUP BY Position"

            Using cmd As New SqlCommand(sql, conn)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim pos As String = reader("Position").ToString().Trim()
                        If counts.ContainsKey(pos) Then
                            counts(pos) = Convert.ToInt32(reader("total"))
                        End If
                    End While
                End Using
            End Using

            If staffChart.Series.Count > 0 Then
                staffChart.Series(0).Points.Clear()
                For Each r As String In roles
                    staffChart.Series(0).Points.AddXY(r, counts(r))
                Next
                staffChart.Invalidate()
            End If
        Catch ex As Exception
            MsgBox("Staff chart error: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadGenderChart(genderChart As System.Windows.Forms.DataVisualization.Charting.Chart)
        If genderChart Is Nothing Then Return

        Try
            EnsureConnectionOpen()

            Dim maleCount As Integer = 0
            Dim femaleCount As Integer = 0

            Dim sql As String =
                "SELECT gender, COUNT(*) AS total " &
                "FROM dbo.patient_information " &
                "WHERE gender IN ('Male', 'Female') " &
                "GROUP BY gender"

            Using cmd As New SqlCommand(sql, conn)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim g As String = reader("gender").ToString().Trim()
                        Dim cnt As Integer = Convert.ToInt32(reader("total"))
                        If g.Equals("Male", StringComparison.OrdinalIgnoreCase) Then
                            maleCount = cnt
                        ElseIf g.Equals("Female", StringComparison.OrdinalIgnoreCase) Then
                            femaleCount = cnt
                        End If
                    End While
                End Using
            End Using

            If genderChart.Series.Count > 0 Then
                genderChart.Series(0).Points.Clear()
                genderChart.Series(0).Points.AddXY("Male", maleCount)
                genderChart.Series(0).Points.AddXY("Female", femaleCount)
                genderChart.Invalidate()
            End If
        Catch ex As Exception
            MsgBox("Gender chart error: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadDailyVisitChart(visitChart As System.Windows.Forms.DataVisualization.Charting.Chart)
        If visitChart Is Nothing Then Return

        Try
            EnsureConnectionOpen()

            Dim daysOrder() As String = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
            Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each d As String In daysOrder
                counts(d) = 0
            Next

            Dim sql As String =
                "SELECT DATENAME(WEEKDAY, created_account) AS day_name, COUNT(*) AS total " &
                "FROM dbo.patient_information " &
                "WHERE created_account IS NOT NULL " &
                "GROUP BY DATENAME(WEEKDAY, created_account)"

            Using cmd As New SqlCommand(sql, conn)
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim dayName As String = reader("day_name").ToString().Trim()
                        If counts.ContainsKey(dayName) Then
                            counts(dayName) = Convert.ToInt32(reader("total"))
                        End If
                    End While
                End Using
            End Using

            If visitChart.Series.Count > 0 Then
                visitChart.Series(0).Points.Clear()
                For Each d As String In daysOrder
                    visitChart.Series(0).Points.AddXY(d, counts(d))
                Next
                visitChart.Invalidate()
            End If
        Catch ex As Exception
            MsgBox("Daily visit chart error: " & ex.Message)
        End Try
    End Sub

    Public Sub RefreshDashboardCharts(
        Optional visitChart As System.Windows.Forms.DataVisualization.Charting.Chart = Nothing,
        Optional staffChart As System.Windows.Forms.DataVisualization.Charting.Chart = Nothing,
        Optional genderChart As System.Windows.Forms.DataVisualization.Charting.Chart = Nothing
    )
        If visitChart IsNot Nothing Then LoadDailyVisitChart(visitChart)
        If staffChart IsNot Nothing Then LoadStaffChart(staffChart)
        If genderChart IsNot Nothing Then LoadGenderChart(genderChart)
    End Sub

    ' ===================== REAL-TIME NOTIFICATIONS =====================

    Public Sub LogNotification(actionText As String, Optional actorOverride As String = "")
        Try
            EnsureConnectionOpen()

            Dim actorName As String = actorOverride.Trim()
            If String.IsNullOrWhiteSpace(actorName) Then
                If Not String.IsNullOrWhiteSpace(Label1.Text) AndAlso Label1.Text <> "Unknown user" Then
                    actorName = Label1.Text.Trim()
                ElseIf Not String.IsNullOrWhiteSpace(currentUsername) Then
                    actorName = currentUsername.Trim()
                Else
                    actorName = "System User"
                End If
            End If

            Dim sql As String =
                "INSERT INTO dbo.system_notifications (actor_name, action_text, created_at) " &
                "VALUES (@actor_name, @action_text, GETDATE())"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@actor_name", SqlDbType.VarChar, 150).Value = actorName
                cmd.Parameters.Add("@action_text", SqlDbType.VarChar, 255).Value = actionText.Trim()
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub LoadNotificationsGrid(notifGrid As DataGridView, Optional maxRows As Integer = 20)
        If notifGrid Is Nothing Then Return

        Try
            EnsureConnectionOpen()

            Dim selectedRowIdx As Integer = If(notifGrid.CurrentRow IsNot Nothing, notifGrid.CurrentRow.Index, -1)
            notifGrid.Rows.Clear()

            Dim sql As String =
                "SELECT TOP (@topRows) actor_name, action_text, created_at " &
                "FROM dbo.system_notifications " &
                "ORDER BY notif_id DESC"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@topRows", SqlDbType.Int).Value = Math.Max(1, maxRows)

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim message As String = reader("actor_name").ToString().Trim() & " " &
                                              reader("action_text").ToString().Trim()
                        notifGrid.Rows.Add(message)
                    End While
                End Using
            End Using

            If selectedRowIdx >= 0 AndAlso selectedRowIdx < notifGrid.Rows.Count Then
                notifGrid.CurrentCell = notifGrid.Rows(selectedRowIdx).Cells(0)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function FindNotificationGrid(parent As Control) As DataGridView
        If parent Is Nothing Then Return Nothing
        For Each c As Control In parent.Controls
            If TypeOf c Is DataGridView AndAlso c.Name.IndexOf("notif", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return DirectCast(c, DataGridView)
            End If
            Dim childGrid As DataGridView = FindNotificationGrid(c)
            If childGrid IsNot Nothing Then Return childGrid
        Next
        Return Nothing
    End Function

    Private Sub notifTimer_Tick(sender As Object, e As EventArgs) Handles notifTimer.Tick
        ProfileAdmin()

        Dim grid As DataGridView = FindNotificationGrid(adminDashboard)
        If grid IsNot Nothing Then
            LoadNotificationsGrid(grid)
        End If
    End Sub

    ' ===================== FORM EVENTS =====================

    Private Sub AdminFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
        notifTimer.Interval = 3000
        notifTimer.Start()

        ChangeUsrCnrl(adminDashboard, Guna2Panel3)

        ProfileAdmin()
        adminDashboard.RefreshDashboardData()

        Dim grid As DataGridView = FindNotificationGrid(adminDashboard)
        If grid IsNot Nothing Then
            LoadNotificationsGrid(grid)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm tt")
        Label7.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy")
    End Sub

    Private Sub AdminFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        notifTimer.Stop()
        Application.Exit()
    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click
        LogForm.Show()
        Me.Hide()
    End Sub

    Private Sub dashboardbtn_Click(sender As Object, e As EventArgs) Handles dashboardbtn.Click
        ChangeUsrCnrl(adminDashboard, Guna2Panel3)
        dashboardbtn.BackColor = Color.FromArgb(4, 196, 80)
        ProfileAdmin()
        adminDashboard.RefreshDashboardData()

        Dim grid As DataGridView = FindNotificationGrid(adminDashboard)
        If grid IsNot Nothing Then
            LoadNotificationsGrid(grid)
        End If
    End Sub

    Private Sub patmangementbtn_Click(sender As Object, e As EventArgs) Handles patmangementbtn.Click
        patmangementbtn.BackColor = Color.FromArgb(4, 196, 80)
        Dim patrec As New PatientRecord
        ChangeUsrCnrl(patrec, Guna2Panel3)
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

    Private Sub dashboardbtn_Leave(sender As Object, e As EventArgs) Handles dashboardbtn.Leave
        dashboardbtn.BackColor = Color.Transparent
    End Sub

    Private Sub patmangementbtn_Leave(sender As Object, e As EventArgs) Handles patmangementbtn.Leave
        patmangementbtn.BackColor = Color.Transparent
    End Sub

    Private Sub staffmanagementbtn_Leave(sender As Object, e As EventArgs) Handles staffmanagementbtn.Leave
        staffmanagementbtn.BackColor = Color.Transparent
    End Sub

    Private Sub appointmentbtn_Leave(sender As Object, e As EventArgs) Handles appointmentbtn.Leave
        appointmentbtn.BackColor = Color.Transparent
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Adminrep.Click
        Adminrep.BackColor = Color.Transparent
        Dim Adminreports As New Adminreports
        ChangeUsrCnrl(Adminreports, Guna2Panel3)
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

End Class