
Module Profile
    Public currentUsername As String = ""

    Public Sub ChangeUsrCnrl(usrContrl As UserControl, panel As Panel)
        panel.Controls.Clear()
        usrContrl.Dock = DockStyle.Fill
        panel.Controls.Add(usrContrl)
    End Sub

End Module
