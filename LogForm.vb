Public Class LogForm
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        Me.Hide()
        RegisterForm.Show()
    End Sub

    Private Sub logBtn_Click(sender As Object, e As EventArgs) Handles logBtn.Click
        MessageBox.Show("Button clicked!")

        ContentForm.Show()
        Me.Hide()
    End Sub
End Class