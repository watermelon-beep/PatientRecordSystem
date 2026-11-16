Imports System.Data.SqlClient

Public Class PatientRecord

    ' Relative width of each column:
    ' 0 ID, 1 First, 2 Middle, 3 Surname, 4 Age, 5 Birthdate, 6 Gender,
    ' 7 Address, 8 Emergency, 9 Civil, 10 Phone, 11 Image, 12 Edit, 13 Delete, 14 View
    Private ReadOnly ratios() As Single = {55, 80, 80, 90, 40, 85, 65, 240, 190, 75, 100, 75, 70, 70, 70}

    Private Sub PatientRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StyleGrid()
        LoadPatients("")
    End Sub

    ' ---------- LOAD / SEARCH ----------
    Private Sub LoadPatients(search As String)
        Try
            PatientTbl.Rows.Clear()

            query = "SELECT * FROM patient_info " &
                    "WHERE CAST(PatientID AS VARCHAR(10)) LIKE @search " &
                    "OR FirstName LIKE @search OR MiddleName LIKE @search OR Surname LIKE @search " &
                    "ORDER BY PatientID"

            Using comm As New SqlCommand(query, conn)
                comm.Parameters.AddWithValue("@search", "%" & search & "%")

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

    ' ---------- EVENTS ----------
    Private Sub addpatbtn_Click(sender As Object, e As EventArgs) Handles addpatbtn.Click
        AdminFrm.Hide()
        PatientInfo.Show()
    End Sub

    Private Sub searchbtn_Click(sender As Object, e As EventArgs) Handles searchbtn.Click
        LoadPatients(searchtxbx.Text.Trim())
    End Sub

    Private Sub searchtxbx_TextChanged(sender As Object, e As EventArgs) Handles searchtxbx.TextChanged
        LoadPatients(searchtxbx.Text.Trim())
    End Sub

    Private Sub PatientTbl_Resize(sender As Object, e As EventArgs) Handles PatientTbl.Resize
        ApplyColumnWidths()
    End Sub

    ' ---------- COLUMN WIDTHS (pixel based, always fits the grid) ----------
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

    Private Sub viewallbtn_Click(sender As Object, e As EventArgs) Handles viewallbtn.Click
        searchtxbx.Clear()
        LoadPatients("")
    End Sub
End Class