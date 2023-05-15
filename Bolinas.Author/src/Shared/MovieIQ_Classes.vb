Imports System.IO
Imports System.Xml

Public Class cMovieIQ_Product

#Region "PROPERTIES"

    Private Const XMLNamespace As String = "http://www.rcdb.net/cinefile"
    Private Const SchemaLocation As String = "http://www.rcdb.net/cinefile ../xsd/cinefile.xsd"
    Private Const XMLXSI As String = "http://www.w3.org/2001/XMLSchema-instance"

    Public Id As Integer
    Public Language As eMovieIQ_Language
    Public Type As eMovieIQ_ProductType
    Public Title As String
    Public Studio As String
    Public Released As String
    Public ParentWorkId As String

    Public CONTRIBUTORS As List(Of cMovieIQ_CONTRIBUTOR_ITEM)
    Public MUSIC As List(Of cMovieIQ_MUSIC_ITEM)
    Public MOVIE_FACTS As List(Of cMovieIQ_FACT)
    Public EVENTS As cRCDb_EVENT_CONTAINER

#End Region 'PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
    End Sub

    Public Sub New(ByRef BP As cBolinas_AuthoringProject)
        Id = BP.Product_Id
        Language = BP.Product_Lang
        Type = BP.Product_Type
        Title = BP.Product_Title
        Studio = BP.Product_Studio
        Released = BP.Product_ReleasedDate
        ParentWorkId = BP.Product_ParentWorkId

        CONTRIBUTORS = New List(Of cMovieIQ_CONTRIBUTOR_ITEM)
        For Each c As cRCDb_CONTRIBUTOR_ITEM In BP.CONTRIBUTORS.Items
            CONTRIBUTORS.Add(New cMovieIQ_CONTRIBUTOR_ITEM(c, BP.FILMOGRAPHY, ParentWorkId))
        Next

        MUSIC = New List(Of cMovieIQ_MUSIC_ITEM)
        For Each t As cRCDb_MUSIC_ITEM In BP.SOUNDTRACK.Items
            MUSIC.Add(New cMovieIQ_MUSIC_ITEM(t))
        Next

        MOVIE_FACTS = New List(Of cMovieIQ_FACT)
        For Each f As cRCDb_FACT_ITEM In BP.FACTOGRAPHY.Items
            MOVIE_FACTS.Add(New cMovieIQ_FACT(f))
        Next

        EVENTS = BP.EVENTS
        'For Each e As cRCDb_EVENT_ITEM In BP.EVENTS.Items
        '    EVENTS.Add(New cMovieIQ_EVENT(e))
        'Next

    End Sub

#End Region 'CONSTRUCTOR

#Region "PUBLIC METHODS"

    Public Function ExportXMLTo(ByRef oPath As String) As Boolean
        Try
            If File.Exists(oPath) Then File.Delete(oPath)

            Dim d As XmlDocument = BuildXMLDoc()
            d.Save(oPath)

            'VALIDATE
            Dim v As List(Of String) = Validate(oPath)
            If v.Count > 0 Then
                MsgBox("Movie IQ output XML validation failed. See end of output file for error details.")
                Dim fs As New FileStream(oPath, FileMode.Open, FileAccess.ReadWrite)
                fs.Position = fs.Length
                Dim sw As New StreamWriter(fs)
                sw.WriteLine(vbNewLine)
                sw.WriteLine(vbNewLine)
                sw.WriteLine(vbNewLine)
                sw.WriteLine(vbNewLine)
                sw.WriteLine("==================================" & vbNewLine)
                sw.WriteLine("VALIDATION ERRORS" & vbNewLine)
                sw.WriteLine("==================================" & vbNewLine)
                For Each s As String In v
                    sw.WriteLine(s & vbNewLine)
                    sw.WriteLine("*********" & vbNewLine)
                    sw.WriteLine(vbNewLine)
                Next
                sw.Close()
                fs.Close()
                sw.Dispose()
                fs.Dispose()
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Throw New Exception("Problem with ExportXMLTo(). Error: " & ex.Message)
        End Try
    End Function

    Public Shared Function Validate(ByRef XMLDoc As String) As List(Of String)
        Try
            Dim xsd As Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("RCDb.Bolinas.Author.cinefile.xsd")
            Dim v As New cXmlValidator()
            v.ValidateXmlAgainstSchema(XMLDoc, xsd)
            xsd.Dispose()
            Return v.ValidationErrors
        Catch ex As Exception
            Throw New Exception("Problem in Validate(). Error: " & ex.Message, ex)
        End Try
    End Function

#End Region 'PUBLIC METHODS

#Region "PRIVATE METHODS"

    Private Function BuildXMLDoc() As XmlDocument
        Try
            'CREATE DOC
            Dim doc As New XmlDocument

            'SETUP DECLARATION
            Dim dec As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
            doc.InsertBefore(dec, doc.DocumentElement)

            'SETUP ROOT NODE
            Dim rootNode As XmlElement = doc.CreateElement("cinefile")
            rootNode.SetAttribute("xmlns", XMLNamespace)
            Dim scLoc As XmlAttribute = doc.CreateAttribute("xsi", "schemaLocation", XMLXSI)
            scLoc.Value = SchemaLocation
            rootNode.SetAttributeNode(scLoc)

            'ADD THE PRODUCT
            rootNode.AppendChild(Me.ToXMLElement(doc))

            'APPEND ROOT
            doc.AppendChild(rootNode)

            Return doc
        Catch ex As Exception
            Throw New Exception("Problem in BuildXMLDocument(). Error: " & ex.Message)
        End Try
    End Function

    Private Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim xe As XmlElement
            Dim xt As XmlText

            Dim out As XmlElement = xd.CreateElement("product")
            out.SetAttribute("id", Id)
            out.SetAttribute("lang", Language.ToString)
            out.SetAttribute("type", Type.ToString)

            xe = xd.CreateElement("title")
            xt = xd.CreateTextNode(Title)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("studio")
            xt = xd.CreateTextNode(Studio)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("released")
            xt = xd.CreateTextNode(Released)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            Dim Contributors As XmlElement = ContributorsToXML(xd)
            out.AppendChild(Contributors)

            Dim Music As XmlElement = MusicToXML(xd)
            out.AppendChild(Music)

            Dim Movie_Facts As XmlElement = FactsToXML(xd)
            out.AppendChild(Movie_Facts)

            Dim Events As XmlElement = EventsToXML(xd)
            out.AppendChild(Events)

            Return out
        Catch ex As Exception
            Throw New Exception("Problem in ToXMLElement(). Error: " & ex.Message)
        End Try
    End Function

    Private Function ContributorsToXML(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("contributors")
            For Each c As cMovieIQ_CONTRIBUTOR_ITEM In CONTRIBUTORS
                out.AppendChild(c.ToXMLElement(xd))
            Next
            Return out
        Catch ex As Exception
            Throw New Exception("Problem in ContributorsToXML(). Error: " & ex.Message)
        End Try
    End Function

    Private Function MusicToXML(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("music")
            For Each c As cMovieIQ_MUSIC_ITEM In MUSIC
                out.AppendChild(c.ToXMLElement(xd))
            Next
            Return out
        Catch ex As Exception
            Throw New Exception("Problem in MusicToXML(). Error: " & ex.Message)
        End Try
    End Function

    Private Function FactsToXML(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("movie_facts")
            For Each c As cMovieIQ_FACT In MOVIE_FACTS
                out.AppendChild(c.ToXMLElement(xd))
            Next
            Return out
        Catch ex As Exception
            Throw New Exception("Problem in FactsToXML(). Error: " & ex.Message)
        End Try
    End Function

    Private Function EventsToXML(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("events")
            EVENTS.AttachXMLNodes_MovieIQ(out, EVENTS.RunningTimeOffset)
            Return out
        Catch ex As Exception
            Throw New Exception("Problem in EventsToXML(). Error: " & ex.Message)
        End Try
    End Function

#End Region 'PRIVATE METHODS

End Class

#Region "ELEMENTAL CLASSES"

Public Class cMovieIQ_CONTRIBUTOR_ITEM

    Public Id As Integer
    Public Name As String
    Public Role As String = ""   'eMovieIQ_Role
    Public CharacterName As String = ""

    Public FILMOGRAPHY As List(Of cMovieIQ_WORK)

    Public Sub New()
        FILMOGRAPHY = New List(Of cMovieIQ_WORK)
    End Sub

    Public Sub New(ByRef c As cRCDb_CONTRIBUTOR_ITEM, ByRef nFilmography As cRCDb_FILMOGRAPHY, ByVal ParentWorkId As String)
        Try
            Id = c.ContributorId
            Name = c.ContributorName
            Dim fi As cRCDb_FILMOGRAPHY_ITEM = nFilmography.FindPerson(ParentWorkId, Id)
            If fi IsNot Nothing Then
                Role = fi.Role
                CharacterName = fi.CharacterName
            Else
                Role = ""
                CharacterName = ""
                Debug.WriteLine("Person not found: " & Id & " Work: " & ParentWorkId)
            End If

            FILMOGRAPHY = New List(Of cMovieIQ_WORK)
            Dim fil As List(Of cRCDb_FILMOGRAPHY_ITEM) = nFilmography.GetPersonFilmographyData(Id)
            If fil Is Nothing OrElse fil.Count < 1 Then
                Debug.WriteLine("Failed to find any associated filmography items for person: " & Id)
            Else
                For Each i As cRCDb_FILMOGRAPHY_ITEM In fil
                    FILMOGRAPHY.Add(New cMovieIQ_WORK(i))
                Next
            End If

        Catch ex As Exception
            Throw New Exception("Problem in New() cMovieIQ_CONTRIBUTOR_ITEM from cRCDb_CONTRIBUTOR_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("contributor")
            out.SetAttribute("id", Id)

            Dim xe As XmlElement
            Dim xt As XmlText

            xe = xd.CreateElement("name")
            xt = xd.CreateTextNode(Name)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("role")
            xt = xd.CreateTextNode(Role.ToString)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("character_name")
            xt = xd.CreateTextNode(CharacterName)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            Dim FO As XmlElement = xd.CreateElement("filmography")
            For Each c As cMovieIQ_WORK In FILMOGRAPHY
                FO.AppendChild(c.ToXMLElement(xd))
            Next
            out.AppendChild(FO)

            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
        End Try
    End Function

End Class

Public Class cMovieIQ_MUSIC_ITEM

    Public Id As Integer
    Public Title As String
    Public Album As String
    Public Artist As String
    Public Label As String
    Public Duration As String

    Public Sub New()
    End Sub

    Public Sub New(ByRef t As cRCDb_MUSIC_ITEM)
        Try
            Id = t.Id
            Title = t.Title
            Album = t.Album
            Artist = t.Artist
            Label = t.Label
            Duration = t.Time
        Catch ex As Exception
            Throw New Exception("Problem in New() cMovieIQ_MUSIC_ITEM from cRCDb_MUSIC_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("track")
            out.SetAttribute("id", Id)

            Dim xe As XmlElement
            Dim xt As XmlText

            xe = xd.CreateElement("title")
            xt = xd.CreateTextNode(Title)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("album")
            xt = xd.CreateTextNode(Album)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("artist")
            xt = xd.CreateTextNode(Artist)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("label")
            xt = xd.CreateTextNode(Label)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("duration")
            xt = xd.CreateTextNode(Duration)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
        End Try
    End Function

End Class

Public Class cMovieIQ_FACT

    Public Id As Integer
    Public Type As eMovieIQ_FactType
    Public Generic As Boolean
    Public Description As String

    Public Sub New()
    End Sub

    Public Sub New(ByRef t As cRCDb_FACT_ITEM)
        Try
            Id = t.Id
            Type = eMovieIQ_FactType.TRIVIA
            Generic = t.Generic
            Description = t.Description
        Catch ex As Exception
            Throw New Exception("Problem in New() cMovieIQ_FACT from cRCDb_FACT_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("movie_fact")
            out.SetAttribute("id", Id)
            out.SetAttribute("type", Type.ToString)
            out.SetAttribute("generic", Generic.ToString.ToLower)

            Dim xt As XmlText
            xt = xd.CreateTextNode(Description)
            out.AppendChild(xt)

            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
        End Try
    End Function

End Class

'Public Class cMovieIQ_EVENT
'    Inherits cRCDb_EVENT_ITEM

'    Public Sub New(ByRef e As cRCDb_EVENT_ITEM)
'        Me.Id = e.Id
'        Me.Start = e.Start
'        Me.End = e.End
'        Me.ACTIVE_CAST = e.ACTIVE_CAST
'        Me.ACTIVE_FACTS = e.ACTIVE_FACTS
'        Me.ACTIVE_SOUNDTRACK = e.ACTIVE_SOUNDTRACK
'    End Sub

'    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
'        Try
'            Dim out As XmlElement = xd.CreateElement("event")
'            out.SetAttribute("id", Id)
'            out.SetAttribute("start", Start)
'            out.SetAttribute("end", [End])

'            For Each c As cRCDb_CONTRIBUTOR_ITEM In Me.ACTIVE_CAST
'                out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(c)).ToXMLElement(xd))
'            Next

'            For Each f As cRCDb_FACT_ITEM In Me.ACTIVE_FACTS
'                out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(f)).ToXMLElement(xd))
'            Next

'            For Each m As cRCDb_MUSIC_ITEM In Me.ACTIVE_SOUNDTRACK
'                out.AppendChild(New cMovieIQ_EVENT_REFERENCE(New cRCDb_EVENT_REFERENCE(m)).ToXMLElement(xd))
'            Next

'            Return out
'        Catch ex As Exception
'            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
'        End Try
'    End Function

'End Class

Public Class cMovieIQ_EVENT_REFERENCE
    Inherits cRCDb_EVENT_REFERENCE

    Public Sub New(ByRef er As cRCDb_EVENT_REFERENCE)
        Me.Id = er.Id
        Me.Type = er.Type
    End Sub

    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("reference")
            out.SetAttribute("type", Type.ToString)
            out.SetAttribute("id", Id)
            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
        End Try
    End Function

End Class

Public Class cMovieIQ_WORK

    Public Id As Integer
    Public Title As String
    Public Studio As String
    Public Released As Date
    Public Role As String 'eMovieIQ_Role

    Public Sub New()
    End Sub

    Public Sub New(ByRef FI As cRCDb_FILMOGRAPHY_ITEM)
        Try
            Id = FI.WorkId
            Title = FI.WorkName
            Studio = FI.WorkStudio
            Released = FI.WorkDate
            Role = FI.Role
        Catch ex As Exception
            Throw New Exception("Problem in New() cMovieIQ_WORK from cRCDb_FILMOGRAPHY_ITEM. Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function ToXMLElement(ByRef xd As XmlDocument) As XmlElement
        Try
            Dim out As XmlElement = xd.CreateElement("work")
            out.SetAttribute("id", Id)

            Dim xe As XmlElement
            Dim xt As XmlText

            xe = xd.CreateElement("title")
            xt = xd.CreateTextNode(Title)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("studio")
            xt = xd.CreateTextNode(Studio)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("released")
            xt = xd.CreateTextNode(Released.ToString("yyyy-MM-dd"))
            out.AppendChild(xe)
            xe.AppendChild(xt)

            xe = xd.CreateElement("role")
            xt = xd.CreateTextNode(Role.ToString)
            out.AppendChild(xe)
            xe.AppendChild(xt)

            Return out
        Catch ex As Exception
            Throw New Exception("Failure in ToXMLElement(). Error: " & ex.Message, ex)
        End Try
    End Function

End Class

Public Enum eMovieIQ_Language
    EN
    SP
    GR
    JP
    FR
End Enum

Public Enum eMovieIQ_ProductType
    BD = 0
    DVD = 1
End Enum

'Public Enum eMovieIQ_Role
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

Public Enum eMovieIQ_FactType
    TRIVIA
End Enum

#End Region 'ELEMENTAL CLASSES
