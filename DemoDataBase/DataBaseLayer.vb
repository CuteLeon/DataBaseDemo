Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Configuration
Imports System.Reflection

Public Class DataBaseLayer
    ''' <summary>
    ''' 数据库连接串
    ''' </summary>
    ''' <remarks></remarks>
    Private connectionString As String = String.Empty

    ''' <summary>
    ''' 数据库类型
    ''' </summary>
    ''' <remarks></remarks>
    Private dbType As String = String.Empty

    ''' <summary>
    ''' 数据库连接字符串(web.config来配置)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConntionString()
        Get
            Return connectionString
        End Get
        Set(ByVal value)
            connectionString = value
        End Set
    End Property

    ''' <summary>
    ''' 数据库类型
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property dbTypeString()
        Get
            Return DBType
        End Get
        Set(ByVal value)
            dbType = value
        End Set
    End Property

#Region "转换参数"
    Private Function iDbPara(ByVal ParaName As String, ByVal DataType As String) As IDbDataParameter
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return GetSqlPara(ParaName, DataType)
            Case "Access"
                Return GetOleDbPara(ParaName, DataType)
            Case Else
                Return GetSqlPara(ParaName, DataType)
        End Select
    End Function

    Private Function GetSqlPara(ByVal ParaName As String, ByVal DataType As String) As SqlParameter
        Select Case DataType
            Case "Decimal"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.Decimal)
            Case "Varchar"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.VarChar)
            Case "DateTime"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.DateTime)
            Case "Iamge"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.Image)
            Case "Int"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.Int)
            Case "Text"
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.NText)
            Case Else
                Return New SqlClient.SqlParameter(ParaName, SqlDbType.VarChar)
        End Select
    End Function

    Private Function GetOleDbPara(ByVal ParaName As String, ByVal DataType As String) As OleDbParameter
        Select Case DataType
            Case "Decimal"
                Return New OleDbParameter(ParaName, Data.DbType.Decimal)
            Case "Varchar"
                Return New OleDbParameter(ParaName, Data.DbType.String)
            Case "DateTime"
                Return New OleDbParameter(ParaName, Data.DbType.DateTime)
            Case "Iamge"
                Return New OleDbParameter(ParaName, Data.DbType.Binary)
            Case "Int"
                Return New OleDbParameter(ParaName, Data.DbType.Int32)
            Case "Text"
                Return New OleDbParameter(ParaName, Data.DbType.String)
            Case Else
                Return New OleDbParameter(ParaName, Data.DbType.String)
        End Select
    End Function

#End Region

#Region "创建 Connection 和 Command"
    ''' <summary>
    ''' 取得数据库连接对象
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetConnection() As IDbConnection
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlConnection(Me.ConntionString)
            Case "Access"
                Return New OleDbConnection(Me.ConntionString)
            Case Else
                Return New SqlConnection(Me.ConntionString)
        End Select
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Sql">检索条件</param>
    ''' <param name="iConn">数据库连接对象</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCommand(ByVal Sql As String, ByVal iConn As IDbConnection) As IDbCommand
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlCommand(Sql, CType(iConn, SqlConnection))
            Case "Access"
                Return New OleDbCommand(Sql, CType(iConn, OleDbConnection))
            Case Else
                Return New SqlCommand(Sql, CType(iConn, SqlConnection))
        End Select
    End Function

    ''' <summary>
    ''' 连接到数据源时执行命令
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCommand() As IDbCommand
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlCommand()
            Case "Access"
                Return New OleDbCommand()
            Case Else
                Return New SqlCommand()
        End Select
    End Function

    ''' <summary>
    ''' 连接到数据源时的适配器
    ''' </summary>
    ''' <param name="Sql">检索条件</param>
    ''' <param name="iConn">数据库连接对象</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAdapater(ByVal Sql As String, ByVal iConn As IDbConnection) As IDataAdapter
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlDataAdapter(Sql, CType(iConn, SqlConnection))
            Case "Access"
                Return New OleDbDataAdapter(Sql, CType(iConn, OleDbConnection))
            Case Else
                Return New SqlDataAdapter(Sql, CType(iConn, SqlConnection))
        End Select
    End Function

    ''' <summary>
    ''' 连接到数据源时的适配器
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAdapater() As IDataAdapter
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlDataAdapter()
            Case "Access"
                Return New OleDbDataAdapter()
            Case Else
                Return New SqlDataAdapter()
        End Select
    End Function

    ''' <summary>
    ''' 连接到数据源时的适配器
    ''' </summary>
    ''' <param name="iCmd">连接到数据源时的命令</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAdapater(ByVal iCmd As IDbCommand) As IDataAdapter
        Select Case Me.dbTypeString
            Case "SqlServer"
                Return New SqlDataAdapter(CType(iCmd, SqlCommand))
            Case "Access"
                Return New OleDbDataAdapter(CType(iCmd, OleDbCommand))
            Case Else
                Return New SqlDataAdapter(CType(iCmd, SqlCommand))
        End Select
    End Function
#End Region

#Region "执行简单SQL语句"
    ''' <summary>
    ''' 执行SQL语句，返回影响的记录数
    ''' </summary>
    ''' <param name="SqlString">SQL语句</param>
    ''' <returns>影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSql(ByVal SqlString As String) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As IDbCommand = GetCommand(SqlString, iConn)
                iConn.Open()
                Try
                    Dim rows As Integer = iCmd.ExecuteNonQuery()
                    Return rows
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryRows(ByVal sqlString As String) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            Dim ds As DataSet = New DataSet()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                iAdapter.Fill(ds)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
            Return Convert.ToUInt32(ds.Tables(0).Rows(0).Item(0))
        End Using
    End Function

    ''' <summary>
    ''' 执行多条SQL语句，实现数据库事务
    ''' </summary>
    ''' <param name="SQLStringList">多条SQL语句</param>
    ''' <remarks></remarks>
    Public Sub ExecuteSqlTran(ByVal SQLStringList As ArrayList)
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using iCmd As IDbCommand = GetCommand()
                iCmd.Connection = iConn
                Using iDbTran As IDbTransaction = iConn.BeginTransaction()
                    iCmd.Transaction = iDbTran
                    Try
                        Dim strsql As String = String.Empty
                        For n As Integer = 0 To SQLStringList.Count
                            strsql = SQLStringList(n).ToString
                            If strsql.Trim().Length > 1 Then
                                iCmd.CommandText = strsql
                                iCmd.ExecuteNonQuery()
                            End If
                        Next
                        iDbTran.Commit()
                    Catch ex As Exception
                        iDbTran.Rollback()
                        Throw New Exception(ex.Message)
                    Finally
                        If iConn.State <> ConnectionState.Closed Then
                            iConn.Close()
                        End If
                    End Try
                End Using
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 执行带一个存储过程参数的的SQL语句
    ''' </summary>
    ''' <param name="SqlString">SQL语句</param>
    ''' <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
    ''' <returns>影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSql(ByVal SqlString As String, ByVal content As String) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As IDbCommand = GetCommand(SqlString, iConn)
                Dim myParameter As IDataParameter = Me.iDbPara("@content", "Text")
                myParameter.Value = content
                iCmd.Parameters.Add(myParameter)
                iConn.Open()
                Try
                    Dim rows As Integer = iCmd.ExecuteNonQuery()
                    Return rows
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
    ''' </summary>
    ''' <param name="SqlString">SQL语句</param>
    ''' <param name="fs">图像字节,数据库的字段类型为image的情况</param>
    ''' <returns>影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSqlInsertImg(ByVal SqlString As String, ByVal fs As Byte()) As Integer

        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As IDbCommand = GetCommand(SqlString, iConn)
                Dim myParameter As IDataParameter = Me.iDbPara("@pic", "Iamge")
                myParameter.Value = fs
                iCmd.Parameters.Add(myParameter)
                iConn.Open()
                Try
                    Dim rows As Integer = iCmd.ExecuteNonQuery()
                    Return rows
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Function

    Public Function ExecuteSqlInsertPic(ByVal SqlString As String, ByVal fs As Byte()) As Integer

        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As SqlCommand = GetCommand(SqlString, iConn)
                iCmd.Parameters.Add("@pic", SqlDbType.Image).Value = fs
                iConn.Open()
                Try
                    Dim rows As Integer = iCmd.ExecuteNonQuery()
                    Return rows
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 执行一条计算查询结果语句，返回查询结果（object）
    ''' </summary>
    ''' <param name="SqlString">计算查询结果语句</param>
    ''' <returns>查询结果（object）</returns>
    ''' <remarks></remarks>
    Public Function GetSingle(ByVal SqlString As String) As Object
        Using iConn As IDbConnection = GetConnection()
            Using iCmd As IDbCommand = GetCommand(SqlString, iConn)
                iConn.Open()
                Try
                    Dim obj As Object = iCmd.ExecuteScalar()
                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        Return Nothing
                    Else
                        Return obj
                    End If
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Function

    ''' <summary>
    '''  执行查询语句，返回IDataAdapter
    ''' </summary>
    ''' <param name="strSQL">查询语句</param>
    ''' <returns>IDataAdapter</returns>
    ''' <remarks></remarks>
    Public Function ExecuteReader(ByVal strSQL As String) As IDataAdapter
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(strSQL, iConn)
                Return iAdapter
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句，返回DataSet
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal sqlString As String, ByVal tn As String) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            Dim ds As DataSet = New DataSet()
            iConn.Open()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                CType(iAdapter, SqlDataAdapter).Fill(ds, tn)
                Return ds
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句，返回DataSet
    ''' </summary>
    ''' <param name="sqlString">检索条件</param>
    ''' <param name="dataSet">数据集合</param>
    ''' <param name="startIndex">开始索引</param>
    ''' <param name="pageSize">页面记录大小</param>
    ''' <param name="tableName">表名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal sqlString As String, ByVal dataSet As DataSet, _
                          ByVal startIndex As Integer, ByVal pageSize As Integer, ByVal tableName As String) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                CType(iAdapter, OleDbDataAdapter).Fill(dataSet, startIndex, pageSize, tableName)
                Return dataSet
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句，向XML文件写入数据
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <param name="xmlPath">XML文件路径</param>
    ''' <remarks></remarks>
    Public Sub WriteToXml(ByVal sqlString As String, ByVal xmlPath As String)
        'Query(sqlString).WriteXml(xmlPath)
    End Sub

    ''' <summary>
    ''' 执行查询语句
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQuery(ByVal sqlString As String) As DataTable
        Using iConn As IDbConnection = Me.GetConnection()
            Dim ds As DataSet = New DataSet()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                iAdapter.Fill(ds)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
            Return ds.Tables(0)
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryDS(ByVal sqlString As String) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            Dim ds As DataSet = New DataSet()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                iAdapter.Fill(ds)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
            Return ds
        End Using
    End Function

    ''' <summary>
    ''' 执行SQL语句，返回影响的记录数
    ''' </summary>
    ''' <param name="SqlString">SQL语句</param>
    ''' <returns>影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSqlNum(ByVal SqlString As String) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            Dim ds As DataSet = New DataSet()
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(SqlString, iConn)
                iAdapter.Fill(ds)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try

            Return ds.Tables(0).Rows.Count
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句
    ''' </summary>
    ''' <param name="SqlString">查询语句</param>
    ''' <param name="Proc">存储过程名称</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQuery(ByVal SqlString As String, ByVal Proc As String) As DataTable
        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As IDbCommand = GetCommand(SqlString, iConn)
                iCmd.CommandType = CommandType.StoredProcedure
                Dim ds As DataSet = New DataSet()
                Try
                    Dim IDataAdapter As IDataAdapter = Me.GetAdapater(SqlString, iConn)
                    IDataAdapter.Fill(ds)
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
                Return ds.Tables(0)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句，返回DataView
    ''' </summary>
    ''' <param name="Sql">查过条件</param>
    ''' <returns>DataView</returns>
    ''' <remarks></remarks>DataView
    Public Function ExeceuteDataView(ByVal Sql As String) As DataView
        Using iConn As IDbConnection = Me.GetConnection()
            Using iCmd As IDbCommand = GetCommand(Sql, iConn)
                Dim ds As DataSet = New DataSet()
                Try
                    Dim IDataAdapter As IDataAdapter = Me.GetAdapater(Sql, iConn)
                    IDataAdapter.Fill(ds)
                    Return ds.Tables(0).DefaultView
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                Finally
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try

            End Using
        End Using
    End Function

#End Region

#Region "执行带参数的SQL语句"
    ''' <summary>
    ''' 执行SQL语句，返回影响的记录数
    ''' </summary>
    ''' <param name="SQLString">SQL语句</param>
    ''' <param name="iParms">参数</param>
    ''' <returns>影响的记录数</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSql(ByVal SQLString As String, ByVal iParms As IDataParameter()) As Integer

        Using iConn As IDbConnection = Me.GetConnection()
            Dim iCmd As IDbCommand = GetCommand()
            Try
                PrepareCommand(iCmd, iConn, Nothing, SQLString, iParms)
                Dim rows As Integer = iCmd.ExecuteNonQuery()
                iCmd.Parameters.Clear()
                Return rows
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                iCmd.Dispose()
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行多条SQL语句，实现数据库事务。
    ''' </summary>
    ''' <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
    ''' <remarks></remarks>
    Public Sub ExecuteSqlTran(ByVal SQLStringList As Hashtable)
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using iTrans As IDbTransaction = iConn.BeginTransaction()
                Dim iCmd As SqlCommand = GetCommand()
                Dim tmpsql As String = "insert into dbo.tb_Client(id,ClientName,ClientUnit,Tel,Address)values(@id,@ClientName,@ClientUnit,@Tel,@Address)"
                Dim tmp As String()
                Try
                    Dim cmdText As String = String.Empty
                    ''Dim iParms As IDataParameter
                    For Each myDE As DictionaryEntry In SQLStringList
                        cmdText = myDE.Key.ToString()
                        ' ''iParms = CType(myDE.Value, IDataParameter)
                        ' ''InitCommand(iCmd, iConn, iTrans, cmdText, iParms)
                        ' ''iCmd.ExecuteNonQuery()
                        ' ''iCmd.Parameters.Clear()
                        tmp = myDE.Value.ToString.Split(",")
                        With iCmd
                            .Connection = iConn
                            .CommandText = tmpsql
                            .Transaction = iTrans
                            .CommandType = CommandType.Text
                            .Parameters.Add("@id", SqlDbType.Char).Value = tmp(0)
                            .Parameters.Add("@ClientName", SqlDbType.VarChar).Value = tmp(1)
                            .Parameters.Add("@ClientUnit", SqlDbType.VarChar).Value = tmp(2)
                            .Parameters.Add("@Tel", SqlDbType.Char).Value = tmp(3)
                            .Parameters.Add("@Address", SqlDbType.VarChar).Value = tmp(4)
                            .ExecuteNonQuery()
                            .Parameters.Clear()

                        End With

                    Next

                    iTrans.Commit()
                Catch ex As Exception
                    iTrans.Rollback()
                    Throw
                Finally
                    iCmd.Dispose()
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
            End Using
        End Using
    End Sub

    Public Function ExecuteSqlTran(ByVal SQLStringList As String) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using iTrans As IDbTransaction = iConn.BeginTransaction()
                Dim iCmd As IDbCommand = GetCommand()
                Dim intRows As Integer = 0
                Try
                    With iCmd
                        .Connection = iConn
                        .CommandText = SQLStringList
                        .Transaction = iTrans
                        intRows = .ExecuteNonQuery()
                    End With
                    iTrans.Commit()
                Catch ex As Exception
                    iTrans.Rollback()
                    Throw
                Finally
                    iCmd.Dispose()
                    If iConn.State <> ConnectionState.Closed Then
                        iConn.Close()
                    End If
                End Try
                Return intRows
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 执行一条计算查询结果语句，返回查询结果（object）
    ''' </summary>
    ''' <param name="SQLString">计算查询结果语句</param>
    ''' <param name="iParms"></param>
    ''' <returns>查询结果（object）</returns>
    ''' <remarks></remarks>
    Public Function GetSingle(ByVal SQLString As String, ByVal iParms As IDataParameter()) As Object
        Using iConn As IDbConnection = Me.GetConnection()
            Dim iCmd As IDbCommand = GetCommand()
            Try
                PrepareCommand(iCmd, iConn, Nothing, SQLString, iParms)
                Dim obj As Object = iCmd.ExecuteScalar()
                iCmd.Parameters.Clear()
                If obj Is Nothing OrElse obj Is DBNull.Value Then
                    Return Nothing
                Else
                    Return obj
                End If
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                iCmd.Dispose()
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    '''  执行查询语句，返回IDataReader
    ''' </summary>
    ''' <param name="SQLString">查询语句</param>
    ''' <param name="iParms">参数</param>
    ''' <returns>IDataReader</returns>
    ''' <remarks></remarks>
    Public Function ExecuteReader(ByVal SQLString As String, ByVal iParms As IDataParameter()) As IDataReader
        Using iConn As IDbConnection = Me.GetConnection()
            Dim iCmd As IDbCommand = GetCommand()
            Try
                PrepareCommand(iCmd, iConn, Nothing, SQLString, iParms)
                Dim iReader As IDataReader = iCmd.ExecuteReader()
                iCmd.Parameters.Clear()
                Return iReader
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                iCmd.Dispose()
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行查询语句，返回DataSet
    ''' </summary>
    ''' <param name="sqlString">查询语句</param>
    ''' <param name="iParms">参数</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal sqlString As String, ByVal iParms As IDataParameter()) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            Dim iCmd As IDbCommand = GetCommand()
            PrepareCommand(iCmd, iConn, Nothing, sqlString, iParms)
            Try
                Dim iAdapter As IDataAdapter = Me.GetAdapater(sqlString, iConn)
                Dim ds As DataSet = New DataSet()
                iAdapter.Fill(ds)
                iCmd.Parameters.Clear()
                Return ds
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                iCmd.Dispose()
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 初始化Command
    ''' </summary>
    ''' <param name="iCmd"></param>
    ''' <param name="iConn">数据库连接</param>
    ''' <param name="iTrans"></param>
    ''' <param name="cmdText"></param>
    ''' <param name="iParms">参数</param>
    ''' <remarks></remarks>
    Private Sub PrepareCommand(ByRef iCmd As IDbCommand, ByVal iConn As IDbConnection, _
                               ByVal iTrans As IDbTransaction, ByVal cmdText As String, ByVal iParms As IDataParameter())
        If iConn.State <> ConnectionState.Open Then
            iConn.Open()
        End If
        iCmd = Me.GetCommand()
        iCmd.Connection = iConn
        iCmd.CommandText = cmdText
        If Not iTrans Is Nothing Then
            iCmd.Transaction = iTrans
        End If
        iCmd.CommandType = CommandType.Text
        If Not iParms Is Nothing Then
            For Each parm As IDataParameter In iParms
                iCmd.Parameters.Add(parm)
            Next
        End If
    End Sub

    ''' <summary>
    ''' 初始化Command
    ''' </summary>
    ''' <param name="iCmd"></param>
    ''' <param name="iConn">数据库连接</param>
    ''' <param name="iTrans"></param>
    ''' <param name="cmdText"></param>
    ''' <param name="iParms">参数</param>
    ''' <remarks></remarks>
    Private Sub InitCommand(ByRef iCmd As IDbCommand, ByVal iConn As IDbConnection, _
                            ByVal iTrans As IDbTransaction, ByVal cmdText As String, ByVal iParms As IDataParameter)
        If iConn.State <> ConnectionState.Open Then
            iConn.Open()
        End If
        iCmd = Me.GetCommand()
        iCmd.Connection = iConn
        iCmd.CommandText = cmdText
        If Not iTrans Is Nothing Then
            iCmd.Transaction = iTrans
        End If
        iCmd.CommandType = CommandType.Text
        If Not iParms Is Nothing Then
            iCmd.Parameters.Add(iParms)
        End If
    End Sub
#End Region

#Region "存储过程操作"
    ''' <summary>
    ''' 执行存储过程
    ''' </summary>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns>SqlDataReader</returns>
    ''' <remarks></remarks>
    Public Function RunProcedure(ByVal storedProcName As String, ByVal parameters As IDataParameter()) As SqlDataReader
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using sqlCmd As SqlCommand = BuildQueryCommand(iConn, storedProcName, parameters)
                Return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 执行存储过程
    ''' </summary>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <param name="tableName">DataSet结果中的表名</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Function RunProcedure(ByVal storedProcName As String, ByVal parameters As IDataParameter(), ByVal tableName As String) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            Try
                Dim DataSet As DataSet = New DataSet()
                iConn.Open()
                Dim iDA As IDataAdapter = Me.GetAdapater()
                iDA = Me.GetAdapater(BuildQueryCommand(iConn, storedProcName, parameters))
                CType(iDA, SqlDataAdapter).Fill(DataSet, tableName)
                Return DataSet
            Catch ex As Exception
                Return Nothing
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行存储过程
    ''' </summary>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <param name="startIndex">开始记录索引</param>
    ''' <param name="pageSize">页面记录大小</param>
    ''' <param name="tableName">DataSet结果中的表名</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Function RunProcedure(ByVal storedProcName As String, ByVal parameters As IDataParameter(), _
                                 ByVal startIndex As Integer, ByVal pageSize As Integer, ByVal tableName As String) As DataSet
        Using iConn As IDbConnection = Me.GetConnection()
            Try
                Dim dataSet As DataSet = New DataSet()
                Dim iDA As IDataAdapter = Me.GetAdapater()
                iDA = Me.GetAdapater(BuildQueryCommand(iConn, storedProcName, parameters))
                CType(iDA, SqlDataAdapter).Fill(dataSet, startIndex, pageSize, tableName)
                Return dataSet
            Catch ex As Exception
                Return Nothing
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行存储过程 填充已经存在的DataSet数据集
    ''' </summary>
    ''' <param name="storeProcName">存储过程名称</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <param name="dataSet">要填充的数据集</param>
    ''' <param name="tableName">要填充的表名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RunProcedure(ByVal storeProcName As String, ByVal parameters As IDataParameter(), ByRef dataSet As DataSet, ByVal tableName As String) As DataSet

        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Try
                Dim iDA As IDataAdapter = Me.GetAdapater()
                iDA = Me.GetAdapater(BuildQueryCommand(iConn, storeProcName, parameters))
                CType(iDA, SqlDataAdapter).Fill(dataSet, tableName)
                Return dataSet
            Catch ex As Exception
                Return Nothing
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
    End Function

    ''' <summary>
    ''' 执行存储过程并返回受影响的行数
    ''' </summary>
    ''' <param name="storedProcName">存储过程名称</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RunProcedureNoQuery(ByVal storedProcName As String, ByVal parameters As IDataParameter()) As Integer
        Dim intResult As Integer = 0
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using scmd As SqlCommand = BuildQueryCommand(iConn, storedProcName, parameters)
                intResult = scmd.ExecuteNonQuery()
            End Using
            If iConn.State <> ConnectionState.Closed Then
                iConn.Close()
            End If
        End Using
        Return intResult
    End Function

    Public Function RunProcedureNoQuery(ByVal storedProcName As String, ByVal parameters As IDataParameter) As Integer
        Dim intResult As Integer = 0
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Try
                Dim scmd As SqlCommand = BuildQueryCommand(iConn, storedProcName, parameters)
                scmd.ExecuteNonQuery()
                intResult = 1
            Catch ex As Exception
                intResult = 0
            Finally
                If iConn.State <> ConnectionState.Closed Then
                    iConn.Close()
                End If
            End Try
        End Using
        Return intResult
    End Function

    ''' <summary>
    ''' 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
    ''' </summary>
    ''' <param name="iConn">数据库连接</param>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns>SqlCommand</returns>
    ''' <remarks></remarks>
    Private Function BuildQueryCommand(ByVal iConn As IDbConnection, _
                                       ByVal storedProcName As String, ByVal parameters As IDataParameter) As SqlCommand

        Dim iCmd As IDbCommand = GetCommand(storedProcName, iConn)
        iCmd.CommandType = CommandType.StoredProcedure
        iCmd.Parameters.Add(parameters)

        Return iCmd
    End Function

    ''' <summary>
    ''' 执行存储过程并返回受影响的行数
    ''' </summary>
    ''' <param name="storeProcName">存储过程名称</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RunProcedureExecuteScalar(ByVal storeProcName As String, ByVal parameters As IDataParameter()) As String
        Dim strResult As String = String.Empty
        Using iConn As IDbConnection = Me.GetConnection()
            iConn.Open()
            Using scmd As SqlCommand = BuildQueryCommand(iConn, storeProcName, parameters)
                Dim obj As Object = scmd.ExecuteScalar()
                If obj Is Nothing Then
                    strResult = String.Empty
                Else
                    strResult = obj.ToString
                End If
            End Using
        End Using
        Return strResult
    End Function

    ''' <summary>
    ''' 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
    ''' </summary>
    ''' <param name="iConn">数据库连接</param>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns>SqlCommand</returns>
    ''' <remarks></remarks>
    Private Function BuildQueryCommand(ByVal iConn As IDbConnection, _
                                       ByVal storedProcName As String, ByVal parameters As IDataParameter()) As SqlCommand

        Dim iCmd As IDbCommand = GetCommand(storedProcName, iConn)
        iCmd.CommandType = CommandType.StoredProcedure
        If parameters Is Nothing Then
            Return iCmd
        End If
        For Each parameter As IDataParameter In parameters
            iCmd.Parameters.Add(parameter)
        Next
        Return iCmd
    End Function

    ''' <summary>
    ''' 执行存储过程，返回影响的行数
    ''' </summary>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <param name="rowsAffected">影响的行数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RunProcedure(ByVal storedProcName As String, _
                                 ByVal parameters As IDataParameter(), ByRef rowsAffected As Integer) As Integer
        Using iConn As IDbConnection = Me.GetConnection()
            Dim intresult As Integer = 0
            iConn.Open()
            Using sqlCmd As SqlCommand = BuildIntCommand(iConn, storedProcName, parameters)
                rowsAffected = sqlCmd.ExecuteNonQuery()
                intresult = Convert.ToInt32(sqlCmd.Parameters("ReturnValue").Value)
            End Using
            If iConn.State <> ConnectionState.Closed Then
                iConn.Close()
            End If
            Return intresult
        End Using
    End Function

    ''' <summary>
    ''' 创建 SqlCommand 对象实例(用来返回一个整数值)
    ''' </summary>
    ''' <param name="iConn">数据库连接</param>
    ''' <param name="storedProcName">存储过程名</param>
    ''' <param name="parameters">存储过程参数</param>
    ''' <returns>SqlCommand 对象实例</returns>
    ''' <remarks></remarks>
    Private Function BuildIntCommand(ByVal iConn As IDbConnection, ByVal storedProcName As String, ByVal parameters As IDataParameter()) As SqlCommand
        Dim sqlCmd As SqlCommand = BuildQueryCommand(iConn, storedProcName, parameters)
        sqlCmd.Parameters.Add(New SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, False, 0, 0, String.Empty, DataRowVersion.Default, Nothing))
        Return sqlCmd
    End Function
#End Region
End Class
