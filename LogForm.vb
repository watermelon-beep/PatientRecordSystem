Public Class LogForm

    Private userLog As New UserLog

    Private Sub LogForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Application.Exit()
    End Sub

    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' ===== MINIMIZE / MAXIMIZE (hindi ginagalaw ang design) =====
        Me.AutoSize = False
        Me.MaximumSize = Size.Empty        ' walang limit sa laki ng window
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.MinimumSize = New Size(600, 400)

        ' Itala ang orihinal na laki ng login screen bago ilagay sa panel
        Dim designW As Integer = userLog.Width
        Dim designH As Integer = userLog.Height

        Guna2Panel3.Controls.Clear()
        userLog.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(userLog)

        ' Kapag mas maliit ang window kaysa sa design, may scrollbar imbes na putol
        Guna2Panel3.AutoScroll = True
        Guna2Panel3.AutoScrollMinSize = New Size(designW, designH)

    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

End Class