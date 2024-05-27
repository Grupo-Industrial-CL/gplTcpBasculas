Imports System.Configuration
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports Utilidades.Util
Public Class tcpBascula
    Private clienteB1 As TcpClient
    Private clienteB2 As TcpClient
    Private nwStreamB1 As NetworkStream
    Private nwStreamB2 As NetworkStream

    Private puertoA1 As Integer = CInt(ConfigurationManager.AppSettings.Get("puertoBascula1").ToString()) '1235
    Private IpA1 As String = ConfigurationManager.AppSettings.Get("ipBascula1").ToString()  '"172.22.13.66"

    Private puertoB2 As Integer = CInt(ConfigurationManager.AppSettings.Get("puertoBascula2").ToString()) '1235
    Private IpB2 As String = ConfigurationManager.AppSettings.Get("ipBascula2").ToString()  '"172.22.13.66"
    Public Sub New()

    End Sub

    Private Function ConnectSendReceive(scadenaEnvio As String, ByVal id_Bascula As Basculas) As Byte()
        Try
            'intentamos crear el endPoint
            'If String.IsNullOrEmpty(pIp) Then Return Nothing
            'If Not IPAddress.TryParse(pIp, Nothing) Then Return Nothing
            Dim pSendData As Byte() = Encoding.UTF8.GetBytes(scadenaEnvio)
            Dim vIp As IPAddress = Nothing
            Dim vEndPoint As IPEndPoint = Nothing

            Select Case id_Bascula
                Case Basculas.Bascula1
                    vIp = IPAddress.Parse(IpA1)
                    vEndPoint = New IPEndPoint(vIp, CInt(puertoA1))
                Case Basculas.Bascula2
                    vIp = IPAddress.Parse(IpB2)
                    vEndPoint = New IPEndPoint(vIp, CInt(puertoB2))
            End Select

            'Timeout en 0.5 segundos. para envio  
            Dim seTimeout As Integer = 500
            'Timeout en 0.5 segundos. para response
            Dim reTimeout As Integer = 1000

            'para todos los mensajes pequeñosa que responde  la bascula 1024 bytes deverian ser suficientes   
            Dim vMessageLength As Integer = 1024
            Dim vServerResponse As Byte() = Nothing

            'creamos el bloque using ahora con un soket 
            Using gSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

                Try
                    gSocket.Connect(vEndPoint)
                Catch ex As Exception
                    FnlogApp(ex.Message)
                    Return Nothing
                End Try
                If Not gSocket.Connected Then
                    FnlogApp("Error no se pudo establacer comunicacion con las basculas.")
                    Return Nothing
                End If

                'establecemos el time out al soket y enviamos los bytes del mensaje
                gSocket.SendTimeout = seTimeout
                gSocket.Send(pSendData)

                'recivimos los mensajes  quiero probar con las bascular el timeOut Maximo necesario para la espera de respuestas
                gSocket.ReceiveTimeout = reTimeout
                Dim vBuffer(vMessageLength - 1) As Byte
                Dim vNumOfBytesReceived As Integer = 0
                Try
                    vNumOfBytesReceived = gSocket.Receive(vBuffer, 0, vMessageLength, SocketFlags.None)
                Catch ex As Exception
                    FnlogApp(ex.Message)
                    Return Nothing
                End Try

                'Return received bytes.  
                ReDim vServerResponse(vNumOfBytesReceived - 1)
                Array.Copy(vBuffer, vServerResponse, vNumOfBytesReceived)

                'Disconnect como estamos usando un bloque using el soket se desconectara de forma automatica el llegar a este punto.  
            End Using

            Return vServerResponse
        Catch ex As Exception
            FnlogApp(ex.Message)
            Return Nothing
            'guardar en log
        End Try

    End Function



    Public Function DameEstado(ByVal idBascula As Basculas) As EstadoBascula
        Try
            'Debugger.Launch()
            Dim cadenaEnvio As String = "<ESTADO>"
            Dim vServerResponse As Byte()
            Dim RespuestaCadena As String = ""
            Dim respuesta As EstadoBascula = New EstadoBascula

            vServerResponse = ConnectSendReceive(scadenaEnvio:=cadenaEnvio,
                                                 id_Bascula:=idBascula)

            If vServerResponse Is Nothing Then
                FnlogApp("El servidor no esta disponible / No se recibió respuesta del servidor.")
                Return New EstadoBascula
            End If

            RespuestaCadena = Encoding.ASCII.GetString(vServerResponse)

            Dim n As Integer = (From c In RespuestaCadena Where c = ";" Select c).Count()

            If n = 8 Then

                Dim Datos() As String = Nothing
                RespuestaCadena = RespuestaCadena.Replace("<", "")
                RespuestaCadena = RespuestaCadena.Replace(">", "")
                Datos = RespuestaCadena.Split(";")

                respuesta.Peso = CDbl(NoNull(Trim(Datos(0)), "D"))
                respuesta.CodEstado = Trim(Datos(1))
                respuesta.EstadoDescripcion = DameDescripcionEstado(respuesta.CodEstado)
                respuesta.Operario = Trim(Datos(2))
                respuesta.Matricula = Trim(Datos(3))
                respuesta.PMA = CDbl(NoNull(Trim(Datos(4)), "D"))
                respuesta.Carga = CDbl(NoNull(Trim(Datos(5)), "D"))
                respuesta.Silo = Trim(Datos(6))
                respuesta.CargaFinal = CDbl(NoNull(Trim(Datos(7)), "D"))
                respuesta.CadenaOriginal = RespuestaCadena
                respuesta.Creado = True
            End If
            Return respuesta
        Catch ex As Exception
            FnlogApp(ex.Message)
            DameEstado = New EstadoBascula
        End Try
    End Function
    Public Function CargaDatos(ByVal idBascula As Basculas, ByVal codOperario As String, ByVal matricula As String, ByVal pma As Double, ByVal carga As Double, ByVal silo As String) As EstadoComando
        Try
            Dim cadenaEnvio As String = "<CARGA;" & codOperario & ";" & matricula & ";" & pma & ";" & carga & ";" & silo & ";>"
            Dim vServerResponse As Byte()
            Dim RespuestaCadena As String = ""
            Dim respuesta As EstadoComando = New EstadoComando

            vServerResponse = ConnectSendReceive(scadenaEnvio:=cadenaEnvio,
                                                 id_Bascula:=idBascula)

            If vServerResponse Is Nothing Then
                FnlogApp("El servidor no esta disponible / No se recibió respuesta del servidor.")
                respuesta.CadenaOriginal = RespuestaCadena
                respuesta.ExiteError = True
                respuesta.CodError = "1"
                respuesta.DetalleError = "El servidor no esta disponible / No se recibió respuesta del servidor."
                Return respuesta
            End If

            RespuestaCadena = Encoding.ASCII.GetString(vServerResponse)

            If RespuestaCadena.Trim() = "<OK>" Then
                respuesta.CadenaOriginal = RespuestaCadena
                respuesta.ExiteError = False
                respuesta.CodError = ""
                respuesta.DetalleError = ""
            Else
                If Mid(RespuestaCadena, 1, 6) = "<ERROR" Then
                    Dim DatosErr() As String = Nothing
                    DatosErr = RespuestaCadena.Split(";")
                    respuesta.CadenaOriginal = RespuestaCadena
                    respuesta.ExiteError = True
                    respuesta.CodError = DatosErr(1)
                    respuesta.DetalleError = DameDescripcionError(DatosErr(1).Trim)
                End If

            End If
            Return respuesta
        Catch ex As Exception
            FnlogApp(ex.Message)
            Throw
        End Try
    End Function
    Public Function FinCarga(ByVal idBascula As Basculas) As EstadoComando
        Try
            Dim cadenaEnvio As String = "<FIN>"
            Dim vServerResponse As Byte()
            Dim RespuestaCadena As String = ""
            Dim respuesta As EstadoComando = New EstadoComando

            vServerResponse = ConnectSendReceive(scadenaEnvio:=cadenaEnvio,
                                                 id_Bascula:=idBascula)

            If vServerResponse Is Nothing Then
                FnlogApp("El servidor no esta disponible / No se recibió respuesta del servidor.")
                respuesta.CadenaOriginal = RespuestaCadena
                respuesta.ExiteError = True
                respuesta.CodError = "1"
                respuesta.DetalleError = "El servidor no esta disponible / No se recibió respuesta del servidor."
                Return respuesta
            End If

            RespuestaCadena = Encoding.ASCII.GetString(vServerResponse)

            If RespuestaCadena.Trim() = "<OK>" Then
                respuesta.CadenaOriginal = RespuestaCadena
                respuesta.ExiteError = False
                respuesta.CodError = ""
                respuesta.DetalleError = ""
            Else
                If Mid(RespuestaCadena, 1, 6) = "<ERROR" Then
                    Dim DatosErr() As String = Nothing
                    DatosErr = RespuestaCadena.Split(";")
                    respuesta.CadenaOriginal = RespuestaCadena
                    respuesta.ExiteError = True
                    respuesta.CodError = DatosErr(1)
                    respuesta.DetalleError = DameDescripcionError(DatosErr(1).Trim)
                End If

            End If
            Return respuesta
        Catch ex As Exception
            FnlogApp(ex.Message)
            Throw
        End Try
    End Function

    Private Function DameDescripcionEstado(CodigoEstado As String) As String
        'R -> bascula en reposo, sin camion en bascula. No disponible para carga
        'E -> bascula en reposo, con camion en báscula esperando la entrada de datos
        'I -> camion en bascula con datos cargados
        'C -> bascula cargando
        'M -> Falta Material, en pausa
        'P -> bascula en pausa
        'F -> carga en báscula finalicada con camion aun sobre la bascula
        Select Case CodigoEstado
            Case EstadoB.ReposoSINCamion
                Return "En Reposo"

            Case EstadoB.ReposoConCamion
                Return "Esperando datos Camion"

            Case EstadoB.EsperandoInicioCarga
                Return "Esperando inicio carga"

            Case EstadoB.Cargando
                Return "Cargando"

            Case EstadoB.Pausa
                Return "En pausa"

            Case EstadoB.PausaFaltaMaterial
                Return "Falta Material"

            Case EstadoB.FinalizadaConCamion
                Return "Carga Finalizada"

            Case Else
                Return "NO CONTEMPLADO"
        End Select

    End Function
    Private Function DameDescripcionError(CodigoEstado As String) As String
        '1 -> Error No definido
        '2 -> Error de estado: Se envian datos de carga sin estar en estado "E", o se envia fin carga sin estar en estado "M".
        '3 -> Error de operario: Operario omitido.
        '4 -> Error de matricula: Matricula omitida. 
        '5 -> Error de pma: Dato PMA omitido o supera el PMA Maximo configurado en el visor.
        '6 -> Error de carga: Dato carga supera el PMA introducido.
        '7 -> Error de silo: Silo omitido o no se encuentra en la base de datos del visor.

        Select Case CodigoEstado
            Case CodigoErrores.error1
                Return "Error No definido"
            Case CodigoErrores.error2
                Return "Error de estado: Se envian datos de carga sin estar en estado E, o se envia fin carga sin estar en estado M."
            Case CodigoErrores.error3
                Return "Error de operario: Operario omitido."
            Case CodigoErrores.error4
                Return "Error de matricula: Matricula omitida."
            Case CodigoErrores.error5
                Return "Error de pma: Dato PMA omitido o supera el PMA Maximo configurado en el visor."
            Case CodigoErrores.error6
                Return "Error de carga: Dato carga supera el PMA introducido."
            Case CodigoErrores.error7
                Return "Error de silo: Silo omitido o no se encuentra en la base de datos del visor."
            Case Else
                Return "Error Desconocido."
        End Select

    End Function
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
