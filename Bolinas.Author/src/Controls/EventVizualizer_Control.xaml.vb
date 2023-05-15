Imports SMT.Multimedia.Players.DVD

Partial Public Class EventVizualizer_Control

#Region "FIELDS & PROPERTIES"

    Private WithEvents EC As cRCDb_EVENT_CONTAINER
    Private ReadOnly Property TRT() As Short
        Get
            If PLAYER Is Nothing Then Return 7200
            Return PLAYER.CurrentTitleDurationInSeconds
        End Get
    End Property
    Private WithEvents PLAYER As cDVDPlayer
    Public Event evProgressUpdate()
    Public Event evEventSelected(ByRef e As cRCDb_EVENT_ITEM, ByRef SetName As String)
    Public ReadOnly Property SelectedEvent() As cRCDb_EVENT_ITEM
        Get
            If _SelectedEvent IsNot Nothing Then
                Return _SelectedEvent
            Else
                If PLAYER IsNot Nothing Then
                    Return EC.Scenes.CurrentItem(PLAYER.CurrentRunningTime_InSeconds)
                Else
                    Return Nothing
                End If
            End If
        End Get
    End Property
    Private _SelectedEvent As cRCDb_EVENT_ITEM

#End Region 'FIELDS & PROPERTIES

#Region "CONSTRUCTOR"

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub TimelineViewer_Control_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        SetupTimemarks()
    End Sub

#End Region 'FORM

#Region "FUNCTIONALITY"

#Region "FUNCTIONALITY:SETUP"

    Public Sub Setup(ByRef nPLAYER As cDVDPlayer, ByRef nEventContainers As cRCDb_EVENT_CONTAINER)
        EC = nEventContainers
        EC.SetScenesActive(True)
        PLAYER = nPLAYER
        SetupTimemarks()
        RenderEventSets()
    End Sub

    Private Sub RenderEventSets()
        Try
            spEventSets.Children.Clear()
            Dim esi As EventSetItem_Control

            'SCENES
            esi = New EventSetItem_Control(EC.Scenes, EC, ZoomPercent, PLAYER)
            AddHandler esi.evEventSelected, AddressOf HandleEventBoxSelected
            Me.spEventSets.Children.Add(esi)

            For Each e As cRCDb_EVENT_COLLECTION In EC.EventSets
                esi = New EventSetItem_Control(e, EC, ZoomPercent, PLAYER)
                AddHandler esi.evEventSelected, AddressOf HandleEventBoxSelected
                Me.spEventSets.Children.Add(esi)
            Next
        Catch ex As Exception
            MessageBox.Show("Problem in RenderEventSets(). Error: " & ex.Message, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region 'FUNCTIONALITY:SETUP

#Region "FUNCTIONALITY:SETS"

    Public Sub NewSet()
        Dim n As String = InputBox("Set name:", My.Settings.APPLICATION_NAME_SHORT)
        If n = "" Then
            MsgBox("Set name cannot be empty.")
            Exit Sub
        End If
        If InStr(n.ToLower, "scene") Then
            MsgBox("Invalid name. Set name may not include the word 'scene'.")
            Exit Sub
        End If
        EC.EventSets.Add(New cRCDb_EVENT_COLLECTION(n))
        RenderEventSets()
    End Sub

    Public Sub DeleteSet()
        If MessageBox.Show("Are you sure you wish to delete the active event set?" & vbNewLine & "Name = " & EC.ActiveSet.Name, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        EC.DeleteActiveSet()
        RenderEventSets()
    End Sub

#End Region 'FUNCTIONALITY:SETS

#Region "FUNCTIONALITY:EVENTS"

    Public Sub NewEvent()
        If EC.DVDTitleNumber = Nothing Then
            If PLAYER Is Nothing OrElse PLAYER.CurrentDomain <> SMT.Multimedia.DirectShow.DvdDomain.Title Then
                Dim tt As String = InputBox("Which DVD Title Number do you wish to associate this event container with?", My.Settings.APPLICATION_NAME_SHORT, 1)
                If String.IsNullOrEmpty(tt) OrElse Not IsNumeric(tt) Then
                    MessageBox.Show("Invalid DVD Title Number.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
                    Exit Sub
                End If
                EC.DVDTitleNumber = tt
            Else
                EC.DVDTitleNumber = PLAYER.CurrentTitle
                EC.DVDTitleDuration = PLAYER.CurrentTitleDurationInSeconds
            End If
        End If

        If PLAYER IsNot Nothing Then
            Select Case PLAYER.CurrentDomain
                Case SMT.Multimedia.DirectShow.DvdDomain.Title
                    If PLAYER.CurrentTitle <> EC.DVDTitleNumber Then
                        MessageBox.Show("The DVD Title Number currently playing does not match the DVD Title Number assigned to this EVENT collection." & vbNewLine & vbNewLine & "New event creation failed.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
                        Exit Sub
                    Else
                        'confirm the trt is set
                        EC.DVDTitleDuration = PLAYER.CurrentTitleDurationInSeconds
                    End If
                Case SMT.Multimedia.DirectShow.DvdDomain.VideoTitleSetMenu, SMT.Multimedia.DirectShow.DvdDomain.VideoManagerMenu
                    MessageBox.Show("The DVD is currently playing in MENU SPACE." & vbNewLine & vbNewLine & "New event will be assigned to DVD Title Number " & EC.DVDTitleNumber & ".", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Case Else
                    MessageBox.Show("Event creation is not permitted in the active domain (" & PLAYER.CurrentDomain.ToString & ")." & vbNewLine & vbNewLine & "New event creation failed.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
                    Exit Sub
            End Select
        End If

        Dim r As New Random
        Dim nE As cRCDb_EVENT_ITEM

        Dim st As String
        If PLAYER Is Nothing Then
            st = InputBox("Start Time:")
            If Not IsNumeric(st) OrElse st < 0 Or (EC.DVDTitleDuration > 0 And st > EC.DVDTitleDuration) Then
                MsgBox("Invalid value.")
                Exit Sub
            End If

            'Dim et As String = InputBox("End Time:")
            'If Not IsNumeric(et) Then
            '    MsgBox("Invalid value.")
            '    Exit Sub
            'End If
        Else
            st = PLAYER.CurrentRunningTime_InSeconds
        End If

        'calculate end time
        Dim et As String
        If EC.DVDTitleDuration = 0 Then
            et = st + 10
        Else
            et = EC.ActiveSet.GetEndTime(st, EC.DVDTitleDuration)
        End If
        nE = New cRCDb_EVENT_ITEM(r.Next, st, et)

        If EC.ActiveSet.EventTimesOverlap(nE.Start, nE.End) Then
            MsgBox("The new event overlaps with an existing event. You need to create overlapping events in a new set.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
            Exit Sub
        End If
        EC.ActiveSet.Items.Add(nE)
        RenderEventSets()
    End Sub

    Public Sub TrimCurrentEvent()
        EC.ActiveSet.CurrentItem(PLAYER.CurrentRunningTime_InSeconds).End = PLAYER.CurrentRunningTime_InSeconds
        RenderEventSets()
    End Sub

#End Region 'FUNCTIONALITY:EVENTS

#Region "FUNCTIONALITY:ZOOM"

    Private Property ZoomFactor() As Double
        Get
            Return _ZoomFactor
        End Get
        Set(ByVal value As Double)
            _ZoomFactor = Math.Round(value, 2)
        End Set
    End Property
    Private _ZoomFactor As Double = 0.5

    Private ReadOnly Property ZoomPercent() As Double
        Get
            Select Case ZoomFactor
                Case 0
                    Return 0.1
                Case 0.05
                    Return 0.15
                Case 0.1
                    Return 0.2
                Case 0.15
                    Return 0.3
                Case 0.2
                    Return 0.4
                Case 0.25
                    Return 0.5
                Case 0.3
                    Return 0.6
                Case 0.35
                    Return 0.7
                Case 0.4
                    Return 0.8
                Case 0.45
                    Return 0.9
                Case 0.5
                    Return 1
                Case 0.55
                    Return 1.4
                Case 0.6
                    Return 1.8
                Case 0.65
                    Return 2.2
                Case 0.7
                    Return 2.6
                Case 0.75
                    Return 3
                Case 0.8
                    Return 3.4
                Case 0.85
                    Return 3.8
                Case 0.9
                    Return 4.2
                Case 0.95
                    Return 4.6
                Case 1
                    Return 5
            End Select
        End Get
    End Property

    Private Sub ZoomIn()
        If ZoomFactor = 1 Then Exit Sub
        ZoomFactor += 0.05
        PerformZoom()
    End Sub

    Private Sub ZoomOut()
        If ZoomFactor = 0 Then Exit Sub
        ZoomFactor -= 0.05
        PerformZoom()
    End Sub

    Private Sub PerformZoom()
        Me.lblZoom.Content = ZoomPercent * 100 & "%"
        Me.cvTimeline.LayoutTransform = New ScaleTransform(ZoomPercent, 1)
        gdEventContainer.Width = cvTimeline.ActualWidth * ZoomPercent
        For Each esi As EventSetItem_Control In spEventSets.Children
            esi.SetZoom(ZoomPercent)
        Next
    End Sub

#End Region 'FUNCTIONALITY:ZOOM

#Region "FUNCTIONALITY:TIME MARKS"

    Private Sub SetupTimemarks()
        Try
            'draw ticks
            Dim x As Double = 4
            Dim y As Double = 20
            Dim tL As Line

            Dim HourTickHeight As Integer = 18
            Dim TenMinTickHeight As Integer = 12
            Dim MinTickHeight As Integer = 8
            Dim SecTickHeight As Integer = 4

            For i As Integer = 0 To TRT Step 10
                tL = New Line
                tL.SnapsToDevicePixels = True
                tL.X1 = x
                tL.Y1 = y
                tL.X2 = x

                If Math.IEEERemainder(i, 3600) = 0 Then
                    tL.Y2 = y - HourTickHeight
                    tL.StrokeThickness = 2.0
                    GoTo RenderTick
                End If
                If Math.IEEERemainder(i, 600) = 0 Then
                    tL.Y2 = y - TenMinTickHeight
                    tL.StrokeThickness = 1.0
                    GoTo RenderTick
                End If
                If Math.IEEERemainder(i, 60) = 0 Then
                    tL.Y2 = y - MinTickHeight
                    tL.StrokeThickness = 1.0
                    GoTo RenderTick
                End If

                'Else-
                tL.Y2 = y - SecTickHeight
                tL.StrokeThickness = 1.0

RenderTick:
                tL.Stroke = New SolidColorBrush(Windows.Media.Colors.Navy)
                Me.cvTimeline.Children.Add(tL)
                x += 5.0
            Next
            cvTimeline.Width = x - 5
            gdEventContainer.Width = cvTimeline.Width
        Catch ex As Exception
            MessageBox.Show("Problem with SetupTimemarks(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'FUNCTIONALITY:TIME MARKS

#Region "FUNCTIONALITY:PLAYBACK TRACKING"

    Private Sub HandleRunningTime() Handles PLAYER.evRunningTimeTick
        For Each c As EventSetItem_Control In Me.spEventSets.Children
            c.DrawPlayPosition(PLAYER.CurrentRunningTime_InSeconds)
        Next
    End Sub

#End Region 'FUNCTIONALTY:PLAYBACK TRACKING

#End Region 'FUNCTIONALITY

#Region "GUI"

    Private Sub btnNewSet_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewSet.Click
        NewSet()
    End Sub

    Private Sub btnDeleteSet_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDeleteSet.Click
        DeleteSet()
    End Sub

    Private Sub btnNewEvent_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewEvent.Click
        NewEvent()
    End Sub

    Private Sub btnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnZoomOut.Click
        Me.slZoom.Value -= slZoom.SmallChange
    End Sub

    Private Sub btnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnZoomIn.Click
        Me.slZoom.Value += slZoom.SmallChange
    End Sub

    Private Sub slZoom_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double)) Handles slZoom.ValueChanged
        If Not Me.IsInitialized Then Exit Sub
        If e.NewValue > e.OldValue Then
            ZoomIn()
        Else
            ZoomOut()
        End If
    End Sub

    Private Sub svHorizontal_ScrollChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.ScrollChangedEventArgs) Handles svHorizontal.ScrollChanged
        cvTimeline.Margin = New Thickness(e.HorizontalOffset * -1, 0, 4, 0)
        For Each esi As EventSetItem_Control In Me.spEventSets.Children
            esi.SetBodyLeft(e.HorizontalOffset * -1)
        Next
    End Sub

#End Region 'GUI

    Private Sub Handle_EC_Refresh() Handles EC.RefreshDisplay
        RenderEventSets()
        RaiseEvent evProgressUpdate()
    End Sub

    Private Sub btnTrimCurrentEvent_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnTrimCurrentEvent.Click
        TrimCurrentEvent()
    End Sub

    Private Sub HandleEventBoxSelected(ByRef e As cRCDb_EVENT_ITEM, ByRef SetName As String)
        RaiseEvent evEventSelected(e, SetName)
        _SelectedEvent = e
    End Sub

    Public Sub AddDataItem(ByRef dataO As DataObject)
        If SelectedEvent Is Nothing Then
            MsgBox("No event is selected/active.")
            Exit Sub
        End If
        SelectedEvent.AddReference(dataO, EC.ActiveSet)
    End Sub

    Public Sub SelectNextSet()
        If EC.EventSets IsNot Nothing AndAlso EC.EventSets.Count > 0 Then
            For i As Integer = 0 To spEventSets.Children.Count - 1
                If CType(spEventSets.Children(i), EventSetItem_Control).EventSet.IsSelected Then
                    If i < spEventSets.Children.Count - 1 Then
                        EC.SetItemActive(CType(spEventSets.Children(i + 1), EventSetItem_Control).EventSet, True)
                        Exit Sub
                    Else
                        EC.SetItemActive(CType(spEventSets.Children(0), EventSetItem_Control).EventSet, True)
                        Exit Sub
                    End If

                    'CType(spEventSets.Children(i), EventSetItem_Control).EventSet.IsSelected = False

                    'If i < spEventSets.Children.Count - 1 Then
                    '    CType(spEventSets.Children(i + 1), EventSetItem_Control).EventSet.IsSelected = True
                    'Else
                    '    CType(spEventSets.Children(0), EventSetItem_Control).EventSet.IsSelected = True
                    'End If
                End If
            Next
        End If
    End Sub

End Class
