Imports System.Configuration
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports NegocioGPL.DatosGPL
Imports NegocioGPL
Imports System.Timers

Public Class callWinServiceWcf
    Private serviceHost As ServiceHost = Nothing
    Public tmTemporizador As New System.Timers.Timer
    Private BasculaA As Basculas
    Private BasculaB As Basculas
    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            ' en produccion se tiene que comentar
            ' Debugger.Launch()



            FnlogApp("Host TCP/IP WCF V 1.1 Basculas PL")
            If serviceHost IsNot Nothing Then
                serviceHost.Close()
            End If
            'validar con Antonio Zurita si esos Puertos estan Disponibles o Libres
            ' ya que estos puertos tiene que tener permisos en el fireWall para poder ser alcanzados por la RED

            Dim baseAddressTCP As String = "net.tcp://localhost:9002/BasculaService"

            Dim adrbase As Uri() = {New Uri(baseAddressTCP)}
            Dim mBehave As New ServiceMetadataBehavior()
            Dim tcpb As New NetTcpBinding()

            serviceHost = New ServiceHost(GetType(gplServiceWcfLibrary.HostBascula), adrbase)
            serviceHost.Description.Behaviors.Add(mBehave)

            ' se egragan los extramos  o  TCP  por si el WCF se quiere consumir por medio de .net remoting

            serviceHost.AddServiceEndpoint(GetType(gplServiceWcfLibrary.IhostBascula), tcpb, baseAddressTCP)
            serviceHost.AddServiceEndpoint(GetType(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex")


            ' el servicio tiene que ser instalado por medio de la utilidad instalUtil del CMD como Admin
            FnlogApp("Intentado levantar Service Host en la direccion: " & baseAddressTCP)


            serviceHost.Open()
            If serviceHost.State = CommunicationState.Opened Then
                FnlogApp("Service Host Esta Listo.")
            Else
                FnlogApp("Service No Se Pudo Inicializar")
            End If


            ' se cra conexion a la base de datos
            FnlogApp("Intentando crear la instancia a la base de datos Porteria.")
            'DESARROLLO
            'ConfigurationManager.AppSettings.Set("SISTEMA_ESCOGIDO", "DESARROLLO")
            Datos.ForzarReinicio()
            Datos.ComprobacionesDatos()
            Datos.Usuario = New Usuario("BASCULAS")

            'tmTemporizador = New Timers.Timer
            'AddHandler tmTemporizador.Elapsed, AddressOf ejecutaTarea

            tmTemporizador.Interval = 1000 ' se configura el horario   de ejecucion cada segundo
            AddHandler tmTemporizador.Elapsed, AddressOf tmTemporizador_Tick
            BasculaA = New Basculas(Codigo:=1)
            BasculaB = New Basculas(Codigo:=2)
            'tmTemporizador.Enabled = True
            tmTemporizador.Start()
            FnlogApp("Host TCP/IP  Operativo...")
        Catch ex As Exception
            FnlogApp(ex.Message)
            Me.Stop()
        End Try
    End Sub
    Private Sub tmTemporizador_Tick(sender As Object, e As EventArgs)
        Try

            tmTemporizador.Stop()
            ActualizarEstadoBascula()
            tmTemporizador.Start()
        Catch ex As Exception
            OnStop()
        End Try
    End Sub
    'Private Sub ejecutaTarea(sender As Object, e As ElapsedEventArgs)
    '    Try

    '        tmTemporizador.Stop()
    '        ActualizarEstadoBascula()
    '        tmTemporizador.Start()
    '    Catch ex As Exception
    '        OnStop()
    '    End Try
    'End Sub
    Private Sub ActualizarEstadoBascula()
        Try
            'Debugger.Launch()
            Dim TcpBascula As gplServiceWcfLibrary.tcpBascula
            Dim stdBasA As gplServiceWcfLibrary.EstadoBascula
            Dim stdBasB As gplServiceWcfLibrary.EstadoBascula



            If Datos.Usuario.Creado Then

                ' TcpBascula.Open()
                TcpBascula = New gplServiceWcfLibrary.tcpBascula

                stdBasA = TcpBascula.DameEstado(gplServiceWcfLibrary.Basculas.Bascula1)
                stdBasB = TcpBascula.DameEstado(gplServiceWcfLibrary.Basculas.Bascula2)

                If stdBasA.Creado = True Then
                    BasculaA.ActulizarValoresBascula(_dPeso:=stdBasA.Peso,
                                                     _sEstadoDes:=stdBasA.EstadoDescripcion,
                                                     _sCodEstado:=stdBasA.CodEstado,
                                                     _sOperario:=stdBasA.Operario,
                                                     _sMatricula:=stdBasA.Matricula,
                                                     _dPMA:=stdBasA.PMA,
                                                     _dCarga:=stdBasA.Carga,
                                                     _sSilo:=stdBasA.Silo,
                                                     _dCargaFinal:=stdBasA.CargaFinal,
                                                     _sCadenaOriginal:=stdBasA.CadenaOriginal,
                                                     _iSegundoRetraso:=0,
                                                     _dtFechaValor:=Now)
                    FnlogApp("Lectura Bascula A: " & stdBasA.CadenaOriginal)
                End If

                If stdBasB.Creado = True Then
                    BasculaB.ActulizarValoresBascula(_dPeso:=stdBasB.Peso,
                                                     _sEstadoDes:=stdBasB.EstadoDescripcion,
                                                     _sCodEstado:=stdBasB.CodEstado,
                                                     _sOperario:=stdBasB.Operario,
                                                     _sMatricula:=stdBasB.Matricula,
                                                     _dPMA:=stdBasB.PMA,
                                                     _dCarga:=stdBasB.Carga,
                                                     _sSilo:=stdBasB.Silo,
                                                     _dCargaFinal:=stdBasB.CargaFinal,
                                                     _sCadenaOriginal:=stdBasB.CadenaOriginal,
                                                     _iSegundoRetraso:=0,
                                                     _dtFechaValor:=Now)

                    FnlogApp("Lectura Bascula B: " & stdBasB.CadenaOriginal)
                End If

                ' TcpBascula.Close()
            End If
        Catch ex As Exception
            FnlogApp(ex.Message)
        End Try
    End Sub
    Protected Overrides Sub OnStop()
        ' Agregue el código aquí para realizar cualquier anulación necesaria para detener el servicio.
        Try
            If serviceHost IsNot Nothing Then
                FnlogApp("Intentado Cerrar Service Host")
                serviceHost.Close()

                If serviceHost.State = CommunicationState.Closed Then
                    FnlogApp("Service Host se ha cerrado..")
                End If
                serviceHost = Nothing
            End If
            tmTemporizador.Stop()
        Catch ex As Exception
            FnlogApp(ex.Message)
        End Try
    End Sub
    Private Sub FnlogApp(ByVal sMsg As String)
        Try

            Dim Ruta As String = My.Application.Info.DirectoryPath
            Dim oSW As New IO.StreamWriter(Ruta & "\Log" + Now.Date.ToString("yyyy-MM-dd") + ".txt", True)
            Dim scomando As String = String.Empty
            oSW.WriteLine(Now & " || Evento: " & sMsg)
            oSW.Flush()
            oSW.Close()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub


End Class
