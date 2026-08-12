Public Class Dashboard

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Form1.Roundbtn(viewPatientbtn, 40)
        Form1.RoundPanel(totalPatientPnl, 20)
        Form1.RoundPanel(todaysAppPnl, 20)
        Form1.RoundPanel(totalConsultPnl, 20)
    End Sub

End Class
