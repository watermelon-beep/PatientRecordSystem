Public Class Dashboard

    Private roundCorner As New RoundCorner()
    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ContentForm.dashboardBtn.BackColor = Color.FromArgb(65, 174, 88)

        roundCorner.Rounddatagrid(todayAppDgv, 20)
        roundCorner.Rounddatagrid(recentPatDgv, 20)
        roundCorner.RoundPanel(totalMPanel, 20)
        roundCorner.RoundPanel(totalFPanel, 20)
    End Sub
End Class
