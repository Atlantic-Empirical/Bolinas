Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

#Region "CONTRIBUTORS"

Public Class cRCDb_CONTRIBUTOR_COLLECTION

    Public Items As List(Of cRCDb_CONTRIBUTOR_ITEM)
    Public ImportDateUTC As DateTime
    Public DataExtractedFromDB As DateTime

    Public Sub New()
        Items = New List(Of cRCDb_CONTRIBUTOR_ITEM)
    End Sub

    Public Sub New(ByVal nPath As String)
        Try
            Select Case Path.GetExtension(nPath).ToLower
                Case ".csv"
                    Items = New List(Of cRCDb_CONTRIBUTOR_ITEM)
                    Dim fs As New FileStream(nPath, FileMode.Open)
                    Dim sw As New StreamReader(fs)
                    sw.ReadLine()
                    While Not sw.EndOfStream
                        Items.Add(New cRCDb_CONTRIBUTOR_ITEM(sw.ReadLine))
                    End While
                    fs.Close()
                Case ".xml"
                    Dim valRes As List(Of String) = xmlHelper.Validate(nPath, eRCDb_INPUT_DATA_TYPE.CONTRIBUTOR)
                    If valRes.Count > 0 Then
                        Dim sb As New System.Text.StringBuilder
                        For Each s As String In valRes
                            sb.Append(s & vbNewLine)
                        Next
                        MsgBox("Input file XML validation failed. Import canceled." & vbNewLine & "Validation Errors: " & sb.ToString, MsgBoxStyle.Exclamation)
                        Exit Sub
                    Else
                        Items = New List(Of cRCDb_CONTRIBUTOR_ITEM)
                        Dim xd As New XmlDocument
                        xd.Load(nPath)
                        Dim rn As XmlElement = xd.Item("dataroot")
                        DataExtractedFromDB = DateTime.Parse(rn.GetAttribute("generated"))
                        For Each n As XmlNode In rn.ChildNodes
                            Items.Add(New cRCDb_CONTRIBUTOR_ITEM(CType(n, XmlElement)))
                        Next
                    End If
                Case Else
                    Throw New Exception("Invalid file format (extension).")
            End Select

            ImportDateUTC = DateTime.UtcNow
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_CONTRIBUTOR_COLLECTION. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function GetContributorById(ByVal ContributorId As String) As cRCDb_CONTRIBUTOR_ITEM
        Try
            Dim agSearchObj As New cRCDb_CONTRIBUTOR_ITEM
            agSearchObj.ContributorId = ContributorId
            Return Items.Find(New PredicateWrapper(Of cRCDb_CONTRIBUTOR_ITEM, cRCDb_CONTRIBUTOR_ITEM)(agSearchObj, AddressOf ContributorItemMatch_byWorkAndContributor))
        Catch ex As Exception
            Throw New Exception("Problem with GetContributorById(). Error: " & ex.Message, ex)
        End Try
    End Function

    Private Shared Function ContributorItemMatch_byWorkAndContributor(ByVal item As cRCDb_CONTRIBUTOR_ITEM, ByVal argument As cRCDb_CONTRIBUTOR_ITEM) As Boolean
        Return item.ContributorId = argument.ContributorId
    End Function

End Class

Public Class cRCDb_CONTRIBUTOR_ITEM

    Public Property ContributorName() As String
        Get
            Return _ContributorName
        End Get
        Set(ByVal value As String)
            _ContributorName = value
        End Set
    End Property
    Private _ContributorName As String

    Public Property CharacterName() As String
        Get
            Return _CharacterName
        End Get
        Set(ByVal value As String)
            _CharacterName = value
        End Set
    End Property
    Private _CharacterName As String

    Public Property ContributorId() As String
        Get
            Return _ContributorId
        End Get
        Set(ByVal value As String)
            _ContributorId = value
        End Set
    End Property
    Private _ContributorId As String

    Public Property Billing() As Integer
        Get
            Return _Billing
        End Get
        Set(ByVal value As Integer)
            _Billing = value
        End Set
    End Property
    Private _Billing As Integer

    Public Sub New()
    End Sub

    Public Sub New(ByRef line As String)
        Try
            Dim s() As String = Split(line, ",")
            If s.Length <> 2 Then Throw New Exception("Malformed Contributor CSV.")
            ContributorId = s(0)
            ContributorName = s(1)
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_CONTRIBUTOR_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub New(ByRef xe As XmlElement)
        Try
            ContributorId = xmlHelper.GetInnerText(xe, "ReferenceID")
            ContributorName = xmlHelper.GetInnerText(xe, "ContributorName")
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_CONTRIBUTOR_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub New(ByVal nReferenceId As Integer, ByVal nContributorName As String)
        ContributorId = nReferenceId
        ContributorName = nContributorName
    End Sub

End Class

#End Region 'CONTRIBUTORS

#Region "FILMOGRAPHY"

Public Class cRCDb_FILMOGRAPHY

    Public Items As List(Of cRCDb_FILMOGRAPHY_ITEM)
    Public ImportDateUTC As DateTime
    Public DataExtractedFromDB As DateTime

    Public Sub New()
        Items = New List(Of cRCDb_FILMOGRAPHY_ITEM)
    End Sub

    Public Sub New(ByVal nPath As String)
        Try
            Select Case Path.GetExtension(nPath).ToLower
                Case ".csv"
                    Items = New List(Of cRCDb_FILMOGRAPHY_ITEM)
                    Dim fs As New FileStream(nPath, FileMode.Open)
                    Dim sw As New StreamReader(fs)
                    sw.ReadLine()
                    While Not sw.EndOfStream
                        Items.Add(New cRCDb_FILMOGRAPHY_ITEM(sw.ReadLine))
                    End While
                    fs.Close()
                Case ".xml"
                    Dim valRes As List(Of String) = xmlHelper.Validate(nPath, eRCDb_INPUT_DATA_TYPE.FILMOGRAPHY)
                    If valRes.Count > 0 Then
                        Dim sb As New System.Text.StringBuilder
                        For Each s As String In valRes
                            sb.Append(s & vbNewLine)
                        Next
                        MsgBox("Input file XML validation failed. Import canceled." & vbNewLine & "Validation Errors: " & sb.ToString, MsgBoxStyle.Exclamation)
                        Exit Sub
                    Else
                        Items = New List(Of cRCDb_FILMOGRAPHY_ITEM)
                        Dim xd As New XmlDocument
                        xd.Load(nPath)
                        Dim rn As XmlElement = xd.Item("dataroot")
                        DataExtractedFromDB = DateTime.Parse(rn.GetAttribute("generated"))
                        For Each n As XmlNode In rn.ChildNodes
                            Items.Add(New cRCDb_FILMOGRAPHY_ITEM(CType(n, XmlElement)))
                        Next
                    End If
                Case Else
                    Throw New Exception("Invalid file format (extension).")
            End Select

            ImportDateUTC = DateTime.UtcNow

        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FILMOGRAPHY. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function FindPerson(ByVal WorkId As String, ByVal ContributorId As String) As cRCDb_FILMOGRAPHY_ITEM
        Dim agSearchObj As New cRCDb_FILMOGRAPHY_ITEM
        agSearchObj.WorkId = WorkId
        agSearchObj.ContributorId = ContributorId
        Return Items.Find(New PredicateWrapper(Of cRCDb_FILMOGRAPHY_ITEM, cRCDb_FILMOGRAPHY_ITEM)(agSearchObj, AddressOf FilmographyItemMatch_byWorkAndContributor))
    End Function

    Public Function GetPersonFilmographyData(ByVal ContributorId As String) As List(Of cRCDb_FILMOGRAPHY_ITEM)
        Dim agSearchObj As New cRCDb_FILMOGRAPHY_ITEM
        agSearchObj.ContributorId = ContributorId
        Return Items.FindAll(New PredicateWrapper(Of cRCDb_FILMOGRAPHY_ITEM, cRCDb_FILMOGRAPHY_ITEM)(agSearchObj, AddressOf FilmographyItemMatch_byContributor))
    End Function

    Public Function GetCastForWork(ByVal WorkId As String) As List(Of cRCDb_FILMOGRAPHY_ITEM)
        Dim agSearchObj As New cRCDb_FILMOGRAPHY_ITEM
        agSearchObj.WorkId = WorkId
        Return Items.FindAll(New PredicateWrapper(Of cRCDb_FILMOGRAPHY_ITEM, cRCDb_FILMOGRAPHY_ITEM)(agSearchObj, AddressOf FilmographyItemMatch_byWork))
    End Function

#Region "PRIVATE"

    Private Shared Function FilmographyItemMatch_byWorkAndContributor(ByVal item As cRCDb_FILMOGRAPHY_ITEM, ByVal argument As cRCDb_FILMOGRAPHY_ITEM) As Boolean
        Return item.WorkId = argument.WorkId And item.ContributorId = argument.ContributorId
    End Function

    Private Shared Function FilmographyItemMatch_byContributor(ByVal item As cRCDb_FILMOGRAPHY_ITEM, ByVal argument As cRCDb_FILMOGRAPHY_ITEM) As Boolean
        Return item.ContributorId = argument.ContributorId
    End Function

    Private Shared Function FilmographyItemMatch_byWork(ByVal item As cRCDb_FILMOGRAPHY_ITEM, ByVal argument As cRCDb_FILMOGRAPHY_ITEM) As Boolean
        Return item.WorkId = argument.WorkId
    End Function

#End Region 'PRIVATE

End Class

Public Class cRCDb_FILMOGRAPHY_ITEM

    Public Property Id() As Integer
        Get
            Return _Id
        End Get
        Set(ByVal value As Integer)
            _Id = value
        End Set
    End Property
    Private _Id As Integer

    Public Property ContributorId() As Integer
        Get
            Return _ContributorId
        End Get
        Set(ByVal value As Integer)
            _ContributorId = value
        End Set
    End Property
    Private _ContributorId As Integer

    Public Property CharacterName() As String
        Get
            Return _CharacterName
        End Get
        Set(ByVal value As String)
            _CharacterName = value
        End Set
    End Property
    Private _CharacterName As String

    Public Property Role() As String 'eRCDb_Role
        Get
            Return _Role
        End Get
        Set(ByVal value As String) 'eRCDb_Role
            _Role = value
        End Set
    End Property
    Private _Role As String 'eRCDb_Role

    Public Property WorkId() As String
        Get
            Return _WorkId
        End Get
        Set(ByVal value As String)
            _WorkId = value
        End Set
    End Property
    Private _WorkId As String

    Public Property WorkDate() As Date
        Get
            Return _WorkDate
        End Get
        Set(ByVal value As Date)
            _WorkDate = value
        End Set
    End Property
    Private _WorkDate As Date

    Public Property WorkStudio() As String
        Get
            Return _WorkStudio
        End Get
        Set(ByVal value As String)
            _WorkStudio = value
        End Set
    End Property
    Private _WorkStudio As String

    Public Property WorkName() As String
        Get
            Return _WorkName
        End Get
        Set(ByVal value As String)
            _WorkName = value
        End Set
    End Property
    Private _WorkName As String

    Public Sub New()
    End Sub

    Public Sub New(ByRef line As String)
        Try
            Dim s() As String = Split(line, ",")
            If s.Length <> 7 Then Throw New Exception("Malformed Filmography CSV.")
            Id = s(0)
            ContributorId = s(1)
            CharacterName = s(2)
            Role = s(3) '[Enum].Parse(GetType(eRCDb_Role), s(3).Replace(" ", "_"))
            WorkId = s(4)
            WorkDate = DateTime.Parse(s(5))
            WorkStudio = s(6)
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FILMOGRAPHY_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub New(ByRef xe As XmlElement)
        Try
            Id = xmlHelper.GetInnerText(xe, "ID")
            ContributorId = xmlHelper.GetInnerText(xe, "Contributor_ID")
            CharacterName = xmlHelper.GetInnerText(xe, "CharacterName")
            Role = xmlHelper.GetInnerText(xe, "Role")
            WorkId = xmlHelper.GetInnerText(xe, "WorkID")
            WorkDate = [Date].Parse(xmlHelper.GetInnerText(xe, "WorkDate"))
            WorkStudio = xmlHelper.GetInnerText(xe, "WorkStudio")
            WorkName = xmlHelper.GetInnerText(xe, "WorkName")
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FILMOGRAPHY_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

#End Region 'FILMOGRAPHY

#Region "SOUNDTRACK"

Public Class cRCDb_SOUNDTRACK

    Public Items As List(Of cRCDb_MUSIC_ITEM)
    Public ImportDateUTC As DateTime
    Public DataExtractedFromDB As DateTime

    Public Sub New()
        Items = New List(Of cRCDb_MUSIC_ITEM)
    End Sub

    Public Sub New(ByVal nPath As String)
        Try
            Select Case Path.GetExtension(nPath).ToLower
                Case ".csv"
                    Items = New List(Of cRCDb_MUSIC_ITEM)
                    Dim fs As New FileStream(nPath, FileMode.Open)
                    Dim sw As New StreamReader(fs)
                    sw.ReadLine()
                    While Not sw.EndOfStream
                        Items.Add(New cRCDb_MUSIC_ITEM(sw.ReadLine))
                    End While
                    fs.Close()
                Case ".xml"
                    Dim valRes As List(Of String) = xmlHelper.Validate(nPath, eRCDb_INPUT_DATA_TYPE.SOUNDTRACK)
                    If valRes.Count > 0 Then
                        Dim sb As New System.Text.StringBuilder
                        For Each s As String In valRes
                            sb.Append(s & vbNewLine)
                        Next
                        MsgBox("Input file XML validation failed. Import canceled." & vbNewLine & "Validation Errors: " & sb.ToString, MsgBoxStyle.Exclamation)
                        Exit Sub
                    Else
                        Items = New List(Of cRCDb_MUSIC_ITEM)
                        Dim xd As New XmlDocument
                        xd.Load(nPath)
                        Dim rn As XmlElement = xd.Item("dataroot")
                        DataExtractedFromDB = DateTime.Parse(rn.GetAttribute("generated"))
                        For Each n As XmlNode In rn.ChildNodes
                            Items.Add(New cRCDb_MUSIC_ITEM(CType(n, XmlElement)))
                        Next
                    End If
                Case Else
                    Throw New Exception("Invalid file format (extension).")
            End Select

            ImportDateUTC = DateTime.UtcNow
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_SOUNDTRACK. Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

Public Class cRCDb_MUSIC_ITEM

    <XmlIgnore()> _
    Public ReadOnly Property IsInitalized()
        Get
            Return Not String.IsNullOrEmpty(Title)
        End Get
    End Property

    Public Property Id() As Integer
        Get
            Return _Id
        End Get
        Set(ByVal value As Integer)
            _Id = value
        End Set
    End Property
    Private _Id As Integer

    Public Property Album() As String
        Get
            Return _Album
        End Get
        Set(ByVal value As String)
            _Album = value
        End Set
    End Property
    Private _Album As String

    Public Property Artist() As String
        Get
            Return _Artist
        End Get
        Set(ByVal value As String)
            _Artist = value
        End Set
    End Property
    Private _Artist As String

    Public Property Time() As Integer
        Get
            Return _Time
        End Get
        Set(ByVal value As Integer)
            _Time = value
        End Set
    End Property
    Private _Time As Integer

    Public Property Label() As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
        End Set
    End Property
    Private _Label As String

    ''' <summary>
    ''' This seems not to be a date but, instead, a year.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property ReleaseDate() As Short
        Get
            Return _ReleaseDate
        End Get
        Set(ByVal value As Short)
            _ReleaseDate = value
        End Set
    End Property
    Private _ReleaseDate As Short

    Public Property ReferenceId() As Integer
        Get
            Return _ReferenceId
        End Get
        Set(ByVal value As Integer)
            _ReferenceId = value
        End Set
    End Property
    Private _ReferenceId As Integer

    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
        End Set
    End Property
    Private _Title As String

    ''' <summary>
    ''' Again, this should be renamed ParentWorkId.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property WorkId() As Integer
        Get
            Return _WorkId
        End Get
        Set(ByVal value As Integer)
            _WorkId = value
        End Set
    End Property
    Private _WorkId As Integer

    Public Property Notes() As String
        Get
            Return _Notes
        End Get
        Set(ByVal value As String)
            _Notes = value
        End Set
    End Property
    Private _Notes As String

    Public Sub New()
    End Sub

    Public Sub New(ByRef line As String)
        Try
            Dim s() As String = Split(line, ",")
            If s.Length <> 10 Then Throw New Exception("Malformed Music CSV.")
            Id = s(0)
            Album = s(1)
            Artist = s(2)
            Time = s(3)
            Label = s(4)
            ReleaseDate = s(5)
            ReferenceId = s(6)
            Title = s(7)
            WorkId = s(8)
            Notes = s(9)
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_MUSIC_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub New(ByRef xe As XmlElement)
        Try
            Id = xmlHelper.GetInnerText(xe, "ID")
            Album = xmlHelper.GetInnerText(xe, "Album")
            Artist = xmlHelper.GetInnerText(xe, "Artist")
            Time = xmlHelper.GetInnerText(xe, "Time")
            Label = xmlHelper.GetInnerText(xe, "Label")
            ReleaseDate = xmlHelper.GetInnerText(xe, "ReleaseDate")
            ReferenceId = xmlHelper.GetInnerText(xe, "ReferenceID")
            Title = xmlHelper.GetInnerText(xe, "Title")
            WorkId = xmlHelper.GetInnerText(xe, "WorkID")
            Notes = xmlHelper.GetInnerText(xe, "Notes")
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_MUSIC_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

#End Region 'SOUNDTRACK

#Region "FACTS"

Public Class cRCDb_FACTOGRAPHY

    Public Items As List(Of cRCDb_FACT_ITEM)
    Public ImportDateUTC As DateTime
    Public DataExtractedFromDB As DateTime

    Public Sub New()
        Items = New List(Of cRCDb_FACT_ITEM)
    End Sub

    Public Sub New(ByVal nPath As String)
        Try
            Select Case Path.GetExtension(nPath).ToLower
                Case ".csv"
                    Items = New List(Of cRCDb_FACT_ITEM)
                    Dim fs As New FileStream(nPath, FileMode.Open)
                    Dim sw As New StreamReader(fs)
                    sw.ReadLine()
                    While Not sw.EndOfStream
                        Items.Add(New cRCDb_FACT_ITEM(sw.ReadLine))
                    End While
                    fs.Close()
                Case ".xml"
                    Dim valRes As List(Of String) = xmlHelper.Validate(nPath, eRCDb_INPUT_DATA_TYPE.FACTS)
                    If valRes.Count > 0 Then
                        Dim sb As New System.Text.StringBuilder
                        For Each s As String In valRes
                            sb.Append(s & vbNewLine)
                        Next
                        MsgBox("Input file XML validation failed. Import canceled." & vbNewLine & "Validation Errors: " & sb.ToString, MsgBoxStyle.Exclamation)
                        Exit Sub
                    Else
                        Items = New List(Of cRCDb_FACT_ITEM)
                        Dim xd As New XmlDocument
                        xd.Load(nPath)
                        Dim rn As XmlElement = xd.Item("dataroot")
                        DataExtractedFromDB = DateTime.Parse(rn.GetAttribute("generated"))
                        For Each n As XmlNode In rn.ChildNodes
                            Items.Add(New cRCDb_FACT_ITEM(CType(n, XmlElement)))
                        Next
                    End If
                Case Else
                    Throw New Exception("Invalid file format (extension).")
            End Select

            ImportDateUTC = DateTime.UtcNow
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FACTOGRAPHY. Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

Public Class cRCDb_FACT_ITEM

    Public Property Id() As Integer
        Get
            Return _Id
        End Get
        Set(ByVal value As Integer)
            _Id = value
        End Set
    End Property
    Private _Id As Integer

    Public Property FactReference() As String
        Get
            Return _FactReference
        End Get
        Set(ByVal value As String)
            _FactReference = value
        End Set
    End Property
    Private _FactReference As String

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property
    Private _Description As String

    Public Property Generic() As Boolean
        Get
            Return _Generic
        End Get
        Set(ByVal value As Boolean)
            _Generic = value
        End Set
    End Property
    Private _Generic As Boolean

    Public Property FactReferenceType() As eRCDb_FactReferenceType
        Get
            Return _FactReferenceType
        End Get
        Set(ByVal value As eRCDb_FactReferenceType)
            _FactReferenceType = value
        End Set
    End Property
    Private _FactReferenceType As eRCDb_FactReferenceType

    Public Property FactReferenceId() As String
        Get
            Return _FactReferenceId
        End Get
        Set(ByVal value As String)
            _FactReferenceId = value
        End Set
    End Property
    Private _FactReferenceId As String

    Public Property Source1() As String
        Get
            Return _Source1
        End Get
        Set(ByVal value As String)
            _Source1 = value
        End Set
    End Property
    Private _Source1 As String

    Public Property Source2() As String
        Get
            Return _Source2
        End Get
        Set(ByVal value As String)
            _Source2 = value
        End Set
    End Property
    Private _Source2 As String

    Public Property Source3() As String
        Get
            Return _Source3
        End Get
        Set(ByVal value As String)
            _Source3 = value
        End Set
    End Property
    Private _Source3 As String

    Public Sub New()
    End Sub

    Public Sub New(ByRef line As String)
        Try
            Dim s() As String = Split(line, ",")
            If s.Length <> 9 Then Throw New Exception("Malformed Fact CSV.")
            Id = s(0)
            FactReference = s(1)
            Description = s(2).Replace("@@@", ",")
            [Generic] = Boolean.Parse(s(3))
            FactReferenceType = [Enum].Parse(GetType(eRCDb_FactReferenceType), s(4).ToUpper)
            FactReferenceId = s(5)
            Source1 = s(6)
            Source2 = s(7)
            Source3 = s(8)
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FACT_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub New(ByRef xe As XmlElement)
        Try
            Id = xmlHelper.GetInnerText(xe, "FactID")
            FactReference = xmlHelper.GetInnerText(xe, "ReferenceID")
            Description = xmlHelper.GetInnerText(xe, "Description")
            [Generic] = Boolean.Parse(xmlHelper.GetInnerText(xe, "Generic").Replace("YES", "TRUE").Replace("NO", "FALSE"))
            FactReferenceType = [Enum].Parse(GetType(eRCDb_FactReferenceType), xmlHelper.GetInnerText(xe, "FactReferenceType").ToUpper)
            FactReferenceId = xmlHelper.GetInnerText(xe, "FactReferenceID")
            Source1 = xmlHelper.GetInnerText(xe, "Source1")
            Source2 = xmlHelper.GetInnerText(xe, "Source2")
            Source3 = xmlHelper.GetInnerText(xe, "Source3")
        Catch ex As Exception
            Throw New Exception("Problem in New() cRCDb_FACT_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

#End Region 'FACTS

#Region "EVENTS"

Public Class cRCDb_EVENT_CONTAINER

    Public DVDTitleNumber As Integer
    Public DVDTitleDuration As Integer
    Public RunningTimeOffset As Integer = 0

    Public Scenes As cRCDb_EVENT_COLLECTION
    Public EventSets As List(Of cRCDb_EVENT_COLLECTION)
    Public Event RefreshDisplay()

    Public Sub New()
        Scenes = New cRCDb_EVENT_COLLECTION("SCENES")
        EventSets = New List(Of cRCDb_EVENT_COLLECTION)
    End Sub

    Public Property ActiveSet() As cRCDb_EVENT_COLLECTION
        Get
            If Scenes.IsSelected Then Return Scenes
            For Each es As cRCDb_EVENT_COLLECTION In EventSets
                If es.IsSelected Then Return es
            Next
            Return Nothing
        End Get
        Set(ByVal value As cRCDb_EVENT_COLLECTION)
            DeselectAllSetsAndScenes()
            SetItemActive(value, True)
        End Set
    End Property

    Public Sub NewSet(ByVal Name As String)
        EventSets.Add(New cRCDb_EVENT_COLLECTION(Name))
    End Sub

    ''' <summary>
    ''' Outputs MovieIQ XML for the EVENTS node
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AttachXMLNodes_MovieIQ(ByRef XE As XmlElement, ByVal RunningTimeOffset As Integer)
        Dim OutputItems As New cRCDb_EVENT_COLLECTION
        JoinEventCollections(OutputItems, Scenes)
        For Each s As cRCDb_EVENT_COLLECTION In EventSets
            JoinEventCollections(OutputItems, s)
        Next
        'go through the output collection adding nodes
        For Each e As cRCDb_EVENT_ITEM In OutputItems.ItemsSorted
            XE.AppendChild(e.ToXMLElement_MovieIQ(XE.OwnerDocument, RunningTimeOffset))
        Next
    End Sub

    Private Shared Sub JoinEventCollections(ByRef CoreCollection As cRCDb_EVENT_COLLECTION, ByRef JoinCollection As cRCDb_EVENT_COLLECTION)
        Try
            Dim tEM As cRCDb_EVENT_ITEM
            For Each e As cRCDb_EVENT_ITEM In JoinCollection.ItemsSorted
                tEM = CoreCollection.Items.Find(New PredicateWrapper(Of cRCDb_EVENT_ITEM, cRCDb_EVENT_ITEM)(e, AddressOf EventItemMatch_byStartAndEndTimes))
                If tEM Is Nothing Then
                    CoreCollection.Items.Add(e)
                Else
                    'tEM.SCENE ' do nothing with SCENE on output for MovieIQ
                    tEM.CAST.AddRange(e.CAST)
                    tEM.SOUNDTRACK = e.SOUNDTRACK
                    tEM.FACTS.AddRange(e.FACTS)
                End If
            Next
        Catch ex As Exception
            Throw New Exception("Problem with JoinEventCollections(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub SetScenesActive(ByVal Active As Boolean)
        DeselectAllSetsAndScenes()
        Scenes.IsSelected = Active
    End Sub

    Public Sub SetItemActive(ByRef ES As cRCDb_EVENT_COLLECTION, ByVal Active As Boolean)
        DeselectAllSetsAndScenes()
        Dim i As Integer = EventSets.IndexOf(ES)
        If i = -1 Then
            Scenes.IsSelected = Active
        Else
            EventSets(i).IsSelected = Active
        End If
    End Sub

    Public Sub DeselectAllSetsAndScenes()
        Scenes.IsSelected = False
        For Each es As cRCDb_EVENT_COLLECTION In EventSets
            es.IsSelected = False
        Next
    End Sub

    Public Sub DeleteActiveSet()
        If ActiveSet.IsScenes Then
            MsgBox("Cannot delete SCENES.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation)
            Exit Sub
        Else
            EventSets.Remove(ActiveSet)
        End If
    End Sub

    Public Function GetEventSetByName(ByVal SearchName As String) As cRCDb_EVENT_COLLECTION
        If SearchName = "SCENES" Then Return Scenes
        For Each s As cRCDb_EVENT_COLLECTION In EventSets
            If SearchName = s.Name Then
                Return s
            End If
        Next
        Return Nothing
    End Function

    Public Sub ForceRefresh()
        RaiseEvent RefreshDisplay()
    End Sub

    Private Shared Function EventItemMatch_byStartAndEndTimes(ByVal item As cRCDb_EVENT_ITEM, ByVal item2 As cRCDb_EVENT_ITEM) As Boolean
        Return item.Start = item2.Start And item.End = item2.End
    End Function

    Private Shared Function EventItemMatch_byCurrentRunningTimeSeconds(ByVal item As cRCDb_EVENT_ITEM, ByVal nCurrentRunningTimeSeconds As Integer) As Boolean
        Return item.Start <= nCurrentRunningTimeSeconds And item.End >= nCurrentRunningTimeSeconds
    End Function

    Public Function GetActiveEvents(ByVal CRT As Integer) As List(Of cRCDb_EVENT_ITEM)
        Dim out As New List(Of cRCDb_EVENT_ITEM)
        'SCENES
        out.AddRange(Scenes.Items.FindAll(New PredicateWrapper(Of cRCDb_EVENT_ITEM, Integer)(CRT, AddressOf EventItemMatch_byCurrentRunningTimeSeconds)))
        'OTHER SETS
        For Each es As cRCDb_EVENT_COLLECTION In EventSets
            out.AddRange(es.Items.FindAll(New PredicateWrapper(Of cRCDb_EVENT_ITEM, Integer)(CRT, AddressOf EventItemMatch_byCurrentRunningTimeSeconds)))
        Next
        Return out
    End Function

    Public ReadOnly Property SceneCoverage_Percent() As Byte
        Get
            If DVDTitleDuration = Nothing OrElse DVDTitleDuration = -1 Then Return 0
            Dim SecondsCovered As Short = 0
            For Each e As cRCDb_EVENT_ITEM In Scenes.ItemsSorted
                SecondsCovered += e.End - e.Start
            Next
            Dim d As Double = SecondsCovered / DVDTitleDuration
            Return Math.Round(d * 100, 0)

            'Dim last As Integer = 0
            'Dim d As Double
            'Dim i As Integer
            'If Scenes.ItemsSorted(0).Start > 0 Then Return 0
            'For Each e As cRCDb_EVENT_ITEM In Scenes.ItemsSorted
            '    If e.Start > last + 1 Then
            '        'we've found a discontinuity
            '        d = e.End / DVDTitleDuration
            '        i = Math.Round(d * 100, 0)
            '        Return CByte(i)
            '    Else
            '        last = e.End
            '    End If
            'Next
            'd = last / DVDTitleDuration
            'i = Math.Round(d * 100, 0)
            'Return CByte(i)
        End Get
    End Property

    Public ReadOnly Property SceneCoverage_Minutes() As Short
        Get
            If SceneCoverage_Percent = 0 Then Return 0
            If SceneCoverage_Percent = 100 Then Return DVDTitleDuration
            Return ((SceneCoverage_Percent / 100) * DVDTitleDuration) / 60
        End Get
    End Property

    Public ReadOnly Property CastCount() As Short
        Get
            Dim out As Short = 0
            For Each es As cRCDb_EVENT_COLLECTION In EventSets
                For Each ei As cRCDb_EVENT_ITEM In es.Items
                    For Each ci As cRCDb_CONTRIBUTOR_ITEM In ei.CAST
                        out += 1
                    Next
                Next
            Next
            Return out
        End Get
    End Property

    Public ReadOnly Property MusicTrackCount() As Short
        Get
            Dim out As Short = 0
            For Each es As cRCDb_EVENT_COLLECTION In EventSets
                For Each ei As cRCDb_EVENT_ITEM In es.Items
                    If ei.SOUNDTRACK.IsInitalized Then
                        out += 1
                    End If
                Next
            Next
            Return out
        End Get
    End Property

    Public ReadOnly Property FactCount() As Short
        Get
            Dim out As Short = 0
            For Each es As cRCDb_EVENT_COLLECTION In EventSets
                For Each ei As cRCDb_EVENT_ITEM In es.Items
                    For Each ci As cRCDb_FACT_ITEM In ei.FACTS
                        out += 1
                    Next
                Next
            Next
            Return out
        End Get
    End Property

End Class

Public Class cRCDb_EVENT_COLLECTION

    Public Name As String
    Public Items As List(Of cRCDb_EVENT_ITEM)


    <XmlIgnore()> _
    Public Property IsSelected() As Boolean
        Get
            Return _IsSelected
        End Get
        Set(ByVal value As Boolean)
            _IsSelected = value
            RaiseEvent Selection(_IsSelected)
        End Set
    End Property
    <XmlIgnore()> _
    Private _IsSelected As Boolean = False
    Public Event Selection(ByVal IsSelected As Boolean)

    <XmlIgnore()> _
    Public ReadOnly Property IsScenes() As Boolean
        Get
            If String.IsNullOrEmpty(Name) Then Return False
            Return Name.ToLower = "scenes"
        End Get
    End Property

    <XmlIgnore()> _
    Public ReadOnly Property ItemsSorted() As List(Of cRCDb_EVENT_ITEM)
        Get
            'Dim out As List(Of cRCDb_SCENE_ITEM) = Items
            'out.OrderBy(cRCDb_SCENE_ITEM >= cRCDb_SCENE_ITEM.StartTime)
            Dim q = From p In Items Order By p.Start
            'Dim s = Expression.Parameter(GetType(cRCDb_SCENE_ITEM), "p")
            'Dim x = Expression.Lambda(Of Func(Of Person, DateTime))(Expression.[Property](s, "DateOfBirth"), s)
            'q = q.OrderBy(x.Compile())
            Return q.ToList
        End Get
    End Property

    Public Sub New()
        Items = New List(Of cRCDb_EVENT_ITEM)
    End Sub

    Public Sub New(ByVal nName As String)
        Name = nName
        Items = New List(Of cRCDb_EVENT_ITEM)
    End Sub

    Public Function EventTimesOverlap(ByVal St As Short, ByVal En As Short, Optional ByVal IgnoreSelf As cRCDb_EVENT_ITEM = Nothing) As Boolean
        For Each e As cRCDb_EVENT_ITEM In Items
            If IgnoreSelf IsNot Nothing Then
                If e Is IgnoreSelf Then GoTo NextItem
            End If
            If St >= e.Start And St <= e.End Then Return True
            If En >= e.Start And En <= e.End Then Return True
NextItem:
        Next
        Return False
    End Function

    Public Function ContainsScene(ByRef Sc As cRCDb_SCENE_ITEM) As Boolean
        For Each i As cRCDb_EVENT_ITEM In Items
            If i.SCENE.IsInitalized Then
                If i.SCENE.Id = Sc.Id Then Return True
            End If
        Next
        Return False
    End Function

    Public Function GetEndTime(ByVal s As Integer, ByVal TitleDuration As Integer) As Integer
        For i As Integer = 0 To ItemsSorted.Count - 1
            If ItemsSorted(i).Start > s Then
                'this is the first item, from the beginning that starts after our item, so its start time is our new end time
                If i > 0 Then
                    If i > 0 And ItemsSorted(i - 1).End >= s Then ItemsSorted(i - 1).End = s - 1 'trim the previous event
                End If
                Return ItemsSorted(i).Start - 1
            End If
        Next
        'this is the only or last item in the set
        If ItemsSorted.Count > 0 Then
            If ItemsSorted(ItemsSorted.Count - 1).End >= s Then ItemsSorted(ItemsSorted.Count - 1).End = s - 1 'trim the previous event
        End If
        Return TitleDuration
    End Function

    Public ReadOnly Property CurrentItem(ByVal crt As Integer) As cRCDb_EVENT_ITEM
        Get
            For Each ev As cRCDb_EVENT_ITEM In ItemsSorted
                If ev.Start <= crt And ev.End >= crt Then Return ev
            Next
            Return Nothing
        End Get
    End Property

End Class

Public Class cRCDb_EVENT_ITEM

    Public Property Id() As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
        End Set
    End Property
    Private _Id As String

    Public Property Start() As Integer
        Get
            Return _Start
        End Get
        Set(ByVal value As Integer)
            _Start = value
        End Set
    End Property
    Private _Start As Integer

    Public Property [End]() As Integer
        Get
            Return _End
        End Get
        Set(ByVal value As Integer)
            _End = value
        End Set
    End Property
    Private _End As Integer

    Public SCENE As cRCDb_SCENE_ITEM

    ''' <summary>
    ''' CONTRIBUTOR Ids
    ''' </summary>
    ''' <remarks></remarks>
    Public CAST As List(Of cRCDb_CONTRIBUTOR_ITEM)

    ''' <summary>
    ''' MUSIC_ITEM Ids
    ''' </summary>
    ''' <remarks></remarks>
    Public SOUNDTRACK As cRCDb_MUSIC_ITEM 'List(Of cRCDb_MUSIC_ITEM)

    ''' <summary>
    ''' FACT Ids
    ''' </summary>
    ''' <remarks></remarks>
    Public FACTS As List(Of cRCDb_FACT_ITEM)

    Public Sub New()
        SCENE = New cRCDb_SCENE_ITEM
        CAST = New List(Of cRCDb_CONTRIBUTOR_ITEM)
        FACTS = New List(Of cRCDb_FACT_ITEM)
        SOUNDTRACK = New cRCDb_MUSIC_ITEM 'New List(Of cRCDb_MUSIC_ITEM)
    End Sub

    Public Sub New(ByVal nId As String, ByVal nStart As String, ByVal nEnd As String)
        SCENE = New cRCDb_SCENE_ITEM
        CAST = New List(Of cRCDb_CONTRIBUTOR_ITEM)
        FACTS = New List(Of cRCDb_FACT_ITEM)
        SOUNDTRACK = New cRCDb_MUSIC_ITEM 'New List(Of cRCDb_MUSIC_ITEM)
        If String.IsNullOrEmpty(nId) Then
            Dim r As New Random
            Id = r.Next
        Else
            Id = nId
        End If
        Start = nStart
        [End] = nEnd
    End Sub

    'Public Sub New(ByVal s As cRCDb_SCENE_ITEM)
    '    Id = s.Id.ToString
    '    Start = s.StartTime
    '    [End] = s.EndTime
    '    ACTIVE_CAST = New List(Of cRCDb_CONTRIBUTOR_ITEM)
    '    ACTIVE_FACTS = New List(Of cRCDb_FACT_ITEM)
    '    ACTIVE_SOUNDTRACK = New List(Of cRCDb_MUSIC_ITEM)

    '    For Each c As cRCDb_CONTRIBUTOR_ITEM In s.ACTIVE_CAST
    '        REFERENCES.Add(New cRCDb_EVENT_REFERENCE(c))
    '    Next
    '    For Each t As cRCDb_MUSIC_ITEM In s.ACTIVE_SOUNDTRACK
    '        REFERENCES.Add(New cRCDb_EVENT_REFERENCE(t))
    '    Next
    '    For Each f As cRCDb_FACT_ITEM In s.ACTIVE_FACTS
    '        REFERENCES.Add(New cRCDb_EVENT_REFERENCE(f))
    '    Next
    'End Sub

    Public Function ToXMLElement_MovieIQ(ByRef xd As XmlDocument, ByVal Offset As Integer) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("event")
            out.SetAttribute("id", Id)
            out.SetAttribute("start", Start + Offset)
            out.SetAttribute("end", [End] + Offset)

            Dim tC As cRCDb_CONTRIBUTOR_ITEM
            For Each c As cRCDb_CONTRIBUTOR_ITEM In Me.CAST
                tC = New cRCDb_CONTRIBUTOR_ITEM(c.ContributorId, c.ContributorName)
                out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(tC)).ToXMLElement(xd))
            Next

            For Each f As cRCDb_FACT_ITEM In Me.FACTS
                out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(f)).ToXMLElement(xd))
            Next

            'For Each m As cRCDb_MUSIC_ITEM In Me.SOUNDTRACK
            '    out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(m)).ToXMLElement(xd))
            'Next
            out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(SOUNDTRACK)).ToXMLElement(xd))

            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement_MovieIQ(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Sub AddReference(ByRef o As IDataObject, ByRef EventSet As cRCDb_EVENT_COLLECTION)
        Dim Types() As String = o.GetFormats()
        Select Case Types(0).ToLower
            Case "scene"
                If SCENE.IsInitalized Then
                    If MessageBox.Show("Replace scene?" & vbNewLine & "Old scene = " & SCENE.Name, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
                End If
                Dim s As cRCDb_SCENE_ITEM = CType(o.GetData("scene"), cRCDb_SCENE_ITEM)
                If EventSet.ContainsScene(s) Then
                    MessageBox.Show("This scene is already used in the SCENES set. Association failed.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
                    Exit Sub
                End If
                SCENE = s
            Case "cast"
                CAST.Add(CType(o.GetData("cast"), cRCDb_CONTRIBUTOR_ITEM))
            Case "music"
                If SOUNDTRACK.IsInitalized Then
                    If MessageBox.Show("Replace track?" & vbNewLine & "Old track = " & SOUNDTRACK.Title, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
                End If
                SOUNDTRACK = CType(o.GetData("music"), cRCDb_MUSIC_ITEM)
            Case "fact"
                FACTS.Add(CType(o.GetData("fact"), cRCDb_FACT_ITEM))
            Case Else
                Throw New Exception("Unexpected. The drop data type is not supported.")
        End Select
    End Sub

    Public Sub RemoveAllReferences()
        SCENE = New cRCDb_SCENE_ITEM
        CAST = New List(Of cRCDb_CONTRIBUTOR_ITEM)
        FACTS = New List(Of cRCDb_FACT_ITEM)
        SOUNDTRACK = New cRCDb_MUSIC_ITEM 'New List(Of cRCDb_MUSIC_ITEM)
    End Sub

End Class

Public Class cRCDb_EVENT_REFERENCE

    Public Property Id() As String
        Get
            Return _Id
        End Get
        Set(ByVal value As String)
            _Id = value
        End Set
    End Property
    Private _Id As String

    Public Property Type() As eRCDb_EventReferenceType
        Get
            Return _Type
        End Get
        Set(ByVal value As eRCDb_EventReferenceType)
            _Type = value
        End Set
    End Property
    Private _Type As eRCDb_EventReferenceType

    Public Sub New()
    End Sub

    Public Sub New(ByVal nId As String, ByVal nType As eRCDb_EventReferenceType)
        Id = nId
        Type = nType
    End Sub

    Public Sub New(ByRef c As cRCDb_CONTRIBUTOR_ITEM)
        Id = c.ContributorId
        Type = eRCDb_EventReferenceType.CONTRIBUTOR
    End Sub

    Public Sub New(ByRef t As cRCDb_MUSIC_ITEM)
        Id = t.Id
        Type = eRCDb_EventReferenceType.TRACK
    End Sub

    Public Sub New(ByRef f As cRCDb_FACT_ITEM)
        Id = f.Id
        Type = eRCDb_EventReferenceType.MOVIE_FACT
    End Sub

End Class

#End Region 'EVENTS

#Region "SCENES"

Public Class cRCDb_SCENE_COLLECTION

    Public Items As List(Of cRCDb_SCENE_ITEM)

    '<XmlIgnore()> _
    'Public ReadOnly Property ItemsSorted() As List(Of cRCDb_SCENE_ITEM)
    '    Get
    '        'Dim out As List(Of cRCDb_SCENE_ITEM) = Items
    '        'out.OrderBy(cRCDb_SCENE_ITEM >= cRCDb_SCENE_ITEM.StartTime)
    '        Dim q = From p In Items Order By p.StartTime
    '        'Dim s = Expression.Parameter(GetType(cRCDb_SCENE_ITEM), "p")
    '        'Dim x = Expression.Lambda(Of Func(Of Person, DateTime))(Expression.[Property](s, "DateOfBirth"), s)
    '        'q = q.OrderBy(x.Compile())
    '        Return q.ToList
    '    End Get
    'End Property

    Public Sub New()
        Items = New List(Of cRCDb_SCENE_ITEM)
    End Sub

    'Public Sub InitializePostDeserialize()
    '    For Each s As cRCDb_SCENE_ITEM In Items
    '        s.Parent = Me
    '    Next
    'End Sub

    'Public Function FindScene(ByVal nCurrentRunningTimeSeconds As Integer) As cRCDb_SCENE_ITEM
    '    Try
    '        Return Items.Find(New PredicateWrapper(Of cRCDb_SCENE_ITEM, Integer)(nCurrentRunningTimeSeconds, AddressOf SceneItemMatch_byCurrentRunningTimeSeconds))
    '    Catch ex As Exception
    '        Throw New Exception("Problem with FindScene(). Error: " & ex.Message, ex)
    '    End Try
    'End Function

    'Private Shared Function SceneItemMatch_byCurrentRunningTimeSeconds(ByVal item As cRCDb_SCENE_ITEM, ByVal nCurrentRunningTimeSeconds As Integer) As Boolean
    '    Return item.StartTime <= nCurrentRunningTimeSeconds And item.EndTime >= nCurrentRunningTimeSeconds
    'End Function

    'Public Function GetNewSceneStartTime(ByVal nCurrentRunningTimeInSec As Integer) As Integer
    '    If Items.Count = 0 Then Return 0 'its the first
    '    Dim its As List(Of cRCDb_SCENE_ITEM) = ItemsSorted
    '    For i As Integer = its.Count - 1 To 0 Step -1
    '        If nCurrentRunningTimeInSec = its(i).StartTime Then
    '            Throw New Exception("Scene already exists at this start time.")
    '        Else
    '            If nCurrentRunningTimeInSec < its(i).StartTime Then
    '                its(i).EndTime = nCurrentRunningTimeInSec - 1
    '                If i = 0 Then
    '                    'it's the new first item
    '                    Return 0
    '                Else
    '                    Return nCurrentRunningTimeInSec
    '                End If
    '            End If
    '        End If
    '    Next
    '    Return nCurrentRunningTimeInSec
    'End Function

    'Public Function GetNewSceneEndTime(ByVal nNewSceneStartTime As Integer, ByVal nTitleDurInSec As Short) As Integer
    '    If Items.Count = 0 Then Return nTitleDurInSec 'it is the first item
    '    Dim its As List(Of cRCDb_SCENE_ITEM) = ItemsSorted

    '    For i As Integer = its.Count - 1 To 0 Step -1
    '        If nNewSceneStartTime < its(i).StartTime Then
    '            If i <> 0 Then its(i - 1).EndTime = nNewSceneStartTime - 1
    '            Return its(i).StartTime - 1
    '        End If
    '    Next
    '    'For i As Integer = 0 To its.Count - 1
    '    '    If nNewSceneStartTime > its(i).StartTime Then
    '    '        its(i).EndTime = nNewSceneStartTime - 1
    '    '        If i = its.Count - 1 Then
    '    '            'its the new last item
    '    '            Return nTitleDurInSec
    '    '        Else
    '    '            'there is a scene after this new one
    '    '            Return its(i + 1).StartTime - 1
    '    '        End If
    '    '    End If
    '    'Next
    '    its(its.Count - 1).EndTime = nNewSceneStartTime - 1
    '    Return nTitleDurInSec
    'End Function

End Class

Public Class cRCDb_SCENE_ITEM

    <XmlIgnore()> _
    Public ReadOnly Property IsInitalized()
        Get
            Return Not String.IsNullOrEmpty(Name)
        End Get
    End Property

    '<XmlIgnore()> _
    'Public WriteOnly Property Parent() As cRCDb_SCENE_COLLECTION
    '    Set(ByVal value As cRCDb_SCENE_COLLECTION)
    '        _Parent = value
    '    End Set
    'End Property
    'Private _Parent As cRCDb_SCENE_COLLECTION

    Public Id As Integer
    Public ReadOnly Property Number() As Integer
        Get
            Return 0 '_Parent.ItemsSorted.IndexOf(Me) + 1
        End Get
    End Property
    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property
    Private _Name As String
    Public Description As String
    '''' <summary>
    '''' Seconds
    '''' </summary>
    '''' <remarks></remarks>
    'Public StartTime As UInt32
    '''' <summary>
    '''' Seconds
    '''' </summary>
    '''' <remarks></remarks>
    'Public EndTime As UInt32

    Public Sub New()
        '_Parent = New cRCDb_SCENE_COLLECTION
        Dim r As New Random
        Id = r.Next
    End Sub

    Public Sub New(ByVal nName As String, ByVal nDescription As String)
        Dim r As New Random
        Id = r.Next
        Name = nName
        Description = nDescription
    End Sub

    'Public Sub New(ByRef nParent As cRCDb_SCENE_COLLECTION, ByVal nName As String, ByVal nStartTime As String, ByVal nEndTime As String)
    '    Dim r As New Random
    '    Id = r.Next
    '    Name = nName
    '    StartTime = nStartTime
    '    EndTime = nEndTime
    '    _Parent = nParent
    'End Sub


End Class

#End Region 'SCENES

#Region "ENUMS"

'Public Enum eRCDb_Role
'    Actor
'    Director
'    Producer
'    Screenwriter
'    Executive_Producer
'    Cinematographer
'    Composer
'    Production_Design
'    Co_Producer
'    Film_Editing
'    Costumer_Design
'    Casting
'    Associate_Producer
'End Enum

Public Enum eRCDb_FactReferenceType
    FACT
    CONTRIBUTOR
End Enum

Public Enum eRCDb_INPUT_DATA_TYPE
    CONTRIBUTOR
    FILMOGRAPHY
    SOUNDTRACK
    FACTS
End Enum

Public Enum eRCDb_EventReferenceType
    CONTRIBUTOR
    MOVIE_FACT
    TRACK
End Enum

#End Region 'ENUMS
