Public Class Logout

    Public Event logoutConfirmed()
    Public Event logoutCanceled()

    Private Sub yesBtn_Click(sender As Object, e As EventArgs) Handles yesBtn.Click

        RaiseEvent logoutConfirmed()

    End Sub

    Private Sub noBtn_Click(sender As Object, e As EventArgs) Handles noBtn.Click

        RaiseEvent logoutCanceled()

    End Sub
End Class
