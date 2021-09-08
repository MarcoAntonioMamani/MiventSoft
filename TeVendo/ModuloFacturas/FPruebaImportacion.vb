Imports Negocio.AccesoLogica
Public Class FPruebaImportacion
    Private Sub FPruebaImportacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        Dim myFileDialog As New OpenFileDialog()
        Dim xSheet As String = ""
        Dim dt As DataTable
        With myFileDialog
            .Filter = "Excel Files |*.*"
            .Title = "Open File"
            .ShowDialog()
        End With
        If myFileDialog.FileName.ToString <> "" Then
            Dim ExcelFile As String = myFileDialog.FileName.ToString



            Try
                dt = ExcelToDatatable(ExcelFile, "inventario")





            Catch ex As Exception
                MsgBox("Inserte un nombre valido de la Hoja que desea importar", MsgBoxStyle.Information, "Informacion")
            Finally

            End Try
        End If

        Dim dt02 As DataTable = dt

        Dim TablaImagenes As DataTable = L_prCargarImagenesRecepcion(-1)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim dtprecio As DataTable = ListPreciosDetalles(-1)


            Dim PrecioCosto As String = dt.Rows(i).Item("PrecioCosto")
            PrecioCosto = PrecioCosto.Replace(",", ".")

            Dim PrecioVenta As String = dt.Rows(i).Item("PrecioVenta")
            PrecioVenta = PrecioVenta.Replace(",", ".")
            Dim PrecioMayoreo As String = dt.Rows(i).Item("PrecioMayoreo")
            PrecioMayoreo = PrecioMayoreo.Replace(",", ".")

            dtprecio.Rows(0).Item("precio") = PrecioCosto
            dtprecio.Rows(1).Item("precio") = PrecioVenta
            dtprecio.Rows(2).Item("precio") = PrecioMayoreo


            L_prProductoInsertar("", dt.Rows(i).Item("Codigo"), dt.Rows(i).Item("Codigo"), dt.Rows(i).Item("Descripcion"),
                                                dt.Rows(i).Item("Descripcion"), dt.Rows(i).Item("stockminimo"), 1,
                                                dt.Rows(i).Item("categoria"), 1, 1, 10, 13, 17, 20, 12088, 1, TablaImagenes, dtprecio)
        Next


        MsgBox("Se ha cargado la importacion correctamente", MsgBoxStyle.Information, "Importado con exito")
    End Sub
End Class