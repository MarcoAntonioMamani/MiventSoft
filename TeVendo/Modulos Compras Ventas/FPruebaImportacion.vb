Imports Negocio.AccesoLogica
Imports System.IO
Public Class FPruebaImportacion
    Private Sub FPruebaImportacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        L_prAbrirConexion("DESKTOP-T84OJOU", "sa", "123", "MinventSoftRepuestoJhon")
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
        ''importarExcelClientes()
        'importarExcelClientes()
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
                dt = ExcelToDatatable(ExcelFile, "Productos")
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
            '' StockMinimo=Precio Precio compra   conversion =precio venta 
            Dim id As String = ""
            '  ByRef _numi As String, _CodigoExterno As String,
            '                                        _CodigoBarra As String, _NombreProducto As String,
            '_Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer,
            '_EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
            '_AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer, _UnidadMaximaId As Integer,
            '_conversion As Double, _dtImagenes As DataTable

            Dim Marca As String = dt.Rows(i).Item("Marca")
            Dim idMarca As Integer = 0
            Existe(L_prLibreriaDetalleGeneral(3), "cnnum", Marca, idMarca)
            If (idMarca = -1) Then
                Dim idNewMarca As String = ""
                L_prClasificadorGrabar(idNewMarca, 3, Marca)
                idMarca = Integer.Parse(idNewMarca)
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim SubGrupo As String = dt.Rows(i).Item("SUBGRUPO")
            Dim idSubGrupo As Integer = 0
            Existe(L_prLibreriaDetalleGeneral(4), "cnnum", SubGrupo, idSubGrupo)
            If (idSubGrupo = -1) Then
                Dim idNewSubGrupo As String = ""
                L_prClasificadorGrabar(idNewSubGrupo, 3, Marca)
                idSubGrupo = Integer.Parse(idNewSubGrupo)
            End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim Medida As String = dt.Rows(i).Item("MEDIDA")
            Dim idMedida As Integer = 0
            Existe(L_prLibreriaDetalleGeneral(5), "cnnum", Medida, idMedida)
            If (idMedida = -1) Then
                Dim idNewMedida As String = ""
                L_prClasificadorGrabar(idNewMedida, 5, Medida)
                idMedida = Integer.Parse(idNewMedida)
            End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim Grupo As String = dt.Rows(i).Item("GRUPO")
            Dim idGrupo As Integer = 0
            Existe(L_prCategoriaGeneral(), "id", Grupo, idGrupo)
            If (idGrupo = -1) Then
                'L_prCategoriaInsertar(id, tbdescripcion.Text, tbdescripcion.Text, 1, "", 1, 1)
                Dim idNewGrupo As String = ""
                L_prCategoriaInsertar(idNewGrupo, Grupo, "", 1, "", 1, 1)
                idGrupo = Integer.Parse(idNewGrupo)
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Dim detalle As String



            detalle = dt.Rows(i).Item("Producto") + " " + dt.Rows(i).Item("MARCA") + " " + dt.Rows(i).Item("OE") +
                " " + dt.Rows(i).Item("Fabrica") + " " + dt.Rows(i).Item("Grupo") + " " + dt.Rows(i).Item("SUBGRUPO") +
                " " + dt.Rows(i).Item("MEDIDA")

            Res = L_prProductoInsertarDistralKCP(id, "", "",
                                        nombre, nombre,
                                        3, 1, dt.Rows(i).Item("CategoriaId"),
                                        1, 1, dt.Rows(i).Item("ComercialId"), dt.Rows(i).Item("PresentacionId"),
                                        dt.Rows(i).Item("FormulacionID"), 20, 22, 1,
                                             TablaImagenes)

            'dt.Rows(i).Item("IdSistema") = id
        Next


        '''''''' Tienda   '''''''''''''''
        'Dim dtdetalle As DataTable = L_prListarDetalleMovimiento(-1)
        ''a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        ''    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        'For i As Integer = 0 To dt.Rows.Count - 1 Step 1

        '    If (dt.Rows(i).Item("Inventario") > 0) Then

        '        _prAddDetalleVenta(dtdetalle)

        '        dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
        '        dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("Inventario")
        '    End If


        'Next

        'L_prMovimientoInsertar("", 4, 1, "Inventario Inicial Migrado",
        '                                 1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)




        ''''''' Cuarto A   '''''''''''''''
        'dtdetalle = L_prListarDetalleMovimiento(-1)
        ''a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        ''    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        'For i As Integer = 0 To dt.Rows.Count - 1 Step 1

        '    If (dt.Rows(i).Item("CUARTOA") > 0) Then

        '        _prAddDetalleVenta(dtdetalle)

        '        dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("ProductoId") = dt.Rows(i).Item("IdSistema")
        '        dtdetalle.Rows(dtdetalle.Rows.Count - 1).Item("Cantidad") = dt.Rows(i).Item("CUARTOA")
        '    End If


        'Next

        'L_prMovimientoInsertar("", 4, 2, "Inventario Inicial Migrado Cuarto A",
        '                                 1, Now.Date.ToString("yyyy/MM/dd"), dtdetalle, 1, 0)




        MsgBox("Se ha cargado la importacion correctamente", MsgBoxStyle.Information, "Importado con exito")

    End Sub



    Function Existe(dt As DataTable, column As Integer, value As String, ByRef id As Integer)
        id = -1

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item(column).ToString.ToUpper = value.ToUpper) Then
                id = dt.Rows(i).Item("id")
                Return True
            End If
        Next
        Return False
    End Function

    Sub importarExcelClientes()
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
                dt = ExcelToDatatable(ExcelFile, "clientes")
            Catch ex As Exception
                MsgBox("Inserte un nombre valido de la Hoja que desea importar", MsgBoxStyle.Information, "Informacion")
            Finally

            End Try
        End If

        Dim dt02 As DataTable = dt

        Dim Res As Boolean
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            '' StockMinimo=Precio Precio compra   conversion =precio venta 
            Dim id As String = ""
            '_Id As String, IdZona As Integer, IdPrecio As Integer, CodigoExterno As String, NombreCliente As String,
            '                               Direccion As String, Telefono As String, TipoDocumento As Integer,
            '                     NroDocumento As String,
            '                               RazonSocial As String, nit As String, estado As Integer,
            'FechaIngreso As String, Latitud As Double, Longitud As Double, TipoNegocio As Integer, Referencia As String

            Res = InsertarCliente(id, dt.Rows(i).Item("ZonaId"), dt.Rows(i).Item("PrecioId"), "",
                                  dt.Rows(i).Item("nombrecompleto"), dt.Rows(i).Item("Direccion"), dt.Rows(i).Item("Celular"), 1, dt.Rows(i).Item("CI"),
                                  dt.Rows(i).Item("nombrecompleto"), dt.Rows(i).Item("Nit"), 1, Now.Date.ToString("yyyy/MM/dd"), dt.Rows(i).Item("Latitud"), dt.Rows(i).Item("Longitud"),
dt.Rows(i).Item("idtipoNegocio"), dt.Rows(i).Item("Referencia"))

            'dt.Rows(i).Item("IdSistema") = id
        Next
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