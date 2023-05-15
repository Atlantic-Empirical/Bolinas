Partial Public Class TrackEditor_Dialog

    Private TI As cRCDb_MUSIC_ITEM

    Public Sub New(ByRef nMusicItem As cRCDb_MUSIC_ITEM)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        TI = nMusicItem
        RenderItem()
    End Sub

    Private Sub Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderItem()
    End Sub

    Private Sub RenderItem()
        lblAlbum.Content = TI.Album
        lblArtist.Content = TI.Artist
        lblId.Content = TI.Id
        lblLabel.Content = TI.Label
        lblReferenceId.Content = TI.ReferenceId
        lblReleaseDate.Content = TI.ReleaseDate
        lblTime.Content = TI.Time
        lblTitle.Content = TI.Title
        txtNotes.Text = TI.Notes
    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        TI.Notes = txtNotes.Text
        DialogResult = True
        Close()
    End Sub

    Private Sub lblAlbum_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblAlbum.MouseLeftButtonUp
        Dim input As String = InputBox("New Album", My.Settings.APPLICATION_NAME_SHORT, TI.Album)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Album.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Album = input
            RenderItem()
        End If
    End Sub

    Private Sub lblArtist_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblArtist.MouseLeftButtonUp
        Dim input As String = InputBox("New Artist", My.Settings.APPLICATION_NAME_SHORT, TI.Artist)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Artist.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Artist = input
            RenderItem()
        End If
    End Sub

    Private Sub lblId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblId.MouseLeftButtonUp
        Dim input As String = InputBox("New Id", My.Settings.APPLICATION_NAME_SHORT, TI.Id)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Id.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Id = input
            RenderItem()
        End If

    End Sub

    Private Sub lblLabel_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblLabel.MouseLeftButtonUp
        Dim input As String = InputBox("New Label", My.Settings.APPLICATION_NAME_SHORT, TI.Label)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Label.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Label = input
            RenderItem()
        End If

    End Sub

    Private Sub lblReferenceId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblReferenceId.MouseLeftButtonUp
        Dim input As String = InputBox("New Reference Id", My.Settings.APPLICATION_NAME_SHORT, TI.ReferenceId)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Reference Id.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.ReferenceId = input
            RenderItem()
        End If

    End Sub

    Private Sub lblReleaseDate_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblReleaseDate.MouseLeftButtonUp
        MsgBox("nop")
        'Dim input As String = InputBox("New Release Date", My.Settings.APPLICATION_NAME_SHORT, TI.ReleaseDate)
        'If String.IsNullOrEmpty(input) Then
        '    MessageBox.Show("Invalid Release Date.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        'Else
        '    TI.ReleaseDate = input
        '    RenderItem()
        'End If
    End Sub

    Private Sub lblTime_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblTime.MouseLeftButtonUp
        Dim input As String = InputBox("New Time", My.Settings.APPLICATION_NAME_SHORT, TI.Time)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Time.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Time = input
            RenderItem()
        End If
    End Sub

    Private Sub lblTitle_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblTitle.MouseLeftButtonUp
        Dim input As String = InputBox("New Title", My.Settings.APPLICATION_NAME_SHORT, TI.Title)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Title.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            TI.Title = input
            RenderItem()
        End If
    End Sub

End Class
