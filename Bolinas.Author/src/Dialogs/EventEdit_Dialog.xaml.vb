Imports SMT.Multimedia.Utility.Timecode
Imports SMT.Multimedia.Players.DVD

Partial Public Class EventEdit_Dialog

    Private EI As cRCDb_EVENT_ITEM
    Private ES As cRCDb_EVENT_COLLECTION
    Private EC As cRCDb_EVENT_CONTAINER

    Public Sub New(ByRef nEventItem As cRCDb_EVENT_ITEM, ByRef nEventSet As cRCDb_EVENT_COLLECTION, ByRef nEventContainer As cRCDb_EVENT_CONTAINER)
        InitializeComponent()
        EI = nEventItem
        ES = nEventSet
        EC = nEventContainer
    End Sub

    Private Sub EventEdit_Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderEvent()
    End Sub

    Private Sub RenderEvent()
        Me.lblStartTime.Content = New cTimecode(CUInt(EI.Start), eFramerate.NTSC).ToString_NoFrames
        Me.lblEndTime.Content = New cTimecode(CUInt(EI.End), eFramerate.NTSC).ToString_NoFrames
        Me.lblId.Content = EI.Id
        Me.lblSet.Content = ES.Name
        cbSets.Items.Clear()
        cbSets.Items.Add("SCENES")
        For Each c As cRCDb_EVENT_COLLECTION In EC.EventSets
            If c.Name <> ES.Name Then
                Me.cbSets.Items.Add(c.Name)
            End If
        Next
        cbSets.SelectedIndex = -1

        If ES.Name.ToLower <> "scenes" Then
            Me.lblSceneName.IsEnabled = False
            Me.lblSceneName.Content = ""
        Else
            Me.lblSceneName.Content = EI.SCENE.Name
        End If

        Me.lvCast.Items.Clear()
        For Each c As cRCDb_CONTRIBUTOR_ITEM In EI.CAST
            Me.lvCast.Items.Add(c)
        Next

        'Me.lvMusic.Items.Clear()
        'For Each m As cRCDb_MUSIC_ITEM In EI.SOUNDTRACK
        '    Me.lvMusic.Items.Add(m)
        'Next
        If EI.SOUNDTRACK.IsInitalized Then
            Me.lblTrack_Title.Content = EI.SOUNDTRACK.Title
        End If

        Me.lvFacts.Items.Clear()
        For Each f As cRCDb_FACT_ITEM In EI.FACTS
            Me.lvFacts.Items.Add(f)
        Next

    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = False
        Me.Close()
    End Sub

    Private Sub lblStartTime_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblStartTime.MouseLeftButtonUp
        Dim newStart As String = InputBox("New event start time (in seconds): ", "", EI.Start)
        If newStart = "" OrElse Not IsNumeric(newStart) OrElse newStart > 32000 OrElse ES.EventTimesOverlap(newStart, EI.End, EI) OrElse CInt(newStart) > CInt(EI.End) OrElse CInt(newStart) < 0 Then
            MsgBox("Invalid start time. It must be non-null, numeric, not after the end of this event, and must not overlap with another event in this set.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
            Exit Sub
        End If
        EI.Start = newStart
        RenderEvent()
        EC.ForceRefresh()
    End Sub

    Private Sub lblEndTime_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblEndTime.MouseLeftButtonUp
        Dim newEnd As String = InputBox("New event end time (in seconds): ", "", EI.End)
        If newEnd = "" OrElse Not IsNumeric(newEnd) OrElse newEnd > 32000 OrElse ES.EventTimesOverlap(EI.Start, newEnd, EI) OrElse CInt(newEnd) < CInt(EI.Start) OrElse CInt(newEnd) > EC.DVDTitleDuration Then
            MsgBox("Invalid start time. It must be non-null, numeric, not before the start of this event, and must not overlap with another event in this set.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
            Exit Sub
        End If
        EI.End = newEnd
        RenderEvent()
        EC.ForceRefresh()
    End Sub

    Private Sub lblSceneName_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblSceneName.MouseLeftButtonUp
        Dim newSceneName As String = InputBox("New scene name: ", "", EI.SCENE.Name)
        If newSceneName = "" Then
            MsgBox("Invalid scene name. It must be non-null.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
            Exit Sub
        End If
        EI.SCENE.Name = newSceneName
        RenderEvent()
    End Sub

    Private Sub cbSets_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbSets.SelectionChanged
        Me.lblSet.Visibility = Windows.Visibility.Visible
        Me.cbSets.Visibility = Windows.Visibility.Hidden
        If cbSets.SelectedItem Is Nothing Then Exit Sub
        Dim targSet As cRCDb_EVENT_COLLECTION = EC.GetEventSetByName(cbSets.SelectedItem.ToString)
        If targSet.EventTimesOverlap(EI.Start, EI.End) Then
            MessageBox.Show("This event overlaps with an existing event in the set: " & cbSets.SelectedItem.ToString & ". Move cancelled.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        Else
            ES.Items.Remove(EI)
            targSet.Items.Add(EI)
            ES = targSet
            EC.ForceRefresh()
            RenderEvent()
        End If
    End Sub

    Private Sub lblId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblId.MouseLeftButtonUp
        Dim newId As String = InputBox("New event Id: ", "", EI.Id)
        If newId = "" OrElse Not IsNumeric(newId) Then
            MsgBox("Invalid Id. It must be non-null and numeric.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
            Exit Sub
        End If
        EI.Id = newId
        RenderEvent()
    End Sub

    Private Sub lblSet_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblSet.MouseLeftButtonUp
        Me.lblSet.Visibility = Windows.Visibility.Hidden
        Me.cbSets.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub lvCast_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCast.MouseDoubleClick
        If lvCast.SelectedItem Is Nothing Then Exit Sub
        Dim dlg As New CastEditor_Dialog(lvCast.SelectedItem)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        RenderEvent()
    End Sub

    Private Sub lvFacts_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvFacts.MouseDoubleClick
        If lvFacts.SelectedItem Is Nothing Then Exit Sub
        Dim dlg As New FactEditor_Dialog(lvFacts.SelectedItem)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        RenderEvent()
    End Sub

    'Private Sub lvMusic_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    If lvMusic.SelectedItem Is Nothing Then Exit Sub
    '    If MessageBox.Show("Delete the selected item?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
    '    EI.SOUNDTRACK.Remove(lvMusic.SelectedItem)
    '    RenderEvent()
    'End Sub

    Private Sub lblFillNext_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblFillNext.MouseLeftButtonUp
        Dim i As Integer = ES.ItemsSorted.IndexOf(EI)
        If ES.ItemsSorted.Count = i + 1 Then
            If EC.DVDTitleDuration <> Nothing And EC.DVDTitleDuration > 0 Then
                EI.End = EC.DVDTitleDuration
            Else
                MsgBox("This is the last event in the set.")
            End If
        Else
            EI.End = ES.ItemsSorted(i + 1).Start - 1
        End If
        RenderEvent()
        EC.ForceRefresh()
    End Sub

    Private Sub lblFillPrior_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblFillPrior.MouseLeftButtonUp
        Dim i As Integer = ES.ItemsSorted.IndexOf(EI)
        If i = 0 Then
            EI.Start = 0
        Else
            EI.Start = ES.ItemsSorted(i - 1).End + 1
        End If
        RenderEvent()
        EC.ForceRefresh()
    End Sub

    Private Sub lblMUSIC_edit_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblMUSIC_edit.MouseLeftButtonUp
        If Not EI.SOUNDTRACK.IsInitalized Then Exit Sub
        Dim dlg As New TrackEditor_Dialog(EI.SOUNDTRACK)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        RenderEvent()
    End Sub

    Private Sub lblMUSIC_remove_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblMUSIC_remove.MouseLeftButtonUp
        EI.SOUNDTRACK = New cRCDb_MUSIC_ITEM
        EC.ForceRefresh()
        RenderEvent()
    End Sub

    Private Sub lblScene_edit_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblScene_edit.MouseLeftButtonUp
        If Not EI.SCENE.IsInitalized Then Exit Sub
        Dim dlg As New SceneEditor_Dialog(EI.SCENE)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        RenderEvent()
    End Sub

    Private Sub lblSCENE_remove_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblSCENE_remove.MouseLeftButtonUp
        EI.SCENE = New cRCDb_SCENE_ITEM
        EC.ForceRefresh()
        RenderEvent()
    End Sub

#Region "CONTEXT MENU"

    Private Sub lvCast_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCast.MouseRightButtonUp
        If lvCast.SelectedItem Is Nothing Then Exit Sub
        ShowItemContextMenu(sender, e.GetPosition(sender))
    End Sub

    Private Sub lvFacts_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvFacts.MouseRightButtonUp
        If lvCast.SelectedItem Is Nothing Then Exit Sub
        ShowItemContextMenu(sender, e.GetPosition(sender))
    End Sub

    Private WithEvents ItemContextMenu As ContextMenu

    Private Sub ShowItemContextMenu(ByRef lv As ListView, ByRef p As System.Windows.Point)
        Try
            ItemContextMenu = New System.Windows.Controls.ContextMenu
            ItemContextMenu.HasDropShadow = True
            ItemContextMenu.Placement = Primitives.PlacementMode.Relative
            ItemContextMenu.HorizontalOffset = p.X
            ItemContextMenu.VerticalOffset = p.Y
            ItemContextMenu.PlacementTarget = lv
            ItemContextMenu.Tag = lv.SelectedItem

            Dim tMI As System.Windows.Controls.MenuItem
            Dim sep As System.Windows.Controls.Separator

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Remove"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_ItemContextMenu_EventRemove
            ItemContextMenu.Items.Add(tMI)

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Edit"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_ItemContextMenu_ShowEditor
            ItemContextMenu.Items.Add(tMI)

            'sep = New System.Windows.Controls.Separator
            'sep.SnapsToDevicePixels = True
            'sep.Height = 1
            'StreamContextMenu.Items.Add(sep)

            ItemContextMenu.IsOpen = True

        Catch ex As Exception
            Throw New Exception("Problem with ShowItemContextMenu(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub Handle_ItemContextMenu_EventRemove(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim n As String = ItemContextMenu.Tag.GetType.Name
        If InStr(n.ToLower, "contributor") Then
            Dim c As cRCDb_CONTRIBUTOR_ITEM = ItemContextMenu.Tag
            If MessageBox.Show("Remove " & c.ContributorName & " from event?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
            EI.CAST.Remove(c)
        End If
        If InStr(n.ToLower, "fact") Then
            Dim f As cRCDb_FACT_ITEM = ItemContextMenu.Tag
            If MessageBox.Show("Remove fact (" & f.Id & ") from event?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
            EI.FACTS.Remove(f)
        End If
        RenderEvent()
        EC.ForceRefresh()
    End Sub

    Private Sub Handle_ItemContextMenu_ShowEditor(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim n As String = ItemContextMenu.Tag.GetType.Name
        If InStr(n.ToLower, "contributor") Then
            Dim dlg As New CastEditor_Dialog(ItemContextMenu.Tag)
            dlg.Owner = Me
            Dim b As Boolean = dlg.ShowDialog
        End If
        If InStr(n.ToLower, "fact") Then
            Dim dlg As New FactEditor_Dialog(ItemContextMenu.Tag)
            dlg.Owner = Me
            Dim b As Boolean = dlg.ShowDialog
        End If
        RenderEvent()
    End Sub

#End Region 'CONTEXT MENU

End Class
