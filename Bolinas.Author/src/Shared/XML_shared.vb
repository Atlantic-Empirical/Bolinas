Imports System.IO
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Schema

Public Class cXmlValidator

    Private _validationErrors As New List(Of String)()

    ''' <summary> 
    ''' Get any error messages that were found validating the XML document against 
    ''' an XSD schema document. 
    ''' </summary> 
    Public ReadOnly Property ValidationErrors() As List(Of String)
        Get
            Return _validationErrors
        End Get
    End Property

    Private Sub OnValidationEvent(ByVal sender As Object, ByVal e As ValidationEventArgs)
        _validationErrors.Add(e.Message)
    End Sub

    Public Sub ValidateXmlAgainstSchema(ByVal xmlFileName As String, ByVal xsdFileName As String)
        Using xsdReader As XmlReader = XmlReader.Create(xsdFileName)
            ' Read the schema from a file. 
            Dim schema As XmlSchema = XmlSchema.Read(xsdReader, AddressOf OnValidationEvent)

            ' Setup the XmlReader for schema validation. 
            Dim settings As New XmlReaderSettings()
            settings.ValidationType = ValidationType.Schema
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings

            AddHandler settings.ValidationEventHandler, AddressOf OnValidationEvent

            ' Add the schema document. 
            settings.Schemas.Add(schema)

            ' Read each element of the document. Any errors will raise a ValidationEvent. 
            Using reader As XmlReader = XmlReader.Create(xmlFileName, settings)
                While reader.Read()
                End While
            End Using
        End Using
    End Sub

    Public Sub ValidateXmlAgainstSchema(ByRef xmlStream As String, ByRef xsdStream As Stream)
        Try
            Using xsdReader As XmlReader = XmlReader.Create(xsdStream)
                ' Read the schema from a file. 
                Dim schema As XmlSchema = XmlSchema.Read(xsdReader, AddressOf OnValidationEvent)

                ' Setup the XmlReader for schema validation. 
                Dim settings As New XmlReaderSettings()
                settings.ValidationType = ValidationType.Schema
                settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings

                AddHandler settings.ValidationEventHandler, AddressOf OnValidationEvent

                ' Add the schema document. 
                settings.Schemas.Add(schema)

                ' Read each element of the document. Any errors will raise a ValidationEvent. 
                Using reader As XmlReader = XmlReader.Create(xmlStream, settings)
                    While reader.Read()
                    End While
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Problem with ValidateXmlAgainstSchema(). Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

Module xmlHelper

    Public Function GetInnerText(ByRef xe As XmlElement, ByVal ElementName As String) As String
        Try
            Dim nl As XmlNodeList = xe.GetElementsByTagName(ElementName)
            If nl Is Nothing Then Return ""
            If nl.Count < 1 Then Return ""
            Return nl.Item(0).InnerText
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function Validate(ByRef nPath As String, ByRef Type As eRCDb_INPUT_DATA_TYPE) As List(Of String)
        Try
            Dim xsd As Stream = Nothing
            Select Case Type
                Case eRCDb_INPUT_DATA_TYPE.CONTRIBUTOR
                    xsd = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RCDb.Bolinas.Author.CONTRIBUTORS.xsd")
                Case eRCDb_INPUT_DATA_TYPE.FILMOGRAPHY
                    xsd = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RCDb.Bolinas.Author.FILMOGRAPHY.xsd")
                Case eRCDb_INPUT_DATA_TYPE.SOUNDTRACK
                    xsd = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RCDb.Bolinas.Author.SOUNDTRACK.xsd")
                Case eRCDb_INPUT_DATA_TYPE.FACTS
                    xsd = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RCDb.Bolinas.Author.FACTS.xsd")
            End Select
            If xsd Is Nothing Then Throw New Exception("Could not identify XSD type.")
            Dim v As New cXmlValidator()
            v.ValidateXmlAgainstSchema(nPath, xsd)
            xsd.Dispose()
            Return v.ValidationErrors
        Catch ex As Exception
            Throw New Exception("Problem in Validate(). Error: " & ex.Message, ex)
        End Try
    End Function

End Module
