<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Dashboard
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.totalPatientPnl = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.todaysAppPnl = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.totalConsultPnl = New System.Windows.Forms.Panel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.viewPatientbtn = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.totalPatientPnl.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.todaysAppPnl.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.totalConsultPnl.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1574, 129)
        Me.Panel1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft YaHei UI", 26.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ButtonFace
        Me.Label1.Location = New System.Drawing.Point(691, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(255, 46)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "DASHBOARD"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.TableLayoutPanel1)
        Me.Panel2.Location = New System.Drawing.Point(276, 208)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1003, 163)
        Me.Panel2.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.totalPatientPnl, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.todaysAppPnl, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.totalConsultPnl, 2, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1003, 163)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'totalPatientPnl
        '
        Me.totalPatientPnl.BackColor = System.Drawing.Color.Transparent
        Me.totalPatientPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.totalPatientPnl.Controls.Add(Me.Label3)
        Me.totalPatientPnl.Controls.Add(Me.PictureBox1)
        Me.totalPatientPnl.Controls.Add(Me.Label2)
        Me.totalPatientPnl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.totalPatientPnl.Location = New System.Drawing.Point(5, 5)
        Me.totalPatientPnl.Margin = New System.Windows.Forms.Padding(5)
        Me.totalPatientPnl.Name = "totalPatientPnl"
        Me.totalPatientPnl.Size = New System.Drawing.Size(324, 153)
        Me.totalPatientPnl.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Impact", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(168, 72)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 34)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "120"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.PatientRecordSystem.My.Resources.Resources.charImg
        Me.PictureBox1.Location = New System.Drawing.Point(55, 34)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(69, 66)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft YaHei UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(164, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(115, 22)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Total Patient"
        '
        'todaysAppPnl
        '
        Me.todaysAppPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.todaysAppPnl.Controls.Add(Me.Label4)
        Me.todaysAppPnl.Controls.Add(Me.PictureBox2)
        Me.todaysAppPnl.Controls.Add(Me.Label5)
        Me.todaysAppPnl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.todaysAppPnl.Location = New System.Drawing.Point(339, 5)
        Me.todaysAppPnl.Margin = New System.Windows.Forms.Padding(5)
        Me.todaysAppPnl.Name = "todaysAppPnl"
        Me.todaysAppPnl.Size = New System.Drawing.Size(324, 153)
        Me.todaysAppPnl.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Impact", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(174, 72)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 34)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "18"
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.PatientRecordSystem.My.Resources.Resources.charImg
        Me.PictureBox2.Location = New System.Drawing.Point(61, 34)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(69, 66)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 4
        Me.PictureBox2.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft YaHei UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(164, 18)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(119, 44)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Today/s " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Appointment" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'totalConsultPnl
        '
        Me.totalConsultPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.totalConsultPnl.Controls.Add(Me.Label6)
        Me.totalConsultPnl.Controls.Add(Me.PictureBox3)
        Me.totalConsultPnl.Controls.Add(Me.Label7)
        Me.totalConsultPnl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.totalConsultPnl.Location = New System.Drawing.Point(673, 5)
        Me.totalConsultPnl.Margin = New System.Windows.Forms.Padding(5)
        Me.totalConsultPnl.Name = "totalConsultPnl"
        Me.totalConsultPnl.Size = New System.Drawing.Size(325, 153)
        Me.totalConsultPnl.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Impact", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(189, 78)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(44, 34)
        Me.Label6.TabIndex = 8
        Me.Label6.Text = "43"
        '
        'PictureBox3
        '
        Me.PictureBox3.Image = Global.PatientRecordSystem.My.Resources.Resources.charImg
        Me.PictureBox3.Location = New System.Drawing.Point(76, 40)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(69, 66)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 7
        Me.PictureBox3.TabStop = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft YaHei UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(180, 18)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(114, 44)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "Total " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Consultation" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'DataGridView1
        '
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.White
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3})
        Me.DataGridView1.GridColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(174, Byte), Integer), CType(CType(88, Byte), Integer))
        Me.DataGridView1.Location = New System.Drawing.Point(271, 464)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView1.Size = New System.Drawing.Size(1003, 434)
        Me.DataGridView1.TabIndex = 2
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Column1.HeaderText = "ID"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Column2.HeaderText = "Name"
        Me.Column2.Name = "Column2"
        '
        'Column3
        '
        Me.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Column3.HeaderText = "Lat Visit"
        Me.Column3.Name = "Column3"
        '
        'viewPatientbtn
        '
        Me.viewPatientbtn.BackColor = System.Drawing.Color.Transparent
        Me.viewPatientbtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.viewPatientbtn.FlatAppearance.BorderSize = 3
        Me.viewPatientbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.viewPatientbtn.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.viewPatientbtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(174, Byte), Integer), CType(CType(88, Byte), Integer))
        Me.viewPatientbtn.Location = New System.Drawing.Point(1041, 932)
        Me.viewPatientbtn.Name = "viewPatientbtn"
        Me.viewPatientbtn.Size = New System.Drawing.Size(203, 62)
        Me.viewPatientbtn.TabIndex = 3
        Me.viewPatientbtn.Text = "View All Patient"
        Me.viewPatientbtn.UseVisualStyleBackColor = False
        '
        'Dashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Controls.Add(Me.viewPatientbtn)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Dashboard"
        Me.Size = New System.Drawing.Size(1574, 1041)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.totalPatientPnl.ResumeLayout(False)
        Me.totalPatientPnl.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.todaysAppPnl.ResumeLayout(False)
        Me.todaysAppPnl.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.totalConsultPnl.ResumeLayout(False)
        Me.totalConsultPnl.PerformLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents totalPatientPnl As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents todaysAppPnl As Panel
    Friend WithEvents totalConsultPnl As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents Label7 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents viewPatientbtn As Button
End Class
