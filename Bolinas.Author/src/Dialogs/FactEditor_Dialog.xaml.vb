Partial Public Class FactEditor_Dialog

    Private FI As cRCDb_FACT_ITEM

    Public Sub New(ByRef nFactItem As cRCDb_FACT_ITEM)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        FI = nFactItem
        RenderItem()
    End Sub

    Private Sub Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderItem()
    End Sub

    Private Sub RenderItem()
        lblId.Content = FI.Id
        txtDescripion.Text = FI.Description
    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        FI.Description = txtDescripion.Text
        DialogResult = True
        Close()
    End Sub

    Private Sub lblId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblId.MouseLeftButtonUp
        Dim id As String = InputBox("New Id", My.Settings.APPLICATION_NAME_SHORT, FI.Id)
        If String.IsNullOrEmpty(id) OrElse Not IsNumeric(id) Then
            MessageBox.Show("Invalid Id.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            FI.Id = id
            RenderItem()
        End If
    End Sub

End Class
