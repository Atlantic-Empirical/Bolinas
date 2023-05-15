Partial Public Class SceneEditor_Dialog

    Private SI As cRCDb_SCENE_ITEM

    Public Sub New(ByRef nSceneItem As cRCDb_SCENE_ITEM)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        SI = nSceneItem
    End Sub

    Private Sub Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderItem()
    End Sub

    Private Sub RenderItem()
        lblId.Content = SI.Id
        lblName.Content = SI.Name
        txtDescripion.Text = SI.Description
    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        SI.Description = txtDescripion.Text
        DialogResult = True
        Close()
    End Sub

    Private Sub lblId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblId.MouseLeftButtonUp
        Dim id As String = InputBox("New Id", My.Settings.APPLICATION_NAME_SHORT, SI.Id)
        If String.IsNullOrEmpty(id) OrElse Not IsNumeric(id) Then
            MessageBox.Show("Invalid Id.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
            Exit Sub
        Else
            SI.Id = id
            RenderItem()
        End If
    End Sub

    Private Sub lblName_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblName.MouseLeftButtonUp
        Dim name As String = InputBox("New Name", My.Settings.APPLICATION_NAME_SHORT, SI.Name)
        If String.IsNullOrEmpty(name) Then
            MessageBox.Show("Invalid Name.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            SI.Name = name
            RenderItem()
        End If
    End Sub

End Class
