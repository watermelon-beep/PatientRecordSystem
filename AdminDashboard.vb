Imports System.Data.SqlClient

Public Class AdminDashboard

    ' ===================== TABLE NG STAFF =====================
    Private Sub DashbooardTable()

        admintbl.Rows.Clear()

        query = "SELECT first_Name, middle_Name, surname, name_extension, age, gender, Position FROM staff_information"

        Using comm As New SqlCommand(query, conn)
            dataRead = comm.ExecuteReader
            While dataRead.Read()
                admintbl.Rows.Add(dataRead("first_Name"),
                                  dataRead("middle_Name"),
                                  dataRead("surname"),
                                  dataRead("name_extension"),
                                  dataRead("age"),
                                  dataRead("gender"),
                                  dataRead("Position"))
            End While
        End Using
        dataRead.Close()

    End Sub

    ' ===================== HELPER: KUMUHA NG BILANG =====================
    Private Function GetCount(sql As String) As Integer
        Using comm As New SqlCommand(sql, conn)
            Dim result = comm.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then Return 0
            Return Convert.ToInt32(result)
        End Using
    End Function

    ' ===================== HELPER: I-SET ANG CARD =====================
    Private Sub SetCard(lblValue As Label, picTrend As Guna.UI2.WinForms.Guna2PictureBox, current As Integer, previous As Integer)
        lblValue.Text = current.ToString()

        If current >= previous Then
            picTrend.Image = My.Resources.trending_up__1_     ' green arrow
        Else
            picTrend.Image = My.Resources.trending_down       ' red arrow
        End If
    End Sub

    ' ===================== LOAD NG DASHBOARD CARDS =====================
    Private Sub LoadDashboardCards()
        Try
            Dim today As String = "CAST(GETDATE() AS DATE)"

            ' 1. Guna2ShadowPanel1 - TOTAL PATIENTS (vs last week)
            Dim totalNow = GetCount("SELECT COUNT(*) FROM patient_info")
            Dim totalPrev = GetCount("SELECT COUNT(*) FROM patient_info WHERE DateRegistered < DATEADD(DAY, -7, " & today & ")")
            SetCard(Label2, Guna2PictureBox1, totalNow, totalPrev)

            ' 2. Guna2ShadowPanel2 - TODAY'S APPOINTMENT (vs yesterday)
            Dim apptNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = " & today)
            Dim apptPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = DATEADD(DAY, -1, " & today & ")")
            SetCard(Label5, Guna2PictureBox2, apptNow, apptPrev)

            ' 3. Guna2ShadowPanel3 - NEW PATIENTS TODAY (vs yesterday)
            Dim newNow = GetCount("SELECT COUNT(*) FROM patient_info WHERE CAST(DateRegistered AS DATE) = " & today)
            Dim newPrev = GetCount("SELECT COUNT(*) FROM patient_info WHERE CAST(DateRegistered AS DATE) = DATEADD(DAY, -1, " & today & ")")
            SetCard(Label8, Guna2PictureBox3, newNow, newPrev)

            ' 4. Guna2ShadowPanel4 - TOTAL STAFF (vs last month)
            '    Kung may column na DateAdded sa staff_information, ito ang gagamitin.
            '    Kung wala pa, magiging pantay ang paghahambing (green arrow).
            Dim staffNow = GetCount("SELECT COUNT(*) FROM staff_information")
            Dim staffPrev = staffNow
            If GetCount("SELECT CASE WHEN COL_LENGTH('staff_information','DateAdded') IS NULL THEN 0 ELSE 1 END") = 1 Then
                staffPrev = GetCount("SELECT COUNT(*) FROM staff_information WHERE DateAdded < DATEADD(MONTH, -1, " & today & ")")
            End If
            SetCard(Label11, Guna2PictureBox4, staffNow, staffPrev)

            ' 5. Guna2ShadowPanel5 - TOTAL MALE PATIENTS (vs last month)
            Dim maleNow = GetCount("SELECT COUNT(*) FROM patient_info WHERE Gender = 'Male'")
            Dim malePrev = GetCount("SELECT COUNT(*) FROM patient_info WHERE Gender = 'Male' AND DateRegistered < DATEADD(MONTH, -1, " & today & ")")
            SetCard(Label14, Guna2PictureBox5, maleNow, malePrev)

            ' 6. Guna2ShadowPanel6 - TOTAL FEMALE PATIENTS (vs last month)
            Dim femNow = GetCount("SELECT COUNT(*) FROM patient_info WHERE Gender = 'Female'")
            Dim femPrev = GetCount("SELECT COUNT(*) FROM patient_info WHERE Gender = 'Female' AND DateRegistered < DATEADD(MONTH, -1, " & today & ")")
            SetCard(Label17, Guna2PictureBox6, femNow, femPrev)

            ' 7. Guna2ShadowPanel7 - PENDING APPOINTMENTS (ngayon, vs parehong araw last week)
            Dim pendNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Pending' AND CAST(AppointmentDate AS DATE) = " & today)
            Dim pendPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Pending' AND CAST(AppointmentDate AS DATE) = DATEADD(DAY, -7, " & today & ")")
            SetCard(Label20, Guna2PictureBox7, pendNow, pendPrev)

            ' 8. Guna2ShadowPanel8 - COMPLETE TODAY (ngayon, vs parehong araw last week)
            Dim compNow = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed' AND CAST(AppointmentDate AS DATE) = " & today)
            Dim compPrev = GetCount("SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed' AND CAST(AppointmentDate AS DATE) = DATEADD(DAY, -7, " & today & ")")
            SetCard(Label23, Guna2PictureBox8, compNow, compPrev)

        Catch ex As Exception
            MessageBox.Show("Error loading dashboard: " & ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===================== FORM LOAD =====================
    Private Sub AdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AdminFrm.dashboardbtn.BackColor = Color.FromArgb(4, 196, 80)

        dbConnection()
        DashbooardTable()
        LoadDashboardCards()

        With Guna2DataGridView2.Rows
            .Add("Karl Benedict Palomo added a new patient")
            .Add("Maria Santos registered a new consultation")
            .Add("John Cruz updated a patient record")
            .Add("Ana Reyes added a new appointment")
            .Add("Mark Dela Cruz completed a consultation")
            .Add("Sofia Garcia cancelled an appointment")
            .Add("James Flores updated patient information")
            .Add("Angela Ramos added a new medical record")
            .Add("Daniel Mendoza registered a new patient")
        End With
    End Sub

End Class