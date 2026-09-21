
Imports System.Data
Imports System.Collections.Generic
Imports System.Text
Imports Janus.Windows.GridEX

Public Module BuscadorJanus

    Public Sub FiltrarGridEX(grilla As GridEX,
                            datosOriginales As DataTable,
                            textoBusqueda As String)

        If grilla Is Nothing OrElse datosOriginales Is Nothing Then
            Exit Sub
        End If

        'Restaurar los datos cuando el buscador esté vacío
        If String.IsNullOrWhiteSpace(textoBusqueda) Then
            grilla.DataSource = datosOriginales
            Exit Sub
        End If

        If grilla.RootTable Is Nothing Then Exit Sub

        '1. Identificar las columnas visibles y vinculadas
        Dim campos As New List(Of String)

        For Each columna As GridEXColumn In grilla.RootTable.Columns

            If Not columna.Visible Then Continue For

            Dim campo As String = columna.DataMember

            'Usar Key como alternativa si no existe DataMember
            If String.IsNullOrEmpty(campo) OrElse
               Not datosOriginales.Columns.Contains(campo) Then

                campo = columna.Key
            End If

            If Not String.IsNullOrEmpty(campo) AndAlso
               datosOriginales.Columns.Contains(campo) AndAlso
               Not campos.Contains(campo) Then

                campos.Add(campo)
            End If

        Next

        'Si no existen columnas visibles vinculadas
        If campos.Count = 0 Then
            grilla.DataSource = datosOriginales
            Exit Sub
        End If

        '2. Separar las palabras de búsqueda
        Dim separadores As Char() = {
            " "c, ","c, ";"c,
            ChrW(9), ChrW(10), ChrW(13)
        }

        Dim palabras As String() =
            textoBusqueda.Trim().Split(
                separadores,
                StringSplitOptions.RemoveEmptyEntries
            )

        '3. Crear una tabla vacía con la estructura original
        Dim resultado As DataTable = datosOriginales.Clone()

        '4. Buscar en todas las columnas visibles
        For Each fila As DataRow In datosOriginales.Rows

            If fila.RowState = DataRowState.Deleted Then
                Continue For
            End If

            Dim contenido As New StringBuilder()

            'Unir los valores de las columnas visibles
            For Each campo As String In campos

                If Not fila.IsNull(campo) Then
                    contenido.Append(" ")
                    contenido.Append(fila(campo).ToString())
                End If

            Next

            Dim textoFila As String = contenido.ToString()
            Dim coincide As Boolean = True

            'Todas las palabras deben encontrarse
            For Each palabra As String In palabras

                If textoFila.IndexOf(
                    palabra,
                    StringComparison.CurrentCultureIgnoreCase
                ) < 0 Then

                    coincide = False
                    Exit For

                End If

            Next

            'Agregar solamente las filas coincidentes
            If coincide Then
                resultado.ImportRow(fila)
            End If

        Next

        '5. Mostrar los resultados
        grilla.DataSource = resultado

    End Sub

End Module