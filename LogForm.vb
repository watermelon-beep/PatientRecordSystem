Public Class LogForm

    Private userLog As New UserLog

    Private Sub LogForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Application.Exit()

    End Sub

    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Guna2Panel3.Controls.Clear()
        userLog.Dock = DockStyle.Fill
        Guna2Panel3.Controls.Add(userLog)

    End Sub

End Class