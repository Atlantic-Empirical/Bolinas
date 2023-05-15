Partial Public Class FilmographyItemEditor_Dialog

    Public Property Id() As Integer
        Get
            Return txtId.Text
        End Get
        Set(ByVal value As Integer)
            txtId.Text = value
        End Set
    End Property

    Public Property ContributorId() As Integer
        Get
            Return txtContributorId.Text
        End Get
        Set(ByVal value As Integer)
            txtContributorId.Text = value
        End Set
    End Property

    Public Property CharacterName() As String
        Get
            Return txtCharacterName.Text
        End Get
        Set(ByVal value As String)
            txtCharacterName.Text = value
        End Set
    End Property

    Public Property Role() As String
        Get
            Return txtRole.Text
        End Get
        Set(ByVal value As String)
            txtRole.Text = value
        End Set
    End Property

    Public Property WorkId() As String
        Get
            Return txtWorkId.Text
        End Get
        Set(ByVal value As String)
            txtWorkId.Text = value
        End Set
    End Property

    Public Property WorkDate() As Date
        Get
            Return dpWorkDate.Text
        End Get
        Set(ByVal value As Date)
            dpWorkDate.Text = value
        End Set
    End Property

    Public Property WorkStudio() As String
        Get
            Return txtWorkStudio.Text
        End Get
        Set(ByVal value As String)
            txtWorkStudio.Text = value
        End Set
    End Property

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        'VALIDATE
        If (txtId.Text <> "" And Not IsNumeric(txtId.Text)) OrElse txtId.Text > Integer.MaxValue Then
            MsgBox("Id is invalid.")
            Exit Sub
        End If
        If (txtContributorId.Text <> "" And Not IsNumeric(txtContributorId.Text)) OrElse txtContributorId.Text > Integer.MaxValue Then
            MsgBox("ContributorId is invalid.")
            Exit Sub
        End If
        'VALIDATE
        DialogResult = True
        Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Close()
    End Sub

End Class
