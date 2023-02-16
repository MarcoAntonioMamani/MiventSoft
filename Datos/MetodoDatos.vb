Imports System.Data
Imports System.Data.SqlClient   'Lib para conexion con SQL Server
'Imports System.Data.OleDb       'Lib para conexion con Access
Public Class MetodoDatos
    Public Shared Function CrearComando(Optional Ip As String = "", Optional UsuarioSql As String = "", Optional ClaveSql As String = "", Optional NombreBD As String = "") As SqlCommand
        Dim _cadenaConexion = Configuracion.CadenaConexion(Ip, UsuarioSql, ClaveSql, NombreBD)
        Dim _conexion As New SqlConnection() 'SqlConnection()
        _conexion.ConnectionString = _cadenaConexion
        Dim _comando As New SqlCommand("SET ARITHABORT ON") 'SqlCommand()
        _comando = _conexion.CreateCommand()
        _comando.CommandText = "SET ARITHABORT ON"
        _comando.CommandType = CommandType.Text
        'abrir
        _comando.Connection.Open()
        Return _comando
    End Function
    Public Shared Function CrearComandoProcedimiento(Optional Ip As String = "", Optional UsuarioSql As String = "", Optional ClaveSql As String = "", Optional NombreBD As String = "") As SqlCommand
        Dim _cadenaConexion = Configuracion.CadenaConexion(Ip, UsuarioSql, ClaveSql, NombreBD)
        Dim _conexion As New SqlConnection() 'SqlConnection()
        _conexion.ConnectionString = _cadenaConexion
        Dim _comando As New SqlCommand("SET ARITHABORT ON") 'SqlCommand()
        _comando = _conexion.CreateCommand()
        _comando.CommandType = CommandType.StoredProcedure
        _comando.CommandText = "SET ARITHABORT ON"
        'abrir
        _comando.Connection.Open()
        Return _comando
    End Function

    Public Shared Function EjecutarComandoSelect(Comando As SqlCommand) As DataTable
        Dim _tabla As New DataTable()
        Try
            If (Comando.Connection.State = 0) Then
                Comando.Connection.Open()
            End If



            Dim _adaptador As New SqlDataAdapter 'SqlDataAdapter()
            _adaptador.SelectCommand = Comando
            Dim read As SqlClient.SqlDataReader = Comando.ExecuteReader


            'While read.Read()
            '    Dim i As Integer = 0
            '    'Tu manejo de los datos
            'End While


            _tabla.Load(read, LoadOption.OverwriteChanges)

            Comando.CommandText = "DBCC DROPCLEANBUFFERS
DBCC FREEPROCCACHE"

            _adaptador.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally

            Comando.Dispose()
            Comando.Connection.Close()
        End Try
        Return _tabla
    End Function

    Public Shared Function EjecutarInsert(Comando As SqlCommand) As Boolean
        Dim _tabla As New DataTable()
        Dim _Err As Boolean = False

        Try
            If (Comando.Connection.State = 0) Then
                Comando.Connection.Open()
            End If
            Comando.ExecuteNonQuery()
        Catch ex As Exception
            MsgBox(ex.Message)
            _Err = True
        Finally
            Comando.Dispose()
            Comando.Connection.Close()
        End Try
        Return _Err
    End Function

    Public Shared Function EjecutarProcedimiento(Comando As SqlCommand) As DataTable
        Dim _tabla As New DataTable()
        Try
            If (Comando.Connection.State = 0) Then
                Comando.Connection.Open()
            End If
            Dim _adaptador As New SqlDataAdapter 'SqlDataAdapter()
            'Comando.CommandText = "SET ARITHABORT ON"
            _adaptador.SelectCommand = Comando
            'Comando.CommandType = CommandType.StoredProcedure

            _adaptador.Fill(_tabla)
        Catch ex As Exception
            MsgBox(ex.Message)
            'Finally
            '    Comando.Connection.Close()
            Comando.Dispose()
            Comando.Connection.Close()
        End Try
        Comando.Dispose()
        Comando.Connection.Close()
        Return _tabla
    End Function

End Class