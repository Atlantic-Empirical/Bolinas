#Region "IMPORTS"

Imports SMT.Multimedia.Utility.Timecode
Imports SMT.Multimedia.Players.M2TS
Imports SMT.DotNet.Serialization.XML
Imports SMT.Multimedia.Players
Imports System.IO
Imports System.Xml.Serialization
Imports SMT.Multimedia.Classes

#End Region 'IMPORTS

Public Class cBolinas_AuthoringProject

#Region "FIELDS"

    Public Name As String
    Public DVDPath As String
    Public AppVersion As String

#End Region 'FIELDS

#Region "PROPERTIES"

    Public ReadOnly Property Progress_Prepare() As Byte
        Get
            Dim out As Byte = 0
            If Not String.IsNullOrEmpty(Product_Id) Then out += 10
            If Not String.IsNullOrEmpty(Product_Title) Then out += 10
            If Not String.IsNullOrEmpty(Product_Studio) Then out += 10
            If Not String.IsNullOrEmpty(Product_ReleasedDate) Then out += 10
            If Not String.IsNullOrEmpty(Product_ParentWorkId) Then out += 10
            If CONTRIBUTORS IsNot Nothing AndAlso CONTRIBUTORS.Items.Count > 0 Then out += 10
            If FILMOGRAPHY IsNot Nothing AndAlso FILMOGRAPHY.Items.Count > 0 Then out += 10
            If SOUNDTRACK IsNot Nothing AndAlso SOUNDTRACK.Items.Count > 0 Then out += 10
            If FACTOGRAPHY IsNot Nothing AndAlso FACTOGRAPHY.Items.Count > 0 Then out += 10
            If EVENTS.DVDTitleNumber <> Nothing AndAlso EVENTS.DVDTitleNumber <> -1 Then out += 10
            Return out
        End Get
    End Property
    Public ReadOnly Property Progress_Collect() As Byte
        Get
            Return EVENTS.SceneCoverage_Percent
        End Get
    End Property
    Public ReadOnly Property Progress_Verify() As Byte
        Get
            Return 0
        End Get
    End Property

#End Region 'PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        CONTRIBUTORS = New cRCDb_CONTRIBUTOR_COLLECTION
        FILMOGRAPHY = New cRCDb_FILMOGRAPHY
        SOUNDTRACK = New cRCDb_SOUNDTRACK
        FACTOGRAPHY = New cRCDb_FACTOGRAPHY
        EVENTS = New cRCDb_EVENT_CONTAINER
        SCENES = New cRCDb_SCENE_COLLECTION
    End Sub

#End Region 'CONSTRUCTOR

#Region "PUBLIC METHODS"

    Public Function GetCast() As List(Of cRCDb_CONTRIBUTOR_ITEM)
        Try
            If CONTRIBUTORS.Items.Count = 0 Or FILMOGRAPHY.Items.Count = 0 Then Return Nothing
            Dim out As New List(Of cRCDb_CONTRIBUTOR_ITEM)
            Dim tCI As cRCDb_CONTRIBUTOR_ITEM
            Dim tFI As cRCDb_FILMOGRAPHY_ITEM
            For Each ci As cRCDb_CONTRIBUTOR_ITEM In CONTRIBUTORS.Items
                tCI = New cRCDb_CONTRIBUTOR_ITEM(ci.ContributorId, ci.ContributorName)
                tFI = FILMOGRAPHY.FindPerson(Me.Product_ParentWorkId, tCI.ContributorId)
                If tFI IsNot Nothing AndAlso (InStr(tFI.Role.ToLower, "actor") Or InStr(tFI.Role.ToLower, "voice")) Then
                    tCI.CharacterName = tFI.CharacterName
                    out.Add(tCI)
                End If
            Next
            Return out
        Catch ex As Exception
            Throw New Exception("Problem with GetCast(). Error: " & ex.Message, ex)
        End Try
    End Function

#End Region 'PUBLIC METHODS

#Region "PRIVATE METHODS"

#End Region 'PRIVATE METHODS

#Region "SERIALIZATION (Project Save/Load)"

    Public Shared Function LoadProjectFromFile(ByVal nProjectPath As String) As cBolinas_AuthoringProject
        If Not File.Exists(nProjectPath) Then Return Nothing
        Dim out As cBolinas_AuthoringProject = DeserializeFromFile(GetType(cBolinas_AuthoringProject), nProjectPath)
        If out.AppVersion Is Nothing OrElse out.AppVersion <> My.Application.Info.Version.ToString Then
            If MessageBox.Show("This project file was created by an earlier version of " & My.Settings.APPLICATION_NAME_SHORT & ". Some problems may occure." & vbNewLine & vbNewLine & "Do you wish to continue?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.No Then
                Return Nothing
            End If
        End If
        Return out
    End Function

    Public Shared Function SaveProject(ByRef Prj As cBolinas_AuthoringProject, ByVal SavePath As String) As Boolean
        Try
            If File.Exists(SavePath) Then File.Delete(SavePath)
            Prj.AppVersion = My.Application.Info.Version.ToString
            Return SerializeToFile(Prj, SavePath)
        Catch ex As Exception
            Throw New Exception("Problem with SaveProject(). Error: " & ex.Message)
        End Try
    End Function

#End Region 'SERIALIZATION (Project Save/Load)

#Region "PRODUCT DATA"

#Region "PRODUCT DATA:GENERAL"

    Public Product_Id As String
    Public Product_Lang As eBolinas_Language
    Public Product_Type As eBolinas_ProductType
    Public Product_Title As String
    Public Product_Studio As String
    Public Product_ReleasedDate As String
    Public Product_ParentWorkId As String

#End Region 'PRODUCT DATA:GENERAL

#Region "PRODUCT DATA"

    Public EVENTS As cRCDb_EVENT_CONTAINER
    Public CONTRIBUTORS As cRCDb_CONTRIBUTOR_COLLECTION
    Public FILMOGRAPHY As cRCDb_FILMOGRAPHY
    Public SOUNDTRACK As cRCDb_SOUNDTRACK
    Public FACTOGRAPHY As cRCDb_FACTOGRAPHY
    Public SCENES As cRCDb_SCENE_COLLECTION

#End Region 'PRODUCT DATA

#End Region 'PRODUCT DATA

#Region "EXPORT"

#Region "EXPORT:MOVIE IQ"

    Public Function ExportMovieIQ(ByVal oPath As String) As Boolean
        Try
            ''DEBUGGING
            ''  CREATE DUMMY EVENT DATA
            'Dim tEv As cRCDb_EVENT_ITEM
            'For i As Integer = 0 To 20
            '    tEv = New cRCDb_EVENT_ITEM(i, i * 10, i * 10 + 20)
            '    For j As Integer = 0 To 3
            '        tEv.ACTIVE_CAST.Add(New cRCDb_CONTRIBUTOR_ITEM(j, "name: " & j))
            '    Next
            '    EVENTS.Items.Add(tEv)
            'Next
            ''DEBUGGING

            Dim MIQ As New cMovieIQ_Product(Me)
            Return MIQ.ExportXMLTo(oPath)
        Catch ex As Exception
            Throw New Exception("Problem in ExportMovieIQ(). Error: " & ex.Message, ex)
        End Try
    End Function

#End Region 'EXPORT:MOVIE IQ

#End Region 'EXPORT

End Class

'Public Class cBolinas_CastItem

'    Public Property CharacterName() As String
'        Get
'            Return _CharacterName
'        End Get
'        Set(ByVal value As String)
'            _CharacterName = value
'        End Set
'    End Property
'    Private _CharacterName As String

'    Public Property ContributorId() As String
'        Get
'            Return _ContributorId
'        End Get
'        Set(ByVal value As String)
'            _ContributorId = value
'        End Set
'    End Property
'    Private _ContributorId As String

'    Public Property ContributorName() As String
'        Get
'            Return _ContributorName
'        End Get
'        Set(ByVal value As String)
'            _ContributorName = value
'        End Set
'    End Property
'    Private _ContributorName As String

'    Public Property Billing() As Integer
'        Get
'            Return _Billing
'        End Get
'        Set(ByVal value As Integer)
'            _Billing = value
'        End Set
'    End Property
'    Private _Billing As Integer

'    Public Sub New(ByVal nContributorId As String, ByVal nContributorName As String)
'        ContributorId = nContributorId
'        ContributorName = nContributorName
'    End Sub

'    Public Function ToContributorItem() As cRCDb_CONTRIBUTOR_ITEM
'        Return New cRCDb_CONTRIBUTOR_ITEM(ContributorId, ContributorName)
'    End Function

'End Class

#Region "ENUMS"

Public Enum eBolinas_ProductType
    BD = 0
    DVD = 1
End Enum

Public Enum eBolinas_Language
    EN
    SP
    GR
    JP
    FR
End Enum

Public Enum eBolinas_EventType
    Scene
    Cast
    Music
    Fact
    Product
End Enum

#End Region 'ENUMS
