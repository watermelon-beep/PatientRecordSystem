Public Class LogForm

    Private userLog As New UserLog

    Private Sub LogForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Application.Exit()

    End Sub

    Private Sub LogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel6.Controls.Clear()
        userLog.Dock = DockStyle.Fill
        Panel6.Controls.Add(userLog)
    End Sub

    Public Sub ShowRegister()

        Dim register As New Register()

        register.Dock = DockStyle.Fill

        Panel6.Controls.Clear()
        Panel6.Controls.Add(register)

    End Sub

    Public Sub ShowLogin()

        Dim userLog As New UserLog()

        userLog.Dock = DockStyle.Fill
        Panel6.Controls.Clear()
        Panel6.Controls.Add(userLog)
    End Sub
End Class