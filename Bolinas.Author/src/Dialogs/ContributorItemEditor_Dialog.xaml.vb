Partial Public Class ContributorItemEditor_Dialog

    Public Property ReferenceId() As String
        Get
            Return Me.txtReferenceId.Text
        End Get
        Set(ByVal value As String)
            txtReferenceId.Text = value
        End Set
    End Property

    Public Property ContributorName() As String
        Get
            Return txtContributorName.Text
        End Get
        Set(ByVal value As String)
            txtContributorName.Text = value
        End Set
    End Property

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub

End Class
