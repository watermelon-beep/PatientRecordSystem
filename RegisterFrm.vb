Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Public Class RegisterFrm



    Private Sub cmbxLblVisible(cmbx As ComboBox, lbl As Label)
        If cmbx.SelectedIndex <> -1 Then
            lbl.Visible = False
        Else
            lbl.Visible = True
        End If
    End Sub

    Sub addStaff()

        If UsernameExists(regusrnmtxbx.Text) Then

            MessageBox.Show("Username already exists.")
        Else
            query = "INSERT INTO staff_information (
                        first_Name, 
                        middle_Name,
                        surname,
                        name_extension,
                        age,
                        birth_place,
                        user_address,
                        gender,
                        civil_status,
                        username,
                        user_password,
                        Position,
                        created_account
                    )
                      VALUES (
                        @first_Name,
                        @middle_Name,
                        @surname,
                        @name_extension,
                        @age,
                        @birth_place,
                        @user_address,
                        @gender,
                        @civil_status,
                        @username,
                        @user_password,
                        @Position,
                        @created_account
                        )"

            Using comm As New SqlClient.SqlCommand(query, conn)

                With comm.Parameters
                    .AddWithValue("@first_Name", frstnmtxbx.Text)
                    .AddWithValue("@middle_Name", mdlnmtxbx.Text)
                    .AddWithValue("@surname", srnmtxbx.Text)
                    .AddWithValue("@name_extension", excbx.Text)
                    .AddWithValue("@age", agetxbx.Text)
                    .AddWithValue("@birth_place", birthtxbx.Text)
                    .AddWithValue("@user_address", addrtxbx.Text)
                    .AddWithValue("@gender", gdrcmbx.Text)
                    .AddWithValue("@civil_status", cvlcbx.Text)
                    .AddWithValue("@username", regusrnmtxbx.Text)
                    .AddWithValue("@user_password", regpasstxbx.Text)
                    .AddWithValue("@Position", rolecbx.Text)
                    .AddWithValue("@created_account", DateTime.Now.ToString("yyyy-MM-dd"))
                End With

                comm.ExecuteNonQuery()

                MsgBox("added")
            End Using
        End If

    End Sub

    Private Sub regbtn_Click(sender As Object, e As EventArgs) Handles regbtn.Click
        If Not Regex.IsMatch(regpasstxbx.Text, "^(?=.*[A-Z])(?=.*[0-9]).{6,}$") Then
            Label20.Visible = True
        Else
            addStaff()
        End If
    End Sub

    Private Sub RegisterFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dbConnection()

        gdrcmbx.Items.Add("Male")
        gdrcmbx.Items.Add("Female")
        gdrcmbx.Items.Add("Prefer not to say")

        cvlcbx.Items.Add("Single")
        cvlcbx.Items.Add("Married")
        cvlcbx.Items.Add("Widowed")
        cvlcbx.Items.Add("Divorced")
        cvlcbx.Items.Add("Prefer not to say")

        excbx.Items.Add("Jr.")
        excbx.Items.Add("Sr.")
        excbx.Items.Add("III")
        excbx.Items.Add("None")

        rolecbx.Items.Add("Admin")
        rolecbx.Items.Add("Barangay Assistance")
        rolecbx.Items.Add("Doctor")
    End Sub

    Private Sub excbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles excbx.SelectedIndexChanged
        cmbxLblVisible(excbx, Label16)
    End Sub

    Private Sub gdrcmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gdrcmbx.SelectedIndexChanged
        cmbxLblVisible(gdrcmbx, Label17)
    End Sub

    Private Sub cvlcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cvlcbx.SelectedIndexChanged
        cmbxLblVisible(cvlcbx, Label18)
    End Sub

    Private Sub rolecbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rolecbx.SelectedIndexChanged
        cmbxLblVisible(rolecbx, Label19)
    End Sub

    Private Sub yrcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles yrcbx.SelectedIndexChanged
        cmbxLblVisible(yrcbx, Label15)
    End Sub

    Private Sub mnthcbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles mnthcbx.SelectedIndexChanged
        cmbxLblVisible(mnthcbx, Label14)
    End Sub

    Private Sub daycmbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles daycmbx.SelectedIndexChanged
        cmbxLblVisible(daycmbx, Label13)
    End Sub

    Private Sub gtloginbtn_Click(sender As Object, e As EventArgs) Handles gtloginbtn.Click

        LogForm.Show()
        Me.Hide()

    End Sub

    Private Sub randpassbtn_Click(sender As Object, e As EventArgs) Handles randpassbtn.Click
        Dim rnd As New Random()

        Dim randomNumber As Integer = rnd.Next(2, 16)

        Dim capital As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim small As String = "abcdefghijklmnopqrstuvwxyz"
        Dim number As String = "0123456789"
        Dim special As String = "!@#$%&*_-^"

        Dim password As String = ""

        password &= capital(rnd.Next(capital.Length))
        password &= small(rnd.Next(small.Length))
        password &= number(rnd.Next(number.Length))
        password &= special(rnd.Next(special.Length))

        Dim allChars As String = capital & small & number & special

        For i As Integer = 1 To randomNumber
            password &= allChars(rnd.Next(allChars.Length))
        Next

        regpasstxbx.Text = password
        cnfrmpasstxbx.Text = password

    End Sub

    Private Sub cpybtn_Click(sender As Object, e As EventArgs) Handles cpybtn.Click
        Clipboard.SetText(regpasstxbx.Text)
    End Sub

    Private Sub regpasstxbx_TextChanged(sender As Object, e As EventArgs) Handles regpasstxbx.TextChanged
        Dim regpass As String = regpasstxbx.Text

        If regpass.Length = 0 Then
            Label4.Text = "empty"
            Label4.ForeColor = Color.Silver
            passbar.Value = 0
        End If
        If regpass.Length > 0 And regpass.Length <= 6 Then
            Label4.Text = "weak"
            Label4.ForeColor = Color.Orange
            passbar.Value = 25
            passbar.ProgressColor = Color.Orange
            passbar.ProgressColor2 = Color.DarkOrange
        ElseIf regpass.Length > 6 And regpass.Length <= 10 Then
            Label4.Text = "strong"
            Label4.ForeColor = Color.LimeGreen
            passbar.Value = 50
            passbar.ProgressColor = Color.LimeGreen
            passbar.ProgressColor2 = Color.Green
        ElseIf regpass.Length > 10 Then
            Label4.Text = "very strong"
            Label4.ForeColor = Color.DarkGreen
            passbar.Value = 100
            passbar.ProgressColor = Color.Green
            passbar.ProgressColor2 = Color.DarkGreen
        End If

    End Sub

End Class