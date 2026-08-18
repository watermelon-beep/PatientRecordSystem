Public Class Dashboard
    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ContentForm.dashboardBtn.BackColor = Color.FromArgb(65, 174, 88)

        Rounddatagrid(todayAppDgv, 20)
        Rounddatagrid(recentPatDgv, 20)
        RoundPanel(totalMPanel, 20)
        RoundPanel(totalFPanel, 20)
    End Sub
End Class
