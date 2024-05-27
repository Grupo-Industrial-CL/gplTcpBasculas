Module Module1

    Sub Main()
        Try

            Dim x As New gplServiceWcfLibrary.tcpBascula()
            Dim resultado As gplServiceWcfLibrary.EstadoComando
            ' resultado = x.DameEstado)
            resultado = x.CargaDatos(idBascula:=gplServiceWcfLibrary.Basculas.Bascula2,
                                     codOperario:="1",
                                     matricula:="CS-7028",
                                     pma:=40500,
                                     carga:=0,
                                     silo:="T38")
            'Using host As New hostBasculaPL.IhostBasculaClient
            '    host.Open()
            '    Dim x As hostBasculaPL.EstadoBascula = host.DameEstado(hostBasculaPL.Basculas.Bascula2)
            '    host.Close()
            'End Using
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

End Module
