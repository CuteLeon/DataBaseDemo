Imports System.Text
Imports System.IO
'Download by http://www.codefans.net
Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim dbl As New DataBaseLayer
        Dim Parameter() As SqlClient.SqlParameter = {New SqlClient.SqlParameter("SQLWhere", SqlDbType.VarChar, _
                                                                 50, ParameterDirection.Input, False, 0, 0, String.Empty, _
                                                                  DataRowVersion.Default, " ")}

        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            'Me.DataGridView1.DataSource = .Query("select * from dbo.tb_year01").Tables(0)

            Me.DataGridView1.DataSource = .RunProcedure("SelectClothes", Parameter, "TEST").Tables("TEST")
            Me.DataGridView1.Refresh()
        End With


    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim startTicks As Integer = My.Computer.Clock.TickCount
        
        
        Dim dbl As New DataBaseLayer
        Dim strSQL As New StringBuilder
        Dim ht As New Hashtable
        Dim Parameter As SqlClient.SqlParameter = New SqlClient.SqlParameter("@IsUpdate", SqlDbType.Char, _
                                        1, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, "0")
        For i As Integer = 12042 To 22042

            With strSQL
                .Append("insert into dbo.tb_Client(id,ClientName,ClientUnit,Tel,Address)values('100000000000000000000000000")
                .Append(i.ToString.PadLeft(5, "0"))
                .Append("','李")
                .Append(i.ToString)
                .Append("','北京")
                .Append(i.ToString)
                .Append("','10")
                .Append(i.ToString)
                .Append("','asaaaa')")
                .Append(vbCrLf)
            End With
        Next
        Dim ws As New ZhuBrothersService.Service
        If ws.WebExcuteSQL("delete from tb_Client") Then
            MsgBox("ok")
        End If


        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            If .ExecuteSqlTran(strSQL.ToString) > 0 Then
                MsgBox("ok")
            End If
        End With
        Dim endTicks As Integer = My.Computer.Clock.TickCount
        MsgBox(endTicks - startTicks)
    End Sub

    ''' <summary>
    ''' 自动序号
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DataGridView1_CellPainting(ByVal sender As Object, _
                                         ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) _
                                         Handles DataGridView1.CellPainting
        If e.ColumnIndex < 0 And e.RowIndex >= 0 Then
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
            Dim indexRect As Rectangle = e.CellBounds
            indexRect.Inflate(-2, -2)
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), e.CellStyle.Font, indexRect, _
                        e.CellStyle.ForeColor, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)
            e.Handled = True
        End If
    End Sub


    

    Public Function WebExcuteQueryUpdateInfo() As DataSet
        Try
            Dim dbl As New DataBaseLayer
            Dim ds As DataSet = New DataSet()
            Dim dt As DataTable
            dbl.ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            dbl.dbTypeString = "SqlServer"
            dt = dbl.ExecuteQuery("SELECT name FROM sysobjects WHERE type = 'U'")
            Dim strSQL As StringBuilder
            Dim tmpdt As DataTable
            Dim strTableName As String = String.Empty
            For Each dr As DataRow In dt.Rows
                strTableName = dr.Item("name").ToString
                If Not strTableName.Contains("tb_Log") Then
                    strSQL = New StringBuilder
                    tmpdt = New DataTable
                    strSQL.Append("SELECT * FROM ")
                    strSQL.Append(strTableName)
                    strSQL.Append(" WHERE IsUpdate = '1'")
                    tmpdt = dbl.ExecuteQuery(strSQL.ToString)
                    If tmpdt.Rows.Count > 0 Then
                        tmpdt.TableName = strTableName
                        ds.Tables.Add(tmpdt.Copy)
                        tmpdt.Dispose()
                    End If
                End If
            Next
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim ds As DataSet = WebExcuteQueryUpdateInfo()
        'WebServiceExcute(ds)
        Me.DataGridView1.DataSource = ds.Tables(0)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim dbl As New DataBaseLayer
        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            Me.Label1.Text = .ExecuteSqlNum("select count(id) from  tb_year03").ToString
        End With
    End Sub

    Public Function WebServiceExcute(ByVal DataInfo As DataSet) As Boolean
        Try
            If DataInfo Is Nothing Then Return False
            Dim dbl As New DataBaseLayer
            Dim webResult As Boolean = False
            Dim ht As New Hashtable
            Dim strSQL As StringBuilder = New StringBuilder
            With dbl
                .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
                .dbTypeString = "SqlServer"
            End With
            For Each dt As DataTable In DataInfo.Tables
                If dt.TableName.Equals("tb_year02") Then
                    If dt.TableName.Contains("tb_year") Then
                        For Each dr As DataRow In dt.Rows
                            GetSQLInfoWithImage(dr, dt, ht, dbl)
                        Next
                    Else
                        For Each dr As DataRow In dt.Rows
                            GetSQLInfo(dr, dt, ht, dbl)
                        Next
                    End If
                End If
            Next
            dbl.ExecuteSqlTran(ht)
            Return webResult
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' 带图像表
    ''' </summary>
    ''' <param name="dr">行对象</param>
    ''' <param name="dt">表对象</param>
    ''' <param name="ht"></param>
    ''' <remarks></remarks>
    Private Sub GetSQLInfoWithImage(ByVal dr As DataRow, ByVal dt As DataTable, ByRef ht As Hashtable, ByRef dbl As DataBaseLayer)
        Dim strSQL As StringBuilder = New StringBuilder
        Dim dtInfo As DataTable = Nothing
        Dim Parameter As SqlClient.SqlParameter
        Dim ImageInfo As Byte() = New Byte() {}
        With strSQL
            .Append("SELECT id,ts FROM ")
            .Append(dt.TableName.Substring(0, dt.TableName.Length - 1) & "1")
            .Append(" WHERE id = '")
            .Append(dr.Item("id").ToString)
            .Append("'")
        End With
        dtInfo = dbl.ExecuteQuery(strSQL.ToString)
        If dtInfo.Rows.Count > 0 Then
            If dtInfo.Rows(0).Item("ts").ToString <= dr.Item("ts").ToString Then
                strSQL = New StringBuilder
                With strSQL
                    .Append("UPDATE ")
                    .Append(dt.TableName.Substring(0, dt.TableName.Length - 1) & "1")
                    .Append(" SET ")
                    For Each col As DataColumn In dt.Columns
                        .Append(col.ColumnName)
                        If col.DataType.Name.Equals("Byte[]") Then
                            .Append(" = @")
                            .Append(col.ColumnName)
                            .Append(",")
                            ImageInfo = CType(dr.Item(col.ColumnName), Byte())
                        ElseIf col.DataType.Name.Equals("Int32") OrElse _
                                 col.DataType.Name.Equals("Decimal") Then
                            .Append(" = ")
                            .Append(dr.Item(col.ColumnName).ToString)
                            .Append(",")
                        Else
                            .Append(" = '")
                            .Append(dr.Item(col.ColumnName).ToString.Trim)
                            .Append("',")
                        End If
                    Next
                    .Append("WHERE id = '")
                    .Append(dr.Item("id").ToString)
                    .Append("'")
                    .Append(vbCrLf)
                End With
                Parameter = New SqlClient.SqlParameter("@pic", SqlDbType.Image, _
                                                    50, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, ImageInfo)
                ht.Add(strSQL.ToString.Replace(",WHERE", " WHERE"), Parameter)
            End If
        Else
            strSQL = New StringBuilder
            With strSQL
                .Append("INSERT INTO ")
                .Append(dt.TableName.Substring(0, dt.TableName.Length - 1) & "1")
                .Append("(")
                For Each col As DataColumn In dt.Columns
                    .Append(col.ColumnName)
                    .Append(",")
                Next
                .Append(") VALUES (")
                For Each col As DataColumn In dt.Columns
                    If col.DataType.Name.Equals("Byte[]") Then
                        .Append("@")
                        .Append(col.ColumnName)
                        .Append(",")
                        ImageInfo = CType(dr.Item(col.ColumnName), Byte())
                    ElseIf col.DataType.Name.Equals("Int32") OrElse _
                                col.DataType.Name.Equals("Decimal") Then
                        strSQL.Append(dr.Item(col.ColumnName).ToString)
                        strSQL.Append(",")
                    Else
                        .Append("'")
                        .Append(dr.Item(col.ColumnName).ToString.Trim)
                        .Append("',")
                    End If
                Next
                .Append(")")
                .Append(vbCrLf)
            End With
            Parameter = New SqlClient.SqlParameter("@pic", SqlDbType.Image, _
                                        50, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, ImageInfo)
            ht.Add(strSQL.ToString.Replace(",)", ")"), Parameter)
        End If
    End Sub

    ''' <summary>
    ''' 带图像表
    ''' </summary>
    ''' <param name="dr">行对象</param>
    ''' <param name="dt">表对象</param>
    ''' <param name="ht"></param>
    ''' <param name="dbl"></param>
    ''' <remarks></remarks>
    Private Sub GetSQLInfo(ByVal dr As DataRow, ByVal dt As DataTable, ByRef ht As Hashtable, ByRef dbl As DataBaseLayer)
        Dim strSQL As StringBuilder = New StringBuilder
        Dim dtInfo As DataTable = Nothing
        Dim Parameter As SqlClient.SqlParameter
        With strSQL
            .Append("SELECT id,ts FROM ")
            .Append(dt.TableName)
            .Append(" WHERE id = '")
            .Append(dr.Item("id").ToString)
            .Append("'")
        End With
        dtInfo = dbl.ExecuteQuery(strSQL.ToString)
        If dtInfo.Rows.Count > 0 Then
            If dtInfo.Rows(0).Item("ts").ToString < dr.Item("ts").ToString Then
                strSQL = New StringBuilder
                With strSQL
                    .Append("UPDATE ")
                    .Append(dt.TableName)
                    .Append(" SET ")
                    For Each col As DataColumn In dt.Columns
                        .Append(col.ColumnName)
                        If col.DataType.Name.Equals("Int32") OrElse col.DataType.Name.Equals("Decimal") Then
                            .Append(" = ")
                            .Append(dr.Item(col.ColumnName).ToString)
                            .Append(",")
                        Else
                            .Append(" = '")
                            .Append(dr.Item(col.ColumnName).ToString.Trim)
                            .Append("',")
                        End If
                    Next
                    .Append("WHERE id = '")
                    .Append(dr.Item("id").ToString)
                    .Append("'")
                    .Append(vbCrLf)
                End With
                Parameter = New SqlClient.SqlParameter("@ts", SqlDbType.Char, _
                                        50, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, dr.Item("ts").ToString)
                ht.Add(strSQL.ToString.Replace(",WHERE", " WHERE"), Parameter)
            End If
        Else
            strSQL = New StringBuilder
            With strSQL
                .Append("INSERT INTO ")
                .Append(dt.TableName.Substring(0, dt.TableName.Length - 1) & "1")
                .Append("(")
                For Each col As DataColumn In dt.Columns
                    .Append(col.ColumnName)
                    .Append(",")
                Next
                .Append(") VALUES (")
                For Each col As DataColumn In dt.Columns
                    If col.DataType.Name.Equals("Int32") OrElse col.DataType.Name.Equals("Decimal") Then
                        strSQL.Append(dr.Item(col.ColumnName).ToString)
                        strSQL.Append(",")
                    Else
                        .Append("'")
                        .Append(dr.Item(col.ColumnName).ToString.Trim)
                        .Append("',")
                    End If
                Next
                .Append(")")
                .Append(vbCrLf)
            End With
            Parameter = New SqlClient.SqlParameter("@ts", SqlDbType.Char, _
                                        50, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, dr.Item("ts").ToString)
            ht.Add(strSQL.ToString.Replace(",)", ")"), Parameter)
        End If
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click


        Dim startTicks As Integer = My.Computer.Clock.TickCount
        Dim ws As New ZhuBrothersService.Service
        Dim dbl As New DataBaseLayer
        dbl.ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
        dbl.dbTypeString = "SqlServer"
        Dim ds As DataSet = dbl.Query("select * from tb_Client", "tb_Client")
        Me.DataGridView1.DataSource = ds.Tables(0)
        If ws.WebServiceExcuteTable(ds) = True Then
            MsgBox("成功！")
        Else
            MsgBox("失败！")
        End If
        Dim endTicks As Integer = My.Computer.Clock.TickCount
        MsgBox(endTicks - startTicks)
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim ws As New ZhuBrothersService.Service
        Dim dt As DataSet = ws.WebExcuteQueryUpdateInfo
        If dt.Tables.Count < 1 Then Return
        Me.DataGridView1.DataSource = dt.Tables(1)
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim dbl As New DataBaseLayer
        dbl.ConntionString = "Data Source=.;Initial Catalog=SulfuratedManage;Persist Security Info=True;User ID=sa;Password=whadmin"
        dbl.dbTypeString = "SqlServer"

        Dim Parameter As SqlClient.SqlParameter = New SqlClient.SqlParameter("@TableName", SqlDbType.Char, _
                                       50, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, "AddTableTestOK")
        If dbl.RunProcedureNoQuery("CreateTable", Parameter) > 0 Then
            MsgBox("ok")
        End If
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Dim dbl As New DataBaseLayer
        dbl.ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
        dbl.dbTypeString = "SqlServer"
        Dim inttmp As Integer = dbl.ExecuteQueryRows("SELECT count(name) FROM sysobjects WHERE type = 'U' AND name ='tb_year1110'")
        MsgBox(inttmp.ToString)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim dbl As New DataBaseLayer
        dbl.ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
        dbl.dbTypeString = "SqlServer"
        Dim dt As DataTable = dbl.ExecuteQuery("SELECT name FROM sysobjects WHERE type = 'U' ORDER BY name")
        For Each dr As DataRow In dt.Rows
            Me.ComboBox1.Items.Add(dr.Item("name").ToString)
        Next
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Dim ws As New ZhuBrothersService.Service
        MsgBox(ws.WebServiceExcuteTableClear("del"))
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Dim ws As New ZhuBrothersService.Service
        MsgBox(ws.WebServiceExcuteTableDelete("tb_year01"))
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Dim byteFile As Byte() = SetFileToByte(Me.TextBox1.Text)
        Dim dbl As New DataBaseLayer
        Dim Parameter() As SqlClient.SqlParameter = {New SqlClient.SqlParameter("eonfile", SqlDbType.Binary, _
                                                                 byteFile.Length, ParameterDirection.Input, False, 0, 0, String.Empty, _
                                                                  DataRowVersion.Default, byteFile)}

        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=SulfuratedManage;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            .ExecuteSql("insert into test(eonfile)values(@eonfile)", Parameter)
        End With
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Dim fileOpen As OpenFileDialog = New OpenFileDialog()
        'fileOpen.Filter = "文本文件 (*.edz)|*.edz"
        If fileOpen.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Me.TextBox1.Text = fileOpen.FileName
        End If
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Dim dbl As New DataBaseLayer

        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            Dim dt As DataTable = .ExecuteQuery("select * from tb_year19")

            SetBytesToFile(dt.Rows(0).Item("pic"), "D:\test.JPG")

        End With

    End Sub


    ''' <summary>
    ''' 保存文件到本地
    ''' </summary>
    ''' <param name="data">二进制文件</param>
    ''' <param name="filePath">文件路径</param>
    ''' <returns>保存是否成功</returns>
    ''' <remarks></remarks>
    Public Function SetBytesToFile(ByVal data As Byte(), ByVal filePath As String) As Boolean
        Try
            Dim fs As FileStream = New FileStream(filePath, FileMode.CreateNew)
            Dim BW As New IO.BinaryWriter(fs)
            BW.Write(data)
            BW.Close()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' 将文件转成二进制
    ''' </summary>
    ''' <param name="fileName">文件路径</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetFileToByte(ByVal fileName As String) As Byte()
        Dim fs As FileStream = New FileStream(fileName, FileMode.Open, FileAccess.Read)
        Dim streamLength As Integer = fs.Length
        Dim data(streamLength) As Byte
        Dim BR As New IO.BinaryReader(fs)
        data = BR.ReadBytes(fs.Length)
        BR.Close()
        'fs.Read(data, 0, streamLength)
        fs.Close()
        Return data
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim startTicks As Integer = My.Computer.Clock.TickCount
        Dim ws As New ZhuBrothersService.Service
        Dim ds As DataSet = ws.WebExcuteQuery("select  * from " + Me.ComboBox1.Text)
        If ds.Tables.Count < 1 Then Return
        Me.DataGridView1.DataSource = ds.Tables(0)
        Dim endTicks As Integer = My.Computer.Clock.TickCount
        MsgBox(endTicks - startTicks)
        ws.Dispose()
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        Dim dbl As New DataBaseLayer
        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            .dbTypeString = "SqlServer"
            Dim startTicks As Integer = My.Computer.Clock.TickCount
            Dim dt As DataTable = .ExecuteQuery("select * from " + Me.ComboBox1.Text)
            Me.DataGridView2.DataSource = dt
            Dim endTicks As Integer = My.Computer.Clock.TickCount
            MsgBox(endTicks - startTicks)
        End With
    End Sub

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        Dim dbl As New DataBaseLayer
        dbl.ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
        dbl.dbTypeString = "SqlServer"
        Dim ds As New DataSet
        Dim dt As DataTable = dbl.ExecuteQuery("select * from tb_baseInfo").Copy
        dt.TableName = "baseInfo"
        Dim strSQL As New StringBuilder
        For Each dr As DataRow In dt.Rows
            'strSQL.Append()
        Next
        With dbl
            .dbTypeString = "SqlServer"
            If .ExecuteSqlTran(strSQL.ToString) > 0 Then
                MsgBox("ok")
            End If
        End With
    End Sub

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        Dim startTicks As Integer = My.Computer.Clock.TickCount
        Dim ws As New ZhuBrothersService.Service
        Dim dbl As New DataBaseLayer
        Dim strSQL As New StringBuilder
        Dim ht As Hashtable = New Hashtable
        Dim tmp As String = "insert into dbo.tb_Client(id,ClientName,ClientUnit,Tel,Address)values(@id,@ClientName,@ClientUnit,@Tel,@Address)"
        For i As Integer = 10000 To 10100
            strSQL = New StringBuilder
            With strSQL
                .Append("insert into dbo.tb_Client(id,ClientName,ClientUnit,Tel,Address)values('100000000000000000000000000")
                .Append(i.ToString.PadLeft(5, "0"))
                .Append("','李")
                .Append(i.ToString)
                .Append("','北京")
                .Append(i.ToString)
                .Append("','10")
                .Append(i.ToString)
                .Append("','asaaaa')")
                .Append(vbCrLf)
                'ht.Add(i, "100000000000000000000000000" + i.ToString.PadLeft(5, "0") + ",李" + i.ToString + ",北京,1,li")
                ws.WebExcuteSQL(strSQL.ToString)
            End With
        Next

        ' ''With dbl
        ' ''    .dbTypeString = "SqlServer"
        ' ''    .ExecuteSqlTran(ht)
        ' ''End With

        Dim endTicks As Integer = My.Computer.Clock.TickCount
        MsgBox(endTicks - startTicks)
    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        Dim ws As New ZhuBrothersService.Service
        If ws.WebExcuteSQL("delete from " + Me.ComboBox1.Text) Then
            MsgBox("ok")
        End If
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        Dim ws As New ZhuBrothersService.Service
        Dim sql As StringBuilder
        Dim data As Byte() = SetFileToByte("D:\ico.JPG")
        Dim Parameter As SqlClient.SqlParameter = New SqlClient.SqlParameter("@pic", SqlDbType.Image, _
                                                data.Length, ParameterDirection.Input, False, 0, 0, String.Empty, DataRowVersion.Default, data)
        For i As Integer = 1 To 1000
            sql = New StringBuilder
            With sql
                .Append("INSERT INTO [dbo].[tb_year01]([id] ,[BarCode],[NationalName],[ClothesYears],[ProfessionType],[ProfessionName],[ClothesName]")
                .Append(",[Sex],[Season],[Craft],[MaterialType],[ClothesColor],[pic],[Amount],[StockNum],[DepotID],[ImagePath],[useFrequency],[rent],[costPrice]")
                .Append(",[Operator],[ExcutesDate],[ts],[df],[remark],[IsUpdate],[IsHide])VALUES('")
                .Append("1000000000000000000000000000")
                .Append(i.ToString.PadLeft(4, "0"))
                .Append("','010701042222090780001','中国','明朝','宫廷','公主','斗篷','女','夹层','毛边','纱','白色',@pic,1,1,")
                .Append("'10000000000000000000000000000001','',0,0.00,0.00,'管理员','2011-11-11','2011-11-11 20:48:31','0','")
                .Append(Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Append("','0','0')")
            End With
            ''Dim startTicks As Integer = My.Computer.Clock.TickCount
            'If ws.WebExecuteSQLImage(sql.ToString, data) Then
            ''Dim endTicks As Integer = My.Computer.Clock.TickCount
            ''MsgBox(endTicks - startTicks)
            'End If
        Next
        MsgBox("ok")

        ''Dim dbl As New DataBaseLayer
        ''With dbl
        ''    .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
        ''    .dbTypeString = "SqlServer"
        ''    .ExecuteSqlInsertPic(sql.ToString, data)
        ''End With

        ''Dim endTicks As Integer = My.Computer.Clock.TickCount
        ''MsgBox(endTicks - startTicks)
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Me.PictureBox1.Image = SetByteToImage(Me.DataGridView1.Rows(0).Cells(12).Value)
    End Sub

    Public Function SetByteToImage(ByVal mybyte As Byte()) As Image
        Dim Image As Image
        Dim mymemorystream As MemoryStream = New MemoryStream(mybyte, 0, mybyte.Length)
        Try
            Image = Drawing.Image.FromStream(mymemorystream)
            Return Image
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub Button21_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click
        Dim dbl As New DataBaseLayer
        Dim ds As New DataSet
        Dim dt As DataTable
        With dbl
            .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            dt = .ExecuteQuery("select top 20 * from tb_year03").Copy
        End With
        dt.TableName = "tb_year01"
        ds.Tables.Add(dt)
        Dim strLog As String = String.Empty
        Dim startTicks As Integer = My.Computer.Clock.TickCount
        Me.Label1.Text = startTicks.ToString
        Application.DoEvents()
        Dim ws As New ZhuBrothersService.Service
        If ws.WebExecuteSQLImage(ds) Then
            Dim endTicks As Integer = My.Computer.Clock.TickCount
            MsgBox(endTicks - startTicks)
        Else
            MsgBox("no")
        End If
        ws.Dispose()

    End Sub

    Private Sub ex()

    End Sub

    Public Function WebExecuteSQLImage(ByVal DataInfo As DataSet) As Boolean
        Try
            Dim dbl As New DataBaseLayer
            With dbl
                .ConntionString = "Data Source=.;Initial Catalog=ClothesManager;Persist Security Info=True;User ID=sa;Password=whadmin"
            End With
            Dim dt As DataTable = DataInfo.Tables(0)
            Dim strSQL As StringBuilder
            Dim ImageInfo As Byte() = Nothing
            For Each dr As DataRow In dt.Rows
                strSQL = New StringBuilder
                With strSQL
                    .Append("INSERT INTO ")
                    .Append(dt.TableName)
                    .Append("(")
                    For Each col As DataColumn In dt.Columns
                        .Append(col.ColumnName)
                        .Append(",")
                    Next
                    .Append(") VALUES (")
                    For Each col As DataColumn In dt.Columns
                        If col.DataType.Name.Equals("Byte[]") Then
                            .Append("@")
                            .Append(col.ColumnName)
                            .Append(",")
                            If Not String.IsNullOrEmpty(dr.Item(col.ColumnName).ToString) Then
                                ImageInfo = CType(dr.Item(col.ColumnName), Byte())
                            End If
                        ElseIf col.DataType.Name.Equals("Int32") OrElse _
                                 col.DataType.Name.Equals("Decimal") Then
                            .Append(dr.Item(col.ColumnName).ToString)
                            .Append(",")
                        Else
                            .Append("'")
                            .Append(dr.Item(col.ColumnName).ToString.Trim)
                            .Append("',")
                        End If
                    Next
                    .Append(")")
                End With
                dbl.ExecuteSqlInsertImg(strSQL.ToString.Replace(",)", ")"), ImageInfo)
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
