Imports System.Data.SqlClient

Public Class PatientRecord

    ' 0 ID, 1 First, 2 Middle, 3 Surname, 4 Age, 5 Birthdate, 6 Gender,
    ' 7 Address, 8 Emergency, 9 Civil, 10 Phone, 11 Image, 12 Edit, 13 Delete, 14 View
    Private ReadOnly ratios() As Single = {55, 80, 80, 90, 40, 85, 65, 240, 190, 75, 100, 75, 70, 70, 70}
    Private isLoading As Boolean = True

    Private Sub PatientRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()

        selectionposcmbx.Items.Clear()
        selectionposcmbx.Items.AddRange({"All", "Male", "Female", "Single", "Married"})
        selectionposcmbx.SelectedIndex = 0

        StyleGrid()
        LoadPatients("")
        isLoading = False
        ShowFirstPatient()
    End Sub

    ' ---------- LOAD / SEARCH / FILTER ----------
    Private Sub LoadPatients(search As String)
        Try
            PatientTbl.Rows.Clear()

            Dim filter As String = "All"
            If selectionposcmbx.SelectedItem IsNot Nothing Then filter = selectionposcmbx.SelectedItem.ToString()

            query = "SELECT * FROM patient_info " &
                    "WHERE (CAST(PatientID AS VARCHAR(10)) LIKE @search " &
                    "OR FirstName LIKE @search OR MiddleName LIKE @search OR Surname LIKE @search) " &
                    "AND (@filter = 'All' OR gender = @filter OR civilstatus = @filter) " &
                    "ORDER BY PatientID"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.AddWithValue("@search", "%" & search & "%")
                comm.Parameters.AddWithValue("@filter", filter)

                Using dataRead = comm.ExecuteReader()
                    While dataRead.Read
                        Dim img As Object = If(IsDBNull(dataRead("PatientImage")), Nothing, dataRead("PatientImage"))

                        PatientTbl.Rows.Add(dataRead("PatientID"),
                                            dataRead("FirstName"),
                                            dataRead("MiddleName"),
                                            dataRead("Surname"),
                                            dataRead("Age"),
                                            Convert.ToDateTime(dataRead("birthdate")).ToString("dd/MM/yyyy"),
                                            dataRead("Gender"),
                                            dataRead("Address"),
                                            dataRead("EmergencyContactInformation"),
                                            dataRead("civilstatus"),
                                            dataRead("Phonenumber"),
                                            img,
                                            "Edit",
                                            "Delete",
                                            "View")
                    End While
                End Using
            End Using

            For Each r As DataGridViewRow In PatientTbl.Rows
                r.Height = 80
            Next

            ColorButtons()
            PatientTbl.CurrentCell = Nothing
            PatientTbl.ClearSelection()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ' ---------- BANNER (yellow circle) ----------
    Private Sub ShowFirstPatient()
        If PatientTbl.Rows.Count > 0 Then
            Dim id As String = PatientTbl.Rows(0).Cells(0).Value.ToString()
            ViewPatientProfile(id)
            ViewPhoto(id)
        Else
            ClearBanner()
        End If
    End Sub

    Private Sub ClearBanner()
        Label10.Text = ""
        Label2.Text = "No patient found"
        Label3.Text = ""
        Label12.Text = ""
        Label6.Text = ""
        Label7.Text = ""
        Label5.Text = ""
        Label8.Text = ""
        Label4.Text = ""
        Label9.Text = ""
        Label11.Text = ""
        profileimg.Image = Nothing
    End Sub

    Private Sub ViewPatientProfile(id As String)
        Try
            query = "SELECT * FROM patient_info WHERE PatientID = @id"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.AddWithValue("@id", id)
                Using dataRead = comm.ExecuteReader()
                    If dataRead.Read Then
                        Label10.Text = "Patient"
                        Label2.Text = (dataRead("FirstName").ToString() & " " &
                                       dataRead("MiddleName").ToString() & " " &
                                       dataRead("Surname").ToString()).Replace("  ", " ")
                        Label3.Text = dataRead("age").ToString()
                        Label12.Text = "Registered patient"
                        Label6.Text = dataRead("gender").ToString()
                        Label7.Text = dataRead("civilstatus").ToString()
                        Label5.Text = dataRead("Address").ToString()
                        Label8.Text = dataRead("Phonenumber").ToString()
                        Label4.Text = dataRead("PatientID").ToString()
                        Label9.Text = Convert.ToDateTime(dataRead("birthdate")).ToString("MMMM dd, yyyy")
                        Label11.Text = "N/A"
                    End If
                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ViewPhoto(id As String)
        Try
            query = "SELECT PatientImage FROM patient_info WHERE PatientID = @id"
            Using comm As New SqlCommand(query, conn)
                comm.Parameters.AddWithValue("@id", id)
                Using dataRead = comm.ExecuteReader()
                    If dataRead.Read AndAlso Not IsDBNull(dataRead("PatientImage")) Then
                        Dim img() As Byte = CType(dataRead("PatientImage"), Byte())
                        Using ms As New IO.MemoryStream(img)
                            profileimg.Image = New Bitmap(Image.FromStream(ms))
                        End Using
                    Else
                        profileimg.Image = Nothing
                    End If
                End Using
            End Using
        Catch ex As Exception
            profileimg.Image = Nothing
        End Try
    End Sub

    ' ---------- EVENTS ----------
    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        PatientInfo.Show()
    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged
        If isLoading Then Return
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub selectionposcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectionposcmbx.SelectedIndexChanged
        If isLoading Then Return
        LoadPatients(searchtxbx.Text.Trim())
        ShowFirstPatient()
    End Sub

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        isLoading = True
        searchtxbx.Clear()
        selectionposcmbx.SelectedIndex = 0
        isLoading = False
        LoadPatients("")
        ShowFirstPatient()
    End Sub

    ' ---------- EDIT / DELETE / VIEW ----------
    Private Sub PatientTbl_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PatientTbl.CellContentClick
        If e.RowIndex < 0 Then Return

        Dim id As String = Convert.ToString(PatientTbl.Rows(e.RowIndex).Cells(0).Value)
        Dim fullName As String = Convert.ToString(PatientTbl.Rows(e.RowIndex).Cells(1).Value) & " " &
                                 Convert.ToString(PatientTbl.Rows(e.RowIndex).Cells(3).Value)

        Select Case e.ColumnIndex
            Case 14     ' VIEW
                ViewPatientProfile(id)
                ViewPhoto(id)

            Case 13     ' DELETE
                If MessageBox.Show("Are you sure you want to delete " & fullName & "?",
                                   "Confirm Delete", MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning) = DialogResult.Yes Then
                    Try
                        query = "DELETE FROM patient_info WHERE PatientID = @id"
                        Using comm As New SqlCommand(query, conn)
                            comm.Parameters.AddWithValue("@id", id)
                            comm.ExecuteNonQuery()
                        End Using

                        LoadPatients(searchtxbx.Text.Trim())
                        ShowFirstPatient()
                        MessageBox.Show("Patient deleted.", "Delete",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MsgBox(ex.Message)
                    End Try
                End If

            Case 12     ' EDIT
                MessageBox.Show("Edit for patient ID " & id & " is not ready yet.",
                                "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select
    End Sub

    Private Sub PatientTbl_Resize(sender As Object, e As EventArgs) Handles PatientTbl.Resize
        ApplyColumnWidths()
    End Sub

    ' ---------- COLUMN WIDTHS ----------
    Private Sub ApplyColumnWidths()
        If PatientTbl.Columns.Count = 0 Then Return

        Dim total As Single = 0
        For Each r In ratios
            total += r
        Next

        Dim avail As Integer = PatientTbl.ClientSize.Width - 2
        If avail <= 0 Then Return

        PatientTbl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        For i As Integer = 0 To Math.Min(ratios.Length, PatientTbl.Columns.Count) - 1
            With PatientTbl.Columns(i)
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .MinimumWidth = 30
                .Width = CInt(avail * ratios(i) / total)
                .Resizable = DataGridViewTriState.False
                .SortMode = DataGridViewColumnSortMode.NotSortable
            End With
        Next
    End Sub

    ' ---------- STYLE ----------
    Private Sub StyleGrid()
        With PatientTbl
            ApplyColumnWidths()

            For Each i In {0, 4, 5, 6, 9, 10}
                If i < .Columns.Count Then .Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Next

            If .Columns.Count > 11 AndAlso TypeOf .Columns(11) Is DataGridViewImageColumn Then
                CType(.Columns(11), DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            End If

            For i As Integer = 12 To 14
                If i < .Columns.Count AndAlso TypeOf .Columns(i) Is DataGridViewButtonColumn Then
                    CType(.Columns(i), DataGridViewButtonColumn).FlatStyle = FlatStyle.Flat
                End If
            Next

            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .RowTemplate.Height = 80
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToOrderColumns = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 48

            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.FromArgb(225, 235, 228)
            .BackgroundColor = Color.White
            .ScrollBars = ScrollBars.Vertical

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(76, 175, 80)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            Dim row1 As Color = Color.FromArgb(236, 247, 239)
            Dim row2 As Color = Color.White

            .DefaultCellStyle.BackColor = row2
            .DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9)
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DefaultCellStyle.SelectionBackColor = row2
            .DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30)
            .AlternatingRowsDefaultCellStyle.BackColor = row1
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = row1
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30)

            .ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(76, 175, 80)
            .ThemeStyle.HeaderStyle.ForeColor = Color.White
            .ThemeStyle.RowsStyle.BackColor = row2
            .ThemeStyle.RowsStyle.SelectionBackColor = row2
            .ThemeStyle.AlternatingRowsStyle.BackColor = row1
            .ThemeStyle.AlternatingRowsStyle.SelectionBackColor = row1
        End With
    End Sub

    ' ---------- EDIT / DELETE / VIEW COLORS ----------
    Private Sub ColorButtons()
        Dim colors() As Color = {Color.FromArgb(76, 175, 80),
                                 Color.FromArgb(211, 74, 74),
                                 Color.FromArgb(52, 120, 190)}

        For Each r As DataGridViewRow In PatientTbl.Rows
            For k As Integer = 0 To 2
                Dim idx As Integer = 12 + k
                If idx < PatientTbl.Columns.Count Then
                    With r.Cells(idx).Style
                        .BackColor = colors(k)
                        .ForeColor = Color.White
                        .SelectionBackColor = colors(k)
                        .SelectionForeColor = Color.White
                        .Alignment = DataGridViewContentAlignment.MiddleCenter
                    End With
                End If
            Next
        Next
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint
    End Sub

End Class