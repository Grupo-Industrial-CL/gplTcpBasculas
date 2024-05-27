' NOTA: puede usar el comando "Cambiar nombre" del menú contextual para cambiar el nombre de clase "Service1" en el código y en el archivo de configuración a la vez.
Imports gplServiceWcfLibrary

Public Class HostBascula
    Implements IhostBascula

    Protected connBascula As New tcpBascula()

    Public Function CargaDatos(numBascula As Basculas, _codOperario As String, _matricula As String, _pma As Double, _carga As Double, _silo As String) As EstadoComando Implements IhostBascula.CargaDatos
        Try
            Return connBascula.CargaDatos(idBascula:=numBascula,
                                          codOperario:=_codOperario,
                                          matricula:=_matricula,
                                          pma:=_pma,
                                          carga:=_carga,
                                          silo:=_silo)
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function DameEstado(numBascula As Basculas) As EstadoBascula Implements IhostBascula.DameEstado
        Try
            Return connBascula.DameEstado(idBascula:=numBascula)
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function FinCarga(numBascula As Basculas) As EstadoComando Implements IhostBascula.FinCarga
        Try
            Return connBascula.FinCarga(idBascula:=numBascula)
        Catch ex As Exception
            Throw
        End Try
    End Function
    Public Sub New()

    End Sub
End Class
