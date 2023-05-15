Partial Public Class CastEditor_Dialog

    Private CI As cRCDb_CONTRIBUTOR_ITEM

    Public Sub New(ByRef nContributorItem As cRCDb_CONTRIBUTOR_ITEM)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        CI = nContributorItem
        RenderItem()
    End Sub

    Private Sub Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderItem()
    End Sub

    Private Sub RenderItem()
        lblId.Content = CI.ContributorId
        lblCharacterName.Content = CI.CharacterName
        lblContributorName.Content = CI.ContributorName
        lblBilling.Content = CI.Billing
    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        DialogResult = True
        Close()
    End Sub

    Private Sub lblId_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblId.MouseLeftButtonUp
        Dim id As String = InputBox("New Id", My.Settings.APPLICATION_NAME_SHORT, CI.ContributorId)
        If String.IsNullOrEmpty(id) OrElse Not IsNumeric(id) Then
            MessageBox.Show("Invalid Id.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            CI.ContributorId = id
            RenderItem()
        End If
    End Sub

    Private Sub lblBilling_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblBilling.MouseLeftButtonUp
        Dim input As String = InputBox("New Billing", My.Settings.APPLICATION_NAME_SHORT, CI.Billing)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Billing.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            CI.Billing = input
            RenderItem()
        End If
    End Sub

    Private Sub lblCharacterName_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblCharacterName.MouseLeftButtonUp
        Dim input As String = InputBox("New Character Name", My.Settings.APPLICATION_NAME_SHORT, CI.CharacterName)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Character Name.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            CI.CharacterName = input
            RenderItem()
        End If
    End Sub

    Private Sub lblContributorName_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblContributorName.MouseLeftButtonUp
        Dim input As String = InputBox("New Contributor Name", My.Settings.APPLICATION_NAME_SHORT, CI.ContributorName)
        If String.IsNullOrEmpty(input) Then
            MessageBox.Show("Invalid Contributor Name.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
            Exit Sub
        Else
            CI.ContributorName = input
            RenderItem()
        End If
    End Sub

End Class
