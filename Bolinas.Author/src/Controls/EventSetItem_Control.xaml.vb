Imports SMT.Multimedia.Players.DVD

Partial Public Class EventSetItem_Control

    Private EC As cRCDb_EVENT_CONTAINER
    Public WithEvents EventSet As cRCDb_EVENT_COLLECTION
    Private _ZoomPercent As Double = 1
    Private PLAYER As cDVDPlayer
    Private PosLine As Line
    Public Event evEventSelected(ByRef e As cRCDb_EVENT_ITEM, ByRef SetName As String)

    Private Sub HandleSelection(ByVal nIsSelected As Boolean) Handles EventSet.Selection
        'IsSelected = nIsSelected
        rbSelected.IsChecked = nIsSelected
        RenderEventDisplay()
    End Sub

    Public Sub New(ByRef nEvs As cRCDb_EVENT_COLLECTION, ByRef nEC As cRCDb_EVENT_CONTAINER, ByVal ZoomPercent As Double, ByRef nDVDPlayer As cDVDPlayer)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        EventSet = nEvs
        EC = nEC
        Width = Double.NaN
        _ZoomPercent = ZoomPercent
        _Expanded = False
    End Sub

    Public Property Expanded() As Boolean
        Get
            Return _Expanded
        End Get
        Set(ByVal value As Boolean)
            'todo: expand/collapse
            _Expanded = value
            If _Expanded Then
                Me.Height = Double.NaN
                'Me.cvBoxParent.Height = Double.NaN
                Me.cvBoxParent.Height = 60
                Me.btnExpand.Content = "-"
            Else
                Me.Height = 24
                Me.cvBoxParent.Height = 24
                Me.btnExpand.Content = "+"
            End If
            RenderEventDisplay()
        End Set
    End Property
    Private _Expanded As Boolean = False

    Public Sub SetBodyLeft(ByVal factor As Integer)
        cvBoxParent.Margin = New Thickness(factor, 0, 0, 0)
    End Sub

    Public Sub SetZoom(ByVal ZoomPercent As Double)
        _ZoomPercent = ZoomPercent
        RenderEventDisplay()
    End Sub

    Private Sub EventSetItem_Control_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderEventDisplay()
    End Sub

    Private Sub RenderEventDisplay()
        Try
            Me.cvBoxParent.Children.Clear()

            Dim EventBox As New System.Windows.Controls.Border
            For i As Integer = 0 To EventSet.Items.Count - 1

                If EventSet.Items(i).Start > EventSet.Items(i).End Then
                    MessageBox.Show("Illegal start/end times for this event (start = " & EventSet.Items(i).Start & " end = " & EventSet.Items(i).End & "). The event will be removed.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.None)
                    EventSet.Items.RemoveAt(i)
                    GoTo NextEvent
                End If

                'Draw event box
                EventBox = New System.Windows.Controls.Border
                EventBox.Name = "bx_" & EventSet.Items(i).Start & "_" & EventSet.Items(i).End
                EventBox.SnapsToDevicePixels = True
                EventBox.Width = (EventSet.Items(i).End - EventSet.Items(i).Start) / 2
                EventBox.Height = IIf(Expanded, 56, 20)
                EventBox.Tag = EventSet.Items(i)
                EventBox.AllowDrop = True

                If EventSet.IsSelected Then
                    EventBox.Background = GetSelectedColor(New SolidColorBrush(Colors.DarkGray))
                Else
                    EventBox.Background = New SolidColorBrush(Colors.WhiteSmoke)
                End If

                EventBox.BorderBrush = New SolidColorBrush(Colors.Gray)
                EventBox.BorderThickness = New Thickness(2)
                EventBox.CornerRadius = New CornerRadius(2)
                Canvas.SetTop(EventBox, 2)
                Canvas.SetLeft(EventBox, CDbl((EventSet.Items(i).Start / 2) + 4)) 'this is where the zoom factor could come into play
                AddHandler EventBox.MouseLeftButtonUp, AddressOf Handle_EventBox_MouseLeftButtonUp
                AddHandler EventBox.MouseRightButtonUp, AddressOf Handle_EventBox_MouseRightButtonUp
                AddHandler EventBox.DragEnter, AddressOf Handle_EventBox_DragEnter
                AddHandler EventBox.DragOver, AddressOf Handle_EventBox_DragOver
                AddHandler EventBox.DragLeave, AddressOf Handle_EventBox_DragLeave
                AddHandler EventBox.Drop, AddressOf Handle_EventBox_Drop

                'AddHandler EventBox.MouseMove, AddressOf Handle_EventBox_MouseMove
                'AddHandler EventBox.MouseLeftButtonDown, AddressOf Handle_EventBox_LeftButtonDown

                cvBoxParent.Children.Add(EventBox)

                Dim sp As New StackPanel
                EventBox.Child = sp

                If Expanded Then
                    'draw the associated data items
                    If EventSet.Items(i).SCENE.IsInitalized Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Scene, Expanded, EventBox.Width))
                    End If
                    For Each c As cRCDb_CONTRIBUTOR_ITEM In EventSet.Items(i).CAST
                        sp.Children.Add(GetLine(eBolinas_EventType.Cast, Expanded, EventBox.Width))
                    Next
                    If EventSet.Items(i).SOUNDTRACK.IsInitalized Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Music, Expanded, EventBox.Width))
                    End If
                    For Each f As cRCDb_FACT_ITEM In EventSet.Items(i).FACTS
                        sp.Children.Add(GetLine(eBolinas_EventType.Fact, Expanded, EventBox.Width))
                    Next
                Else
                    'draw the associated data items
                    If Not String.IsNullOrEmpty(EventSet.Items(i).SCENE.Name) Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Scene, Expanded, EventBox.Width))
                    End If
                    If EventSet.Items(i).CAST.Count > 0 Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Cast, Expanded, EventBox.Width))
                    End If
                    If EventSet.Items(i).SOUNDTRACK.IsInitalized Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Music, Expanded, EventBox.Width))
                    End If
                    If EventSet.Items(i).FACTS.Count > 0 Then
                        sp.Children.Add(GetLine(eBolinas_EventType.Fact, Expanded, EventBox.Width))
                    End If
                End If
NextEvent:
            Next
            lblEventSetName.Content = EventSet.Name
            Me.cvBoxParent.LayoutTransform = New ScaleTransform(_ZoomPercent, 1)
            rbSelected.IsChecked = EventSet.IsSelected
            If EventSet.IsSelected Then
                gdBoxParent.Background = New SolidColorBrush(Colors.LightSkyBlue)
            Else
                gdBoxParent.Background = New SolidColorBrush(Colors.AliceBlue)
            End If
        Catch ex As Exception
            MsgBox("Problem in RenderEventDisplay(). Error: " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub ShowEventEditor(ByVal Sender As Object)
        Dim ei As cRCDb_EVENT_ITEM
        Dim MI As MenuItem = TryCast(Sender, MenuItem)
        If MI IsNot Nothing Then
            ei = CType(MI.Parent, ContextMenu).Tag
        Else
            Dim BD As Border = TryCast(Sender, Border)
            ei = CType(BD.Tag, cRCDb_EVENT_ITEM)
        End If
        If ei Is Nothing Then Exit Sub
        Dim dlg As New EventEdit_Dialog(ei, EventSet, EC)
        dlg.Owner = My.Application.Windows(0)
        dlg.ShowInTaskbar = False
        Dim b As Boolean = dlg.ShowDialog
        RenderEventDisplay()
    End Sub

    Private Function GetLine(ByVal Type As eBolinas_EventType, ByVal Large As Boolean, ByVal BoxWidth As Double) As Line
        Dim tL As New Line
        tL.X1 = 1
        tL.X2 = BoxWidth - 5
        tL.Y1 = IIf(Large, 5, 3)
        tL.Y2 = IIf(Large, 5, 3)

        Select Case Type
            Case eBolinas_EventType.Scene
                tL.Stroke = New SolidColorBrush(Colors.LightGreen)
            Case eBolinas_EventType.Cast
                tL.Stroke = New SolidColorBrush(Colors.LightBlue)
            Case eBolinas_EventType.Music
                tL.Stroke = New SolidColorBrush(Colors.Pink)
            Case eBolinas_EventType.Fact
                tL.Stroke = New SolidColorBrush(Colors.LightGoldenrodYellow)
        End Select

        tL.StrokeThickness = IIf(Large, 5, 3)
        tL.SnapsToDevicePixels = True
        Return tL
    End Function

    Private Function GetSelectedColor(ByVal B As System.Windows.Media.SolidColorBrush) As SolidColorBrush
        Dim scb As SolidColorBrush = CType(B, SolidColorBrush)
        Dim c As System.Windows.Media.Color = scb.Color
        Return New SolidColorBrush(System.Windows.Media.Color.FromArgb(100, c.R, c.G, c.B))
    End Function

    Private Sub btnExpand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnExpand.Click
        Expanded = Not Expanded
    End Sub

    Private rbMutex As Boolean = False

    Private Sub rbSelected_Checked(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles rbSelected.Checked
        If rbMutex Then
            rbMutex = False
            Exit Sub
        End If
        If EventSet.IsSelected <> rbSelected.IsChecked Then
            EC.SetItemActive(EventSet, rbSelected.IsChecked)
        End If
    End Sub

    'Private Sub Handle_EventSetSelected(ByVal IsSelected As Boolean) Handles EventSet.Selection
    '    rbMutex = True
    '    rbSelected.IsChecked = IsSelected
    'End Sub

    Public Sub DrawPlayPosition(ByVal CRT As Integer)
        'ok, how many pixels per second? 
        'half a pixel per second (10 seconds = 5 pixels)
        'what about the zoom factor?
        If PosLine IsNot Nothing Then Me.cvBoxParent.Children.Remove(PosLine)
        Dim left As Double = 4 + (CRT / 2) '* _ZoomPercent
        PosLine = New Line
        PosLine.X1 = left
        PosLine.X2 = left
        PosLine.Y1 = 0
        PosLine.Y2 = Me.cvBoxParent.ActualHeight
        PosLine.Stroke = New SolidColorBrush(Colors.Red)
        PosLine.StrokeThickness = 3
        PosLine.SnapsToDevicePixels = False
        Me.cvBoxParent.Children.Add(PosLine)
    End Sub

#Region "FUNCTIONALITY:EVENT BOX"

    Private LastLeftClickTicks As Long

    Private Sub Handle_EventBox_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
        Try
            Dim bd As System.Windows.Controls.Border = TryCast(sender, System.Windows.Controls.Border)
            If bd Is Nothing Then Exit Sub
            Dim ts As New TimeSpan(DateTime.Now.Ticks - LastLeftClickTicks)
            'Debug.WriteLine(ts.TotalMilliseconds)
            If ts.TotalMilliseconds < 160 Then
                'this is a double click
                ShowEventEditor(sender)
            Else
                'this eventbox has been selected
                Dim ei As cRCDb_EVENT_ITEM
                Dim MI As MenuItem = TryCast(sender, MenuItem)
                If MI IsNot Nothing Then
                    ei = CType(MI.Parent, ContextMenu).Tag
                Else
                    Dim BDR As Border = TryCast(sender, Border)
                    ei = CType(BDR.Tag, cRCDb_EVENT_ITEM)
                End If
                RaiseEvent evEventSelected(ei, EventSet.Name)
            End If
            LastLeftClickTicks = DateTime.Now.Ticks
        Catch ex As Exception
            MsgBox("Problem with Handle_EventBox_MouseLeftButtonUp(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Handle_EventBox_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
        Try
            Dim bd As System.Windows.Controls.Border = TryCast(sender, System.Windows.Controls.Border)
            If bd Is Nothing Then Exit Sub
            Dim p As Point = e.GetPosition(sender)
            ShowEventBoxContextMenu(bd, p)
        Catch ex As Exception
            Throw New Exception("Problem with Handle_EventBox_MouseRightButtonUp(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub Handle_EventBox_DragEnter(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
        Dim bd As System.Windows.Controls.Border = TryCast(sender, System.Windows.Controls.Border)
        If bd Is Nothing Then Exit Sub

        If EventSet.IsScenes Then
            e.Effects = DragDropEffects.Move
        Else
            Dim fs() As String = e.Data.GetFormats
            Select Case fs(0)
                Case "scene"
                    e.Effects = DragDropEffects.None
                Case "cast"
                    e.Effects = DragDropEffects.Move
                Case "fact"
                    e.Effects = DragDropEffects.Move
                Case "music"
                    e.Effects = DragDropEffects.Move
                Case Else
                    e.Effects = DragDropEffects.None
            End Select
        End If
        e.Handled = True
    End Sub

    Private Sub Handle_EventBox_DragOver(ByVal sender As Object, ByVal e As DragEventArgs)
        Dim bd As System.Windows.Controls.Border = TryCast(sender, System.Windows.Controls.Border)
        If bd Is Nothing Then Exit Sub

        If EventSet.IsScenes Then
            e.Effects = DragDropEffects.Move
        Else
            Dim fs() As String = e.Data.GetFormats
            Select Case fs(0)
                Case "scene"
                    e.Effects = DragDropEffects.None
                Case "cast"
                    e.Effects = DragDropEffects.Move
                Case "fact"
                    e.Effects = DragDropEffects.Move
                Case "music"
                    e.Effects = DragDropEffects.Move
                Case Else
                    e.Effects = DragDropEffects.None
            End Select
        End If
        e.Handled = True
    End Sub

    Private Sub Handle_EventBox_DragLeave(ByVal sender As Object, ByVal e As DragEventArgs)
        e.Effects = DragDropEffects.None
        e.Handled = True
    End Sub

    Private Sub Handle_EventBox_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs)
        Try
            Dim bd As System.Windows.Controls.Border = TryCast(sender, System.Windows.Controls.Border)
            If bd Is Nothing Then Exit Sub

            If PLAYER IsNot Nothing Then
                If PLAYER.CurrentDomain = SMT.Multimedia.DirectShow.DvdDomain.Title Then
                    If PLAYER.CurrentTitle <> EC.DVDTitleNumber Then
                        If MessageBox.Show("WARNING: You are adding data to the event that is not associated with the DVD Title currently playing.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
                    End If
                End If
            End If

            Dim ei As cRCDb_EVENT_ITEM = bd.Tag
            'If EventSet.IsScenes AndAlso e.Data.GetFormats(0).ToString.ToLower = "scene" Then
            ' this check is now down in AddReference() below
            'End If
            ei.AddReference(e.Data, EventSet)
            RenderEventDisplay()
        Catch ex As Exception
            Throw New Exception("Problem with Handle_EventBox_Drop(). Error: " & ex.Message, ex)
        End Try
    End Sub

    'Private DragStartPoint As System.Windows.Point

    'Private Sub Handle_EventBox_LeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    DragStartPoint = e.GetPosition(cvBoxParent)
    'End Sub

    'Private Sub Handle_EventBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
    '    Dim mousePos As Windows.Point = e.GetPosition(cvBoxParent)
    '    Dim diff As Vector = DragStartPoint - mousePos
    '    If e.LeftButton = MouseButtonState.Pressed And Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance And Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance Then
    '        'need to pass along both the event and it's container set
    '        Dim Os(1) As Object
    '        Os(0) = CType(sender, Border).Tag
    '        Os(1) = EventSet
    '        Dim DragDataObject As New DataObject("event-set", Os)
    '        DragDrop.DoDragDrop(cvBoxParent, DragDataObject, DragDropEffects.All)
    '    End If
    'End Sub

#Region "FUNCTIONALITY:EVENT BOX:CONTEXT MENU"

    Private WithEvents EventBoxContextMenu As ContextMenu

    Private Sub ShowEventBoxContextMenu(ByRef EventBox As Border, ByRef p As System.Windows.Point)
        Try
            EventBoxContextMenu = New System.Windows.Controls.ContextMenu
            EventBoxContextMenu.HasDropShadow = True
            EventBoxContextMenu.Placement = Primitives.PlacementMode.Relative
            EventBoxContextMenu.HorizontalOffset = p.X
            EventBoxContextMenu.VerticalOffset = p.Y
            EventBoxContextMenu.PlacementTarget = EventBox
            EventBoxContextMenu.Tag = EventBox.Tag

            Dim tMI As System.Windows.Controls.MenuItem
            Dim sep As System.Windows.Controls.Separator

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Delete"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_EventBoxContextMenu_EventDelete
            EventBoxContextMenu.Items.Add(tMI)

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Edit Times"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_EventBoxContextMenu_EventEditTimes
            EventBoxContextMenu.Items.Add(tMI)

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Remove Data"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_EventBoxContextMenu_RemoveData
            EventBoxContextMenu.Items.Add(tMI)

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Event Editor"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_EventBoxContextMenu_ShowEventEditor
            EventBoxContextMenu.Items.Add(tMI)

            'sep = New System.Windows.Controls.Separator
            'sep.SnapsToDevicePixels = True
            'sep.Height = 1
            'StreamContextMenu.Items.Add(sep)

            EventBoxContextMenu.IsOpen = True

        Catch ex As Exception
            Throw New Exception("Problem with ShowEventBoxContextMenu(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub Handle_EventBoxContextMenu_EventDelete(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim MI As MenuItem = TryCast(sender, MenuItem)
        If MI Is Nothing Then Exit Sub
        Dim ei As cRCDb_EVENT_ITEM = CType(MI.Parent, ContextMenu).Tag
        If MessageBox.Show("Are you sure you wish to delete the event?" & vbNewLine & "Start = " & ei.Start & "  End = " & ei.End, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        EventSet.Items.Remove(ei)
        RenderEventDisplay()
    End Sub

    Private Sub Handle_EventBoxContextMenu_EventEditTimes(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim MI As MenuItem = TryCast(sender, MenuItem)
        If MI Is Nothing Then Exit Sub
        Dim ei As cRCDb_EVENT_ITEM = CType(MI.Parent, ContextMenu).Tag
        Dim st As String = InputBox("Start Time:", "", ei.Start)
        If Not IsNumeric(st) Then
            MsgBox("Invalid value.")
            Exit Sub
        End If
        Dim et As String = InputBox("End Time:", "", ei.End)
        If Not IsNumeric(et) Then
            MsgBox("Invalid value.")
            Exit Sub
        End If
        If EC.ActiveSet.EventTimesOverlap(st, et, ei) Then

        End If

        ei.Start = st
        ei.End = et
        RenderEventDisplay()
    End Sub

    Private Sub Handle_EventBoxContextMenu_ShowEventEditor(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        ShowEventEditor(sender)
    End Sub

    Private Sub Handle_EventBoxContextMenu_RemoveData(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim MI As MenuItem = TryCast(sender, MenuItem)
        If MI Is Nothing Then Exit Sub
        Dim ei As cRCDb_EVENT_ITEM = CType(MI.Parent, ContextMenu).Tag
        If MessageBox.Show("Are you sure you wish to remove all data references for this event?" & vbNewLine & "Start = " & ei.Start & "  End = " & ei.End, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        ei.RemoveAllReferences()
        RenderEventDisplay()
    End Sub

#End Region 'FUNCTIONALITY:EVENT BOX:CONTEXT MENU

#End Region 'FUNCTIONALITY:EVENT BOX 

#Region "FUNCTIONALITY:BOX DROP"

    'Private Sub cvBoxParent_DragEnter(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles cvBoxParent.DragEnter
    '    Dim fs() As String = e.Data.GetFormats()
    '    If fs(0) = "event-set" Then
    '        e.Effects = DragDropEffects.Copy
    '    End If
    'End Sub

    'Private Sub cvBoxParent_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles cvBoxParent.Drop

    'End Sub

#End Region 'FUNCTIONALITY:BOX DROP

End Class
