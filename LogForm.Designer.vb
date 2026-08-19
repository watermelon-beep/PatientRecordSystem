<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LogForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.usrnmLogtxtbx = New System.Windows.Forms.TextBox()
        Me.logBtn = New System.Windows.Forms.Button()
        Me.card = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.showPassCb = New System.Windows.Forms.CheckBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.passLogPnl = New System.Windows.Forms.Panel()
        Me.passLogTxbx = New System.Windows.Forms.TextBox()
        Me.userLogPnl = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.card.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.passLogPnl.SuspendLayout()
        Me.userLogPnl.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1033, 1041)
        Me.Panel1.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(174, Byte), Integer), CType(CType(88, Byte), Integer))
        Me.Panel3.Controls.Add(Me.PictureBox1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel3.Location = New System.Drawing.Point(0, 720)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1033, 321)
        Me.Panel3.TabIndex = 1
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Right
        Me.PictureBox1.Image = Global.PatientRecordSystem.My.Resources.Resources.ChatGPT_Image_Aug_13__2026__06_00_20_PM
        Me.PictureBox1.Location = New System.Drawing.Point(693, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(340, 321)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft YaHei UI", 60.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Label1.Location = New System.Drawing.Point(41, 248)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(958, 318)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Barangay Health Clinic" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Patient Record" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "System"
        '
        'usrnmLogtxtbx
        '
        Me.usrnmLogtxtbx.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.usrnmLogtxtbx.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.usrnmLogtxtbx.Dock = System.Windows.Forms.DockStyle.Fill
        Me.usrnmLogtxtbx.Font = New System.Drawing.Font("Microsoft YaHei UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.usrnmLogtxtbx.Location = New System.Drawing.Point(10, 15)
        Me.usrnmLogtxtbx.Multiline = True
        Me.usrnmLogtxtbx.Name = "usrnmLogtxtbx"
        Me.usrnmLogtxtbx.Size = New System.Drawing.Size(438, 38)
        Me.usrnmLogtxtbx.TabIndex = 2
        '
        'logBtn
        '
        Me.logBtn.BackColor = System.Drawing.Color.Gainsboro
        Me.logBtn.FlatAppearance.BorderSize = 0
        Me.logBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.logBtn.Font = New System.Drawing.Font("Microsoft YaHei UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.logBtn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.logBtn.Location = New System.Drawing.Point(172, 448)
        Me.logBtn.Name = "logBtn"
        Me.logBtn.Size = New System.Drawing.Size(178, 46)
        Me.logBtn.TabIndex = 3
        Me.logBtn.Text = "Log in"
        Me.logBtn.UseVisualStyleBackColor = False
        '
        'card
        '
        Me.card.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.card.Controls.Add(Me.Label4)
        Me.card.Controls.Add(Me.showPassCb)
        Me.card.Controls.Add(Me.Panel2)
        Me.card.Controls.Add(Me.passLogPnl)
        Me.card.Controls.Add(Me.userLogPnl)
        Me.card.Controls.Add(Me.Label2)
        Me.card.Controls.Add(Me.logBtn)
        Me.card.Location = New System.Drawing.Point(1227, 89)
        Me.card.Name = "card"
        Me.card.Size = New System.Drawing.Size(535, 791)
        Me.card.TabIndex = 4
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft YaHei UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Gainsboro
        Me.Label4.Location = New System.Drawing.Point(207, 539)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(111, 19)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Forget Password"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'showPassCb
        '
        Me.showPassCb.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.showPassCb.AutoSize = True
        Me.showPassCb.Font = New System.Drawing.Font("Microsoft YaHei UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.showPassCb.ForeColor = System.Drawing.Color.Gainsboro
        Me.showPassCb.Location = New System.Drawing.Point(63, 384)
        Me.showPassCb.Name = "showPassCb"
        Me.showPassCb.Size = New System.Drawing.Size(141, 24)
        Me.showPassCb.TabIndex = 8
        Me.showPassCb.Text = "Show Password"
        Me.showPassCb.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(65, Byte), Integer), CType(CType(174, Byte), Integer), CType(CType(88, Byte), Integer))
        Me.Panel2.Controls.Add(Me.TableLayoutPanel1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(535, 100)
        Me.Panel2.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label3.Font = New System.Drawing.Font("Microsoft YaHei UI", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Label3.Location = New System.Drawing.Point(181, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(172, 100)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "LOG IN"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'passLogPnl
        '
        Me.passLogPnl.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.passLogPnl.Controls.Add(Me.passLogTxbx)
        Me.passLogPnl.Location = New System.Drawing.Point(44, 309)
        Me.passLogPnl.Name = "passLogPnl"
        Me.passLogPnl.Padding = New System.Windows.Forms.Padding(10, 15, 0, 0)
        Me.passLogPnl.Size = New System.Drawing.Size(448, 53)
        Me.passLogPnl.TabIndex = 6
        '
        'passLogTxbx
        '
        Me.passLogTxbx.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.passLogTxbx.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.passLogTxbx.Dock = System.Windows.Forms.DockStyle.Fill
        Me.passLogTxbx.Font = New System.Drawing.Font("Microsoft YaHei UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.passLogTxbx.Location = New System.Drawing.Point(10, 15)
        Me.passLogTxbx.Multiline = True
        Me.passLogTxbx.Name = "passLogTxbx"
        Me.passLogTxbx.Size = New System.Drawing.Size(438, 38)
        Me.passLogTxbx.TabIndex = 2
        '
        'userLogPnl
        '
        Me.userLogPnl.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.userLogPnl.Controls.Add(Me.usrnmLogtxtbx)
        Me.userLogPnl.Location = New System.Drawing.Point(44, 225)
        Me.userLogPnl.Name = "userLogPnl"
        Me.userLogPnl.Padding = New System.Windows.Forms.Padding(10, 15, 0, 0)
        Me.userLogPnl.Size = New System.Drawing.Size(448, 53)
        Me.userLogPnl.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Gainsboro
        Me.Label2.Location = New System.Drawing.Point(208, 711)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(120, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "don't have an account?"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label3, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(535, 100)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'LogForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1904, 1041)
        Me.Controls.Add(Me.card)
        Me.Controls.Add(Me.Panel1)
        Me.MinimumSize = New System.Drawing.Size(1024, 600)
        Me.Name = "LogForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form2"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.card.ResumeLayout(False)
        Me.card.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.passLogPnl.ResumeLayout(False)
        Me.passLogPnl.PerformLayout()
        Me.userLogPnl.ResumeLayout(False)
        Me.userLogPnl.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents usrnmLogtxtbx As TextBox
    Friend WithEvents logBtn As Button
    Friend WithEvents card As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents userLogPnl As Panel
    Friend WithEvents passLogPnl As Panel
    Friend WithEvents passLogTxbx As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents showPassCb As CheckBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
