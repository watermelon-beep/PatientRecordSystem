Imports System.Data.SqlClient

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
        addStaff()
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
End Class