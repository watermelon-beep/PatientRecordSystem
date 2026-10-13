Imports System.Security.AccessControl
Imports System.Windows.Forms.VisualStyles
Imports Guna.UI2.WinForms

Module Profile
    Public currentUsername As String = ""

    Public Sub selectedBtn(currentBtn As Guna2Button)

        currentBtn = currentBtn

        currentBtn.FillColor = Color.WhiteSmoke
        currentBtn.BorderRadius = 20
        currentBtn.ForeColor = Color.Black
        currentBtn.CustomizableEdges.BottomRight = False
        currentBtn.CustomizableEdges.TopRight = False
    End Sub
End Module
