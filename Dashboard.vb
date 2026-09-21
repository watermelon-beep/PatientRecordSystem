Imports System.Windows.Forms.DataVisualization.Charting

Public Class Dashboard


    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ContentForm.dashboardBtn.BackColor = Color.FromArgb(65, 174, 88)

        Rounddatagrid(todayAppDgv, 20)
        RoundPanel(totalMPanel, 20)
        RoundPanel(totalFPanel, 20)


        With todayAppDgv.Rows
            .Add("8:00 AM", "Juan Dela Cruz", "General Consultation")
            .Add("8:15 AM", "Maria Santos", "Prenatal Check-up")
            .Add("8:30 AM", "Pedro Reyes", "Follow-up")
            .Add("8:45 AM", "Ana Garcia", "Vaccination")
            .Add("9:00 AM", "Mark Villanueva", "General Consultation")
            .Add("9:15 AM", "Sofia Mendoza", "Medical Certificate")
            .Add("9:30 AM", "Carlos Bautista", "Check-up")
            .Add("9:45 AM", "Angela Ramos", "Follow-up")
            .Add("10:00 AM", "Daniel Navarro", "General Consultation")
            .Add("10:15 AM", "Camille Flores", "Vaccination")
            .Add("10:30 AM", "Miguel Aquino", "Blood Pressure Check")
            .Add("10:45 AM", "Patricia Castillo", "General Consultation")
            .Add("11:00 AM", "Gabriel Torres", "Follow-up")
            .Add("11:15 AM", "Nicole Fernandez", "Prenatal Check-up")
            .Add("11:30 AM", "Ryan Mercado", "Medical Certificate")
            .Add("1:00 PM", "Isabella Cruz", "General Consultation")
            .Add("1:15 PM", "Nathaniel Ramos", "Check-up")
            .Add("1:30 PM", "Beatrice Flores", "Vaccination")
            .Add("1:45 PM", "Joshua Mendoza", "Follow-up")
            .Add("2:00 PM", "Christine Aquino", "General Consultation")
            .Add("2:15 PM", "Rafael Santos", "Blood Pressure Check")
            .Add("2:30 PM", "Samantha Reyes", "Prenatal Check-up")
            .Add("2:45 PM", "Kevin Bautista", "General Consultation")
            .Add("3:00 PM", "Andrea Navarro", "Follow-up")
            .Add("3:15 PM", "Francis Garcia", "Medical Certificate")
        End With

        Chart1.Series.Clear()

        Dim series As New Series("Patients")

        series.ChartType = SeriesChartType.Line

        With series.Points
            .AddXY("January", 10)
            .AddXY("February", 15)
            .AddXY("March", 20)
            .AddXY("April", 12)
            .AddXY("May", 18)
            .AddXY("June", 25)
            .AddXY("July", 30)
            .AddXY("August", 22)
            .AddXY("September", 28)
            .AddXY("October", 55)
            .AddXY("November", 10)
            .AddXY("December", 25)
        End With

        staffTbl.Rows.Add("kian", "john", "👁 Edit")
        For Each point As DataPoint In series.Points
            point.ToolTip = point.YValues(0).ToString()
        Next

        Chart1.Series.Add(series)

        Dim countPatients As Integer = todayAppDgv.Rows.Count
        Label3.Text = countPatients.ToString()
    End Sub


    Private Sub staffTbl_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles staffTbl.CellContentClick

    End Sub
End Class
