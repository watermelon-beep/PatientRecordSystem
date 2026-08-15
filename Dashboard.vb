Public Class Dashboard

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        contentForm.Roundbtn(viewPatientbtn, 40)
        contentForm.RoundPanel(totalPatientPnl, 20)
        contentForm.RoundPanel(todaysAppPnl, 20)
        ContentForm.RoundPanel(totalConsultPnl, 20)
        ContentForm.Rounddatagrid(dashboardTbl, 20)
        ContentForm.dashboardBtn.BackColor = Color.FromArgb(65, 174, 88)
    End Sub

End Class
