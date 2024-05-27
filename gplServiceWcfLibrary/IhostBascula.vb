' NOTA: puede usar el comando "Cambiar nombre" del menú contextual para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
<ServiceContract()>
Public Interface IhostBascula

    <OperationContract()>
    Function DameEstado(ByVal idBascula As Basculas) As EstadoBascula
    <OperationContract()>
    Function CargaDatos(numBascula As Basculas, _codOperario As String, _matricula As String, _pma As Double, _carga As Double, _silo As String) As EstadoComando
    <OperationContract()>
    Function FinCarga(ByVal idBascula As Basculas) As EstadoComando

End Interface

' Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
' Puede agregar archivos XSD al proyecto. Después de compilar el proyecto, puede usar directamente los tipos de datos definidos aquí, con el espacio de nombres "gplServiceWcfLibrary.ContractType".

<DataContract()>
Public Class EstadoBascula
    <DataMember()>
    Public Property Peso As Double
    <DataMember()>
    Public Property EstadoDescripcion As String
    <DataMember()>
    Public Property CodEstado As String
    <DataMember()>
    Public Property Operario As String
    <DataMember()>
    Public Property Matricula As String
    <DataMember()>
    Public Property PMA As Double
    <DataMember()>
    Public Property Carga As Double
    <DataMember()>
    Public Property Silo As String
    <DataMember()>
    Public Property CargaFinal As Double
    <DataMember()>
    Public Property CadenaOriginal As String
    <DataMember()>
    Public Property Creado As Boolean
    Public Sub New()
        LimpiarValores()
    End Sub
    Public Sub LimpiarValores()
        Peso = 0
        EstadoDescripcion = String.Empty
        CodEstado = String.Empty
        Operario = String.Empty
        Matricula = String.Empty
        PMA = 0
        Carga = 0
        Silo = String.Empty
        CargaFinal = 0
        CadenaOriginal = String.Empty
        Creado = False
    End Sub
End Class

<DataContract()>
Public Class EstadoComando
    <DataMember()>
    Public Property ExiteError As Boolean
    <DataMember()>
    Public Property CodError As String
    <DataMember()>
    Public Property DetalleError As String
    <DataMember()>
    Public Property CadenaOriginal As String

    Public Sub New()
        LimpiarValores()
    End Sub
    Public Sub LimpiarValores()
        ExiteError = False
        CodError = String.Empty
        DetalleError = String.Empty
        CadenaOriginal = String.Empty
    End Sub
End Class



Public Structure EstadoB
    Public Const PausaFaltaMaterial As String = "M" 'báscula en pausa a causa de falta de material.
    Public Const ReposoConCamion As String = "E" 'báscula en reposo con camión en báscula esperando la entrada de datos.
    Public Const ReposoSINCamion As String = "R" 'báscula en reposo sin camión en báscula. No disponible para carga.
    Public Const EsperandoInicioCarga As String = "I" 'camión en báscula con datos cargados y esperando el inicio.
    Public Const Cargando As String = "C" 'báscula cargando.
    Public Const Pausa As String = "P" 'báscula en pausa.
    Public Const FinalizadaConCamion As String = "F" 'carga en báscula finalizada con camión todavía en báscula.
End Structure

Public Structure CodigoErrores
    Const error1 As String = "1"
    Const error2 As String = "2"
    Const error3 As String = "3"
    Const error4 As String = "4"
    Const error5 As String = "5"
    Const error6 As String = "6"
    Const error7 As String = "7"
End Structure
Public Enum Basculas
    Bascula1 = 1
    Bascula2 = 2
End Enum





