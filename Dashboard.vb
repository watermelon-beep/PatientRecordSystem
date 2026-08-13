Public Class Dashboard

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        contentForm.Roundbtn(viewPatientbtn, 40)
        contentForm.RoundPanel(totalPatientPnl, 20)
        contentForm.RoundPanel(todaysAppPnl, 20)
        contentForm.RoundPanel(totalConsultPnl, 20)
    End Sub

End Class
