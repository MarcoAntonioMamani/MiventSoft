﻿Imports Negocio.AccesoLogica
Imports System.IO
Public Class FPruebaImportacion
    Private Sub FPruebaImportacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        L_prAbrirConexion("DESKTOP-SB2Q2F5\SQLSERVER2017", "sa", "123", "MinventSoftKailiIndustrial")
    End Sub

    Public Shared Function ExcelToDatatable(ByVal _xlPath As String, ByVal _namePage As String) As System.Data.DataTable
        Dim xlApp As Microsoft.Office.Interop.Excel.Application = New Microsoft.Office.Interop.Excel.Application()
        Dim xlBook As Microsoft.Office.Interop.Excel.Workbook = xlApp.Workbooks.Open(_xlPath)
        Dim workSheet As Microsoft.Office.Interop.Excel.Worksheet = CType(xlBook.Worksheets(_namePage), Microsoft.Office.Interop.Excel.Worksheet)
        Dim rowIndex As Object = 1
        Dim dt As System.Data.DataTable = New System.Data.DataTable()
        Dim row As DataRow
        Dim temp As Integer = 1

        While (CType(workSheet.Cells(rowIndex, temp), Microsoft.Office.Interop.Excel.Range)).Value2 IsNot Nothing
            dt.Columns.Add(Convert.ToString((CType(workSheet.Cells(rowIndex, temp), Microsoft.Office.Interop.Excel.Range)).Value2))
            temp += 1
        End While

        rowIndex = Convert.ToInt32(rowIndex) + 1
        Dim columnCount As Integer = temp
        temp = 1

        While (CType(workSheet.Cells(rowIndex, temp), Microsoft.Office.Interop.Excel.Range)).Value2 IsNot Nothing
            row = dt.NewRow()

            For i As Integer = 1 To columnCount - 1
                row(i - 1) = Convert.ToString((CType(workSheet.Cells(rowIndex, i), Microsoft.Office.Interop.Excel.Range)).Value2)
            Next

            dt.Rows.Add(row)
            rowIndex = Convert.ToInt32(rowIndex) + 1
            temp = 1
        End While

        xlApp.Workbooks.Close()
        Return dt
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        importarExcel()
    End Sub
    Sub importarExcel()
        Dim myFileDialog As New OpenFileDialog()
        Dim xSheet As String = ""
        Dim dt As DataTable
        With myFileDialog
            .Filter = "Excel Files |*.xlsx"
            .Title = "Open File"
            .ShowDialog()
        End With
        If myFileDialog.FileName.ToString <> "" Then
            Dim ExcelFile As String = myFileDialog.FileName.ToString



            Try
                dt = ExcelToDatatable(ExcelFile, "productos")
            Catch ex As Exception
                MsgBox("Inserte un nombre valido de la Hoja que desea importar", MsgBoxStyle.Information, "Informacion")
            Finally

            End Try
        End If

        Dim dt02 As DataTable = dt

        Dim Res As Boolean
        Dim TablaImagenes As DataTable

        TablaImagenes = L_prCargarImagenesRecepcion(-1)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            '' StockMinimo=Precio Minimo  Atributo=Cliente Id
            Dim id As String = ""
            Res = L_prProductoInsertar(id, dt.Rows(i).Item("Codigo"), "", dt.Rows(i).Item("Producto"), "", dt.Rows(i).Item("Facturado"), 1, 1, 1, 1, 10, 13, 17, 20, 22, dt.Rows(i).Item("Mayorista"), TablaImagenes, dt.Rows(i).Item("Compra"))

            dt.Rows(i).Item("IdSistema") = id
        Next


        ''''''' Tienda   '''''''''''''''
        Dim dtdetalle As DataTable = L_prListarDetalleMovimiento(-1)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("TIENDA") > 0) Then

                _prAddDetalleVenta(dtdetalle)

                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("TIENDA")
            End If


        Next

        L_prMovimientoInsertar("", 4, 1, "Inventario Inicial Migrado Tienda",
                                         1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)




        ''''''' Cuarto A   '''''''''''''''
        dtdetalle = L_prListarDetalleMovimiento(-1)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("CUARTOA") > 0) Then

                _prAddDetalleVenta(dtdetalle)

                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("CUARTOA")
            End If


        Next

        L_prMovimientoInsertar("", 4, 2, "Inventario Inicial Migrado Cuarto A",
                                         1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)

        ''''''' Cuarto B   '''''''''''''''
        dtdetalle = L_prListarDetalleMovimiento(-1)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("CUARTOB") > 0) Then

                _prAddDetalleVenta(dtdetalle)

                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("CUARTOB")
            End If


        Next

        L_prMovimientoInsertar("", 4, 3, "Inventario Inicial Migrado Cuarto B",
                                         1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)

        ''''''' Cuarto C   '''''''''''''''
        dtdetalle = L_prListarDetalleMovimiento(-1)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("CUARTOC") > 0) Then

                _prAddDetalleVenta(dtdetalle)

                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
                dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("CUARTOC")
            End If


        Next

        L_prMovimientoInsertar("", 4, 4, "Inventario Inicial Migrado Cuarto C",
                                         1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)


        MsgBox("Se ha cargado la importacion correctamente", MsgBoxStyle.Information, "Importado con exito")

    End Sub
    Private Sub _prAddDetalleVenta(ByRef dtDetalle As DataTable)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado ,Sum(stock .Cantidad )as stock
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 30, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        dtDetalle.Rows.Add(_GenerarId(dtDetalle) + 1, 0, 0, "", 0, "20200101", CDate("2020/01/01"), Bin.GetBuffer, 0, 0)
    End Sub
    Public Function _GenerarId(dt As DataTable)

        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(dt.Rows(i).Item("Id")), 0, dt.Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
End Class