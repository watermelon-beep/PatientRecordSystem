Imports System.Windows.Forms.DataVisualization.Charting

Public Class NewDashboard

    Private Sub NewDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label2.Text = DateTime.Now.ToString("MMMM dd, yyyy")
        Label3.Text = DateTime.Now.ToString("dddd")
        Label4.Text = DateTime.Now.ToString("hh:mm tt")

        Chart1.Series.Clear()

        Dim ser As New Series("Appointments")

        ser.ChartType = SeriesChartType.Area

        With ser.Points
            .AddXY("Monday", 5)
            .AddXY("Tuesday", 10)
            .AddXY("Wednesday", 1)
            .AddXY("Thursday", 20)
            .AddXY("Friday", 2)
        End With

        Chart1.Series.Add(ser)

        Chart2.Series.Clear()

        Dim series As New Series("Patients")

        series.ChartType = SeriesChartType.Doughnut

        With series.Points
            .AddXY("Male", 10)
            .AddXY("Female", 15)
            .AddXY("not prefer to say", 20)
        End With

        Chart2.Series.Add(series)

    End Sub

    Private Sub Guna2ShadowPanel3_Paint(sender As Object, e As PaintEventArgs)

    End Sub

    Private Sub Guna2Panel8_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel8.Paint

    End Sub
End Class
